using System;
using System.Diagnostics;
using System.Windows.Forms;
using TwainHelper.Properties;
using System.Drawing;

namespace TwainHelper
{
	/// <summary>
	/// 
	/// </summary>
	class ContextMenus
	{

        /// <summary>
        /// Is the About box displayed?
        /// </summary>
        bool isSettingsLoaded = false;

        // Add the default menu options.
        ContextMenuStrip menu = new ContextMenuStrip();
        ToolStripMenuItem item;
        ToolStripMenuItem item2;
        ToolStripMenuItem item3;
        ToolStripSeparator sep;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create()
		{			
            // Folder to Watch.
            item = new ToolStripMenuItem();
			item.Text = "Settings";
			item.Click += new EventHandler(WindowTitletoWatch_Click);
			item.Image = Resources.search_icon;
			menu.Items.Add(item);

			// Run at Logon.
			item2 = new ToolStripMenuItem();
			item2.Text = "Run At Startup";
            if (Properties.Settings.Default.RunAtStartup == true)
            {
                item2.Checked = true;
            }
            item2.Click += new EventHandler(RunAtStartup_Click);            
            //item.Image = Resources.About;
            menu.Items.Add(item2);

			// Separator.
			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			// Exit.
			item3 = new ToolStripMenuItem();
			item3.Text = "Exit";
			item3.Click += new System.EventHandler(Exit_Click);
			//item3.Image = Resources.Exit;
			menu.Items.Add(item3);

			return menu;
		}

		/// <summary>
		/// Handles the Click event of the Explorer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void WindowTitletoWatch_Click(object sender, EventArgs e)
		{
            if (!isSettingsLoaded)
            {
                isSettingsLoaded = true;
                new TwainSettings().ShowDialog();
                isSettingsLoaded = false;
            }           
		}

		/// <summary>
		/// Handles the Click event of the About control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void RunAtStartup_Click(object sender, EventArgs e)
		{            
            bool RunAtStartup = Properties.Settings.Default.RunAtStartup;
            if (RunAtStartup == true)
            {
                Properties.Settings.Default.RunAtStartup = false;
                Properties.Settings.Default.Save();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("TwainHelper", false);
                item2.Checked = false;
            }
            else if (RunAtStartup == false)
            {
                Properties.Settings.Default.RunAtStartup = true;
                Properties.Settings.Default.Save();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("TwainHelper", Application.ExecutablePath);
                item2.Checked = true;
            }                     
		}

		/// <summary>
		/// Processes a menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Exit_Click(object sender, EventArgs e)
		{
			// Quit without further ado.
			Application.Exit();
		}
	}
}