using System;
using System.Windows;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using System.Threading.Tasks;

namespace EvoxtaADReset
{
    public partial class MainWindow : Window
    {
        private ADUser? currentUser;
        private bool passwordVisible = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadDomain();
            CheckForUpdates();
        }

        void LoadDomain()
        {
            try
            {
                var domain = Domain.GetComputerDomain();
                DomainText.Text = $"Domain: {domain.Name}";
            }
            catch
            {
                DomainText.Text = "Domain: Unknown";
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            var help = new HelpWindow();
            help.Owner = this;
            help.ShowDialog();
        }

        async void CheckForUpdates()
        {
            try
            {
                HttpClient client = new HttpClient();

                var latest = await client.GetStringAsync(AppInfo.LatestVersionURL);
                latest = latest.Trim();

                if (latest != AppInfo.Version)
                {
                    var updateWindow = new UpdateWindow(latest);
                    updateWindow.Owner = this;
                    updateWindow.ShowDialog();
                }
            }
            catch
            {
                // silent failure
            }
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                PasswordVisible.Text = PasswordHidden.Password;
                PasswordHidden.Visibility = Visibility.Collapsed;
                PasswordVisible.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordHidden.Password = PasswordVisible.Text;
                PasswordHidden.Visibility = Visibility.Visible;
                PasswordVisible.Visibility = Visibility.Collapsed;
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var password = PasswordGenerator.Generate(16);

            PasswordHidden.Password = password;
            PasswordVisible.Text = password;
        }

private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(UsernameBox.Text))
        return;

    try
    {
        currentUser = ADHelper.GetUser(UsernameBox.Text);

        if (currentUser == null)
        {
            MessageBox.Show(
                "The specified user could not be found in Active Directory.",
                "User Not Found",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        FirstNameText.Text = $"First Name: {currentUser.FirstName}";
        LastNameText.Text = $"Last Name: {currentUser.LastName}";
        DisplayNameText.Text = $"Display Name: {currentUser.DisplayName}";
        LogonNameText.Text = $"Logon Name: {currentUser.SamAccountName}";
        AccountStatusText.Text = $"Status: {(currentUser.Enabled ? "Enabled" : "Disabled")}";

        DisableCheck.IsChecked = !currentUser.Enabled;
    }
    catch (System.DirectoryServices.ActiveDirectory.ActiveDirectoryObjectNotFoundException)
    {
        MessageBox.Show(
            "This computer is not connected to an Active Directory domain.\n\nPassword Control requires domain connectivity to query users.",
            "Domain Not Available",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"An unexpected error occurred while querying Active Directory.\n\n{ex.Message}",
            "Active Directory Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("No user loaded.");
                return;
            }

            string password = passwordVisible
                ? PasswordVisible.Text
                : PasswordHidden.Password;

            if (!string.IsNullOrEmpty(password))
                ADHelper.ResetPassword(currentUser.SamAccountName, password);

            if (DisableCheck.IsChecked == true)
                ADHelper.DisableAccount(currentUser.SamAccountName);

            if (ResetCheck.IsChecked == true)
                ADHelper.ForcePasswordReset(currentUser.SamAccountName);

            MessageBox.Show("Changes applied.");
        }
    }
}