using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Xml;
using System.Management;
using System.IO;
using System.Configuration;
using System.Web.Configuration;
using System.Text;
using Microsoft.ManagementConsole;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;





namespace Frontend
{
    /// <summary>
    /// Summary description for ConfigMgr 2020 R2
    /// </summary>
    [WebService(Name = "ZTI Frontend", Description = "Frontend for ConfigMgr 2012 developed by Johan Arwidmark", Namespace = "http://www.deploymentresearch.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // [System.Web.Script.Services.ScriptService]
    public class ConfigMgr : System.Web.Services.WebService
    {
        #region Instance variables
        
        String ConfigMgrSiteServer;
        String ConfigMgrSiteCode;
        String LDAPStartingOU;
        String FQDNDomainName;
        String CMDBConnectionString;
       
        #endregion


        #region Constructor
 
        public ConfigMgr()
        {

            // Read the Frontend parameters from web.config
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: Read the Frontend parameters from web.config");
            
            Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(null);
 
            ConfigMgrSiteServer = WebConfigurationManager.AppSettings["ConfigMgrSiteServer"];
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: ConfigMgriteServer value from web.config is: " + ConfigMgrSiteServer);
            ConfigMgrSiteCode = WebConfigurationManager.AppSettings["ConfigMgrSiteCode"];
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: ConfigMgrSiteCode value from web.config is: " + ConfigMgrSiteCode);
            FQDNDomainName = WebConfigurationManager.AppSettings["FQDNDomainName"];
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: FQDNDomainName value from web.config is: " + FQDNDomainName);
            LDAPStartingOU = WebConfigurationManager.AppSettings["LDAPStartingOU"];
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: LDAPStartingOU value from web.config is: " + LDAPStartingOU);

            // Build the Connection string for the ConfigMgr database
            CMDBConnectionString = "Network Library=DBMSSOCN;Data Source=" + ConfigMgrSiteServer + ";Initial Catalog=CM_" + ConfigMgrSiteCode + ";Integrated Security=SSPI";
            Trace.WriteLine(DateTime.Now + ": ConfigMgr: CMDBConnectionString is: " + CMDBConnectionString);

        }


        #endregion


        #region Web methods

        
        [WebMethod]
        public string GetOUList()
        {

            Trace.WriteLine(DateTime.Now + ": GetOUList: Starting Web Service");

            string LDAPDomainName = "DC=" + FQDNDomainName.Replace(".", ",DC=");
            Trace.WriteLine(DateTime.Now + ": GetOUList: Connecting to the " + FQDNDomainName + " domain.");
            
            string LDAPPath = string.Empty;
            if (LDAPStartingOU == string.Empty)
            {
                Trace.WriteLine(DateTime.Now + ": GetOUList: LDAPStartingOU is blank ");
                LDAPPath = "LDAP://" + LDAPDomainName;
            }
            else
            {
                Trace.WriteLine(DateTime.Now + ": GetOUList: LDAPStartingOU is set to: " + LDAPStartingOU);
                LDAPPath = "LDAP://" + LDAPStartingOU + "," + LDAPDomainName;
            }


            Trace.WriteLine(DateTime.Now + ": GetOUList: LDAPPath is: " + LDAPPath);


            try
            {
                // Connect to AD and get OU List
                DirectoryEntry rootEntry = new
                DirectoryEntry(LDAPPath);
                DirectorySearcher dsFindOUs = new DirectorySearcher(rootEntry);

                dsFindOUs.Filter = "(objectClass=organizationalUnit)";
                dsFindOUs.SearchScope = SearchScope.Subtree;
                dsFindOUs.PropertiesToLoad.Add("Name");
                dsFindOUs.PropertyNamesOnly = true;
                dsFindOUs.Sort.Direction = SortDirection.Ascending;
                dsFindOUs.Sort.PropertyName = "Name";
               
                // Create a new XmlDocumnt object
                XmlDocument XMLOUList = new XmlDocument();
                                
                // Create the root node
                XmlNode root = XMLOUList.CreateElement("xml");
                XMLOUList.AppendChild(root);

                foreach(SearchResult result in dsFindOUs.FindAll() )
                {

                // Create an element "OUListItem"
                XmlElement OUListItem = XMLOUList.CreateElement("OUListItem");

                // Create the Name attribute, and set its value
                XmlAttribute Name = XMLOUList.CreateAttribute("Name");
                Name.Value = result.GetDirectoryEntry().Properties["Name"].Value.ToString();

                // Add the attribute to the element
                OUListItem.Attributes.Append(Name);

                // Create the distinguishedName attribute, and set its value
                XmlAttribute distinguishedName = XMLOUList.CreateAttribute("distinguishedName");
                distinguishedName.Value = result.GetDirectoryEntry().Properties["distinguishedName"].Value.ToString();

                // Add the attribute to the element
                OUListItem.Attributes.Append(distinguishedName);

                // Add "OUListItem" to root
                root.AppendChild(OUListItem);

                }

                Trace.WriteLine(DateTime.Now + ": GetOUList: Returning Active Directory OU List.");
                return XMLOUList.InnerXml;

            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + ": GetOUList: Unhandled exception finding provider namespace on server " + e.ToString());
                return DateTime.Now + ": GetOUList: Connection could not be made to Active Directory";
                
            }

        }

        [WebMethod]
        public Boolean MoveComputerToOU(String MACHINEOBJECTOU, String OSDComputerName)
        {

            Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Starting Web Service");
            Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: MACHINEOBJECTOU received was: " + MACHINEOBJECTOU);
            Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: OSDComputerName received was: " + OSDComputerName);

            String CurrentOU = string.Empty;
            Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Connecting to " + FQDNDomainName + ".");

            try
            {

                // Connect to AD
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, FQDNDomainName);

                string controller = AD.ConnectedServer;
                Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Connected to " + string.Format("Domain Controller: {0}", controller));

                ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(AD, OSDComputerName);

                if (computer != null)
                {
                    Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Machine found in AD, continue.");
                    // Get Parent OU 
                    DirectoryEntry deComputer = computer.GetUnderlyingObject() as DirectoryEntry;
                    DirectoryEntry deComputerContainer = deComputer.Parent;

                    CurrentOU = string.Format("{0}".Trim(), deComputerContainer.Properties["distinguishedName"].Value);

                    Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: CurrentOU is " + CurrentOU);

                    // Verify if the selected OU is the same as the current OU
                    if (String.Equals(MACHINEOBJECTOU, CurrentOU, StringComparison.OrdinalIgnoreCase))
                    {
                        Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Selected OU is the same as current OU, or machine does not exist in AD, do nothing ");
                    }
                    else
                    {
                        Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Selected OU is not the same as currentOU, move computer to selected OU ");

                        // Move the computer object
                        DirectoryEntry NewParent = new DirectoryEntry("LDAP://" + MACHINEOBJECTOU);
                        deComputer.MoveTo(NewParent);
                    }
                }
                else
                {
                    Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Machine not found in AD, assuming new machine, skipping move operation.");
                }

            }


            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + ": MoveComputerToOU: Unhandled exception finding provider namespace on server " + e.ToString());
                return false;

            }


            return true;

        }

        [WebMethod]
        public String GetTaskSequenceList()
        {

            Trace.WriteLine(DateTime.Now + ": GetTaskSequenceList: Starting Web Service");

            // Connect to Database Server
            
            try
			{
                SqlConnection conn = new SqlConnection(CMDBConnectionString);

				conn.Open();
		 	    SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Select AdvertisementID,PackageName FROM v_AdvertisementInfo WHERE PackageID IN(SELECT PkgID FROM vSMS_TaskSequencePackage Where TS_Type = 2) AND CollectionID = 'SMS000US' ";
                Trace.WriteLine(DateTime.Now + ": GetTaskSequenceList: SQL String is: " + cmd.CommandText);
			    SqlDataReader thisReader = cmd.ExecuteReader();

                // Create a new XmlDocumnt object
                XmlDocument XMLTaskSequenceList = new XmlDocument();

                // Create the root node
                XmlNode root = XMLTaskSequenceList.CreateElement("xml");
                XMLTaskSequenceList.AppendChild(root);


				while (thisReader.Read())
				{

                    // Create an element "TSListItem"
                    XmlElement TSListItem = XMLTaskSequenceList.CreateElement("TSListItem");

                    // Create the "CollectionID" attribute, and set its value
                    XmlAttribute CollectionID = XMLTaskSequenceList.CreateAttribute("AdvertisementID");
                    CollectionID.Value = thisReader["AdvertisementID"].ToString();

                    // Add the "CollectionID" attribute to the element
                    TSListItem.Attributes.Append(CollectionID);

                    // Create the "Name" attribute, and set its value
                    XmlAttribute Name = XMLTaskSequenceList.CreateAttribute("PackageName");
                    Name.Value = thisReader["PackageName"].ToString();

                    // Add the "Name" attribute to the element
                    TSListItem.Attributes.Append(Name);

                    // Add "TSListItem" to root
                    root.AppendChild(TSListItem);

		 		}
				thisReader.Close();
				conn.Close();

                return XMLTaskSequenceList.InnerXml;

			}
			catch (SqlException e)
			{
                Trace.WriteLine(DateTime.Now + ": GetTaskSequenceList: Unhandled exception finding provider namespace on server " + e.ToString());
                return DateTime.Now + ": GetTaskSequenceList: Connection could not be made to the ConfigMgr Database";
            }

            
            

        }







        #endregion
    }
}
