using System.Diagnostics;

namespace JastUsaLibrary.Features.InstallationHandler.Infrastructure
{
    class InstallerHandler
    {
        public static bool runProcess(string filePath, string arguments)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = arguments,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
