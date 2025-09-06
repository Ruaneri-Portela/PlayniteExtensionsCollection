using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class InnoSetupInstallerHandler : IInstallerHandler
    {
        public InstallerType Type => InstallerType.InnoSetup;

        public bool CanHandle(string filePath, InstallerType type)
        {
            if (filePath == null || type != InstallerType.InnoSetup)
            {
                return false;
            }
            return true;
        }

        public bool Install(InstallRequest request)
        {
            var arguments = $"/VERYSILENT /CURRENTUSER /SUPPRESSMSGBOXES /DIR={request.TargetDirectory}";
            return InstallerHandler.runProcess(request.FilePath, arguments);
        }
        public bool Uninstall(InstallRequest request)
        {
            var arguments = "/VERYSILENT /CURRENTUSER /SUPPRESSMSGBOXES";
            if (!string.IsNullOrEmpty(request.FilePath))
            { 
                arguments += $" /DIR={request.FilePath}"; 
            }


            return InstallerHandler.runProcess(request.FilePath, arguments);
        }
    }
}
