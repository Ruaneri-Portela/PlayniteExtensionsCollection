using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;
using System.Diagnostics;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class MsiInstallerHandler : IInstallerHandler
    {
        public InstallerType Type => InstallerType.Msi;

        public bool CanHandle(string filePath, InstallerType type)
        {
            if (filePath == null || type != InstallerType.Msi)
            {
                return false;
            }
            return true;
        }

        public bool Install(InstallRequest request)
        {
            // Stub
            return false;
        }

        public bool Uninstall(InstallRequest request)
        {
            // Stub
            return false;
        }
    }
}
