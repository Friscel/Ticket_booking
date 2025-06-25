using TicketBookingWinForms;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Show login form
        using (var loginForm = new LoginForm())
        {
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Show main form if login successful
                Application.Run(new MainForm(loginForm.IsAdmin, loginForm.CurrentUsername));
            }
        }
    }
}