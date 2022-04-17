using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Net.Mail;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;

namespace KurnaiNetbookHelper
{
    public partial class AddJobGUI : Form
    {
        private MainGUI _Parent;
        private IniFile _Ini = new IniFile();
        private Validate _Validate = new Validate();
        private AD _AD = new AD();
        private CustomizedToolTip _CustomToolTip = new CustomizedToolTip();
        private List<LWTItem>[] _PartList;
        //private List<LWTItem> _Aspire753 = new List<LWTItem>();
        //private List<LWTItem> _Aspire533 = new List<LWTItem>();
        //private List<LWTItem> _Aspire270 = new List<LWTItem>();
        //private List<LWTItem> _Aspire255 = new List<LWTItem>();
        //private List<LWTItem> _TravelmateB113 = new List<LWTItem>();
        private List<LWTItem> _CustomParts = new List<LWTItem>();
        internal LWTItem _ItemToAdd = null;
        internal string _DomainController = "";
        internal string _LabourReason = "";
        private CredentialManagement.Credential _NetworkCredentials = null;
        private Constants _Constants = new Constants();

        private Point _MHoverPos;
        private bool _MHoverDone;
        private Timer _MHoverTimer = new Timer();

        private bool _EmailValid = false;
        private bool _SummaryValid = false;
        private bool _DescriptionValid = false;
        private bool _LWTValid = true;
        private bool _POValid = true;
        private bool _NameValid = false;
        private bool _RequesterNameValid = false;
        private bool _IDValid = false;
        private bool _SerialValid = false;
        private bool _TicketValid = false;
        private bool _PartsValid = true;
        private bool _FirstLoad = true;

        public AddJobGUI(MainGUI parent)
        {
            _Parent = parent;
            InitializeComponent();

            _MHoverTimer.Tick += HoverTick;
            _MHoverTimer.Interval = 1000;

            this.Width = 415;

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("KurnaiNetbookHelper.Resources.LWT.jpg");
            Bitmap image = new Bitmap(myStream);
            
            olvColumn2.Renderer = new BrightIdeasSoftware.ImageRenderer();
            objectListView1.AddDecoration(new BrightIdeasSoftware.EditingCellBorderDecoration { UseLightbox = true });
            objectListView1.SetNativeBackgroundTiledImage(image);
        }

        #region Custom Mouse Hover Events

        private void objectListView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MHoverDone)
            {
                if (_MHoverTimer.Enabled)
                {
                    // Restart timer if the mouse moved too far
                    if (Math.Abs(e.X - _MHoverPos.X) <= 4 && Math.Abs(e.Y - _MHoverPos.Y) <= 4) return;
                    _MHoverTimer.Enabled = false;
                }

                // Start hover timer
                _MHoverPos = e.Location;
                _MHoverTimer.Enabled = true;
            }
        }
        private void objectListView1_MouseLeave(object sender, EventArgs e)
        {
            _MHoverTimer.Enabled = false;
            _MHoverDone = false;
        }
        private void HoverTick(object sender, EventArgs e)
        {
            _MHoverTimer.Enabled = false;
            _MHoverDone = true;

            // Put your hover event code here:
            customizedToolTip = new CustomizedToolTip() { BorderColor = Color.Black, BackColor = SystemColors.Window };
            customizedToolTip.Show("Item", objectListView1);
        }

        #endregion

        private void AddJobGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();

            _DomainController = _Parent._DomainController;
            _AD.Server = _DomainController;
            this._NetworkCredentials = _Parent._NetworkCredentials;

            // Get settings from ini file.
            string iniFile = _Constants.USER_INI_FILE;

            if (File.Exists(iniFile))
            {
                _Ini.Load(iniFile);
                rNameTextBox.Text = _Ini.GetKeyValue(Environment.UserName, "RequesterName");
                emailTextBox.Text = _Ini.GetKeyValue(Environment.UserName, "Email");
            }
            else
            {
                _Ini.SetKeyValue(Environment.UserName, "RequesterName", "");
                _Ini.SetKeyValue(Environment.UserName, "Email", "");
                _Ini.SetKeyValue(Environment.UserName, "Code", "");
                _Ini.Save(iniFile);
            }


            if (emailTextBox.Text != "")
            {
                // Change the colour of the edited textbox to indicate if the field is valid or not.
                if (_Validate.IsEmail(emailTextBox.Text))
                {
                    emailTextBox.ForeColor = Color.DarkGreen;
                    _EmailValid = true;
                }
                else
                {
                    emailTextBox.ForeColor = Color.DarkRed;
                    _EmailValid = false;
                    toolTip.SetToolTip(emailTextBox, "You must enter a valid \"@edumail.vic.gov.au\" email address.");
                }

                // Set focus to the summary textBox.
                this.ActiveControl = summaryTextBox;
            }
            else
            {
                // Set focus to the email textBox.
                this.ActiveControl = emailTextBox;
            }

            // Populate campus combobox.
            for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                campusComboBox.Items.Add(_Constants.CAMPUS_INFO[i]["Name"]);

            this.campusComboBox.SelectedItem = _Parent.campusComboBox.SelectedItem.ToString();

            //Populate model and make comboboxes.
            _PartList = new List<LWTItem>[_Constants.MODEL_INFO.GetLength(0)];

            for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
            {
                _PartList[i] = new List<LWTItem>();
                modelComboBox.Items.Add(_Constants.MODEL_INFO[i]["Name"]);

                if (!makeComboBox.Items.Contains(_Constants.MODEL_INFO[i]["Make"]))
                    makeComboBox.Items.Add(_Constants.MODEL_INFO[i]["Make"]);
            }

            // Set GUI combobox default values.
            if (_Constants.MODEL_INFO.GetLength(0) > 0)
            {
                makeComboBox.SelectedIndex = 0;
                modelComboBox.SelectedIndex = 0;
            }

            UpdatePartLists();
            _FirstLoad = false;

            objectListView1.ClearObjects();

            for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                if (modelComboBox.Text == _Constants.MODEL_INFO[i]["Name"])
                    objectListView1.SetObjects(_PartList[i]);

            //if (modelComboBox.Text == "Aspire 753")
            //    objectListView1.SetObjects(_Aspire753);
            //else if (modelComboBox.Text == "Aspire 533")
            //    objectListView1.SetObjects(_Aspire533);
            //else if (modelComboBox.Text == "Aspire 270")
            //    objectListView1.SetObjects(_Aspire270);
            //else if (modelComboBox.Text == "Aspire 255E")
            //    objectListView1.SetObjects(_Aspire255);
            //else if (modelComboBox.Text == "Travelmate B113")
            //    objectListView1.SetObjects(_TravelmateB113);

            CalculateOrderTotal();

            try
            {
                using (WebClient client = new WebClient())
                    client.DownloadString("http://www.lwt.com.au/KurnaiPriceQuery.aspx?ProductCode=ACE693716");
            }
            catch (Exception)
            {
                WebProxy webProxy = new WebProxy(_Constants.COMMON_INFO["Proxy"]);
                webProxy.Credentials = new NetworkCredential(_Constants.COMMON_INFO["ProxyUsername"], _Constants.COMMON_INFO["ProxyPassword"]);

                WebRequest.DefaultWebProxy = webProxy;
            }
        }

        private void UpdatePartLists()
        {
            for (int i = 0; i < _PartList.GetLength(0); i++)
                _PartList[i].Clear();
            //_Aspire753.Clear();
            //_Aspire533.Clear();
            //_Aspire270.Clear();
            //_Aspire255.Clear();
            //_TravelmateB113.Clear();
            _CustomParts.Clear();

            // Read PartList CSV.
            string dataDir = _Constants.DATA_DIR;
            string filePath = _Constants.PART_LIST;//dataDir + "PartList.csv";
            string pattern = @"""(?:[^\\""]+|\\.)*""";
            Regex regex = new Regex(pattern);

            // Download current parts list if none is found or manual update was pressed.
            if (!File.Exists(filePath) || !_FirstLoad)
            {
                try
                {
                    string sURL = _Constants.LWT_PARTS_URL;
                    string downloadString = this.DownloadWebPage(sURL).Trim();

                    if (downloadString == "Website Under Maintenance - will be up shortly")
                    {
                        MessageBox.Show(this, "LWT's website is currently under maintenance. Please try updating the lists again later.",
                            "Update Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        if (downloadString != "" && downloadString != "ProductBaseType,LWTCode,ManufacturerCode,ProductTitle,SellPriceExGST")
                        {
                            // Add missing parts.
                            if (File.Exists(_Constants.PART_LIST_ADDITIONS))//dataDir + "PartListAdditions.csv"))
                            {
                                string[] customLines = File.ReadAllLines(_Constants.PART_LIST_ADDITIONS);

                                foreach (string customPart in customLines)
                                {
                                    if (customPart == "")
                                        continue;

                                    string[] customPartElement = customPart.Split(',');

                                    try
                                    {
                                        string downloadString2 = this.DownloadWebPage(String.Format(_Constants.LWT_PRICE_LOOKUP_URL, Uri.EscapeDataString(customPartElement[1]))).Trim();
                                        if (downloadString2 == "Website Under Maintenance - will be up shortly")
                                        {
                                            MessageBox.Show(this, "LWT's website is currently under maintenance. Please try updating the lists again later.",
                                                "Update Failed",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                            return;
                                        }
                                        else
                                        {
                                            float price = 0.0f;
                                            if (float.TryParse(downloadString2, out price))
                                            {
                                                downloadString += String.Format("\r\n{0},{1},{2},{3},${4:0.00}", customPartElement[0], customPartElement[1], customPartElement[2], customPartElement[3], price / 11 * 10);
                                            }
                                        }
                                    }
                                    catch (Exception e) { MessageBox.Show(e.Message); }
                                }
                            }

                            File.WriteAllText(filePath, downloadString);
                        }
                    }
                }
                catch (Exception e) { MessageBox.Show(e.Message); }
            }

            if (File.Exists(filePath))
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = sr.ReadLine();

                    if (line == "Website Under Maintenance - will be up shortly")
                    {
                        sr.Close();
                        MessageBox.Show(this, "LWT's website is currently under maintenance. Please try updating the lists again later.",
                            "Update Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            line = Regex.Replace(line, pattern, m => m.Value.Replace(",", ""), RegexOptions.IgnorePatternWhitespace).Replace("\"", "");
                            string[] lineData = line.Split(',');
                            Image image;

                            for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                                if (lineData[0] == _Constants.MODEL_INFO[i]["Name"])//"Aspire 753" || lineData[0] == "Aspire 533" || lineData[0] == "Aspire 270" || lineData[0] == "Aspire 255E" || lineData[0] == "Travelmate B113")
                                {
                                    if (File.Exists(dataDir + @"Images\" + lineData[1] + ".jpg"))
                                        image = Image.FromFile(dataDir + @"Images\" + lineData[1] + ".jpg");
                                    else
                                    {
                                        Stream myStream = myAssembly.GetManifestResourceStream("KurnaiNetbookHelper.Resources.not_available.jpg");
                                        image = new Bitmap(myStream);
                                    }

                                    _PartList[i].Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));

                                    //if (lineData[0] == "Aspire 753")
                                    //    _Aspire753.Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));
                                    //else if (lineData[0] == "Aspire 533")
                                    //    _Aspire533.Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));
                                    //else if (lineData[0] == "Aspire 270")
                                    //    _Aspire270.Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));
                                    //else if (lineData[0] == "Aspire 255E")
                                    //    _Aspire255.Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));
                                    //else if (lineData[0] == "Travelmate B113")
                                    //    _TravelmateB113.Add(new LWTItem(lineData[1], lineData[3], Convert.ToSingle(lineData[4].Replace("$", "")) * 1.1f, 0, image));

                                    break;
                                }
                        }
                    }
                }

            // Read PartList CSV.
            filePath = _Constants.PART_LIST_CUSTOM;//dataDir + @"PartList (" + Environment.UserName + @").csv";

            // If user clicked the manual update button, get current prices.
            if (File.Exists(filePath) && !_FirstLoad)
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int lineIndex = 1; lineIndex < lines.GetLength(0); lineIndex++)
                {
                    if (lines[lineIndex] == "")
                        continue;

                    string[] lineData = Regex.Replace(lines[lineIndex], pattern, m => m.Value.Replace(",", ""), RegexOptions.IgnorePatternWhitespace).Replace("\"", "").Split(',');

                    // Lookup current item price.
                    try
                    {
                        string sURL = String.Format(_Constants.LWT_PRICE_LOOKUP_URL, Uri.EscapeDataString(lineData[0]));
                        string downloadString = this.DownloadWebPage(sURL).Trim();
                        float price = 0.0f;

                        if (downloadString != null && float.TryParse(downloadString, out price) && lineData[3] != downloadString)
                        {
                            lines[lineIndex] = String.Format("{0},{1},{2},{3}", lineData[0], lineData[1], lineData[2], downloadString);
                        }
                    }
                    catch (Exception) { }
                }

                File.WriteAllLines(filePath, lines);
            }

            if (File.Exists(filePath))
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = sr.ReadLine();

                    if (line == "Website Under Maintenance - will be up shortly")
                    {
                        sr.Close();
                        MessageBox.Show(this, "LWT's website is currently under maintenance. Please try updating the lists again later.",
                            "Update Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            line = Regex.Replace(line, pattern, m => m.Value.Replace(",", ""), RegexOptions.IgnorePatternWhitespace).Replace("\"", "");
                            string[] lineData = line.Split(',');
                            Image image;

                            if (File.Exists(dataDir + @"Images\" + lineData[0] + ".jpg"))
                                image = Image.FromFile(dataDir + @"Images\" + lineData[0] + ".jpg");
                            else
                            {
                                Stream myStream = myAssembly.GetManifestResourceStream("KurnaiNetbookHelper.Resources.not_available.jpg");
                                image = new Bitmap(myStream);
                            }

                            LWTItem lwtItem = new LWTItem(lineData[0], lineData[2], Convert.ToSingle(lineData[3]), 0, image);
                            lwtItem.GLCode = lineData[1];

                            _CustomParts.Add(lwtItem);
                        }
                    }
                }
        }

        private void nameSearchButton_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text != "")
            {
                nameSearchButton.Enabled = false;

                try
                {
                    string[,] users = null;

                    if (_NetworkCredentials == null)
                        users = _AD.SearchAD(AD.objectClass.user, AD.returnType.sAMAccountName, nameTextBox.Text);
                    else
                        users = _AD.SearchAD(AD.objectClass.user, AD.returnType.sAMAccountName, nameTextBox.Text, _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""), _NetworkCredentials.Password);

                    if (users.GetLength(0) > 1)
                    {
                        // Display popup for the user to select the correct student ID.
                        MultipleAccountsGUI maGUI = new MultipleAccountsGUI(this, users, 0);
                        if (maGUI.ShowDialog(this) != DialogResult.OK)
                        {
                            maGUI.Dispose();
                            nameSearchButton.Enabled = true;
                            return;
                        }
                        maGUI.Dispose();

                        // Simulate clicking the ID Search button to find the correct name/serial.
                        idSearchButton_Click(sender, e);
                    }
                    else
                    {
                        // Set Student ID.
                        idTextBox.Text = users[0, 1];

                        // Simulate clicking the ID Search button to find the correct name/serial.
                        idSearchButton_Click(sender, e);
                    }
                }
                catch (NullReferenceException nre)
                {
                    MessageBox.Show(this, "Error: " + nre.Message,
                    "Unable to Find Account.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

                nameSearchButton.Enabled = true;
            }
        }

        private void idSearchButton_Click(object sender, EventArgs e)
        {

            idSearchButton.Enabled = false;

            try
            {
                string[,] machine = null;
                string distinguishedName = "";
                string givenName = "";
                string familyName = "";
                string username = "";

                if (_NetworkCredentials == null)
                {
                    distinguishedName = _AD.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, idTextBox.Text, _Constants.COMMON_INFO["LDAPDomain"]);
                    givenName = _AD.AttributeValuesSingleString("givenName", distinguishedName);
                    familyName = _AD.AttributeValuesSingleString("sn", distinguishedName);
                    machine = _AD.SearchAD(AD.objectClass.computer, AD.returnType.serialNumber, distinguishedName);
                }
                else
                {
                    username = _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", "");
                    distinguishedName = _AD.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, idTextBox.Text, _Constants.COMMON_INFO["LDAPDomain"], username, _NetworkCredentials.Password);
                    givenName = _AD.AttributeValuesSingleString("givenName", distinguishedName, username, _NetworkCredentials.Password);
                    familyName = _AD.AttributeValuesSingleString("sn", distinguishedName, username, _NetworkCredentials.Password);
                    machine = _AD.SearchAD(AD.objectClass.computer, AD.returnType.serialNumber, distinguishedName, _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", ""), _NetworkCredentials.Password);
                }

                nameTextBox.Text = String.Format("{0} {1}", givenName, familyName);

                if (machine != null)
                    if (machine.GetLength(0) == 1)
                    {
                        serialTextBox.Text = machine[0, 1];
                    }
                    else if (machine.GetLength(0) > 1)
                    {
                        // Prompt with all machines
                        MultipleAccountsGUI maGUI = new MultipleAccountsGUI(this, machine, 1);
                        if (maGUI.ShowDialog(this) != DialogResult.OK)
                        {
                            maGUI.Dispose();
                            idSearchButton.Enabled = true;
                            return;
                        }
                        maGUI.Dispose();
                    }
            }
            catch (NullReferenceException nre)
            {
                MessageBox.Show(this, "Error: " + nre.Message,
                "Unable to Find Account.",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }

            idSearchButton.Enabled = true;
        }

        private void serialSearchButton_Click(object sender, EventArgs e)
        {
            serialSearchButton.Enabled = false;

            try
            {
                string[,] machine = null;
                string username = "";

                if (_NetworkCredentials == null)
                    machine = _AD.SearchAD(AD.objectClass.computer, AD.returnType.managedBy, serialTextBox.Text);
                else
                {
                    username = _NetworkCredentials.Username.ToUpper().Replace(_Constants.COMMON_INFO["Domain"].ToUpper() + "\\", "");
                    machine = _AD.SearchAD(AD.objectClass.computer, AD.returnType.managedBy, serialTextBox.Text, username, _NetworkCredentials.Password);
                }

                if (machine.GetLength(0) > 1)
                {
                    // Prompt with all machines
                    MultipleAccountsGUI maGUI = new MultipleAccountsGUI(this, machine, 1);
                    if (maGUI.ShowDialog(this) != DialogResult.OK)
                    {
                        maGUI.Dispose();
                        serialSearchButton.Enabled = true;
                        return;
                    }
                    maGUI.Dispose();
                }
                string owner = "LDAP://" + machine[0, 1];
                string studentID = "";
                string givenName = "";
                string familyName = "";

                if (_NetworkCredentials == null)
                {
                    studentID = _AD.AttributeValuesSingleString("sAMAccountName", owner);
                    givenName = _AD.AttributeValuesSingleString("givenName", owner);
                    familyName = _AD.AttributeValuesSingleString("sn", owner);
                }
                else
                {
                    studentID = _AD.AttributeValuesSingleString("sAMAccountName", owner, username, _NetworkCredentials.Password);
                    givenName = _AD.AttributeValuesSingleString("givenName", owner, username, _NetworkCredentials.Password);
                    familyName = _AD.AttributeValuesSingleString("sn", owner, username, _NetworkCredentials.Password);
                }

                nameTextBox.Text = String.Format("{0} {1}", givenName, familyName);
                idTextBox.Text = studentID;
            }
            catch (NullReferenceException nre)
            {
                MessageBox.Show(this, "Error: " + nre.Message,
                    "Unable to Find Account.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            serialSearchButton.Enabled = true;
        }

        private void spiceworksButton_Click(object sender, EventArgs e)
        {
            spiceworksButton.Text = "Sending...";
            spiceworksButton.Enabled = false;

            string displayName = _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["TechEmail"];

            string subject = String.Format("{0} - Netbook - {1} ({2}) - {3}",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"], nameTextBox.Text, idTextBox.Text.ToUpper(), summaryTextBox.Text);

            string body = String.Format("{0}\r\n\r\n#set Serial Number={1}\r\n#relate {1}\r\n#set Location={2}\r\n#set Make={3}\r\n#set Model={4}\r\n#set Name={5}\r\n#set Purchase Order Number={6}\r\n#assign to {7}\r\n#set LWT Number={8}\r\n#cc {9}",
                descriptionTextBox.Text, serialTextBox.Text, campusComboBox.SelectedItem.ToString(), makeComboBox.GetItemText(makeComboBox.SelectedItem),
                modelComboBox.GetItemText(modelComboBox.SelectedItem), nameTextBox.Text, poTextBox.Text, displayName,
                lwtJobIDTextBox.Text, _Constants.COMMON_INFO["ITManagerEmail"]);

            string[] specialTechs = _Constants.COMMON_INFO["STEmail"].Split(';');

            foreach (string tech in specialTechs)
                body += "\r\n#cc " + tech;

            this.EmailJob(subject, body);

            spiceworksButton.Enabled = true;
            spiceworksButton.Text = "Send to Spiceworks";

            EnableSpiceworksButtonIfValid();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            addButton.Text = "Adding...";
            addButton.Enabled = false;

            // Test if the email address is valid.
            if (_Validate.IsEmail(emailTextBox.Text.ToLower()))
            {
                // Save valid email to the iniFile;
                _Ini.SetKeyValue(Environment.UserName, "Email", emailTextBox.Text.ToLower());
            }

            _Parent.MoveRecord(new Job(jobIDTextBox.Text, idTextBox.Text.ToUpper(), nameTextBox.Text, "2"), null, null, false);

            string subject = String.Format("{0} - Netbook - {1} ({2}) - {3}",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"], nameTextBox.Text, idTextBox.Text.ToUpper(), summaryTextBox.Text);

            string body = "<font face=\"Calibri\"> Dear " + _Constants.COMMON_INFO["AdminName"] + ",<br><br>" +
                "A new netbook job has been logged with the following information:<br><br>" +
                "<b>Student:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<i>" + nameTextBox.Text + " (" + idTextBox.Text + ")</i><br>" +
                "<b>Netbook Model:</b>&nbsp;&nbsp;&nbsp;&nbsp;<i>" + makeComboBox.Text + " " + modelComboBox.Text + "</i><br>" +
                "<b>Netbook Serial:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<i>" + serialTextBox.Text + "</i><br>" +
                "<b>Job Type:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<i>" +
                ((poTextBox.Text == "" && lwtJobIDTextBox.Text != "") ? "Warranty" : (poTextBox.Text == "" && lwtJobIDTextBox.Text == "") ? "N/A - Will let you know" : "Purchase") + "</i><br><br>" +
                "Please notify the sender of this email, as well as your campus’ librarians, whether or not the above student is allowed to borrow a netbook from the library.<br><br>" +
                "As per school policy, no netbook can be loaned or returned to the above student without confirmation.<br><br>" +
                "<i><font color=\"RoyalBlue\"><b>NOTE:</b> This email has been automatically generated using the \"Kurnai Netbook Helper\" application.<br>Librarians need not reply to this email.</font></i></font>";

            EmailAdmin(subject, body);

            Close();
        }

        private void EmailJob(string subject, string body)
        {
            // Test if the email address is valid.
            if (_Validate.IsEmail(emailTextBox.Text.ToLower()))
            {
                try
                {
                    using (MailMessage message = new MailMessage(emailTextBox.Text.ToLower(), _Constants.COMMON_INFO["HelpdeskEmail"], subject, body))
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
                    MessageBox.Show(this, String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.", _Constants.COMMON_INFO["HelpdeskEmail"])
                        , "Error: " + ex.Message
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                }
                catch (SmtpException ex)
                {
                    MessageBox.Show(this, String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.", _Constants.COMMON_INFO["HelpdeskEmail"])
                        , "Error: " + ex.Message
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "The entered email address is invalid. Please enter in a valid email address.",
                        "Invalid Email Address"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                this.ActiveControl = emailTextBox;
            }
        }

        private void EmailAdmin(string subject, string body)
        {
            try
            {
                using (MailMessage message = new MailMessage(emailTextBox.Text.ToLower(), _Constants.COMMON_INFO["AdminEmail"], subject, body))
                {
                    message.IsBodyHtml = true;
                    message.Priority = MailPriority.High;
                    message.CC.Add(new MailAddress(emailTextBox.Text.ToLower()));

                    foreach (string email in _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["LibraryEmail"].Split(';'))
                        message.CC.Add(new MailAddress(email));

                    using (SmtpClient client = new SmtpClient(_Constants.COMMON_INFO["SMTPServer"]))
                    {
                        client.Send(message);
                    }
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message,
                    String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.", _Constants.COMMON_INFO["AdminEmail"])
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message,
                    String.Format("Failed to send the updated job details to {0}.\nPlease email your local technician to let them know.", _Constants.COMMON_INFO["AdminEmail"])
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
            }
        }

        private void campusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_Parent.Campus = campusComboBox.SelectedItem.ToString().Substring(0, 3);
            _Parent.campusComboBox.SelectedItem = this.campusComboBox.SelectedItem.ToString();

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsPONumber(poTextBox.Text, campusComboBox.Text) || poTextBox.Text == "")
            {
                poTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(poTextBox, "");
                _POValid = true;
            }
            else
            {
                poTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(poTextBox, "PO number for " + campusComboBox.Text + " is invalid.");
                _POValid = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableGenerateButtonIfValid();

            label16.Text = String.Format("({0} - Netbook - {1} ({2}) - {3})",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"],
                (nameTextBox.Text == "") ? "<Student Name>" : nameTextBox.Text,
                (idTextBox.Text == "") ? "<ID>" : idTextBox.Text,
                (summaryTextBox.Text == "") ? "<Summary>" : summaryTextBox.Text);
        }

        private void modelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
            //    if (
            makeComboBox.SelectedItem = _Constants.MODEL_INFO[modelComboBox.SelectedIndex]["Make"];

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsSerial(serialTextBox.Text, modelComboBox.Text))
            {
                serialTextBox.ForeColor = Color.DarkGreen;
                serialSearchButton.Enabled = true;
                toolTip.SetToolTip(serialTextBox, "");
                _SerialValid = true;
            }
            else
            {
                serialTextBox.ForeColor = Color.DarkRed;
                serialSearchButton.Enabled = false;
                toolTip.SetToolTip(serialTextBox, "The typed serial number is invalid or incomplete for the selected model.");

                //if (modelComboBox.Text == "AO753")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AO753s are 8 digits long and start with \"1500\".");
                //else if (modelComboBox.Text == "TM B113")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for B113s are 8 digits long and start with \"2510\".");
                //else if (modelComboBox.Text == "AO533")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AO533s are 8 digits long and start with \"1030\".");
                //else if (modelComboBox.Text == "AOD270")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AOD270s are 8 digits long and start with \"2170\".");

                _SerialValid = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableWarrantyButtonIfValid();

            if (!customCheckBox.Checked)
            {
                objectListView1.ClearObjects();

                for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                    if (modelComboBox.Text == _Constants.MODEL_INFO[i]["Name"])
                        objectListView1.SetObjects(_PartList[i]);

                //if (modelComboBox.Text == "Aspire 753")
                //    objectListView1.SetObjects(_Aspire753);
                //else if (modelComboBox.Text == "Aspire 533")
                //    objectListView1.SetObjects(_Aspire533);
                //else if (modelComboBox.Text == "Aspire 270")
                //    objectListView1.SetObjects(_Aspire270);
                //else if (modelComboBox.Text == "Aspire 255E")
                //    objectListView1.SetObjects(_Aspire255);
                //else if (modelComboBox.Text == "Travelmate B113")
                //    objectListView1.SetObjects(_TravelmateB113);
            }

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void serialTextBox_TextChanged(object sender, EventArgs e)
        {
            //if (serialTextBox.Text.Length >= 4)
            //{
            //    if (serialTextBox.Text.Substring(0, 4) == "2510")
            //    {
            //        // Set Model to "TM B113".
            //        modelComboBox.SelectedIndex = 0;
            //    }
            //    else if (serialTextBox.Text.Substring(0, 4) == "1500")
            //    {
            //        // Set Model to "AO753"
            //        modelComboBox.SelectedIndex = 1;
            //    }
            //    else if (serialTextBox.Text.Substring(0, 4) == "1030")
            //    {
            //        // Set Model to "AO533"
            //        modelComboBox.SelectedIndex = 2;
            //    }
            //    else if (serialTextBox.Text.Substring(0, 4) == "2170")
            //    {
            //        // Set Model to "AOD270"
            //        modelComboBox.SelectedIndex = 3;
            //    }
            //    else if (serialTextBox.Text.Substring(0, 4) == "1320")
            //    {
            //        // Set Model to "AOD255"
            //        modelComboBox.SelectedIndex = 4;
            //    }
            //}
            for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
            {
                if (Regex.IsMatch(serialTextBox.Text, _Constants.MODEL_INFO[i]["SerialFormat"]))
                {
                    modelComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsSerial(serialTextBox.Text, modelComboBox.Text))
            {
                serialTextBox.ForeColor = Color.DarkGreen;
                serialSearchButton.Enabled = true;
                toolTip.SetToolTip(serialTextBox, "");
                _SerialValid = true;
            }
            else
            {
                serialTextBox.ForeColor = Color.DarkRed;
                serialSearchButton.Enabled = false;
                toolTip.SetToolTip(serialTextBox, "The typed serial number is invalid or incomplete for the selected model.");

                //if (modelComboBox.Text == "Aspire 753")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AO753s are 8 digits long and start with \"1500\".");
                //else if (modelComboBox.Text == "Travelmate B113")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for B113s are 8 digits long and start with \"2510\".");
                //else if (modelComboBox.Text == "Aspire 533")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AO533s are 8 digits long and start with \"1030\".");
                //else if (modelComboBox.Text == "Aspire 270")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AOD270s are 8 digits long and start with \"2170\".");
                //else if (modelComboBox.Text == "Aspire 255E")
                //    toolTip.SetToolTip(serialTextBox, "Valid serial numbers for AOD255s are 8 digits long and start with \"1320\".");

                _SerialValid = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableWarrantyButtonIfValid();

            EnableGenerateButtonIfValid();
        }

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsStudentID(idTextBox.Text))
            {
                idTextBox.ForeColor = Color.DarkGreen;
                idSearchButton.Enabled = true;
                toolTip.SetToolTip(idTextBox, "");
                _IDValid = true;
                idSearchButton.Enabled = true;
            }
            else
            {
                idTextBox.ForeColor = Color.DarkRed;
                idSearchButton.Enabled = false;
                toolTip.SetToolTip(idTextBox, "A student ID must be 3 letters followed by 4 digits.");
                _IDValid = false;
                idSearchButton.Enabled = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableAddButtonIfValid();

            EnableGenerateButtonIfValid();

            label16.Text = String.Format("({0} - Netbook - {1} ({2}) - {3})",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"],
                (nameTextBox.Text == "") ? "<Student Name>" : nameTextBox.Text,
                (idTextBox.Text == "") ? "<ID>" : idTextBox.Text,
                (summaryTextBox.Text == "") ? "<Summary>" : summaryTextBox.Text);
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {
            // Make this textbox be lowercase only.
            int cursorPosition = emailTextBox.SelectionStart;
            emailTextBox.Text = emailTextBox.Text.ToLower();
            emailTextBox.Select(cursorPosition, 0);

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsEmail(emailTextBox.Text))
            {
                emailTextBox.ForeColor = Color.DarkGreen;
                _EmailValid = true;
                toolTip.SetToolTip(emailTextBox, "");

                //IniFile iniFile = new IniFile();
                //iniFile.
            }
            else
            {
                emailTextBox.ForeColor = Color.DarkRed;
                _EmailValid = false;
                toolTip.SetToolTip(emailTextBox, "You must enter a valid \"*@edumail.vic.gov.au\" email address.");
            }

            EnableSpiceworksButtonIfValid();

            EnableWarrantyButtonIfValid();
        }

        private void summaryTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsPhrase(summaryTextBox.Text))
            {
                summaryTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(summaryTextBox, "");
                _SummaryValid = true;
            }
            else
            {
                summaryTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(summaryTextBox, "Summary may only consist of the letters a-z, the space character, single quotes, and minus characters.");
                _SummaryValid = false;
            }

            EnableSpiceworksButtonIfValid();

            label16.Text = String.Format("({0} - Netbook - {1} ({2}) - {3})",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"],
                (nameTextBox.Text == "") ? "<Student Name>" : nameTextBox.Text,
                (idTextBox.Text == "") ? "<ID>" : idTextBox.Text,
                (summaryTextBox.Text == "") ? "<Summary>" : summaryTextBox.Text);
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (descriptionTextBox.Text != "")
            {
                descriptionTextBox.ForeColor = Color.DarkGreen;
                _DescriptionValid = true;
            }
            else
            {
                descriptionTextBox.ForeColor = Color.DarkRed;
                _DescriptionValid = false;
            }

            EnableWarrantyButtonIfValid();
        }

        private void lwtJobIDTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsNumber(lwtJobIDTextBox.Text) || lwtJobIDTextBox.Text == "")
            {
                lwtJobIDTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(lwtJobIDTextBox, "");
                _LWTValid = true;
            }
            else
            {
                lwtJobIDTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(lwtJobIDTextBox, "LWT Job ID may only consist of the digits 0-9.");
                _LWTValid = false;
            }

            EnableSpiceworksButtonIfValid();
        }

        private void poTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsPONumber(poTextBox.Text, campusComboBox.Text) || poTextBox.Text == "")
            {
                poTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(poTextBox, "");
                _POValid = true;
            }
            else
            {
                poTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(poTextBox, "PO number for " + campusComboBox.Text + " must start with a \"" + ((campusComboBox.Text.Substring(0, 1).ToUpper() == "G") ? "P" : campusComboBox.Text.Substring(0, 1).ToUpper()) + "\".");
                _POValid = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableGenerateButtonIfValid();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsPhrase(nameTextBox.Text))
            {
                nameTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(nameTextBox, "");
                _NameValid = true;
                nameSearchButton.Enabled = true;
            }
            else
            {
                nameTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(nameTextBox, "Student name may only consist of the letters a-z, the space character, single quotes, and minus characters.");
                _NameValid = false;
                nameSearchButton.Enabled = false;
            }

            EnableSpiceworksButtonIfValid();

            EnableAddButtonIfValid();

            EnableWarrantyButtonIfValid();

            EnableGenerateButtonIfValid();

            label16.Text = String.Format("({0} - Netbook - {1} ({2}) - {3})",
                _Constants.CAMPUS_INFO[campusComboBox.SelectedIndex]["JobPrefix"],
                (nameTextBox.Text == "") ? "<Student Name>" : nameTextBox.Text,
                (idTextBox.Text == "") ? "<ID>" : idTextBox.Text,
                (summaryTextBox.Text == "") ? "<Summary>" : summaryTextBox.Text);
        }

        private void jobIDTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsNumber(jobIDTextBox.Text))
            {
                jobIDTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(jobIDTextBox, "");
                _TicketValid = true;
            }
            else
            {
                jobIDTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(jobIDTextBox, "Ticket number may only consist of the digits 0-9.");
                _TicketValid = false;
            }

            EnableAddButtonIfValid();
        }

        private void showPurchaseButton_Click(object sender, EventArgs e)
        {
            if (showPurchaseButton.Text == "Show Purchase Info >")
            {
                this.Width = 890;
                showPurchaseButton.Text = "Hide Purchase Info <";
            }
            else
            {
                this.Width = 415;
                showPurchaseButton.Text = "Show Purchase Info >";
            }
        }

        private void customCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (customCheckBox.Checked)
            {
                addButton.Enabled = false;
                spiceworksButton.Enabled = false;

                modelComboBox.Enabled = false;

                accidentRadioButton.Enabled = false;
                deliberateRadioButton.Enabled = false;
                unknownRadioButton.Enabled = false;

                labourRadioButton.Enabled = false;
                largeDeliveryRadioButton.Checked = true;

                emailTextBox.Enabled = false;
                summaryTextBox.Enabled = false;
                descriptionTextBox.Enabled = false;
                lwtJobIDTextBox.Enabled = false;
                jobIDTextBox.Enabled = false;

                nameTextBox.Enabled = false;
                idTextBox.Enabled = false;
                serialTextBox.Enabled = false;

                nameSearchButton.Enabled = false;
                idSearchButton.Enabled = false;
                serialSearchButton.Enabled = false;

                objectListView1.ClearObjects();
                objectListView1.SetObjects(_CustomParts);
            }
            else
            {
                accidentRadioButton.Enabled = true;
                deliberateRadioButton.Enabled = true;
                unknownRadioButton.Enabled = true;

                modelComboBox.Enabled = true;

                labourRadioButton.Enabled = true;
                labourRadioButton.Checked = true;

                emailTextBox.Enabled = true;
                summaryTextBox.Enabled = true;
                descriptionTextBox.Enabled = true;
                lwtJobIDTextBox.Enabled = true;
                jobIDTextBox.Enabled = true;

                nameTextBox.Enabled = true;
                idTextBox.Enabled = true;
                serialTextBox.Enabled = true;

                if (_NameValid)
                    nameSearchButton.Enabled = true;
                if (_IDValid)
                    idSearchButton.Enabled = true;
                if (_SerialValid)
                    serialSearchButton.Enabled = true;

                contextMenuStrip.Items.Clear();

                EnableSpiceworksButtonIfValid();

                EnableAddButtonIfValid();


                objectListView1.ClearObjects();

                for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                    if (modelComboBox.Text == _Constants.MODEL_INFO[i]["Name"])
                        objectListView1.SetObjects(_PartList[i]);

                //if (modelComboBox.Text == "Aspire 753")
                //    objectListView1.SetObjects(_Aspire753);
                //else if (modelComboBox.Text == "Aspire 533")
                //    objectListView1.SetObjects(_Aspire533);
                //else if (modelComboBox.Text == "Aspire 270")
                //    objectListView1.SetObjects(_Aspire270);
                //else if (modelComboBox.Text == "Aspire 255E")
                //    objectListView1.SetObjects(_Aspire255);
                //else if (modelComboBox.Text == "Travelmate B113")
                //    objectListView1.SetObjects(_TravelmateB113);
            }

            CalculateOrderTotal();



            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void CalculateOrderTotal()
        {
            float total = 0.0f;

            // Set quantity to 1 for checked items (which are currently 0), and 0 for all unchecked items.
            for (int i = 0; i < objectListView1.GetItemCount(); i++)
            {
                if (objectListView1.IsChecked(objectListView1.GetModelObject(i)))
                {
                    if (((LWTItem)objectListView1.GetModelObject(i)).Quantity == 0)
                        ((LWTItem)objectListView1.GetModelObject(i)).Quantity = 1;

                    if (!customCheckBox.Checked)
                        objectListView1.RefreshObjects(_PartList[modelComboBox.SelectedIndex]);

                        //if (modelComboBox.Text == "Aspire 753")
                        //    objectListView1.RefreshObjects(_Aspire753);
                        //else if (modelComboBox.Text == "Aspire 533")
                        //    objectListView1.RefreshObjects(_Aspire533);
                        //else if (modelComboBox.Text == "Aspire 270")
                        //    objectListView1.RefreshObjects(_Aspire270);
                        //else if (modelComboBox.Text == "Aspire 255E")
                        //    objectListView1.RefreshObjects(_Aspire255);
                        //else if (modelComboBox.Text == "Travelmate B113")
                        //    objectListView1.RefreshObjects(_TravelmateB113);
                    else
                        objectListView1.RefreshObjects(_CustomParts);

                    total += Convert.ToSingle(((LWTItem)objectListView1.GetModelObject(i)).Price.Replace("$", "")) * ((LWTItem)objectListView1.GetModelObject(i)).Quantity;
                }
                else
                {
                    ((LWTItem)objectListView1.GetModelObject(i)).Quantity = 0;

                    if (!customCheckBox.Checked)
                        objectListView1.RefreshObjects(_PartList[modelComboBox.SelectedIndex]);
                        //if (modelComboBox.Text == "Aspire 753")
                        //    objectListView1.RefreshObjects(_Aspire753);
                        //else if (modelComboBox.Text == "Aspire 533")
                        //    objectListView1.RefreshObjects(_Aspire533);
                        //else if (modelComboBox.Text == "Aspire 270")
                        //    objectListView1.RefreshObjects(_Aspire270);
                        //else if (modelComboBox.Text == "Aspire 255E")
                        //    objectListView1.RefreshObjects(_Aspire255);
                        //else if (modelComboBox.Text == "Travelmate B113")
                        //    objectListView1.RefreshObjects(_TravelmateB113);
                    else
                        objectListView1.RefreshObjects(_CustomParts);
                }
            }

            // Calculate costs of the order.
            if (objectListView1.CheckedObjects.Count > 0)
            {
                if (labourRadioButton.Checked == true)
                    total += Convert.ToSingle(_Constants.COMMON_INFO["LabourCost"]);
                else if (smallDeliveryRadioButton.Checked == true)
                    total += Convert.ToSingle(_Constants.COMMON_INFO["DeliveryCostSmall"]);
                else if (largeDeliveryRadioButton.Checked == true)
                    total += Convert.ToSingle(_Constants.COMMON_INFO["DeliveryCostLarge"]);

                totalTextBox.Text = String.Format("${0:0.00}", total);
            }
            else if (labourRadioButton.Checked == true)
                totalTextBox.Text = String.Format("${0:0.00}", _Constants.COMMON_INFO["LabourCost"]);
            else
                totalTextBox.Text = "$0.00";
        }

        private void labourRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (labourRadioButton.Checked)
                CalculateOrderTotal();

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void largeDeliveryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (largeDeliveryRadioButton.Checked)
                CalculateOrderTotal();

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void smallDeliveryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (smallDeliveryRadioButton.Checked)
                CalculateOrderTotal();

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void serialTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _SerialValid)
            {
                serialSearchButton.PerformClick();
            }

            textBox_KeyDown(sender, e);
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _NameValid)
            {
                nameSearchButton.PerformClick();
            }

            textBox_KeyDown(sender, e);
        }

        private void idTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _IDValid)
            {
                idSearchButton.PerformClick();
            }

            textBox_KeyDown(sender, e);
        }

        /// <summary>
        ///     This event prevents the annoying "Ding" sound when the return key is pressed in a textbox.
        ///     <para>Source: http://stackoverflow.com/a/14798282 </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void objectListView1_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (!objectListView1.IsChecked(objectListView1.SelectedObject))
                e.Cancel = true;
        }

        private void objectListView1_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            ((LWTItem)e.RowObject).Quantity = (uint)e.NewValue;

            if ((uint)e.NewValue == 0)
                e.ListViewItem.Checked = false;

            CalculateOrderTotal();
        }

        private void objectListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            CalculateOrderTotal();

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();
        }

        private void objectListView1_MouseEnter(object sender, EventArgs e)
        {
            _CustomToolTip.Active = false;
            _CustomToolTip.Active = true;
        }

        private void objectListView1_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the mouse button clicked was the Right mouse button.
            if (e.Button == MouseButtons.Right)
            {
                // Clear the context menu.
                contextMenuStrip.Items.Clear();

                if (customCheckBox.Checked && objectListView1.SelectedObject != null)
                {
                    // Right mouse button clicked on the custom list:

                    // Add item-specific information to the context menu.
                    contextMenuStrip.Items.Add(((LWTItem)objectListView1.SelectedObject).Description);
                    contextMenuStrip.Items[0].Enabled = false;

                    contextMenuStrip.Items.Add("-");

                    contextMenuStrip.Items.Add("Remove Selected Item");
                    contextMenuStrip.Items[2].Click += new EventHandler(RemoveMenuItem_Click);

                    contextMenuStrip.Items.Add("Add a New Item");
                    contextMenuStrip.Items[3].Click += new EventHandler(AddMenuItem_Click);
                }
                else
                {
                    // Right mouse button clicked on a netbook list:

                    // Add item-specific information to the context menu.
                    contextMenuStrip.Items.Add("Add a New Item");
                    contextMenuStrip.Items[0].Click += new EventHandler(AddMenuItem_Click);
                }

                // Display the context menu.
                contextMenuStrip.Show(objectListView1, e.Location);
            }
        }

        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            using (AddItemGUI addGUI = new AddItemGUI(this))
            {
                if (addGUI.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                    return;
            }

            objectListView1.ClearObjects();

            if (customCheckBox.Checked)
            {
                _CustomParts.Add(_ItemToAdd);
                objectListView1.SetObjects(_CustomParts);
            }
            else
            {
                for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                    if (modelComboBox.Text == _Constants.MODEL_INFO[i]["Name"])
                    {
                        _PartList[i].Add(_ItemToAdd);
                        objectListView1.SetObjects(_PartList[i]);
                    }
            }
            //else if (modelComboBox.Text == "Aspire 753")
            //{
            //    _Aspire753.Add(_ItemToAdd);
            //    objectListView1.SetObjects(_Aspire753);
            //}
            //else if (modelComboBox.Text == "Aspire 533")
            //{
            //    _Aspire533.Add(_ItemToAdd);
            //    objectListView1.SetObjects(_Aspire533);
            //}
            //else if (modelComboBox.Text == "Aspire 270")
            //{
            //    _Aspire270.Add(_ItemToAdd);
            //    objectListView1.SetObjects(_Aspire270);
            //}
            //else if (modelComboBox.Text == "Aspire 255E")
            //{
            //    _Aspire255.Add(_ItemToAdd);
            //    objectListView1.SetObjects(_Aspire255);
            //}
            //else if (modelComboBox.Text == "Travelmate B113")
            //{
            //    _TravelmateB113.Add(_ItemToAdd);
            //    objectListView1.SetObjects(_TravelmateB113);
            //}

            WriteCustomList();
        }

        private void RemoveMenuItem_Click(object sender, EventArgs e)
        {
            if (_CustomParts.Contains((LWTItem)objectListView1.SelectedObject))
                _CustomParts.Remove((LWTItem)objectListView1.SelectedObject);

            objectListView1.ClearObjects();
            objectListView1.SetObjects(_CustomParts);

            WriteCustomList();
        }

        private void WriteCustomList()
        {
            if (customCheckBox.Checked)
            {
                using (StreamWriter sw = new StreamWriter(_Constants.PART_LIST_CUSTOM))
                {
                    sw.WriteLine("LWTCode,GLCode,ProductTitle,SellPriceIncGST");

                    foreach (LWTItem item in objectListView1.Objects)
                        sw.WriteLine(String.Format("{0},{1},{2},{3:0.00}", item.ItemCode, item.GLCode, item.Description, item.PriceFloat));

                    sw.Close();
                }
            }
            else
            {
                File.AppendAllText(_Constants.PART_LIST_ADDITIONS, 
                    String.Format("{0},{1},,{2},${3:0.00}\r\n", modelComboBox.SelectedItem.ToString(), _ItemToAdd.ItemCode, _ItemToAdd.Description,
                    _ItemToAdd.PriceFloat / 11 * 10));
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            updateButton.Text = "Updating...";
            updateButton.Enabled = false;
            UpdatePartLists();

            objectListView1.ClearObjects();

            if (customCheckBox.Checked)
                objectListView1.SetObjects(_CustomParts);
            else
                for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                    if (modelComboBox.Text == _Constants.MODEL_INFO[i]["Name"])
                        objectListView1.SetObjects(_PartList[i]);
            //else if (modelComboBox.Text == "Aspire 753")
            //    objectListView1.SetObjects(_Aspire753);
            //else if (modelComboBox.Text == "Aspire 533")
            //    objectListView1.SetObjects(_Aspire533);
            //else if (modelComboBox.Text == "Aspire 270")
            //    objectListView1.SetObjects(_Aspire270);
            //else if (modelComboBox.Text == "Aspire 255E")
            //    objectListView1.SetObjects(_Aspire255);
            //else if (modelComboBox.Text == "Travelmate B113")
            //    objectListView1.SetObjects(_TravelmateB113);

            if (objectListView1.CheckedObjects.Count == 0 && !labourRadioButton.Checked)
                _PartsValid = false;
            else
                _PartsValid = true;

            EnableGenerateButtonIfValid();

            updateButton.Enabled = true;
            updateButton.Text = "Update Parts Lists";
        }

        private void warrantyButton_Click(object sender, EventArgs e)
        {
            warrantyButton.Text = "Logging Warranty Job...";
            warrantyButton.Enabled = false;

            _Ini.Load(_Constants.USER_INI_FILE);

            if (rNameTextBox.Text == "")
            {
                if (MessageBox.Show(this, "If you would like your name to be added as the contact for this job, please fill out the \"Person Requesting Order\" field in the \"Purchase Info\" section.\n" +
                    "Would you like to do that now?\n(NOTE: Selecting \"No\" will log the job under the name \"IT Tech\".)",
                        "Name Entry",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (showPurchaseButton.Text == "Show Purchase Info >")
                        showPurchaseButton.PerformClick();

                    this.ActiveControl = rNameTextBox;

                    warrantyButton.Enabled = true;
                    warrantyButton.Text = "Log Warranty Job with LWT";
                    EnableWarrantyButtonIfValid();
                    return;
                }
            }
            else if (_Ini.GetKeyValue(Environment.UserName, "RequesterName") != rNameTextBox.Text)
            {
                _Ini.SetKeyValue(Environment.UserName, "RequesterName", rNameTextBox.Text);
                _Ini.Save(_Constants.USER_INI_FILE);
            }

            int i = campusComboBox.SelectedIndex;
            string entityID = _Constants.CAMPUS_INFO[i]["EntityID"];
            string company = _Constants.CAMPUS_INFO[i]["FullName"];
            string address = _Constants.CAMPUS_INFO[i]["Address"];
            string suburb = _Constants.CAMPUS_INFO[i]["Suburb"];
            string postcode = _Constants.CAMPUS_INFO[i]["Postcode"];
            string phone = _Constants.CAMPUS_INFO[i]["Phone"];
            string modelDescription = _Constants.MODEL_INFO[modelComboBox.SelectedIndex]["LWTDescription"];

            string username = _Ini.GetKeyValue(Environment.UserName, "LWTUsername");

            while (username == "")
            {
                LWTUsernameGUI usernameGUI = new LWTUsernameGUI();

                if (usernameGUI.ShowDialog(this) == DialogResult.OK)
                {
                    _Ini.Load(_Constants.USER_INI_FILE);
                    username = _Ini.GetKeyValue(Environment.UserName, "LWTUsername");
                }
                else
                {
                    warrantyButton.Enabled = true;
                    warrantyButton.Text = "Log Warranty Job with LWT";
                    EnableWarrantyButtonIfValid();
                    return;
                }
            }

            //if (modelComboBox.SelectedIndex == 0)
            //    modelDescription = _Constants.TMB113_LWT_DESCRIPTION;
            //else if (modelComboBox.SelectedIndex == 1)
            //    modelDescription = _Constants.AO753_LWT_DESCRIPTION;
            //else if (modelComboBox.SelectedIndex == 2)
            //    modelDescription = _Constants.AO533_LWT_DESCRIPTION;
            //else if (modelComboBox.SelectedIndex == 3)
            //    modelDescription = _Constants.AOD270_LWT_DESCRIPTION;

            string warrantyURL = String.Format(_Constants.LWT_WARRANTY_URL,
                Uri.EscapeDataString(entityID),
                Uri.EscapeDataString(username),
                Uri.EscapeDataString(rNameTextBox.Text == "" ? "Local Tech" : rNameTextBox.Text),
                Uri.EscapeDataString(company),
                Uri.EscapeDataString(address),
                Uri.EscapeDataString(suburb),
                Uri.EscapeDataString(postcode),
                Uri.EscapeDataString(phone),
                Uri.EscapeDataString(emailTextBox.Text),
                Uri.EscapeDataString(serialTextBox.Text),
                Uri.EscapeDataString(modelDescription),
                Uri.EscapeDataString(_Constants.ROOM_LOCATION),
                Uri.EscapeDataString(descriptionTextBox.Text.Replace(".\r\n", " ").Replace("\r\n", ". ").Replace("\r", "").Replace("\n", "")));

            string jobID = this.DownloadWebPage(warrantyURL);

            if (!jobID.Contains("Website Under Maintenance"))
            {
                lwtJobIDTextBox.Text = jobID;
                MessageBox.Show(this, "Warranty job logged successfully with LWT.\nJob reference number: " + jobID,
                    "Warranty Job Logged Successfully",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "The warranty job was unable to be logged due to the LWT website currently being under maintenance. Please try again later.",
                    "LWT Website Under Maintenance",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            warrantyButton.Enabled = true;
            warrantyButton.Text = "Log Warranty Job with LWT";

            EnableWarrantyButtonIfValid();
        }

        private void poCreateButton_Click(object sender, EventArgs e)
        {
            poCreateButton.Text = "Generating...";
            poCreateButton.Enabled = false;

            _LabourReason = "Repair Netbook";

            if (objectListView1.CheckedObjects.Count == 0)
            {
                //Prompt for labour reason.
                LabourReasonGUI labourGUI = new LabourReasonGUI(this);
                if (labourGUI.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                {
                    poCreateButton.Enabled = true;
                    poCreateButton.Text = "Generate";

                    EnableGenerateButtonIfValid();
                }
            }
            else
            {
                _LabourReason = "Fit Parts (Above)";
            }

            MSWord msWord = new MSWord(this);
            if (msWord.GenerateOrder())
                MessageBox.Show(this, "Purchase order successfully generated.",
                    "Generation Succeeded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Purchase Order generation failed.\nPlease try again or contact your System Administrator.",
                    "Generation Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            _Ini.Load(_Constants.USER_INI_FILE);
            _Ini.SetKeyValue(Environment.UserName, "RequesterName", rNameTextBox.Text);
            _Ini.Save(_Constants.USER_INI_FILE);

            poCreateButton.Enabled = true;
            poCreateButton.Text = "Generate";

            EnableGenerateButtonIfValid();

            if (summaryTextBox.Text == "")
                this.ActiveControl = summaryTextBox;
        }

        private void rNameTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (_Validate.IsPhrase(rNameTextBox.Text))
            {
                rNameTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(rNameTextBox, "");
                _RequesterNameValid = true;
            }
            else
            {
                rNameTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(rNameTextBox, "Requester's name may only consist of the letters a-z, the space character, single quotes, and minus characters.");
                _RequesterNameValid = false;
            }

            EnableGenerateButtonIfValid();
        }

        private void EnableGenerateButtonIfValid()
        {
            // Enable the "Generate" button if all required fields are valid.
            if ((_NameValid && _IDValid && _SerialValid && _POValid && poTextBox.Text != "" && _PartsValid && _RequesterNameValid) ||
                (customCheckBox.Checked && _POValid && poTextBox.Text != "" && _PartsValid && _RequesterNameValid))
                poCreateButton.Enabled = true;
            else
                poCreateButton.Enabled = false;
        }

        private void EnableWarrantyButtonIfValid()
        {
            // Enable the LWT Warranty button if the required fields are valid.
            if (_EmailValid && _DescriptionValid && _SerialValid)
                warrantyButton.Enabled = true;
            else
                warrantyButton.Enabled = false;
        }

        private void EnableSpiceworksButtonIfValid()
        {
            // Enable the "Send to Spiceworks" button if all required fields are valid.
            if (_EmailValid && _SummaryValid && _NameValid && _IDValid && _SerialValid && _POValid && _LWTValid)
                spiceworksButton.Enabled = true;
            else
                spiceworksButton.Enabled = false;
        }

        private void EnableAddButtonIfValid()
        {
            // Enable the "Add to List" button if all required fields are valid.
            if (_NameValid && _IDValid && _TicketValid)
                addButton.Enabled = true;
            else
                addButton.Enabled = false;
        }

        // http://madskristensen.net/post/status-500-errors-when-doing-http-requests
        public string DownloadWebPage(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Headers["Accept-Encoding"] = "gzip";
                request.Headers["Accept-Language"] = "en-us";

                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.AutomaticDecompression = DecompressionMethods.Deflate;

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            catch (Exception)
            {
                return "";
            }

        } 
    }
}
