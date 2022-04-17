using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class LWTUsernameGUI : Form
    {
        private Validate _Validate = new Validate();
        private Constants _Constants = new Constants();

        public LWTUsernameGUI()
        {
            InitializeComponent();
        }

        private void LWTUsernameGUI_Load(object sender, EventArgs e)
        {
            this.ActiveControl = usernameTextBox;
            this.CenterToParent();
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_Validate.IsAlphaNum(usernameTextBox.Text))
            {
                okButton.Enabled = true;
                usernameTextBox.ForeColor = Color.DarkGreen;
            }
            else
            {
                okButton.Enabled = false;
                usernameTextBox.ForeColor = Color.DarkRed;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            IniFile ini = new IniFile();
            
            ini.Load(_Constants.USER_INI_FILE);
            ini.SetKeyValue(Environment.UserName, "LWTUsername", usernameTextBox.Text);
            ini.Save(_Constants.USER_INI_FILE);

            this.Close();
        }
    }
}
