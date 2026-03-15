using System.Net.Http;
using System.Windows;

namespace EvoxtaADReset
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            VersionText.Text = $"Version: {AppInfo.Version}";
            CheckUpdates();
        }

        async void CheckUpdates()
        {
            try
            {
                HttpClient client = new HttpClient();

                var latest = await client.GetStringAsync(AppInfo.LatestVersionURL);
                latest = latest.Trim();

                if (latest == AppInfo.Version)
                {
                    UpdateStatusText.Text = "You are running the latest version.";
                    UpdateButton.Content = "Check Again";
                }
                else
                {
                    UpdateStatusText.Text = $"Update available: {latest}";
                    UpdateButton.Content = "Update Now";

                    UpdateButton.Click += (s, e) =>
                    {
                        var updateWindow = new UpdateWindow(latest);
                        updateWindow.Owner = this;
                        updateWindow.ShowDialog();
                    };
                }
            }
            catch
            {
                UpdateStatusText.Text = "Unable to check for updates.";
            }
        }

        private void CheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            CheckUpdates();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}