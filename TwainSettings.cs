using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TwainHelper
{
    public partial class TwainSettings : Form
    {
        public TwainSettings()
        {
            InitializeComponent();
            
            if (Properties.Settings.Default.WindowTitleToWatch != "")
            {
                textBox1.Text = Properties.Settings.Default.WindowTitleToWatch;
            }

        }

        private void save_button_Click(object sender, System.EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Properties.Settings.Default.WindowTitleToWatch = textBox1.Text;
            }

            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void cancel_button_Click(object sender, System.EventArgs e)
        {
            this.Dispose();
        }

    }
}
