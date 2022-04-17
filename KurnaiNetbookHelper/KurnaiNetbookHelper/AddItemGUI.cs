using System;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class AddItemGUI : Form
    {
        private AddJobGUI _Parent;
        private Validate _Validate = new Validate();
        private Constants _Constants = new Constants();

        private bool _ItemCodeValid = false;
        private bool _DescriptionValid = false;

        public AddItemGUI(AddJobGUI parent)
        {
            InitializeComponent();

            _Parent = parent;
        }

        private void AddItemGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();

            glComboBox.SelectedItem = "86402 - Computer Repairs";

            if (!_Parent.customCheckBox.Checked)
                glComboBox.Enabled = false;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            System.Drawing.Image image;
            float price = 0.0f;

            if (System.IO.File.Exists(_Constants.DATA_DIR + @"Images\" + itemCodeTextBox.Text + ".jpg"))
                image = System.Drawing.Image.FromFile(_Constants.DATA_DIR + @"Images\" + itemCodeTextBox.Text + ".jpg");
            else
            {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream myStream = myAssembly.GetManifestResourceStream("KurnaiNetbookHelper.Resources.not_available.jpg");
                image = new System.Drawing.Bitmap(myStream);
            }

            // Lookup current item price.
            try
            {
                string sURL = String.Format(_Constants.LWT_PRICE_LOOKUP_URL, Uri.EscapeDataString(itemCodeTextBox.Text));

                System.Net.WebProxy webProxy = new System.Net.WebProxy(_Constants.COMMON_INFO["Proxy"]);
                webProxy.Credentials = new System.Net.NetworkCredential(_Constants.COMMON_INFO["ProxyUsername"], _Constants.COMMON_INFO["ProxyPassword"]);

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Proxy = webProxy;
                    string downloadString = client.DownloadString(sURL).Trim();

                    if (downloadString == "Website Under Maintenance - will be up shortly" || downloadString == "")
                    {
                        MessageBox.Show(this, "LWT's website is currently under maintenance. Item added at $0.00. Please try updating the lists again later to update the price.",
                            "LWT Under Maintenance",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    float.TryParse(downloadString, out price);
                }
            }
            catch (Exception) { }

            LWTItem item = new LWTItem(itemCodeTextBox.Text, descriptionTextBox.Text, price, 0, image);
            item.GLCode = glComboBox.SelectedItem.ToString().Substring(0, 5);

            _Parent._ItemToAdd = item;
        }

        private void itemCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_Validate.IsAlphaNum(itemCodeTextBox.Text))
            {
                _ItemCodeValid = true;
                itemCodeTextBox.ForeColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                _ItemCodeValid = false;
                itemCodeTextBox.ForeColor = System.Drawing.Color.DarkRed;
            }

            EnableAddButtonIfValid();
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_Validate.IsPhrase(descriptionTextBox.Text))
            {
                _DescriptionValid = true;
                descriptionTextBox.ForeColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                _DescriptionValid = false;
                descriptionTextBox.ForeColor = System.Drawing.Color.DarkRed;
            }

            EnableAddButtonIfValid();
        }

        private void EnableAddButtonIfValid()
        {
            if (_ItemCodeValid && _DescriptionValid)
                addButton.Enabled = true;
            else
                addButton.Enabled = false;
        }
    }
}
