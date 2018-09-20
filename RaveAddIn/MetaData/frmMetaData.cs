using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace RaveAddIn.MetaData
{
    public partial class frmMetaData : Form
    {
        public readonly BindingList<MetaDataItem> MetaDataItems;

        public frmMetaData(string noun, XmlNode nodMetaData)
        {
            InitializeComponent();

            grdData.AutoGenerateColumns = false;
            grdData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Text = string.Format("{0} Meta Data", noun);

            MetaDataItems = new BindingList<MetaDataItem>();

            foreach (XmlNode nodItem in nodMetaData.SelectNodes("Meta"))
            {
                XmlAttribute att = nodItem.Attributes["name"];
                if (att is XmlAttribute)
                {
                    MetaDataItems.Add(new MetaDataItem(att.InnerText, nodItem.InnerText));
                }
            }

            grdData.DataSource = MetaDataItems;
        }

        private void frmMetaData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
                DialogResult = DialogResult.OK;
        }
    }
}
