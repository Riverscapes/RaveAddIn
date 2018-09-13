using System;
using System.Diagnostics;
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
            Text = string.Format("About {0}", Properties.Resources.ApplicationNameShort);
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
