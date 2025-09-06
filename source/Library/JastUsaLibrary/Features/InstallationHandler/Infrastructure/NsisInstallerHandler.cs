using JastUsaLibrary.Features.InstallationHandler.Application;
using JastUsaLibrary.Features.InstallationHandler.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    public class NsisInstallerHandler : IInstallerHandler
    {
        public InstallerType Type => InstallerType.Nsis;

        public bool CanHandle(string filePath, InstallerType type)
        {
            if (filePath == null || type != InstallerType.Nsis)
            {
                return false;
            }
            return true;
        }

        public bool Install(InstallRequest request)
        {
            var arguments = $"/S /D={request.FilePath}";
            return InstallerHandler.runProcess(request.FilePath,arguments);           
        }

        public bool Uninstall(InstallRequest request)
        {
            var arguments = "/S";
            if (!string.IsNullOrEmpty(request.FilePath))
                arguments += $" /D={request.FilePath}";
            return InstallerHandler.runProcess(request.FilePath, arguments);
        }
    }
}
