using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace EvoxtaADReset
{
    public partial class UpdateWindow : Window
    {
        private readonly string newVersion;

        public UpdateWindow(string version)
        {
            InitializeComponent();
            newVersion = version;
            VersionText.Text = $"Version: {version}";
            LoadUpdateNotes();
        }

        private async void LoadUpdateNotes()
        {
            try
            {
                using HttpClient client = new HttpClient();

                string html = await client.GetStringAsync(
                    AppInfo.UpdateNotesURL(newVersion));

                UpdateNotesBrowser.NavigateToString(html);
            }
            catch (Exception ex)
            {
                UpdateNotesBrowser.NavigateToString(
                    $"<html><body style='font-family:Segoe UI; background:#0f172a; color:#f8fafc;'>" +
                    $"<h3>Unable to load update notes.</h3><p>{System.Net.WebUtility.HtmlEncode(ex.Message)}</p>" +
                    $"</body></html>");
            }
        }

        private async void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNowButton.IsEnabled = false;
                LaterButton.IsEnabled = false;
                StatusText.Text = "Downloading update...";

                using HttpClient client = new HttpClient();

                string tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"EvoxtaADReset_{newVersion}.exe");

                byte[] data = await client.GetByteArrayAsync(
                    AppInfo.UpdateFileURL(newVersion));

                await File.WriteAllBytesAsync(tempFile, data);

                string currentExePath = Process.GetCurrentProcess().MainModule?.FileName
                    ?? throw new InvalidOperationException("Could not determine current executable path.");

                string updaterPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "EvoxtaUpdater.exe");

                if (!File.Exists(updaterPath))
                    throw new FileNotFoundException("EvoxtaUpdater.exe was not found next to Password Control.", updaterPath);

                StatusText.Text = "Installing update and restarting...";

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = $"\"{currentExePath}\" \"{tempFile}\"",
                    UseShellExecute = true
                });

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                StatusText.Text = "Update failed.";
                MessageBox.Show(
                    $"The update could not be installed.\n\n{ex.Message}",
                    "Update Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                UpdateNowButton.IsEnabled = true;
                LaterButton.IsEnabled = true;
            }
        }

        private void Later_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}