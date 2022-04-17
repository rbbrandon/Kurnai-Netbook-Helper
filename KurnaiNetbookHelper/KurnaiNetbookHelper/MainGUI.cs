using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using CredentialManagement;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;

namespace KurnaiNetbookHelper
{
    /// <summary>
    ///     The Main GUI/Program.
    /// </summary>
    public partial class MainGUI : Form
    {
        #region Global Variable Declarations
        // Create collections to store the jobs in.
        private List<Job> _RepairJobList = new List<Job>();
        private List<Job> _CollectJobList = new List<Job>();

        private IniFile _Ini = new IniFile();
        private Constants _Constants = new Constants();
        internal Credential _NetworkCredentials = null;
        private ContextMenuStrip _ListboxContextMenu = new ContextMenuStrip();

        // The location of the Jobs files.
        //private string _MorwellTxtDir = "";
        //private string _ChurchillTxtDir = "";
        //private string _GepTxtDir = "";
        private string _JobTXTDir = "";
        private string _JobTXT = "";
        internal string _DomainController = "";

        //public string Campus { get; set; }
        #endregion

        public MainGUI()
        {
            InitializeComponent();

            //_MorwellTxtDir = @"\\" + _Constants.MORWELL_SERVER + @"\" + _Constants.MORWELL_SHARE + @"\";
            //_ChurchillTxtDir = @"\\" + _Constants.CHURCHILL_SERVER + @"\" + _Constants.CHURCHILL_SHARE + @"\";
            //_GepTxtDir = @"\\" + _Constants.GEP_SERVER + @"\" + _Constants.GEP_SHARE + @"\";

            // Tell windows we are interested in drawing items in ListBox on our own.
            this.repairListBox.DrawItem += new DrawItemEventHandler(this.RepairDrawItemHandler);
            this.collectionListBox.DrawItem += new DrawItemEventHandler(this.CollectDrawItemHandler);

            // Register events for right clicking listbox items.
            this.repairListBox.MouseDown += new MouseEventHandler(repairListBox_MouseDown);
            this.collectionListBox.MouseDown += new MouseEventHandler(collectionListBox_MouseDown);

            this.repairListBox.MouseDoubleClick += new MouseEventHandler(repairListBox_MouseDoubleClick);
            this.collectionListBox.MouseDoubleClick += new MouseEventHandler(collectionListBox_MouseDoubleClick);
        }

        private void MainGUI_Load(object sender, EventArgs e)
        {
            // Create Directories, etc.
            if (!Directory.Exists(_Constants.DATA_DIR))
                Directory.CreateDirectory(_Constants.DATA_DIR);

            if (!Directory.Exists(_Constants.DATA_DIR + "Images"))
                Directory.CreateDirectory(_Constants.DATA_DIR + "Images");

            if (!Directory.Exists(_Constants.DATA_DIR + "Generated Purchase Orders"))
                Directory.CreateDirectory(_Constants.DATA_DIR + "Generated Purchase Orders");

            for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                if (!Directory.Exists(_Constants.DATA_DIR + @"Generated Purchase Orders\" + _Constants.CAMPUS_INFO[i]["Name"]))
                    Directory.CreateDirectory(_Constants.DATA_DIR + @"Generated Purchase Orders\" + _Constants.CAMPUS_INFO[i]["Name"]);

            if (!File.Exists(_Constants.USER_INI_FILE))
                File.CreateText(_Constants.USER_INI_FILE);

            for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                campusComboBox.Items.Add(_Constants.CAMPUS_INFO[i]["Name"]);

            GetCampusLocation();

            // If the IP doesn't match any of the above, then close (program cannot be run outside of the school).
            if (_JobTXT == "")
            {
                string errorMessage = "Cannot detect the network. This app requires connection to ";
                string prefix = "";//Morwell, Churchill, Precinct campuses of Kurnai College to operate.\nThis app will now close.";
                for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                {
                    errorMessage += prefix + _Constants.CAMPUS_INFO[i]["Name"];
                    prefix = ", ";
                }
                errorMessage += " campuses of " + _Constants.COMMON_INFO["CompanyName"] + " to operate.\nThis app will now close.";

                MessageBox.Show(this, errorMessage,
                    "Unable to Detect Network.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }

            string iniFile = _Constants.USER_INI_FILE;

            if (File.Exists(iniFile))
            {
                _Ini.Load(iniFile);
            }
            else
            {
                _Ini.SetKeyValue(Environment.UserName, "RequesterName", "");
                _Ini.SetKeyValue(Environment.UserName, "Email", "");
                _Ini.SetKeyValue(Environment.UserName, "Code", "");
                _Ini.Save(iniFile);
            }
            
            // On GUI load, enable Technician controls if the user is a tech.
            if (_Ini.GetKeyValue(Environment.UserName, "Email") == "")
            {
                using (EmailGUI emailGUI = new EmailGUI(this))
                {
                    if (emailGUI.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                    {
                        Close();
                    }
                }
            }

            // Check is user is not on the domain.
            if (Environment.UserDomainName.ToUpper() != _Constants.COMMON_INFO["Domain"].ToUpper())
            {
                // User is not part of the KURNAI domain.
                // Get credentials from credential store (if any).
                _NetworkCredentials = GetCredentials(_Constants.PROGRAM_NAME);

                bool credsValid = false;

                while (credsValid == false)
                {
                    // Check if credentials were recieved.
                    if (_NetworkCredentials == null)
                    {

                        // No credentials found, so prompt user for domain credentials.
                        using (VistaPrompt vista = new VistaPrompt())
                        {
                            vista.Title = "Domain credentials required";
                            vista.Message = "Please enter credentials to connect to the " + _Constants.COMMON_INFO["Domain"].ToUpper() + " domain.";
                            vista.Domain = _Constants.COMMON_INFO["Domain"].ToUpper();
                            vista.Username = Environment.UserName;
                            vista.ShowSaveCheckBox = true;
                            vista.SaveChecked = true;

                            if (vista.ShowDialog() == CredentialManagement.DialogResult.OK)
                            {
                                _NetworkCredentials = new Credential(vista.Username, vista.Password, _Constants.PROGRAM_NAME);
                                _NetworkCredentials.PersistanceType = PersistanceType.Enterprise;

                                AD ad = new AD();
                                ad.Server = _DomainController;
                                credsValid = ad.Authenticate(_NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                                    _NetworkCredentials.Password, _Constants.COMMON_INFO["LDAPDomain"]);

                                if (credsValid && vista.SaveChecked)
                                    SetCredentials(_NetworkCredentials);
                            }
                            else
                            {
                                MessageBox.Show(this, "Since this program integrates with Active Directory, domain credentials are necessary for correct operation.\n" +
                                    "This program will now close.",
                                    "User Credentials are Required.",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Credential were found- Check if still valid:
                        AD ad = new AD();
                        ad.Server = _DomainController;
                        credsValid = ad.Authenticate(_NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                            _NetworkCredentials.Password, _Constants.COMMON_INFO["LDAPDomain"]);

                        if (credsValid)
                            SetCredentials(_NetworkCredentials);
                        else
                        {
                            using (VistaPrompt vista = new VistaPrompt())
                            {
                                vista.Title = "Domain credentials required";
                                vista.Message = "Please enter credentials to connect to the " + _Constants.COMMON_INFO["Domain"].ToUpper() + " domain.";
                                vista.Domain = _Constants.COMMON_INFO["Domain"].ToUpper();
                                vista.Username = Environment.UserName;

                                if (vista.ShowDialog() == CredentialManagement.DialogResult.OK)
                                {
                                    _NetworkCredentials = new Credential(vista.Username, vista.Password, _Constants.PROGRAM_NAME);
                                    _NetworkCredentials.PersistanceType = PersistanceType.Enterprise;
                                }
                                else
                                {
                                    MessageBox.Show(this, "Since this program integrates with Active Directory, domain credentials are necessary for correct operation.\n" +
                                        "This program will now close.",
                                        "User Credentials are Required.",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    break;
                                }
                            }
                        }
                    }
                }


                // If credsValid is false, then user cancelled the credential input.
                // (using Close() inside the VistaPrompt using statement caused windows to freeze when debugging, hence why it is down here instead)
                if (!credsValid)
                    Close();
            }

            // Update the ListBoxes (to get initial data).
            this.UpdateListBoxes();

            // Enable Tech buttons if user is a tech.
            if (_Ini.GetKeyValue(Environment.UserName, "Code") == ByteArrayToString(new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
                System.Text.ASCIIEncoding.ASCII.GetBytes(Environment.UserName + _Constants.COMMON_INFO["LoginPasswordHash"]))))
            {
                EnableTech();
            }

            // Beta Note:
            //MessageBox.Show(this, "Please Note that this is a beta version of the app. Features may be missing or may not work correctly.\n\n" +
            //    "To-Do List: * Automatically notify Phyllis of job additions;\n" +
            //    //"                    * Change jobs file from Jobs2.txt to Jobs.txt. (Final Step)\n\n" +
            //    "Please contact Robert Brandon if you find any bugs.");

            timer.Enabled = true;
        }

        private void GetCampusLocation()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                    {
                        IPAddress ip2 = IPAddress.Parse(_Constants.CAMPUS_INFO[i]["IPNetworkAddress"]);
                        IPAddress mask = IPAddress.Parse(_Constants.CAMPUS_INFO[i]["IPSubnetMask"]);

                        if (ip.IsInSameSubnet(ip2, mask))
                        {
                            //Campus = _Constants.MORWELL;
                            campusComboBox.SelectedIndex = i;
                            _DomainController = _Constants.CAMPUS_INFO[i]["DomainController"];
                            _JobTXTDir = @"\\" + _Constants.CAMPUS_INFO[i]["FileServer"] + @"\" + _Constants.CAMPUS_INFO[i]["Share"] + @"\";
                            _JobTXT = _JobTXTDir + _Constants.JOB_TXT_FILE;
                        }

                        if (_JobTXT != "")
                            break;
                    }
                }

                if (_JobTXT != "")
                    break;
            }
        }

        #region Custom ListBox Drawing Methods
        private void RepairDrawItemHandler(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics graphics = e.Graphics;
            Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                            Brushes.Gainsboro : new SolidBrush(e.BackColor);
                graphics.FillRectangle(brush, e.Bounds);

            // This line prevents the program crashing when the listbox is empty (or redrawn).
            if (e.Index == -1)
                return;

            string itemText = _RepairJobList[e.Index].ToString();

            using (Font font = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular))
            {
                if (_RepairJobList[e.Index].AllowCollect == 2)
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Black), e.Bounds);
                else if (_RepairJobList[e.Index].AllowCollect == 1)
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Blue), e.Bounds);
                else
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Red), e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void CollectDrawItemHandler(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics graphics = e.Graphics;
            Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                            Brushes.Gainsboro : new SolidBrush(e.BackColor);
                graphics.FillRectangle(brush, e.Bounds);

            // This line prevents the program crashing when the listbox is empty (or redrawn).
            if (e.Index == -1)
                return;

            string itemText = _CollectJobList[e.Index].ToString();

            using (Font font = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular))
            {
                if (_CollectJobList[e.Index].AllowCollect == _Constants.PAYMENTS_VALUE_UNKNOWN)
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Black), e.Bounds);
                else if (_CollectJobList[e.Index].AllowCollect == _Constants.PAYMENTS_VALUE_ALLOW)
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Blue), e.Bounds);
                else
                    graphics.DrawString(itemText, font, new SolidBrush(Color.Red), e.Bounds);
            }

            e.DrawFocusRectangle();
        }
        #endregion

        #region ListBox Context Menu Code
        private void repairListBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if the mouse button clicked was the Right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                // Get the index of the item under the mouse's location.
                int indexOfItemUnderMouse = repairListBox.IndexFromPoint(e.Location);

                // Check if an item was found.
                if (indexOfItemUnderMouse != ListBox.NoMatches)
                {
                    repairListBox.SetSelected(indexOfItemUnderMouse, true);

                    // Get the selected item's box.
                    Rectangle rect = repairListBox.GetItemRectangle(indexOfItemUnderMouse);

                    // See if the mouse's location is within the item's box.
                    if (rect.Contains(e.Location))
                    {
                        // Clear the context menu.
                        _ListboxContextMenu.Items.Clear();

                        // Add item-specific information to the context menu.
                        _ListboxContextMenu.Items.Add(((Job)repairListBox.SelectedItem).StudentName);
                        _ListboxContextMenu.Items[0].Enabled = false;

                        _ListboxContextMenu.Items.Add("-");

                        _ListboxContextMenu.Items.Add(_Constants.PAYMENTS_OVERDUE);
                        if (((Job)repairListBox.SelectedItem).AllowCollect == _Constants.PAYMENTS_VALUE_DENY)
                            ((ToolStripMenuItem)_ListboxContextMenu.Items[2]).Checked = true;
                        _ListboxContextMenu.Items[2].Click += new EventHandler(RepairOverdueMenuItem_Click);

                        _ListboxContextMenu.Items.Add(_Constants.PAYMENTS_PAID);
                        if (((Job)repairListBox.SelectedItem).AllowCollect == _Constants.PAYMENTS_VALUE_ALLOW)
                            ((ToolStripMenuItem)_ListboxContextMenu.Items[3]).Checked = true;
                        _ListboxContextMenu.Items[3].Click += new EventHandler(RepairPaidMenuItem_Click);

                        // Display the context menu.
                        _ListboxContextMenu.Show(repairListBox, e.Location);
                    }
                }
            }
        }

        private void collectionListBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if the mouse button clicked was the Right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                // Get the index of the item under the mouse's location.
                int indexOfItemUnderMouse = collectionListBox.IndexFromPoint(e.Location);

                // Check if an item was found.
                if (indexOfItemUnderMouse != ListBox.NoMatches)
                {
                    collectionListBox.SetSelected(indexOfItemUnderMouse, true);

                    Rectangle rect = collectionListBox.GetItemRectangle(indexOfItemUnderMouse);

                    // See if the mouse's location is within the item's box.
                    if (rect.Contains(e.Location))
                    {
                        // Clear the context menu.
                        _ListboxContextMenu.Items.Clear();

                        // Add item-specific information to the context menu.
                        _ListboxContextMenu.Items.Add(((Job)collectionListBox.SelectedItem).StudentName);
                        _ListboxContextMenu.Items[0].Enabled = false;

                        _ListboxContextMenu.Items.Add("-");

                        _ListboxContextMenu.Items.Add(_Constants.PAYMENTS_OVERDUE);
                        if (((Job)collectionListBox.SelectedItem).AllowCollect == _Constants.PAYMENTS_VALUE_DENY)
                            ((ToolStripMenuItem)_ListboxContextMenu.Items[2]).Checked = true;
                        _ListboxContextMenu.Items[2].Click += new EventHandler(CollectOverdueMenuItem_Click);

                        _ListboxContextMenu.Items.Add(_Constants.PAYMENTS_PAID);
                        if (((Job)collectionListBox.SelectedItem).AllowCollect == _Constants.PAYMENTS_VALUE_ALLOW)
                            ((ToolStripMenuItem)_ListboxContextMenu.Items[3]).Checked = true;
                        _ListboxContextMenu.Items[3].Click += new EventHandler(CollectPaidMenuItem_Click);

                        // Display the context menu.
                        _ListboxContextMenu.Show(collectionListBox, e.Location);
                    }
                }
            }

            if (((Job)collectionListBox.SelectedItem).AllowCollect != 1)
                collectedButton.Enabled = false;
            else
                collectedButton.Enabled = true;
        }

        private void RepairOverdueMenuItem_Click(object sender, EventArgs e)
        {
            ((Job)repairListBox.SelectedItem).AllowCollect = 0;
            this.MoveRecord((Job)repairListBox.SelectedItem, _RepairJobList, _RepairJobList, false);
            repairListBox.Invalidate();
        }

        private void RepairPaidMenuItem_Click(object sender, EventArgs e)
        {
            ((Job)repairListBox.SelectedItem).AllowCollect = 1;
            this.MoveRecord((Job)repairListBox.SelectedItem, _RepairJobList, _RepairJobList, false);
            repairListBox.Invalidate();
        }

        private void CollectOverdueMenuItem_Click(object sender, EventArgs e)
        {
            ((Job)collectionListBox.SelectedItem).AllowCollect = 0;
            this.MoveRecord((Job)collectionListBox.SelectedItem, _CollectJobList, _CollectJobList, false);
            collectionListBox.Invalidate();
            collectedButton.Enabled = false;
        }

        private void CollectPaidMenuItem_Click(object sender, EventArgs e)
        {
            ((Job)collectionListBox.SelectedItem).AllowCollect = 1;
            this.MoveRecord((Job)collectionListBox.SelectedItem, _CollectJobList, _CollectJobList, false);
            collectionListBox.Invalidate();
            collectedButton.Enabled = true;
        }
        #endregion

        #region ListBox Edit
        void repairListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (addJobButton.Enabled == true)
            {
                int index = this.repairListBox.IndexFromPoint(e.Location);

                if (index != ListBox.NoMatches)
                {
                    using (EditJobGUI jobGUI = new EditJobGUI(this, (Job)repairListBox.Items[index], _JobTXT))
                    {
                        if (jobGUI.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            UpdateListBoxes();
                        }
                    }
                }
            }
        }

        void collectionListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (addJobButton.Enabled == true)
            {
                int index = this.collectionListBox.IndexFromPoint(e.Location);

                if (index != ListBox.NoMatches)
                {
                    using (EditJobGUI jobGUI = new EditJobGUI(this, (Job)collectionListBox.Items[index], _JobTXT))
                    {
                        if (jobGUI.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            UpdateListBoxes();
                        }
                    }
                }
            }
        }
        #endregion

        private void closeButton_Click(object sender, EventArgs e)
        {
            // Close this GUI (and therefore the program).
            Close();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (loginButton.Text == _Constants.LOGIN_TEXT)
            {
                // Prompt for login as a Tech.
                using (LoginPasswordGUI loginForm = new LoginPasswordGUI(this))
                {
                    loginForm.ShowDialog(this);
                }
            }
            else
            {
                _Ini.SetKeyValue(Environment.UserName, "Code", "");
                _Ini.Save(_Constants.USER_INI_FILE);
                this.DisableTech();
            }
        }

        /// <summary>
        ///     Enables the technician controls.
        /// </summary>
        public void EnableTech()
        {
            fixedButton.Enabled = true;
            brokenButton.Enabled = true;
            addJobButton.Enabled = true;
            loginButton.Text = _Constants.LOGOUT_TEXT;
        }

        /// <summary>
        ///     Disables the technician controls.
        /// </summary>
        public void DisableTech()
        {
            fixedButton.Enabled = false;
            brokenButton.Enabled = false;
            addJobButton.Enabled = false;
            loginButton.Text = _Constants.LOGIN_TEXT;
        }

        private void fixedButton_Click(object sender, EventArgs e)
        {
            // If no item is selected in the repair ListBox, do nothing,
            // otherwise:
            if (repairListBox.SelectedItem != null)
            {
                fixedButton.Text = "Moving...";
                fixedButton.Enabled = false;
                brokenButton.Enabled = false;

                Job job = repairListBox.SelectedItem as Job;

                // Move the currently selected job from the repair list and into the collect list.
                this.MoveRecord(job, _RepairJobList, _CollectJobList, true);
                
                // Disable student account
                AD ad = new AD();
                ad.Server = _DomainController;
                
                try
                {
                    string distinguishedName = null;

                    if (_NetworkCredentials == null)
                        distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                            _Constants.COMMON_INFO["LDAPDomain"]);
                    else
                        distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                            _Constants.COMMON_INFO["LDAPDomain"], _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                            _NetworkCredentials.Password);

                    ad.Disable(distinguishedName);
                }
                catch (NullReferenceException nre)
                {
                    MessageBox.Show(this, "Error: " + nre.Message,
                    "Unable to Disable Account.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
                

                // Email spiceworks that it's fixed
                this.EmailJob(String.Format(_Constants.EMAIL_SUBJECT_FORMAT, job.JobID, _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"], job.StudentName,
                    job.StudentID), _Constants.EMAIL_TEXT_FIXED);

                fixedButton.Text = "------->";
                fixedButton.Enabled = true;
                brokenButton.Enabled = true;
            }
        }

        private void brokenButton_Click(object sender, EventArgs e)
        {
            // If no item is selected in the collect ListBox, do nothing,
            // otherwise:
            if (collectionListBox.SelectedItem != null)
            {
                brokenButton.Text = "Moving...";
                fixedButton.Enabled = false;
                brokenButton.Enabled = false;
                Job job = collectionListBox.SelectedItem as Job;

                // Move the currently selected job from the collect list and into the repair list.
                this.MoveRecord(job, _CollectJobList, _RepairJobList, false);

                // Enable student account
                AD ad = new AD();
                ad.Server = _DomainController;
                try
                {
                    string distinguishedName = null;

                    if (_NetworkCredentials == null)
                        distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                            _Constants.COMMON_INFO["LDAPDomain"]);
                    else
                        distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                            _Constants.COMMON_INFO["LDAPDomain"], _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                            _NetworkCredentials.Password);

                    ad.Enable(distinguishedName);
                }
                catch (NullReferenceException nre)
                {
                    MessageBox.Show(this, "Error: " + nre.Message,
                    "Unable to Enable Account.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

                // Possibly email spiceworks that it's still broken - prompt?
                this.EmailJob(String.Format(_Constants.EMAIL_SUBJECT_FORMAT, job.JobID, _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"], job.StudentName,
                    job.StudentID), _Constants.EMAIL_TEXT_BROKEN);

                brokenButton.Text = "<-------";
                fixedButton.Enabled = true;
                brokenButton.Enabled = true;
            }
        }

        private void collectedButton_Click(object sender, EventArgs e)
        {
            // If no item is selected in the collect ListBox, do nothing,
            // otherwise:
            if (collectionListBox.SelectedItem != null)
            {
                collectedButton.Enabled = false;
                // Prompt the user if they are sure they want to close the job.
                if (MessageBox.Show(this, "This action will mark the following job as completed:\n" + collectionListBox.SelectedItem.ToString() +
                    "\n\n Are you sure you want to do that?", "Mark Completed?",
                    MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Job job = collectionListBox.SelectedItem as Job;

                    // User selected "Yes", to move the job to null (thus deleting it).
                    this.MoveRecord(job, _CollectJobList, null, false);

                    // Enable Student account
                    AD ad = new AD();
                    ad.Server = _DomainController;
                    try
                    {
                        string distinguishedName = null;

                        if (_NetworkCredentials == null)
                            distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                                _Constants.COMMON_INFO["LDAPDomain"]);
                        else
                            distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                                _Constants.COMMON_INFO["LDAPDomain"], _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                                _NetworkCredentials.Password);
                        
                        ad.Enable(distinguishedName);
                    }
                    catch (NullReferenceException nre)
                    {
                        MessageBox.Show(this, "Error: " + nre.Message,
                        "Unable to Enable Account.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }

                    // Email spiceworks telling it that the netbook's been collected, and close the job.
                    this.EmailJob(String.Format(_Constants.EMAIL_SUBJECT_FORMAT, job.JobID, _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"],
                        job.StudentName, job.StudentID), _Constants.EMAIL_TEXT_COLLECTED);
                }
            }
        }

        private void addJobButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
            addJobButton.Text = "Adding...";
            addJobButton.Enabled = false;
            // Prompt user for details to add a new job.
            using (AddJobGUI addJobForm = new AddJobGUI(this))
            {
                addJobForm.ShowDialog(this);
            }

            addJobButton.Enabled = true;
            addJobButton.Text = "Add Job";
            timer.Start();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            this.UpdateListBoxes();
        }

        private void enableButton_Click(object sender, EventArgs e)
        {
            // If no item is selected in the collect ListBox, do nothing,
            // otherwise:
            if (collectionListBox.SelectedItem != null)
            {
                enableButton.Text = "Enabling...";
                enableButton.Enabled = false;
                Job job = collectionListBox.SelectedItem as Job;

                // Prompt the user if they are sure they want to close the job.
                if (MessageBox.Show(this, "Are you sure that you want to re-enable " + job.StudentName + "'s account?", "Re-Enable Account?",
                    MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Enable Student account
                    AD ad = new AD();
                    ad.Server = _DomainController;
                    try
                    {
                        string distinguishedName = null;

                        if (_NetworkCredentials == null)
                            distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                                _Constants.COMMON_INFO["LDAPDomain"]);
                        else
                            distinguishedName = ad.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, job.StudentID,
                                _Constants.COMMON_INFO["LDAPDomain"], _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""),
                                _NetworkCredentials.Password);

                        ad.Enable(distinguishedName);
                        MessageBox.Show(this, job.StudentName + "'s account has been re-enabled.");
                    }
                    catch (NullReferenceException nre)
                    {
                        MessageBox.Show(this, "Error: " + nre.Message,
                        "Unable to Enable Account.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }

                enableButton.Enabled = true;
                enableButton.Text = "Enable";
            }
        }

        /// <summary>
        ///     Updates the ListBox datasources and refreshes the ListBoxes to reflect the changes.
        /// </summary>
        private void UpdateListBoxes()
        {
            // Stop the timer while jobs are being updated.
            timer.Stop();
            updateButton.Text = "Updating...";
            updateButton.Enabled = false;

            // Read the jobTXT file into jobTXTContents.
            string jobTXTContents;
            try
            {
                using (StreamReader sr = new StreamReader(_JobTXT))
                {
                    jobTXTContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (IOException ioe)
            {
                if (Directory.Exists(_JobTXTDir) && !File.Exists(_JobTXT))
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(_JobTXT))
                        {
                            sw.WriteLine(_Constants.CSV_MARKER_REPAIR);
                            sw.WriteLine();
                            sw.WriteLine(_Constants.CSV_MARKER_COLLECT);
                            sw.Close();
                        }

                        UpdateListBoxes();
                    }
                    catch (IOException ioex)
                    {
                        MessageBox.Show(this, "Error: " + ioex.Message,
                            "Unable to write job file.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        GetCampusLocation();
                        updateButton.Enabled = true;
                        updateButton.Text = "<- Update ->";
                        timer.Start();
                        return;
                    }
                }
                else
                {
                    // Catch a read error.
                    MessageBox.Show(this, "Error: " + ioe.Message,
                        "Unable to read job file.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                GetCampusLocation();
                updateButton.Enabled = true;
                updateButton.Text = "<- Update ->";
                return;
            }

            // Split the contents into an array of lines.
            string[] jobTXTContentsArray = jobTXTContents.Trim().Replace("\r", "").Split('\n');

            // Clear the job lists.
            _RepairJobList.Clear();
            _CollectJobList.Clear();

            // Initialise repairLine, which determines whether a job in the txt file is a repair or collection job.
            bool repairLine = true;

            // For every line in the txt file:
            for (int line = 0; line < jobTXTContentsArray.Length; line++)
            {
                // Check if the current line says "To Collect:" or "To Repair:".
                if (jobTXTContentsArray[line] == _Constants.CSV_MARKER_COLLECT)
                {
                    // If "To Collect:", the following jobs are collection jobs.
                    repairLine = false;
                    continue;
                }
                else if (jobTXTContentsArray[line] == _Constants.CSV_MARKER_REPAIR)
                {
                    // If "To Repair:", the following jobs are repair jobs.
                    repairLine = true;
                    continue;
                }

                // If the job line isnt blank:
                if (jobTXTContentsArray[line] != "")
                {
                    // Split the line by commas.
                    string[] jobTXTContentsArrayElement = jobTXTContentsArray[line].Split(',');

                    // See if there are 3 elements in the line.
                    if (jobTXTContentsArrayElement.Length == 4)
                    {
                        if (repairLine)
                        {
                            // If it's a repair line, add job to the repair list.
                            _RepairJobList.Add(new Job(jobTXTContentsArrayElement[0], jobTXTContentsArrayElement[1], jobTXTContentsArrayElement[2],
                                jobTXTContentsArrayElement[3]));
                        }
                        else
                        {
                            // If it's a collect line, add the job to the collect list.
                            _CollectJobList.Add(new Job(jobTXTContentsArrayElement[0], jobTXTContentsArrayElement[1], jobTXTContentsArrayElement[2],
                                jobTXTContentsArrayElement[3]));
                        }
                    }
                    else if (jobTXTContentsArrayElement.Length == 3)
                    {
                        if (repairLine)
                        {
                            // If it's a repair line, add job to the repair list.
                            _RepairJobList.Add(new Job(jobTXTContentsArrayElement[0], jobTXTContentsArrayElement[1], jobTXTContentsArrayElement[2], "2"));
                        }
                        else
                        {
                            // If it's a collect line, add the job to the collect list.
                            _CollectJobList.Add(new Job(jobTXTContentsArrayElement[0], jobTXTContentsArrayElement[1], jobTXTContentsArrayElement[2], "2"));
                        }
                    }
                }
            }

            // Refresh the listboxes to reflect the new data (if any)
            this.RefreshListBoxes();

            // Disable the "Collected" button if the collection listbox has entries, and the 1st isn't allowed to collect their netbook.
            if (collectionListBox.Items.Count == 0 || ((Job)collectionListBox.Items[0]).AllowCollect != _Constants.PAYMENTS_VALUE_ALLOW)
                collectedButton.Enabled = false;
            else
                collectedButton.Enabled = true;

            updateButton.Enabled = true;
            updateButton.Text = "<- Update ->";

            // Restart the timer for auto-list updates.
            timer.Start();
        }

        /// <summary>
        ///     Refreshes the ListBoxes to display new data.
        /// </summary>
        private void RefreshListBoxes()
        {
            // To refresh a ListBox, its datasource must be set to null and then back to the datasource.
            repairListBox.DataSource = null;
            repairListBox.DataSource = _RepairJobList;

            collectionListBox.DataSource = null;
            collectionListBox.DataSource = _CollectJobList;

            // Disable the "Collected" button if the collection listbox has entries, and the 1st isn't allowed to collect their netbook.
            if (collectionListBox.Items.Count == 0 || collectionListBox.SelectedIndex == ListBox.NoMatches ||
                ((Job)collectionListBox.SelectedItem).AllowCollect != _Constants.PAYMENTS_VALUE_ALLOW)
                collectedButton.Enabled = false;
            else
                collectedButton.Enabled = true;
        }

        /// <summary>
        ///     Move a record (Job) from one collection to another.
        /// </summary>
        /// <param name="record">The job to move.</param>
        /// <param name="fromCollection">A collection to remove the record from.</param>
        /// <param name="toCollection">A collection to add a record into.</param>
        /// <param name="toCollect">Whether the record is being moved in to the "for collection" listbox or not.</param>
        internal void MoveRecord(Job record, List<Job> fromCollection, List<Job> toCollection, bool toCollect)
        {
            // Stop the list auto-update timer while jobs are being moved.
            timer.Stop();

            // Make sure the job is actually is job.
            if (!(record is Job) || record == null)
            {
                MessageBox.Show(this, "Job is not a \"Job\" object, or is null.", "Invalid Job Object.");
                timer.Start();
                return;
            }

            // Read the jobTXT file into jobTXTContents.
            string jobTXTContents;
            try
            {
                jobTXTContents = File.ReadAllText(_JobTXT);
            }
            catch (IOException ioe)
            {
                // Unable to read the job file.
                MessageBox.Show(this, "Error: " + ioe.Message,
                    "Unable to read job file.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                timer.Start();
                return;
            }

            // If both collections are null, then job must be one to add.
            if (fromCollection == null && toCollection == null)
            {
                // Add the job to the repair list.
                _RepairJobList.Add(record);

                if (toCollect)
                {
                    // Add the job to the end of txt file's contents, if it's a collect job.
                    if (jobTXTContents[jobTXTContents.Length - 1] == '\n')
                    {
                        jobTXTContents += String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect);
                    }
                    else
                    {
                        jobTXTContents += String.Format("\r\n{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect);
                    }
                }
                else
                {
                    // Add the job to the "middle" of the txt file's contents (two lines before the "To Collect" line).
                    jobTXTContents = jobTXTContents.Replace("\r\n" + _Constants.CSV_MARKER_COLLECT, String.Format("{0},{1},{2},{3}\r\n\r\n" + _Constants.CSV_MARKER_COLLECT,
                        record.JobID, record.StudentID, record.StudentName, record.AllowCollect));
                }
            }
            else if (fromCollection.Equals(toCollection))
                {
                    jobTXTContents = jobTXTContents.Replace(String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName,
                        _Constants.PAYMENTS_VALUE_DENY),
                        String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect));
                    jobTXTContents = jobTXTContents.Replace(String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName,
                        _Constants.PAYMENTS_VALUE_ALLOW),
                        String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect));
                    jobTXTContents = jobTXTContents.Replace(String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName,
                        _Constants.PAYMENTS_VALUE_UNKNOWN),
                        String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect));
                }
            else
            {
                // If fromCollection isn't null, remove the job from the lists and the txt file.
                if (fromCollection != null)
                {
                    fromCollection.Remove(record);
                    jobTXTContents = jobTXTContents.Replace(String.Format("\r\n{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect), "");
                }

                // If toCollection isn't null, then add the job to the toCollection
                if (toCollection != null)
                {
                    toCollection.Add(record);

                    if (toCollect)
                    {
                        if (jobTXTContents[jobTXTContents.Length - 1] == '\n')
                        {
                            jobTXTContents += String.Format("{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect);
                        }
                        else
                        {
                            jobTXTContents += String.Format("\r\n{0},{1},{2},{3}", record.JobID, record.StudentID, record.StudentName, record.AllowCollect);
                        }
                    }
                    else
                    {
                        jobTXTContents = jobTXTContents.Replace("\r\n" + _Constants.CSV_MARKER_COLLECT, String.Format("{0},{1},{2},{3}\r\n\r\n" + _Constants.CSV_MARKER_COLLECT,
                            record.JobID, record.StudentID, record.StudentName, record.AllowCollect));
                    }
                }
            }

            // Try writing the job data to the txt file:
            try
            {
                File.WriteAllText(_JobTXT, jobTXTContents);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(this, "Error: " + ioe.Message,
                    "Unable to write job file.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                timer.Start();
                return;
            }

            // Restart the timer.
            timer.Start();

            // Update the list boxes to reflect the new changes.
            this.UpdateListBoxes();
        }

        /// <summary>
        ///     Emails job data to the Kurnai helpdesk email account (Spiceworks).
        /// </summary>
        /// <param name="subject">The subject line for the email.</param>
        /// <param name="body">The body of the email to send.</param>
        /// <returns>True/False depending on whether the email was sucessfully sent or not.</returns>
        private bool EmailJob(string subject, string body)
        {
            bool result = true;

            string email = _Ini.GetKeyValue(Environment.UserName, "Email");

            if (email != "" && email != null)
            {
                try
                {
                    using (MailMessage message = new MailMessage(email, _Constants.COMMON_INFO["HelpdeskEmail"], subject, body))
                    {
                        using (SmtpClient client = new SmtpClient(_Constants.COMMON_INFO["SMTPServer"]))
                        {
                            client.Send(message);
                        }
                    }

                    MessageBox.Show(this, "Job information has been successfully updated.",
                        "Job Updated."
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Information);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message,
                        String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.",
                        _Constants.COMMON_INFO["HelpdeskEmail"])
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                    result = false;
                }
                catch (SmtpException ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message,
                        String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.",
                        _Constants.COMMON_INFO["HelpdeskEmail"])
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets login credentials from Windows Credential Manager.
        /// </summary>
        /// <param name="target">The name of the credential.</param>
        /// <returns>The selected credentials, or null if none found.</returns>
        private Credential GetCredentials(string target)
        {
            var cm = new Credential { Target = target };
            if (!cm.Exists())
                return null;

            cm.Load();
            return cm;
        }

        /// <summary>
        ///     Saves the passed credentials into Windows Credential Manager.
        /// </summary>
        /// <param name="cm">The credentials to save.</param>
        /// <returns>Whether the credentials were saved successfully or not.</returns>
        private bool SetCredentials(Credential cm)
        {
            bool result = true;

            for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
            {
                using (Credential cm2 = new Credential { Target = _Constants.CAMPUS_INFO[i]["FileServer"], Type = CredentialType.DomainPassword, Username = cm.Username,
                    Password = cm.Password, PersistanceType = PersistanceType.Enterprise })
                    result = result && cm2.Save();
            }

            return result && cm.Save();
        }

        /// <summary>
        ///     Removes credentials of a given name from the Windows Credential Manager.
        /// </summary>
        /// <param name="target">The name of the credentials to remove.</param>
        private void RemoveCredentials(string target)
        {
            var cm = new Credential { Target = target };
            cm.Delete();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Every 10 seconds, update the listboxes.
            this.UpdateListBoxes();
        }

        private void campusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = campusComboBox.SelectedIndex;
            this.Text = String.Format("Kurnai Netbook Helper ({0}) - © {1}", campusComboBox.SelectedItem.ToString(), _Constants.COPYRIGHT);
            //Campus = _Constants.MORWELL;
            _DomainController = _Constants.CAMPUS_INFO[i]["DomainController"];
            _JobTXTDir = @"\\" + _Constants.CAMPUS_INFO[i]["FileServer"] + @"\" + _Constants.CAMPUS_INFO[i]["Share"] + @"\";
            _JobTXT = _JobTXTDir + _Constants.JOB_TXT_FILE;

            this.UpdateListBoxes();
        }

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            System.Text.StringBuilder sOutput = new System.Text.StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}