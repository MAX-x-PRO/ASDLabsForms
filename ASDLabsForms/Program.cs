using System;
using System.Windows.Forms;
using ASDLabsForms.Labs;
using ASDLabsForms.Labs.ASD_Labs;

namespace ASDLabsForms
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
            Application.Run(new Lab3Form());
            Application.Run(new Lab4Form());
            // Application.Run(new Lab5Form());
            Application.Run(new Lab6Form());
        }
    }
}
