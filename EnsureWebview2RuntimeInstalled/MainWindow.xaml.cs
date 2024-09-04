using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EnsureWebview2RuntimeInstalled
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(State), typeof(MainWindow),new FrameworkPropertyMetadata(State.Default));
        static DependencyProperty ErrorMessageProperty = DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            Run();
        }

        public State State
        {
            get => (State)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        async void Run()
        {
            try
            {
                if (RegistryUtil.GetRuntimeIsInstalled())
                {
                    Application.Current.Shutdown(0);
                    return;
                }

                State = State.Downloading;
                await FileUtil.Download((p) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        progressLabel.Content = p.ProgressString;
                        progressBar.Value = p.Progress;
                        speedLabel.Content = p.Speed;
                    });
                });

                State = State.Installing;
                ErrorMessage = await FileUtil.Install();
                if (!string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    State = State.Error;
                }
                else Application.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                State = State.Error;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }
    }

    public enum State
    {
        Default,
        Downloading,
        Installing,
        Error
    }

    public class StateVisibiltyConverter : IValueConverter
    {
        public StateVisibiltyConverter(State targetState)
        {
            TargetState = targetState;
        }

        public State TargetState { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is State state && state == TargetState)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DownloadingVisibiltyConverter : StateVisibiltyConverter
    {
        public DownloadingVisibiltyConverter() : base(State.Downloading)
        {
        }
    }

    public class InstallingVisibiltyConverter : StateVisibiltyConverter
    {
        public InstallingVisibiltyConverter() : base(State.Installing)
        {
        }
    }

    public class ErrorVisibiltyConverter : StateVisibiltyConverter
    {
        public ErrorVisibiltyConverter() : base(State.Error)
        {
        }
    }
}
