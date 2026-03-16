using System.DirectoryServices.AccountManagement;

namespace EvoxtaADReset
{
    public class ADUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string SamAccountName { get; set; } = "";
        public bool Enabled { get; set; }
    }

    public static class ADHelper
    {
        public static ADUser? GetUser(string username)
{
    using var context = new PrincipalContext(ContextType.Domain);
    var user = UserPrincipal.FindByIdentity(context, username);

    if (user == null)
        return null;

    return new ADUser
    {
        FirstName = user.GivenName ?? "",
        LastName = user.Surname ?? "",
        DisplayName = user.DisplayName ?? "",
        SamAccountName = user.SamAccountName ?? "",
        Enabled = user.Enabled ?? false
    };
}

        public static void ResetPassword(string username, string newPassword)
        {
            using var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, username);

            if (user == null)
                return;

            user.SetPassword(newPassword);
            user.Save();
        }

        public static void DisableAccount(string username)
        {
            using var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, username);

            if (user == null)
                return;

            user.Enabled = false;
            user.Save();
        }

public static void EnableAccount(string username)
{
    using var context = new PrincipalContext(ContextType.Domain);
    var user = UserPrincipal.FindByIdentity(context, username);

    if (user == null)
        return;

    user.Enabled = true;
    user.Save();
}

        public static void ForcePasswordReset(string username)
        {
            using var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, username);

            if (user == null)
                return;

            user.ExpirePasswordNow();
            user.Save();
        }
    }

    public static class PasswordGenerator
    {
        public static string Generate(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new System.Random();

            char[] buffer = new char[length];

            for (int i = 0; i < length; i++)
                buffer[i] = chars[random.Next(chars.Length)];

            return new string(buffer);
        }
    }
}
