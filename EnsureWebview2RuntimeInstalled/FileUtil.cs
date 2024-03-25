using SharpDevLib;
using SharpDevLib.Extensions.Http;
using System.Diagnostics;

namespace EnsureWebview2RuntimeInstalled;

public static class FileUtil
{
    public static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("MicrosoftEdgeWebView2RuntimeInstallerX64.exe");
    public static async void Download()
    {
        try
        {
            CleanTempFile();
            var stream = await ServiceUtil.HttpService.GetStreamAsync(new ParameterOption("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/58f618e2-bf87-4672-933a-425072a73ec5/MicrosoftEdgeWebView2RuntimeInstallerX64.exe")
            {
                OnReceiveProgress = (p) => WindowUtil.SetProgress(p)
            });
            using var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await stream.CopyToAsync(fileStream);
            fileStream.Flush();
            stream.Close();
            fileStream.Close();
        }
        catch (Exception ex)
        {
            WindowUtil.SetDownloadError(ex.Message);
        }
    }

    public static void Run()
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
        }
        catch (Exception ex)
        {
            WindowUtil.SetRunError(ex.Message);
        }
    }

    public static void CleanTempFile()
    {
        if (File.Exists(FilePath)) File.Delete(FilePath);
    }
}
