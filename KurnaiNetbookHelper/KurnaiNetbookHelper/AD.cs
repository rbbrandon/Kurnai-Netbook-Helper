using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace KurnaiNetbookHelper
{
    // Source: http://www.codeproject.com/Articles/18102/Howto-Almost-Everything-In-Active-Directory-via-C
    class AD
    {
        private Constants _Constants = new Constants();
        public string Server { get; set; }
        public enum objectClass
        {
            user, group, computer
        }
        public enum returnType
        {
            distinguishedName, ObjectGUID, sAMAccountName, managedBy, serialNumber
        }

        /// <summary>
        ///     Tests whether a user account is valis (i.e. successfully authenticates against the domain).
        /// </summary>
        /// <param name="userName">The username to connect to the domain controller with.</param>
        /// <param name="password">The password to connect to the domain controller with.</param>
        /// <param name="domain">The LDAP domain to connect to for authentication.</param>
        /// <returns>True/False depending on whether the credentials are valid or not.</returns>
        public bool Authenticate(string userName, string password, string domain)
        {
            bool authentic = false;
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + Server + "/" + domain,
                    userName, password))
                {
                    object nativeObject = entry.NativeObject;
                    authentic = true;
                }
            }
            catch (Exception) { }
            //catch (DirectoryServicesCOMException) { }
            return authentic;
        }

        /// <summary>
        ///     Gets an account's distinguishedName.
        /// </summary>
        /// <param name="objectCls">objectClass.user</param>
        /// <param name="returnValue">returnType.distinguishedName</param>
        /// <param name="objectName">Account's name (sAMAccountName)</param>
        /// <param name="LdapDomain">The domain the account is on (e.g. kurnai.lan)</param>
        /// <returns>String containing the object's distinquishedName/GUID.</returns>
        public string GetObjectDistinguishedName(objectClass objectCls,
            returnType returnValue, string objectName, string LdapDomain)
        {
            return GetObjectDistinguishedName(objectCls, returnValue, objectName, LdapDomain, "", "");
        }

        /// <summary>
        ///     Gets an account's distinguishedName.
        /// </summary>
        /// <param name="objectCls">objectClass.user</param>
        /// <param name="returnValue">returnType.distinguishedName</param>
        /// <param name="objectName">Account's name (sAMAccountName)</param>
        /// <param name="LdapDomain">The domain the account is on (e.g. kurnai.lan)</param>
        /// <param name="username">The username to connect to the domain controller with.</param>
        /// <param name="password">The password to connect to the domain controller with.</param>
        /// <returns>String containing the object's distinquishedName/GUID.</returns>
        public string GetObjectDistinguishedName(objectClass objectCls,
            returnType returnValue, string objectName, string LdapDomain, string username, string password)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = (username == "") ? "LDAP://" + LdapDomain : "LDAP://" + Server + "/" + LdapDomain;

            using (DirectoryEntry entry = (username == "") ? new DirectoryEntry(connectionPrefix) : new DirectoryEntry(connectionPrefix, username, password))
            {
                using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
                {
                    switch (objectCls)
                    {
                        case objectClass.user:
                            mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                            break;
                        case objectClass.group:
                            mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                            break;
                        case objectClass.computer:
                            mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                            break;
                    }
                    
                    SearchResult result = mySearcher.FindOne();

                    if (result == null)
                    {
                        throw new NullReferenceException
                        ("Unable to locate the distinguishedName for the object " +
                        objectName + " in the " + LdapDomain + " domain.");
                    }

                    using (DirectoryEntry directoryObject = result.GetDirectoryEntry())
                    {
                        if (returnValue.Equals(returnType.distinguishedName))
                        {
                            distinguishedName = "LDAP://" + directoryObject.Properties
                                ["distinguishedName"].Value;
                        }

                        if (returnValue.Equals(returnType.ObjectGUID))
                        {
                            distinguishedName = directoryObject.Guid.ToString();
                        }
                    }
                }
            }

            return distinguishedName;
        }

        /// <summary>
        ///     Searches AD for users/computers matching a specific criteria.
        /// </summary>
        /// <param name="objectCls">The ObjectClass to look for.</param>
        /// <param name="returnValue">The value you would like to be returned.</param>
        /// <param name="objectName">The object to look up.</param>
        /// <returns>The values you asked for in an array.</returns>
        public string[,] SearchAD(objectClass objectCls, returnType returnValue, string objectName)
        {
            return SearchAD(objectCls, returnValue, objectName, "", "");
        }

        /// <summary>
        ///     Searches AD for users/computers matching a specific criteria.
        /// </summary>
        /// <param name="objectCls">The ObjectClass to look for.</param>
        /// <param name="returnValue">The value you would like to be returned.</param>
        /// <param name="objectName">The object to look up.</param>
        /// <param name="username">The username to connect to the domain controller with.</param>
        /// <param name="password">The password to connect to the domain controller with.</param>
        /// <returns>The values you asked for in an array.</returns>
        public string[,] SearchAD(objectClass objectCls, returnType returnValue, string objectName, string username, string password)
        {
            string LdapDomain = _Constants.COMMON_INFO["LDAPDomain"];
            string distinguishedName = string.Empty;
            string[,] returnArray = null;
            string connectionPrefix = (username == "") ? "LDAP://" + LdapDomain : "LDAP://" + Server + "/" + LdapDomain;
            
            try
            {
                using (DirectoryEntry entry = (username == "") ? new DirectoryEntry(connectionPrefix) : new DirectoryEntry(connectionPrefix, username, password))
                {
                    using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
                    {
                        switch (objectCls)
                        {
                            case objectClass.user:
                                mySearcher.Filter = "(&(objectClass=user)(cn=*" + objectName.Replace(' ', '*') + "*))";
                                break;
                            case objectClass.computer:
                                mySearcher.Filter = "(&(objectClass=computer)(|(managedBy=" + objectName.Replace("LDAP://" + Server + "/", "").Replace("LDAP://", "") + ")(serialNumber=" + objectName + ")))";
                                break;
                            default:
                                return returnArray;
                        }

                        SearchResultCollection resultList = mySearcher.FindAll();
                        DirectoryEntry[] newArray = null;

                        if (resultList != null && resultList.Count > 0)
                        {
                            newArray = new DirectoryEntry[resultList.Count];

                            for (int count = 0; count < resultList.Count; count++)
                            {
                                newArray[count] = resultList[count].GetDirectoryEntry();
                            }

                            // Only select users that are in years 10-12 (or inactive), and computers.
                            if (objectCls == objectClass.user)
                                newArray = Array.FindAll(newArray, x => !x.Properties["sAMAccountName"].Value.ToString().Contains("$") &&
                                    x.Properties["distinguishedName"].Value.ToString().Contains("OU=Students") &&
                                    (x.Properties["distinguishedName"].Value.ToString().Contains("OU=Year10") ||
                                    x.Properties["distinguishedName"].Value.ToString().Contains("OU=Year 11") ||
                                    x.Properties["distinguishedName"].Value.ToString().Contains("OU=Year 12") ||
                                    x.Properties["distinguishedName"].Value.ToString().Contains("OU=Inactive")));
                            else
                                newArray = Array.FindAll(newArray, x => x.Properties["sAMAccountName"].Value.ToString().Contains("$"));
                        }

                        if (newArray != null && newArray.GetLength(0) > 0)
                        {
                            returnArray = new string[newArray.GetLength(0), 2];

                            for (int count = 0; count < newArray.GetLength(0); count++)
                            {
                                using (DirectoryEntry directoryObject = newArray[count])
                                {
                                    switch (objectCls)
                                    {
                                        case objectClass.user:
                                            object givenName = directoryObject.Properties["givenName"].Value;
                                            object sn = directoryObject.Properties["sn"].Value;

                                            if (givenName == null)
                                                givenName = "";

                                            if (sn == null)
                                                sn = "";

                                            // Shorten distinguishedName to omit the "CN=" part, since we are including the name in a specific format.
                                            string shortDistinguishedName = directoryObject.Properties["distinguishedName"].Value.ToString();
                                            shortDistinguishedName = shortDistinguishedName.Substring(shortDistinguishedName.IndexOf("OU="));

                                            returnArray[count, 0] = String.Format("{0} {1} ({2}) - {3}",
                                                givenName,
                                                sn,
                                                directoryObject.Properties["sAMAccountName"].Value.ToString(),
                                                shortDistinguishedName);
                                            break;
                                        case objectClass.computer:
                                            object description = directoryObject.Properties["description"].Value;
                                            object serialNumber = directoryObject.Properties["serialNumber"].Value;

                                            if (description == null)
                                                description = "";

                                            if (serialNumber == null)
                                                serialNumber = "";

                                            returnArray[count, 0] = String.Format("{0} ({1}) - {2}",
                                                directoryObject.Properties["sAMAccountName"].Value.ToString(),
                                                serialNumber,
                                                description);
                                            break;
                                    }

                                    switch (returnValue)
                                    {
                                        case returnType.distinguishedName:
                                            if (directoryObject.Properties["distinguishedName"].Value != null)
                                                returnArray[count, 1] = directoryObject.Properties["distinguishedName"].Value.ToString();
                                            else
                                                returnArray[count, 1] = "";
                                            break;
                                        case returnType.managedBy:
                                            if (directoryObject.Properties["managedBy"].Value != null)
                                                returnArray[count, 1] = directoryObject.Properties["managedBy"].Value.ToString();
                                            else
                                                returnArray[count, 1] = "";
                                            break;
                                        case returnType.ObjectGUID:
                                            if (directoryObject.Guid != null)
                                                returnArray[count, 1] = directoryObject.Guid.ToString();
                                            else
                                                returnArray[count, 1] = "";
                                            break;
                                        case returnType.sAMAccountName:
                                            if (directoryObject.Properties["sAMAccountName"].Value != null)
                                                returnArray[count, 1] = directoryObject.Properties["sAMAccountName"].Value.ToString();
                                            else
                                                returnArray[count, 1] = "";
                                            break;
                                        case returnType.serialNumber:
                                            if (directoryObject.Properties["serialNumber"].Value != null)
                                                returnArray[count, 1] = directoryObject.Properties["serialNumber"].Value.ToString();
                                            else
                                                returnArray[count, 1] = "";
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string returnTypeString = "";

                            switch (returnValue)
                            {
                                case returnType.distinguishedName:
                                    returnTypeString = "distinquishedName";
                                    break;
                                case returnType.ObjectGUID:
                                    returnTypeString = "ObjectGUID";
                                    break;
                                case returnType.sAMAccountName:
                                    returnTypeString = "sAMAccountName";
                                    break;
                                case returnType.managedBy:
                                    returnTypeString = "managedBy";
                                    break;
                                case returnType.serialNumber:
                                    returnTypeString = "serialNumber";
                                    break;
                            }

                            if (objectCls == objectClass.computer && returnValue == returnType.serialNumber)
                            {
                                throw new NullReferenceException
                                    ("unable to locate the " + returnTypeString + " attribute for the machine currently managed by " +
                                    objectName.Replace("LDAP://", "") + ".");
                            }
                            else
                            {
                                throw new NullReferenceException
                                    ("unable to locate the " + returnTypeString + " attribute for the object " +
                                    objectName.Replace("LDAP://", "") + " in the " + LdapDomain + " domain");
                            }
                        }
                    }

                    entry.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Encountered an error:\r\n{0}", e.Message),
                        "AD Lookup Error"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            }

            return returnArray;
        }

        /// <summary>
        ///     Enables a user account.
        /// </summary>
        /// <param name="userDn">The user account's DistinguishedName to enable in AD.</param>
        public void Enable(string userDn)
        {
            Enable(userDn, "", "");
        }

        /// <summary>
        ///     Enables a user account.
        /// </summary>
        /// <param name="userDn">The user account's DistinguishedName to enable in AD.</param>
        /// <param name="username">The username to connect to AD with.</param>
        /// <param name="password">The password to connect to AD with.</param>
        public void Enable(string userDn, string username, string password)
        {
            userDn = (username == "" || userDn.Contains("LDAP://" + Server + "/")) ? userDn : userDn.Replace("LDAP://", "LDAP://" + Server + "/");

            try
            {
                using (DirectoryEntry user = (username == "") ? new DirectoryEntry(userDn) : new DirectoryEntry(userDn, username, password))
                {
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val & ~0x2;
                    //ADS_UF_NORMAL_ACCOUNT;

                    user.CommitChanges();
                    user.Close();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(String.Format("Encountered an error while enabling {0}'s account:\r\n{1}", userDn, E.Message),
                        "Cannot Enable Account"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Disables a user account.
        /// </summary>
        /// <param name="userDn">The user account's DistinguishedName to disable in AD.</param>
        public void Disable(string userDn)
        {
            Disable(userDn, "", "");
        }

        /// <summary>
        ///     Disables a user account.
        /// </summary>
        /// <param name="userDn">The user account's DistinguishedName to disable in AD.</param>
        /// <param name="username">The username to connect to AD with.</param>
        /// <param name="password">The password to connect to AD with.</param>
        public void Disable(string userDn, string username, string password)
        {
            userDn = (username == "" || userDn.Contains("LDAP://" + Server + "/")) ? userDn : userDn.Replace("LDAP://", "LDAP://" + Server + "/");

            try
            {
                using (DirectoryEntry user = (username == "") ? new DirectoryEntry(userDn) : new DirectoryEntry(userDn, username, password))
                {
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val | 0x2;
                    //ADS_UF_ACCOUNTDISABLE;

                    user.CommitChanges();
                    user.Close();
                }
            }
            catch (Exception E)
            {
                //DoSomethingWith --> E.Message.ToString();
                MessageBox.Show(String.Format("Encountered an error while disabling {0}'s account:\r\n{1}", userDn, E.Message),
                        "Cannot Disable Account"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Get an object's value.
        /// </summary>
        /// <param name="attributeName">The attribute to get.</param>
        /// <param name="objectDn">The distinguishedName of the object to get the attribute's value of.</param>
        /// <returns>Specified attribute's value.</returns>
        public string AttributeValuesSingleString(string attributeName, string objectDn)
        {
            return AttributeValuesSingleString(attributeName, objectDn, "", "");
        }

        /// <summary>
        ///     Get an object's value.
        /// </summary>
        /// <param name="attributeName">The attribute to get.</param>
        /// <param name="objectDn">The distinguishedName of the object to get the attribute's value of.</param>
        /// <param name="username">The username to connect to AD with.</param>
        /// <param name="password">The password to connect to AD with.</param>
        /// <returns>Specified attribute's value.</returns>
        public string AttributeValuesSingleString(string attributeName, string objectDn, string username, string password)
        {
            string strValue = "";
            objectDn = (username == "" || objectDn.Contains("LDAP://" + Server + "/")) ? objectDn : objectDn.Replace("LDAP://", "LDAP://" + Server + "/");
            
            try
            {
                // Returns error if user is not on the domain.
                using (DirectoryEntry ent = (username == "") ? new DirectoryEntry(objectDn) : new DirectoryEntry(objectDn, username, password))
                {
                    // Returns error if serial number is not found to have an owner.
                    strValue = ent.Properties[attributeName].Value.ToString();
                    ent.Close();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Encountered an error while looking up the {0} attribute for {1}:\r\n{2}", attributeName, objectDn, e.Message),
                        "AD Lookup Failed"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
            }

            return strValue;
        }
    }
}
