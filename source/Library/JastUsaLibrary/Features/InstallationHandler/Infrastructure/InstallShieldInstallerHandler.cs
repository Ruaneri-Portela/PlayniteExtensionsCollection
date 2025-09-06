using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class InstallShieldInstallerHandler : IInstallerHandler
    {
        public InstallerType Type => InstallerType.InstallShield;

        public bool CanHandle(string filePath, InstallerType type)
        {
            if (filePath == null || type != InstallerType.InstallShield)
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
