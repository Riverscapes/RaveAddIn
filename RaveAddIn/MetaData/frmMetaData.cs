using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace RaveAddIn.MetaData
{
    public partial class frmMetaData : Form
    {
        public BindingList<MetaDataItem> MetaDataItems { get; private set; }

        private void Init(string noun)
        {
            InitializeComponent();

            grdData.AutoGenerateColumns = false;
            grdData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Text = string.Format("{0} Meta Data", noun);

            MetaDataItems = new BindingList<MetaDataItem>();
            grdData.DataSource = MetaDataItems;
        }

        public frmMetaData(string noun, XmlNode nodMetaData)
        {
            Init(noun);

            foreach (XmlNode nodItem in nodMetaData.SelectNodes("Meta"))
            {
                XmlAttribute att = nodItem.Attributes["name"];
                if (att is XmlAttribute)
                {
                    MetaDataItems.Add(new MetaDataItem(att.InnerText, nodItem.InnerText));
                }
            }

        }

        public frmMetaData(string noun, Dictionary<string, string> metadata)
        {
            Init(noun);

            foreach (KeyValuePair<string, string> items in metadata)
            {
                MetaDataItems.Add(new MetaDataItem(items.Key, items.Value));
            }
        }

        private void frmMetaData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
                DialogResult = DialogResult.OK;
        }
    }
}
