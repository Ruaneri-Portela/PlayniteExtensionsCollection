using JastUsaLibrary.Features.InstallationHandler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JastUsaLibrary.Features.InstallationHandler.Application
{
    public interface IInstallerHandler
    {
        bool CanHandle(string filePath, InstallerType fileContent);
        bool Install(InstallRequest request);
        bool Uninstall(InstallRequest request);
    }
}
