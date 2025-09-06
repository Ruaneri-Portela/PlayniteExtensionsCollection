using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class ArchiveInstallerHandler : IInstallerHandler
    {
        private readonly IInstallerDetector _detector;
        public InstallerType Type => InstallerType.Archive;

        public ArchiveInstallerHandler(IInstallerDetector detector)
        {
            _detector = detector;
        }

        public bool CanHandle(string filePath, InstallerType type)
        {
            if (filePath == null || type != InstallerType.Archive)
            {
                return false;
            }
            return true;
        }

        public bool Install(InstallRequest request)
        {
            var outputDir = request.TargetDirectory ?? Path.Combine(Path.GetDirectoryName(request.FilePath), Path.GetFileNameWithoutExtension(request.FilePath));
            Directory.CreateDirectory(outputDir);
            if (!TryExtractArchive(request.FilePath, outputDir))
            {
                return false;
            }
            return true;
        }

        private bool TryExtractArchive(string filePath, string outputDir)
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    using (var archive = ArchiveFactory.Open(stream))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.IsDirectory)
                            {
                                entry.WriteToDirectory(outputDir, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                            }
                        }

                        return true;
                    }
                }
            }
            catch
            {
                // Failed to extract (not recognized or corrupt)
                return false;
            }
        }

        public bool Uninstall(InstallRequest request)
        {
            // Stub
            if (Directory.Exists(request.TargetDirectory))
            {
                Directory.Delete(request.TargetDirectory, recursive: true);
                return true;
            }
            return false;
        }
    }
}
