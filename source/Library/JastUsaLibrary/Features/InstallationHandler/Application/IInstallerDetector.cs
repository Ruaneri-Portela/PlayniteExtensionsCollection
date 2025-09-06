using JastUsaLibrary.Features.InstallationHandler.Domain;
using System;
using System.Runtime.InteropServices;

namespace JastUsaLibrary.Features.InstallationHandler.Application
{
    public interface IInstallerDetector
    {
        InstallerType LookupInstallerType(string filePath);
    }
}
