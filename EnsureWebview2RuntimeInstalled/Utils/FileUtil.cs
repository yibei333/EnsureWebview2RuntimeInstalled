using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EnsureWebview2RuntimeInstalled
{
    public static class FileUtil
    {
        public static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("MicrosoftEdgeWebView2RuntimeInstallerX64.exe");
        public static async Task Download(Action<HttpProgress> progressAction)
        {
            CleanTempFile();
            await HttpUtil.Download("https://go.microsoft.com/fwlink/p/?LinkId=2124703", FilePath, progressAction);
        }

        public static async Task<string> Install()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var myProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo(FilePath)
                        {
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    };
                    myProcess.Start();

                    while (!myProcess.HasExited)
                    {
                        myProcess.WaitForExit();
                    }

                    var code = myProcess.ExitCode;
                    if (code != 0) throw new Exception("安装失败");
                    myProcess.Dispose();
                    myProcess.Close();
                    CleanTempFile();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });
        }

        public static void CleanTempFile()
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
    }
}


