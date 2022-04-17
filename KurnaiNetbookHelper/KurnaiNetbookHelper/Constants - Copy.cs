namespace KurnaiNetbookHelper
{
    public class Constants
    {
        public Constants()
        {
            PROGRAM_NAME = "KurnaiNetbookHelper";
            LWT_WARRANTY_URL = "http://www.lwt.com.au/Warranty.asp?Automated=1&EntityID={0}&txtUserName={1}&txtContactName={2}&txtContactCompany={3}&txtContactAddress={4}&txtContactSuburb={5}&txtContactPostcode={6}&txtContactPhone={7}&txtContactEmail={8}&txtSerialNumber={9}&txtProductDescription={10}&txtRoomLocation={11}&txtFaultDescription={12}";
            LWT_PRICE_LOOKUP_URL = "http://www.lwt.com.au/KurnaiPriceQuery.aspx?ProductCode={0}";
            LWT_PARTS_URL = "http://www.lwt.com.au/SparePartsPriceList.aspx";

            DATA_DIR = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\";
            INI_FILE = DATA_DIR + "Settings.ini";

            IniFile ini = new IniFile();
            ini.Load(INI_FILE);

            ORDER_TEMP = DATA_DIR + "OrderList(" + System.Environment.UserName + ").csv";
            ORDER_TEMPLATE = DATA_DIR + "PO Template.docx";
            ORDER_TEMPLATE_CUSTOM = DATA_DIR + "PO Template (Custom).docx";

            PROXY_ADDRESS = ini.GetKeyValue("Common", "Proxy");
            if (PROXY_ADDRESS == "")
            {
                PROXY_ADDRESS = "http://proxy.education.netspace.net.au:8080/";
                PROXY_USERNAME = "Kurnai";
                PROXY_PASSWORD = "Kurnai";
            }
            else
            {
                PROXY_USERNAME = ini.GetKeyValue("Common", "ProxyUsername");
                PROXY_PASSWORD = ini.GetKeyValue("Common", "ProxyPassword");
            }

            MORWELL_ENTITY_ID = ini.GetKeyValue("Campus0", "EntityID");
            if (MORWELL_ENTITY_ID == "")
                MORWELL_ENTITY_ID = "812";

            CHURCHILL_ENTITY_ID = ini.GetKeyValue("Campus1", "EntityID");
            if (CHURCHILL_ENTITY_ID == "")
                CHURCHILL_ENTITY_ID = "813";

            GEP_ENTITY_ID = ini.GetKeyValue("Campus2", "EntityID");
            if (GEP_ENTITY_ID == "")
                GEP_ENTITY_ID = "809";

            MORWELL_INT_CODE = "300";
            CHURCHILL_INT_CODE = "200";
            GEP_INT_CODE = "600";

            MORWELL_ADDRESS = "42-60 Bridle Rd";
            CHURCHILL_ADDRESS = "Cnr McDonald Way and Northways Road";
            GEP_ADDRESS = "Northways Road";

            MORWELL_POSTCODE = "3840";
            CHURCHILL_POSTCODE = "3842";
            GEP_POSTCODE = "3842";

            MORWELL_COMPANY_NAME = "Kurnai College - Morwell Campus";
            CHURCHILL_COMPANY_NAME = "Kurnai College - Churchill Junior Campus";
            GEP_COMPANY_NAME = "Kurnai College - Precinct Campus";

            MORWELL_SUBURB = "Morwell";
            CHURCHILL_SUBURB = "Churchill";
            GEP_SUBURB = "Churchill";

            MORWELL_PHONE = "(03) 5165 0600";
            CHURCHILL_PHONE = "(03) 5132 3700";
            GEP_PHONE = "(03) 5132 3800";

            MORWELL_FAX = "(03) 5165 0699";
            CHURCHILL_FAX = "(03) 5132 3799";
            GEP_FAX = "(03) 5132 3828";

            AO753_LWT_DESCRIPTION = "AO753 KURNAI COL MODEL";
            AO533_LWT_DESCRIPTION = "AO533 KURNAI COL MODEL";
            TMB113_LWT_DESCRIPTION = "TMB113 KURNAI COLLEGE";
            AOD270_LWT_DESCRIPTION = "AO 270 DEECD Secondary";
            AOD55_LWT_DESCRIPTION = "AO 255E DEECD Secondary";

            ROOM_LOCATION = "IT Office";

            MORWELL = "Mor";
            CHURCHILL = "Chu";
            GEP = "GEP";

            MORWELL_DC = "10.140.36.32"; // M-Server1
            CHURCHILL_DC = "10.139.8.32"; // C-Server1
            GEP_DC = "10.136.124.32"; // G-Server1

            MORWELL_SERVER = "M-Server2";
            CHURCHILL_SERVER = "C-Server2";
            GEP_SERVER = "G-Server2";

            MORWELL_SHARE = "Distribution$";
            CHURCHILL_SHARE = "Distribution$";
            GEP_SHARE = "NamesFile";

            MORWELL_IP = "10.140";
            CHURCHILL_IP = "10.139";
            GEP_IP = "10.136";

            JOB_TXT_FILE = "Jobs.txt";

            PAYMENTS_OVERDUE = "Deny Loan/Return";
            PAYMENTS_PAID = "Allow Loan/Return";

            PAYMENTS_VALUE_DENY = 0;
            PAYMENTS_VALUE_ALLOW = 1;
            PAYMENTS_VALUE_UNKNOWN = 2;

            TECH_PASSWORD_HASH = "0B78E440F8AC9C3522E4EEA717847A091D942D8E";

            LOGIN_TEXT = "Login";
            LOGOUT_TEXT = "Logout";

            DOMAIN = "KURNAI";
            DOMAIN_LDAP = "DC=KURNAI,DC=LAN";

            CSV_MARKER_REPAIR = "To Repair:";
            CSV_MARKER_COLLECT = "To Collect:";

            EMAIL_SUBJECT_FORMAT = "[Ticket #{0}] {1} - Netbook - {2} ({3})";
            EMAIL_TEXT_FIXED = "Unit Fixed. Student account disabled. Waiting in the library for student collection.";
            EMAIL_TEXT_BROKEN = "Unit still broken.\r\nAccount Re-enabled as repairs are still needing done.";
            EMAIL_TEXT_COLLECTED = "Student Collected netbook.\r\nAccount Re-enabled.\r\n\r\n#close";
            EMAIL_SMTP_SERVER = "10.10.20.22";

            EMAIL_ADDRESS_HELPDESK = "helpdesk@kurnaicollege.vic.edu.au";
            EMAIL_ADDRESS_ADMIN = "wilson.phyllis.p@edumail.vic.gov.au";

            MORWELL_TECH = "brandon.robert.b@edumail.vic.gov.au";
            CHURCHILL_TECH = "haagsma.trevor.j@edumail.vic.gov.au";
            GEP_TECH = "hutchence.rick.r@edumail.vic.gov.au";
            IT_MANAGER = "gowing.dean.d@edumail.vic.gov.au";
            SPECIAL_TECH_1 = "zagar.rocky.r@edumail.vic.gov.au";
            SPECIAL_TECH_2 = "hutchence.rick.r@edumail.vic.gov.au";
            SPECIAL_TECH = new string[2] { SPECIAL_TECH_1, SPECIAL_TECH_2 };
            MORWELL_LIBRARIANS = new string[2] { "kennedy.kaye.k@edumail.vic.gov.au", "webb.alison.m@edumail.vic.gov.au" };

            DELIVERY_COST_SMALL = 16.5f;
            DELIVERY_COST_LARGE = 22.0f;
            LABOUR_COST = 38.5f;
        }

        // Not technically constants, but values won't change during runtime.
        //public string DATA_DIR = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\";
        //public string INI_FILE = DATA_DIR + "Settings.ini";
        //public string ORDER_TEMP = DATA_DIR + "OrderList(" + System.Environment.UserName + ").csv";
        //public string ORDER_TEMPLATE = DATA_DIR + "PO Template.docx";
        //public string ORDER_TEMPLATE_CUSTOM = DATA_DIR + "PO Template (Custom).docx";
        public string DATA_DIR { get; private set; }
        public string INI_FILE { get; private set; }
        public string ORDER_TEMP { get; private set; }
        public string ORDER_TEMPLATE { get; private set; }
        public string ORDER_TEMPLATE_CUSTOM { get; private set; }
        // End "kinda-constants".

        public string PROXY_ADDRESS { get; private set; }
        public string PROXY_USERNAME { get; private set; }
        public string PROXY_PASSWORD { get; private set; }

        public string PROGRAM_NAME { get; private set; }
        public string LWT_WARRANTY_URL { get; private set; }
        public string LWT_PRICE_LOOKUP_URL { get; private set; }
        public string LWT_PARTS_URL { get; private set; }

        public string[][] ENTITY_ID { get; private set; }
        public string MORWELL_ENTITY_ID { get; private set; }
        public string CHURCHILL_ENTITY_ID { get; private set; }
        public string GEP_ENTITY_ID { get; private set; }

        public string MORWELL_INT_CODE { get; private set; }
        public string CHURCHILL_INT_CODE { get; private set; }
        public string GEP_INT_CODE { get; private set; }

        public string MORWELL_ADDRESS { get; private set; }
        public string CHURCHILL_ADDRESS { get; private set; }
        public string GEP_ADDRESS { get; private set; }

        public string MORWELL_POSTCODE { get; private set; }
        public string CHURCHILL_POSTCODE { get; private set; }
        public string GEP_POSTCODE { get; private set; }

        public string MORWELL_COMPANY_NAME { get; private set; }
        public string CHURCHILL_COMPANY_NAME { get; private set; }
        public string GEP_COMPANY_NAME { get; private set; }

        public string MORWELL_SUBURB { get; private set; }
        public string CHURCHILL_SUBURB { get; private set; }
        public string GEP_SUBURB { get; private set; }

        public string MORWELL_PHONE { get; private set; }
        public string CHURCHILL_PHONE { get; private set; }
        public string GEP_PHONE { get; private set; }

        public string MORWELL_FAX { get; private set; }
        public string CHURCHILL_FAX { get; private set; }
        public string GEP_FAX { get; private set; }

        public string AO753_LWT_DESCRIPTION { get; private set; }
        public string AO533_LWT_DESCRIPTION { get; private set; }
        public string TMB113_LWT_DESCRIPTION { get; private set; }
        public string AOD270_LWT_DESCRIPTION { get; private set; }
        public string AOD55_LWT_DESCRIPTION { get; private set; }

        public string ROOM_LOCATION { get; private set; }

        public string MORWELL { get; private set; }
        public string CHURCHILL { get; private set; }
        public string GEP { get; private set; }

        public string MORWELL_DC { get; private set; }
        public string CHURCHILL_DC { get; private set; }
        public string GEP_DC { get; private set; }

        public string MORWELL_SERVER { get; private set; }
        public string CHURCHILL_SERVER { get; private set; }
        public string GEP_SERVER { get; private set; }

        public string MORWELL_SHARE { get; private set; }
        public string CHURCHILL_SHARE { get; private set; }
        public string GEP_SHARE { get; private set; }

        public string MORWELL_IP { get; private set; }
        public string CHURCHILL_IP { get; private set; }
        public string GEP_IP { get; private set; }

        public string JOB_TXT_FILE { get; private set; }

        public string PAYMENTS_OVERDUE { get; private set; }
        public string PAYMENTS_PAID { get; private set; }

        public byte PAYMENTS_VALUE_DENY { get; private set; }
        public byte PAYMENTS_VALUE_ALLOW { get; private set; }
        public byte PAYMENTS_VALUE_UNKNOWN { get; private set; }

        public string TECH_PASSWORD_HASH { get; private set; }

        public string LOGIN_TEXT { get; private set; }
        public string LOGOUT_TEXT { get; private set; }

        public string DOMAIN { get; private set; }
        public string DOMAIN_LDAP { get; private set; }

        public string CSV_MARKER_REPAIR { get; private set; }
        public string CSV_MARKER_COLLECT { get; private set; }

        public string EMAIL_SUBJECT_FORMAT { get; private set; }
        public string EMAIL_TEXT_FIXED { get; private set; }
        public string EMAIL_TEXT_BROKEN { get; private set; }
        public string EMAIL_TEXT_COLLECTED { get; private set; }
        public string EMAIL_SMTP_SERVER { get; private set; }

        public string EMAIL_ADDRESS_HELPDESK { get; private set; }
        public string EMAIL_ADDRESS_ADMIN { get; private set; }

        public string MORWELL_TECH { get; private set; }
        public string CHURCHILL_TECH { get; private set; }
        public string GEP_TECH { get; private set; }
        public string IT_MANAGER { get; private set; }
        public string SPECIAL_TECH_1 { get; private set; }
        public string SPECIAL_TECH_2 { get; private set; }
        public string[] SPECIAL_TECH { get; private set; }
        public string[] MORWELL_LIBRARIANS { get; private set; }

        public float DELIVERY_COST_SMALL { get; private set; }
        public float DELIVERY_COST_LARGE { get; private set; }
        public float LABOUR_COST { get; private set; }
    }
}
