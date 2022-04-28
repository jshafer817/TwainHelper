using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Timers;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Win32;
using System.Timers;

namespace TwainHelper
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        static System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        static DateTime FileReadTime;

        static IntPtr OldchildWindowHandle;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FileReadTime = DateTime.Now;
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.Sleep(1000);
            //Detect if running
            if (Process.GetProcessesByName("TwainHelper").Length > 1)
            {
                MessageBox.Show("Program already running.");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            // Show the system tray icon.					
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                if (Properties.Settings.Default.FirstRun == true)
                {
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.Save();
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.SetValue("TwainHelper", Application.ExecutablePath);
                    bool isSettingsLoaded = false;
                    if (!isSettingsLoaded)
                    {
                        isSettingsLoaded = true;
                        new TwainSettings().ShowDialog();
                        isSettingsLoaded = false;
                    }
                }

                WindowTimer();

                // Make sure the application runs!                          
                Application.Run();
            }
        }

        static public void WindowTimer()
        {
            //Pointer to the Window Timer
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(ActivateWindow);
            aTimer.AutoReset = true;
            aTimer.Interval = 1000;
            aTimer.Start();
        }

        public static void ActivateWindow(Object myObject, EventArgs myEventArgs)
        {
            if (Properties.Settings.Default.WindowTitleToWatch == "")
                return;

            IntPtr childWindowHandle = FindWindowByCaption(IntPtr.Zero, Properties.Settings.Default.WindowTitleToWatch);

            if (OldchildWindowHandle == childWindowHandle)
                return;
        

            // Guard: check if window already has focus.
            if (childWindowHandle == GetForegroundWindow()) return;
            // Show window maximized.

            AllocConsole();
            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
            SetActiveWindow(Process.GetCurrentProcess().MainWindowHandle);
            FreeConsole();            

            //ShowWindow(childWindowHandle, SHOW_MAXIMIZED);
            SetForegroundWindow(childWindowHandle);
            SetActiveWindow(childWindowHandle);

            OldchildWindowHandle = childWindowHandle;
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                using (File.Open(file.FullName, FileMode.Open)) { }

                stream = file.Open
                (
                    FileMode.Open,
                    System.IO.FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}