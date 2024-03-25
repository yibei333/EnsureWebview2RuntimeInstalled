using System;
using System.Net;
using System.Threading.Tasks;

namespace EnsureWebview2RuntimeInstalled
{
    public static class HttpUtil
    {
        public static async Task Download(string url, string savePath, Action<HttpProgress> progressAction)
        {
            using (WebClient client = new WebClient())
            {
                var speed = new TransferFileStatisticsModel(DateTime.Now);
                int lastProgress = 0;
                client.DownloadProgressChanged += (s, e) =>
                {
                    var progress = new HttpProgress(e.TotalBytesToReceive, e.BytesReceived, speed);
                    if (progress.Progress > lastProgress)
                    {
                        lastProgress = progress.Progress;
                        progressAction?.Invoke(progress);
                    }
                };
                await client.DownloadFileTaskAsync(new Uri(url), savePath);
            }
        }
    }

    public class HttpProgress
    {
        private readonly TransferFileStatisticsModel _speed;
        public long Total { get; }
        public long Transfered { get; }

        public int Progress
        {
            get
            {
                if (Total <= 0)
                {
                    return 0;
                }

                int num = (int)((double)Transfered * 100.0 / (double)Total);
                if (num >= 100)
                {
                    return 100;
                }

                return num;
            }
        }

        public string ProgressString => $"{Progress}%";

        public string Speed => _speed.Calc(Transfered);

        internal HttpProgress(long total, long transfered, TransferFileStatisticsModel speed)
        {
            Total = total;
            Transfered = transfered;
            _speed = speed;
        }
    }

    internal class TransferFileStatisticsModel
    {
        public static readonly TimeSpan Peroid = TimeSpan.FromSeconds(1.0);

        public DateTime TransferTime { get; set; }

        public double TotalByteCount { get; set; }

        public string Speed { get; set; }

        public TransferFileStatisticsModel(DateTime downloadTime)
        {
            TransferTime = downloadTime;
            TotalByteCount = 0.0;
            Speed = "0KB/S";
        }

        public string Calc(double currentByteCount)
        {
            DateTime now = DateTime.Now;
            if (now - TransferTime < Peroid)
            {
                return Speed;
            }

            Speed = $"{(int)((currentByteCount - TotalByteCount) / 1024.0 / Peroid.TotalSeconds)}KB/S";
            TotalByteCount = currentByteCount;
            TransferTime = now;
            return Speed;
        }
    }
}
