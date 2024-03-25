using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EnsureWebview2RuntimeInstalled
{
    public static class FileUtil
    {
        public static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("MicrosoftEdgeWebView2RuntimeInstallerX64.exe");
        public static async Task Download(Action<HttpProgress> progressAction)
        {
            try
            {
                CleanTempFile();
                await HttpUtil.Download("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/58f618e2-bf87-4672-933a-425072a73ec5/MicrosoftEdgeWebView2RuntimeInstallerX64.exe", FilePath, progressAction);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
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


