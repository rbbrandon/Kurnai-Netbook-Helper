using System;
using System.Collections.Generic;

namespace KurnaiNetbookHelper
{
    public class Constants
    {
        public Constants()
        {
            PROGRAM_NAME = "KurnaiNetbookHelper";
            COPYRIGHT = "2013, Robert Brandon";
            LWT_WARRANTY_URL = "http://www.lwt.com.au/Warranty.asp?Automated=1&EntityID={0}&txtUserName={1}&txtContactName={2}&txtContactCompany={3}&txtContactAddress={4}&txtContactSuburb={5}&txtContactPostcode={6}&txtContactPhone={7}&txtContactEmail={8}&txtSerialNumber={9}&txtProductDescription={10}&txtRoomLocation={11}&txtFaultDescription={12}";
            LWT_PRICE_LOOKUP_URL = "http://www.lwt.com.au/KurnaiPriceQuery.aspx?ProductCode={0}";
            LWT_PARTS_URL = "http://www.lwt.com.au/SparePartsPriceList.aspx";

            DATA_DIR = AppDomain.CurrentDomain.BaseDirectory + @"Data\";
            USER_INI_FILE = DATA_DIR + "UserSettings.ini";
            PROGRAM_INI_FILE = DATA_DIR + "Settings.ini";

            ORDER_TEMP = DATA_DIR + "OrderList(" + Environment.UserName + ").csv";
            ORDER_TEMPLATE = DATA_DIR + "PO Template.docx";
            ORDER_TEMPLATE_CUSTOM = DATA_DIR + "PO Template (Custom).docx";

            PART_LIST = DATA_DIR + "PartList.csv";
            PART_LIST_ADDITIONS = DATA_DIR + "PartListAdditions.csv";
            PART_LIST_CUSTOM = DATA_DIR + "PartList (" + Environment.UserName + ").csv";

            ROOM_LOCATION = "IT Office";

            JOB_TXT_FILE = "Jobs.txt";

            PAYMENTS_VALUE_DENY = 0;
            PAYMENTS_VALUE_ALLOW = 1;
            PAYMENTS_VALUE_UNKNOWN = 2;

            CSV_MARKER_REPAIR = "To Repair:";
            CSV_MARKER_COLLECT = "To Collect:";

            IniFile ini = new IniFile();
            ini.Load(PROGRAM_INI_FILE);

            // Count the number of campuses in settings.ini:
            int campusCount = 0;
            for (int i = 0; i < ini.Sections.Count; i++)
            {
                if (ini.GetSection("Campus" + i) == null)
                    break;

                campusCount += 1;
            }

            // Count the number of models in settings.ini:
            int modelCount = 0;
            for (int i = 0; i < ini.Sections.Count; i++)
            {
                if (ini.GetSection("Model" + i) == null)
                    break;

                modelCount += 1;
            }

            // Initialise dynamic settings:
            COMMON_INFO = new Dictionary<string, string>();
            CAMPUS_INFO = new Dictionary<string, string>[campusCount];
            MODEL_INFO = new Dictionary<string, string>[modelCount];

            // Read common settings from settings.ini:
            COMMON_INFO.Add("CompanyName", ini.GetKeyValue("Common", "CompanyName"));
            COMMON_INFO.Add("Domain", ini.GetKeyValue("Common", "Domain"));
            COMMON_INFO.Add("LDAPDomain", ini.GetKeyValue("Common", "LDAPDomain"));
            COMMON_INFO.Add("SMTPServer", ini.GetKeyValue("Common", "SMTPServer"));
            COMMON_INFO.Add("StudentIDFormat", ini.GetKeyValue("Common", "StudentIDFormat"));
            COMMON_INFO.Add("HelpdeskEmail", ini.GetKeyValue("Common", "HelpdeskEmail").Replace(" ", ""));
            COMMON_INFO.Add("ITManagerEmail", ini.GetKeyValue("Common", "ITManagerEmail").Replace(" ", ""));
            COMMON_INFO.Add("AdminName", ini.GetKeyValue("Common", "AdminName"));
            COMMON_INFO.Add("AdminEmail", ini.GetKeyValue("Common", "AdminEmail").Replace(" ", ""));
            COMMON_INFO.Add("STEmail", ini.GetKeyValue("Common", "STEmail").Replace(" ", ""));
            COMMON_INFO.Add("DeliveryCostSmall", ini.GetKeyValue("Common", "DeliveryCostSmall"));
            COMMON_INFO.Add("DeliveryCostLarge", ini.GetKeyValue("Common", "DeliveryCostLarge"));
            COMMON_INFO.Add("LabourCost", ini.GetKeyValue("Common", "LabourCost"));
            COMMON_INFO.Add("Proxy", ini.GetKeyValue("Common", "Proxy"));
            COMMON_INFO.Add("ProxyUsername", ini.GetKeyValue("Common", "ProxyUsername"));
            COMMON_INFO.Add("ProxyPassword", ini.GetKeyValue("Common", "ProxyPassword"));
            COMMON_INFO.Add("LoginPasswordHash", ini.GetKeyValue("Common", "LoginPasswordHash"));
            
            // Read campus-specific settings from settings.ini:
            for (int i = 0; i < campusCount; i++)
            {
                if (ini.GetSection("Campus" + i) == null)
                    break;

                CAMPUS_INFO[i] = new Dictionary<string, string>();
                CAMPUS_INFO[i].Add("Name", ini.GetKeyValue("Campus" + i, "Name"));
                CAMPUS_INFO[i].Add("FullName", COMMON_INFO["CompanyName"] + " - " + CAMPUS_INFO[i]["Name"] + " Campus");
                CAMPUS_INFO[i].Add("POFormat", ini.GetKeyValue("Campus" + i, "POFormat"));
                CAMPUS_INFO[i].Add("EntityID", ini.GetKeyValue("Campus" + i, "EntityID"));
                CAMPUS_INFO[i].Add("IntCode", ini.GetKeyValue("Campus" + i, "IntCode"));
                CAMPUS_INFO[i].Add("Address", ini.GetKeyValue("Campus" + i, "Address"));
                CAMPUS_INFO[i].Add("Suburb", ini.GetKeyValue("Campus" + i, "Suburb"));
                CAMPUS_INFO[i].Add("State", ini.GetKeyValue("Campus" + i, "State"));
                CAMPUS_INFO[i].Add("Postcode", ini.GetKeyValue("Campus" + i, "Postcode"));
                CAMPUS_INFO[i].Add("Phone", ini.GetKeyValue("Campus" + i, "Phone"));
                CAMPUS_INFO[i].Add("Fax", ini.GetKeyValue("Campus" + i, "Fax"));
                CAMPUS_INFO[i].Add("JobPrefix", ini.GetKeyValue("Campus" + i, "JobPrefix"));
                CAMPUS_INFO[i].Add("DomainController", ini.GetKeyValue("Campus" + i, "DomainController"));
                CAMPUS_INFO[i].Add("FileServer", ini.GetKeyValue("Campus" + i, "FileServer"));
                CAMPUS_INFO[i].Add("Share", ini.GetKeyValue("Campus" + i, "Share"));
                CAMPUS_INFO[i].Add("IPNetworkAddress", ini.GetKeyValue("Campus" + i, "IPNetworkAddress"));
                CAMPUS_INFO[i].Add("IPSubnetMask", ini.GetKeyValue("Campus" + i, "IPSubnetMask"));
                CAMPUS_INFO[i].Add("TechEmail", ini.GetKeyValue("Campus" + i, "TechEmail").Replace(" ", ""));
                CAMPUS_INFO[i].Add("LibraryEmail", ini.GetKeyValue("Campus" + i, "LibraryEmail").Replace(" ", ""));
            }

            // Read model-specific settings from settings.ini:
            for (int i = 0; i < modelCount; i++)
            {
                if (ini.GetSection("Model" + i) == null)
                    break;

                MODEL_INFO[i] = new Dictionary<string, string>();
                MODEL_INFO[i].Add("Name", ini.GetKeyValue("Model" + i, "Name"));
                MODEL_INFO[i].Add("Make", ini.GetKeyValue("Model" + i, "Make"));
                MODEL_INFO[i].Add("SerialFormat", ini.GetKeyValue("Model" + i, "SerialFormat"));
                MODEL_INFO[i].Add("LWTDescription", ini.GetKeyValue("Model" + i, "LWTDescription"));
            }
            
            // LANGUAGE SECTION.
            PAYMENTS_OVERDUE = "Deny Loan/Return";
            PAYMENTS_PAID = "Allow Loan/Return";

            LOGIN_TEXT = "Login";
            LOGOUT_TEXT = "Logout";

            EMAIL_SUBJECT_FORMAT = "[Ticket #{0}] {1} - Netbook - {2} ({3})";
            EMAIL_TEXT_FIXED = "Unit Fixed. Student account disabled. Waiting in the library for student collection.";
            EMAIL_TEXT_BROKEN = "Unit still broken.\r\nAccount Re-enabled as repairs are still needing done.";
            EMAIL_TEXT_COLLECTED = "Student Collected netbook.\r\nAccount Re-enabled.\r\n\r\n#close";
            // END LANGUAGE SECTION.

            // Release ini resource.
            ini = null;
        }

        public Dictionary<string, string> COMMON_INFO { get; private set; }
        public Dictionary<string, string>[] CAMPUS_INFO { get; private set; }
        public Dictionary<string, string>[] MODEL_INFO { get; private set; }

        public string COPYRIGHT { get; private set; }
        public string DATA_DIR { get; private set; }
        public string USER_INI_FILE { get; private set; }
        public string PROGRAM_INI_FILE { get; private set; }
        public string ORDER_TEMP { get; private set; }
        public string ORDER_TEMPLATE { get; private set; }
        public string ORDER_TEMPLATE_CUSTOM { get; private set; }

        public string PROGRAM_NAME { get; private set; }
        public string LWT_WARRANTY_URL { get; private set; }
        public string LWT_PRICE_LOOKUP_URL { get; private set; }
        public string LWT_PARTS_URL { get; private set; }

        public string PART_LIST { get; private set; }
        public string PART_LIST_ADDITIONS { get; private set; }
        public string PART_LIST_CUSTOM { get; private set; }

        public string ROOM_LOCATION { get; private set; }

        public string JOB_TXT_FILE { get; private set; }

        public string PAYMENTS_OVERDUE { get; private set; }
        public string PAYMENTS_PAID { get; private set; }

        public byte PAYMENTS_VALUE_DENY { get; private set; }
        public byte PAYMENTS_VALUE_ALLOW { get; private set; }
        public byte PAYMENTS_VALUE_UNKNOWN { get; private set; }

        public string LOGIN_TEXT { get; private set; }
        public string LOGOUT_TEXT { get; private set; }

        public string CSV_MARKER_REPAIR { get; private set; }
        public string CSV_MARKER_COLLECT { get; private set; }

        public string EMAIL_SUBJECT_FORMAT { get; private set; }
        public string EMAIL_TEXT_FIXED { get; private set; }
        public string EMAIL_TEXT_BROKEN { get; private set; }
        public string EMAIL_TEXT_COLLECTED { get; private set; }
    }
}
