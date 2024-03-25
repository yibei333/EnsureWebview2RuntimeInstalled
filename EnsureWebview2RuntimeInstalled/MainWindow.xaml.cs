using System.Windows;

namespace EnsureWebview2RuntimeInstalled
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Run();
        }

        async void Run()
        {
            if (RegistryUtil.GetRuntimeIsInstalled())
            {
                App.Current.Shutdown(0);
                return;
            }

            await FileUtil.Download((p) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    progressLabel.Content = p.ProgressString;
                    progressBar.Value = p.Progress;
                    speedLabel.Content = p.Speed;
                });
            });
            var errorMessage = await FileUtil.Install();
            if(!string.IsNullOrWhiteSpace(errorMessage))
            {
                MessageBox.Show($"发生错误:{errorMessage},请重试");

            }
            App.Current.Shutdown(0);
            return;
        }
    }
}
