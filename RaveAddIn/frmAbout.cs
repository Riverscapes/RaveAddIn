using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaveAddIn
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            Text = string.Format("About the {0}", Properties.Resources.ApplicationNameShort);
            lblProductName.Text = Properties.Resources.ApplicationNameLong;
            webBrowser1.Url = new Uri("http://rave.riverscapes.xyz/dotnetack.html");
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void lnkWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(lnkWebSite.Text);
        }

        private void lnkOnlineHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(lnkOnlineHelp.Text);
        }

        private void lnkIssues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(lnkIssues.Text);
        }
    }
}
