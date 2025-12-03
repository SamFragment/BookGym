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

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;
            Login();
        }

        public void SetTargetClass(string className, DateTime dateTime)
        {
            Dispatcher.Invoke(() =>
            {
                TargetClassName.Text = className;
                TargetDate.Text = FormatDate(dateTime);
                TargetTime.Text = dateTime.ToString("h:mm tt");
            });
        }

        public void SetTargetClass(string className, string date, string time)
        {
            Dispatcher.Invoke(() =>
            {
                TargetClassName.Text = className;
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    TargetDate.Text = FormatDate(parsedDate);
                }
                else
                {
                    TargetDate.Text = date;
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