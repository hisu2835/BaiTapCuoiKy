using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Show login form first
            var loginForm = new LoginForm();
            bool loginSuccessful = false;
            string loggedInUser = "";

            loginForm.LoginSuccessful += (username) =>
            {
                loggedInUser = username;
                loginSuccessful = true;
                loginForm.Close(); // Close the login form to exit the Application.Run loop
            };

            // Run the login form
            Application.Run(loginForm);

            // If login was successful, open the main game form
            if (loginSuccessful && !string.IsNullOrEmpty(loggedInUser))
            {
                var gameForm = new Form1(loggedInUser);
                Application.Run(gameForm);
            }
        }
    }
}
