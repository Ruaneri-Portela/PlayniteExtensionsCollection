using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class InstallerDetector : IInstallerDetector
    {
        /// <summary>
        /// Flags and const from native arguments
        /// </summary>
        private const int RT_MANIFEST = 24;
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        /// <summary>
        /// Links native functions with code managed via P-Invoke
        /// </summary>
        private static class _nativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, int lpType);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr LockResource(IntPtr hResData);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
        }

        /// <summary>
        /// Detects if the file is a self-extracting archive (ZIP or 7z) using SharpCompress.
        /// Returns true if it can be opened as a ZIP or 7z archive.
        /// </summary>
        private bool IsSfxArchive(string filePath, ref FileStream fileRef, ref IArchive archiveRef)
        {
            try
            {
                fileRef = File.OpenRead(filePath);
                archiveRef = ArchiveFactory.Open(fileRef);

                return true;
            }
            catch
            {
                fileRef?.Dispose();
                fileRef = null;

                archiveRef = null;
            }

            return false;
        }


        /// <summary>
        /// The idea is to unpack and try to find some other installers, if possible, instead of falling back to uncompressor!
        /// </summary>
        private InstallerType _lookupByUncompression(string filePath)
        {
            FileStream fileStream = null;
            IArchive archive = null;

            if (!IsSfxArchive(filePath, ref fileStream, ref archive))
            {
                return InstallerType.Unknown;
            }

            return InstallerType.Archive;
        }

        /// <summary>
        /// We perform a lookup via XML manifest, using native Windows libraries.
        /// </summary>
        private InstallerType _lookupByManifest(string filePath)
        {
            IntPtr hModule = _nativeMethods.LoadLibraryEx(filePath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
            if (hModule == IntPtr.Zero)
            {
                return InstallerType.Unknown;
            }

            IntPtr hResInfo = _nativeMethods.FindResource(hModule, (IntPtr)1, RT_MANIFEST);
            if (hResInfo == IntPtr.Zero)
            {
                return InstallerType.Unknown;
            }

            IntPtr hResData = _nativeMethods.LoadResource(hModule, hResInfo);
            IntPtr pResource = _nativeMethods.LockResource(hResData);
            uint size = _nativeMethods.SizeofResource(hModule, hResInfo);

            if (pResource == IntPtr.Zero || size == 0)
            {
                return InstallerType.Unknown;
            }

            byte[] buffer = new byte[size];
            Marshal.Copy(pResource, buffer, 0, buffer.Length);

            var ms = new MemoryStream(buffer);

            XmlDocument executableManifest = new XmlDocument();
            executableManifest.Load(ms);

            foreach (XmlNode node in executableManifest.GetElementsByTagName("assemblyIdentity"))
            {
                string name = node.Attributes?["name"]?.Value ?? string.Empty;

                if (name.Contains("NSIS", StringComparison.OrdinalIgnoreCase))
                {
                    return InstallerType.Nsis;
                }

                if (name.Contains("Inno", StringComparison.OrdinalIgnoreCase))
                {
                    return InstallerType.InnoSetup;
                }
            }
            return InstallerType.Unknown;
        }

        /// <summary>
        /// To verify an installer, we first parse the type, discarding installers of the Windows Installer type. 
        /// In the case of an exe file, we will first check the manifest. If we cannot see whether the installer is inside a compressed file, we will simply see if it is possible to decompress and execute without installing.
        /// </summary>
        public InstallerType LookupInstallerType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            switch (extension)
            {
                case ".msi":
                    return InstallerType.Msi;
                case ".exe":
                    {
                        InstallerType result = _lookupByManifest(filePath);
                        if (result != InstallerType.Unknown) { return result; }
                        return _lookupByUncompression(filePath);
                    }
                case ".msix":
                    return InstallerType.Msix;
            }
            return InstallerType.Unknown;
        }

    }
}
