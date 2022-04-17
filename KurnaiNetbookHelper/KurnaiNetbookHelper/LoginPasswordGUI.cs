using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace KurnaiNetbookHelper
{
    public partial class LoginPasswordGUI : Form
    {
        private MainGUI _Parent;
        private Constants _Constants = new Constants();

        public LoginPasswordGUI(MainGUI parent)
        {
            _Parent = parent;
            InitializeComponent();
        }

        private void LoginPassword_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            this.ActiveControl = passwordTextBox;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // Hashing code sourced from http://support.microsoft.com/kb/307020
            string sSourceData = passwordTextBox.Text;
            byte[] tmpSource;
            byte[] tmpHash;
            byte[] tmpHash2;

            // Create a byte array from source data.
            tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);

            // Compute hash based on source data.
            tmpHash = new SHA1CryptoServiceProvider().ComputeHash(tmpSource);
            tmpHash = new SHA1CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(Environment.UserName + ByteArrayToString(tmpHash)));
            tmpHash2 = new SHA1CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(Environment.UserName + _Constants.COMMON_INFO["LoginPasswordHash"]));

            // Check if password is correct.
            if (ByteArrayToString(tmpHash) == ByteArrayToString(tmpHash2))
            {
                // Password matched!
                // Save Tech state to registry.

                // Save password to ini file
                IniFile ini = new IniFile();

                string iniFile = _Constants.USER_INI_FILE;

                if (System.IO.File.Exists(iniFile))
                {
                    ini.Load(iniFile);
                }
                else
                {
                    ini.AddSection(Environment.UserName);
                    ini.SetKeyValue(Environment.UserName, "RequesterName", "");
                    ini.SetKeyValue(Environment.UserName, "Email", "");
                }

                ini.SetKeyValue(Environment.UserName, "Code", ByteArrayToString(tmpHash));
                ini.Save(iniFile);

                // Enable Tech controls.
                _Parent.EnableTech();

                // Close this GUI.
                Close();
            }
            else
            {
                MessageBox.Show(this, "The password entered is incorrect. Please try again.",
                        "Password Error"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            }
        }

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        
    }
}
