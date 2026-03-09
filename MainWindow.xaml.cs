using BookGym.Better;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookGym
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BetterClient _client { get; set; } = new BetterClient();
        private DateTime _targetDateTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;

            // Wire up event handlers programmatically
            TargetDate.SelectedDateChanged += TargetDate_SelectedDateChanged;
            TargetClassName.TextChanged += TargetClassName_TextChanged;
            ConsoleOutput.TextChanged += ConsoleOutput_TextChanged;

            // Start login after window is loaded
            this.Loaded += (s, e) =>
            {
                try
                {
                    // Initialize DatePicker to March 16th
                    TargetDate.SelectedDate = new DateTime(2026, 3, 16);
                    _targetDateTime = TargetDate.SelectedDate ?? DateTime.Now;
                    _client.TargetDate = _targetDateTime.ToString("yyyy-MM-dd");

                    // Initialize TargetClassName
                    _client.TargetClassName = TargetClassName.Text;

                    Login();
                }
                catch (Exception ex)
                {
                    AppendConsoleMessage($"Error during initialization: {ex.Message}");
                }
            };
        }

        public void SetTargetClass(string className, DateTime dateTime)
        {
            Dispatcher.Invoke(() =>
            {
                _targetDateTime = dateTime;
                TargetClassName.Text = className;
                TargetDate.SelectedDate = dateTime;
                TargetTime.Text = dateTime.ToString("h:mm tt");
                _client.TargetDate = dateTime.ToString("yyyy-MM-dd");
            });
        }

        public void SetTargetClass(string className, string date, string time)
        {
            Dispatcher.Invoke(() =>
            {
                TargetClassName.Text = className;
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    _targetDateTime = parsedDate;
                    TargetDate.SelectedDate = parsedDate;
                    _client.TargetDate = parsedDate.ToString("yyyy-MM-dd");
                }

                if (DateTime.TryParse(time, out DateTime parsedTime))
                {
                    TargetTime.Text = parsedTime.ToString("h:mm tt");
                }
                else
                {
                    TargetTime.Text = time;
                }
            });
        }

        private void TargetDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TargetDate.SelectedDate.HasValue)
            {
                _targetDateTime = TargetDate.SelectedDate.Value;
                _client.TargetDate = _targetDateTime.ToString("yyyy-MM-dd");
                AppendConsoleMessage($"Target date set to {_targetDateTime:yyyy-MM-dd}");
            }
        }

        private void TargetClassName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _client.TargetClassName = TargetClassName.Text;
            AppendConsoleMessage($"Target class set to '{TargetClassName.Text}'");
        }

        private string FormatDate(DateTime date)
        {
            string dayOfWeek = date.ToString("dddd");
            int day = date.Day;
            string ordinal = GetOrdinalSuffix(day);
            string month = date.ToString("MMM");
            return $"{dayOfWeek} {day}{ordinal} {month}";
        }

        private string GetOrdinalSuffix(int day)
        {
            if (day >= 11 && day <= 13)
                return "th";

            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _client?.Dispose();
        }

        private void AppendConsoleMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string formattedMessage = $"[{timestamp}] {message}\n";
            ConsoleOutput.AppendText(formattedMessage);
            // Set caret to end to trigger auto-scroll
            ConsoleOutput.CaretIndex = ConsoleOutput.Text.Length;
            ConsoleOutput.ScrollToEnd();
        }

        private void ConsoleOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Auto-scroll to bottom when text changes
            ConsoleOutput.CaretIndex = ConsoleOutput.Text.Length;
            ConsoleOutput.ScrollToEnd();
        }

        public async void Login()
        {
            AppendConsoleMessage("Starting login process...");

            // IMPORTANT: Use secure credential storage in production!
            string username = "BET7019938";
            string password = "Newcastle92!"; // Use your NEW password after changing it

            // Set default target date if not already set via SetTargetClass
            if (string.IsNullOrEmpty(_client.TargetDate))
            {
                _client.TargetDate = _targetDateTime.ToString("yyyy-MM-dd");
            }

            var result = await _client.LoginAsync(username, password, (status) =>
            {
                Dispatcher.Invoke(() => AppendConsoleMessage(status));
            });

            if (!result)
            {
                AppendConsoleMessage("Login process completed with errors");
            }
        }

    }
}