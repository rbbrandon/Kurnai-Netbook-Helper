using System;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class EmailGUI : Form
    {
        private MainGUI _Parent;
        private Constants _Constants = new Constants();

        public EmailGUI(MainGUI parent)
        {
            InitializeComponent();
            _Parent = parent;
        }

        private void EmailGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            this.ActiveControl = emailTextBox;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            IniFile ini = new IniFile();
            ini.Load(_Constants.USER_INI_FILE);
            ini.SetKeyValue(Environment.UserName, "Email", emailTextBox.Text);
            ini.Save(_Constants.USER_INI_FILE);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {
            Validate validate = new Validate();

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (validate.IsEmail(emailTextBox.Text))
            {
                emailTextBox.ForeColor = System.Drawing.Color.DarkGreen;
                toolTip.SetToolTip(emailTextBox, "");
                saveButton.Enabled = true;
                //IniFile iniFile = new IniFile();
                //iniFile.
            }
            else
            {
                emailTextBox.ForeColor = System.Drawing.Color.DarkRed;
                toolTip.SetToolTip(emailTextBox, "You must enter a valid \"*@edumail.vic.gov.au\" email address.");
                saveButton.Enabled = false;
            }
        }
    }
}
