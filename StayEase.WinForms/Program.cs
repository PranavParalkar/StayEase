namespace StayEase.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Login → Dashboard loop (supports logout and re-login)
            while (true)
            {
                using var loginForm = new frmLogin();
                var loginResult = loginForm.ShowDialog();

                if (loginResult != DialogResult.OK || loginForm.LoggedInAccount == null)
                    break;

                using var mainForm = new frmMain(loginForm.LoggedInAccount);
                var mainResult = mainForm.ShowDialog();

                // If user chose logout (DialogResult.Retry), loop back to login
                if (mainResult != DialogResult.Retry)
                    break;
            }
        }
    }
}