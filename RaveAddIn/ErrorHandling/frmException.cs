using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RaveAddIn.ErrorHandling
{
    public partial class frmException : Form
    {
        private Boolean _bDetailsExpanded = true;
        private string _sFormattedException;
        private string _sType;
        private string _NewIssueURL;

        public frmException(string sType, string sFormattedException, string newIssueURL)
        {
            _sType = sType;
            _sFormattedException = sFormattedException;
            _NewIssueURL = newIssueURL;
            InitializeComponent();

            if (string.IsNullOrEmpty(newIssueURL))
            {
                cmdSend.Visible = false;
                txtErrorMessage.Dock = DockStyle.Fill;
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            ChangeFormState(!_bDetailsExpanded);
        }

        private void ChangeFormState(bool bExpand)
        {
            if (bExpand)
            {
                Height = 400;
                _bDetailsExpanded = true;
                btnDetails.Text = "Details <<";
            }
            else
            {
                Height = this.MinimumSize.Height;
                _bDetailsExpanded = false;
                btnDetails.Text = "Details >>";
            }
        }

        private void ExceptionForm_Load(object sender, EventArgs e)
        {
            lblException.Text = _sType;
            txtErrorMessage.Text = _sFormattedException;
            int iWidth = lblException.Width + btnOK.Width + 200;
            if (iWidth < MinimumSize.Width)
            {
                Width = MinimumSize.Width;
            }
            else if (iWidth > Width)
            {
                Width = iWidth;
            }

            ChangeFormState(false);
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtErrorMessage.Text, false, 5, 200);
            MessageBox.Show("The error information has been copied to the clipboard." +
                " You will now be redirected to the web site where you can review existing issues" +
                " and log a new issue if your issue is not already being tracked.", cmdSend.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start(_NewIssueURL);
        }

        /// <summary>
        /// Handle exception in user interface tools
        /// </summary>
        /// <param name="ex">Generic Exception</param>
        /// <param name="UIMessage">Optional main user interface message for form</param>
        /// <remarks></remarks>
        public static void HandleException(Exception ex, string UIMessage, string newIssueURL)
        {
            string formattedException = ExceptionBase.GetExceptionInformation(ex);
            formattedException += Environment.NewLine + "Windows: " + Environment.OSVersion;
            formattedException += Environment.NewLine + "Date: " + DateTime.Now.ToString();
            //
            // Ensure the wait cursor is reverted back to the default cursor before showing any message box
            //
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

            //if the exception has a UIMessage parameter, it overrides the optional UIMessage passed to the method
            if (ex.Data.Contains("UIMessage"))
            {
                UIMessage = ex.Data["UIMessage"].ToString();
            }

            if (string.IsNullOrEmpty(UIMessage))
                UIMessage = ex.Message;

            Debug.WriteLine(formattedException);
            Debug.WriteLine(DateTime.Now);
            frmException myFrm = new frmException(UIMessage, formattedException, newIssueURL);
            myFrm.ShowDialog();
        }
    }
}
