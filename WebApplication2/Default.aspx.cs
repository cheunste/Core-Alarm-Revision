using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
//using Oracle.DataAccess.Client;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

// This is a class that's used to essentually map a computer name, with a node number and a sitename and god knows what else
public class SiteStructure
{

    public SiteStructure(string computerName, int nodeNumber, string siteName, string sitePrefix)
    {
        this.computerName = computerName;
        this.nodeNumber = nodeNumber;
        this.siteName = siteName;
        this.sitePrefix = sitePrefix;

    }

    public string computerName { get; set; }
    public int nodeNumber { get; set; }
    public string siteName { get; set; }
    public string sitePrefix { get; set; }
}

public static class SiteFactory
{
    private static List<SiteStructure> siteList = new List<SiteStructure>();
    private static Dictionary<int, string> siteCacheDict = new Dictionary<int, string>();
    private static Dictionary<string, string> sitePrefixDict = new Dictionary<string, string>();
    private static Dictionary<string, string> specialFilter = new Dictionary<string, string>();


    /// <summary>
    /// Create a new Site class given the a computer name, node number and two string arrays site Name and site prefix
    /// </summary>
    /// <param name="computerName">The host name of the machine</param>
    /// <param name="nodeNumber">The node number of the machine</param>
    /// <param name="siteName">The name of the site. Because there can be muliple sites (ie BB1,BB2), this is a string array</param>
    /// <param name="sitePrefix">The prefix of the site. Because there canb e multiple sites, this is a string array. The positions MUST MATCH the site Name</param>
    public static void createNewSite(string computerName, int nodeNumber, string[] siteName, string[] sitePrefix)
    {
        int temp = siteName.Length;
        for (int i = 0; i < temp; i++)
        {
            SiteStructure s = new SiteStructure(computerName, nodeNumber, siteName[i], sitePrefix[i]);

            //Oddly enough, I can't adccess Dictionary.TryAdd() method
            try
            {
                sitePrefixDict.Add(siteName[i], sitePrefix[i]);
            }
            catch (Exception e)
            {
                //Ignore exception
            }
            siteList.Add(s);

        }

    }

    /// <summary>
    /// Create a filter. Takes a name of a filter and its respective SQL query command string 
    /// </summary>
    /// <param name="filterName">Name of the filter</param>
    /// <param name="command">The SQL command. There must be NO SPACES before and after the command</param>
    public static void createNewFilter(string filterName, string command)
    {
        try
        {
            specialFilter.Add(filterName, " " + command + " ");
        }
        catch (Exception e)
        {

        }
    }

    /// <summary>
    /// REturn the dictioary of special filters created by createNewFilter function 
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, string> getFilters()
    {
        return specialFilter;
    }

    /// <summary>
    /// Clear all dictionaries in the class
    /// </summary>
    public static void clearDictionaries()
    {
        siteCacheDict.Clear();
        specialFilter.Clear();
    }
    public static List<SiteStructure> getSiteList()
    {
        return siteList;
    }
    public static string getPrefixFromSiteName(string siteName)
    {
        string sitePrefixes = "";
        sitePrefixDict.TryGetValue(siteName, out sitePrefixes);
        return sitePrefixes;
    }

    public static string getComputerName(int nodeNumber)
    {

        //If the computer name is already in a cache, then just return it given the node number
        if (siteCacheDict.Count != 0)
        {
            string computerNameInDict = "";
            if (siteCacheDict.TryGetValue(nodeNumber, out computerNameInDict))
            {
                return computerNameInDict;
            }
        }
        //Loop through each created SiteStsturcre and see if the node number and the site prefix matches to the provided ones
        foreach (SiteStructure site in siteList)
        {
            //Only one check in this method. If it isn't equal, then you're SOL
            if (site.nodeNumber == nodeNumber)
            {
                //Throw the nodeNumber  and the computername into a dictionary for quicker access
                siteCacheDict.Add(site.nodeNumber, site.computerName);
                //Return the value to the user
                return site.computerName;
            }
        }

        //REaching here is kinda bad. This means that the Node Number isn't provided in the list you were given...or incomplete condition
        return nodeNumber.ToString();
    }

    /// <summary>
    /// Get a machien Host name given a node number and a site prefix. Note that the prefix should be used iff the Node Number returns multiple Machine Names.
    /// This is an attempt to solve the Shiloh/Ottercreek node number problem
    /// </summary>
    /// <returns>String: Computer name </returns>
    public static string getComputerName(int nodeNumber, string sitePrefix)
    {
        //If the computer name is already in a cache, then just return it given the node number
        if (siteCacheDict.Count != 0)
        {
            string computerNameInDict = "";
            if (siteCacheDict.TryGetValue(nodeNumber, out computerNameInDict))
            {
                return computerNameInDict;
            }

        }
        //Loop through each created SiteStsturcre and see if the node number and the site prefix matches to the provided ones
        foreach (SiteStructure site in siteList)
        {
            //If the Prefx is null, then that means it s a FE or 
            //If the node numbermatches the the site prefix matches that of the 
            //if (site.sitePrefix == null || ((site.nodeNumber == nodeNumber) && (Array.IndexOf(site.sitePrefix, sitePrefix) > -1)))
            if (site.sitePrefix == null || ((site.nodeNumber == nodeNumber) && (site.sitePrefix.Equals(sitePrefix))))
            {
                //Throw the nodeNumber  and the computername into a dictionary for quicker access
                siteCacheDict.Add(site.nodeNumber, site.computerName);
                //Return the value to the user
                return site.computerName;
            }
        }

        //REaching here is kinda bad. This means that the Node Number isn't provided in the list you were given...or incomplete condition
        return nodeNumber.ToString();
    }
}
public partial class alarms_Default : System.Web.UI.Page
{

    private DataTable dtAlarms;
    private DateTime epoch = new DateTime(1970, 1, 1);

    //Contants
    private string NAME_OPTION = "name";
    private long TWO_MONTH_TIME_LIMIT = 5184000000;
    private string ORALCE_SOURCE = "Data Source=PT1-SV-ORACLE02/core.ppmems.us;User ID=HIS;Password=HIS;";

    //The queryDict is a Dictionary that stores the Header (used to display to user) and the query name (used for the SQL portion)
    private Dictionary<string, string> queryDict = new Dictionary<string, string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        // Page is being rendered for the first time
        if (Page.IsPostBack)
        {
            //noop
        }

        //If page is loaded for the first time
        else
        {
            resetParameters();
        }
    }
    protected void lnkParametersReset_Click(object sender, EventArgs e)
    {
        resetParameters();
    }
    protected void lnkParametersApply_Click(object sender, EventArgs e)
    {
        DataTable myDT = applyParameters();
        ListView_Alarms.DataSource = this.queryDict;
        ListView_Alarms.DataSource = myDT;
        ListView_Alarms.DataBind();
        dtAlarms = myDT;
        if (myDT != null)
        {
            lblAlarms.Text = "Alarms Returned: " + ListView_Alarms.Items.Count;
            myDT.Dispose();
        }
    }
    protected void lnkParametersCSV_Click(object sender, EventArgs e)
    {
        DataTable myDT = applyParameters();
        if (myDT != null)
        {
            string fileName = "OracleAlarms_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            //fileName = fileName + (txtParamFilter.Text != "" ? "_FILTERED" : "");
            fileName = fileName + ".xls";
            exportToCSV(myDT, fileName);
        }
    }
    private void clearTextFields()
    {
        txtParamLogList.Text = "";
        txtParamName.Text = "";
        txtParamDTStart.Text = "";
        txtParamDTEnd.Text = "";
        lblAlarms.Text = "";
    }
    /// <summary>
    /// Create dictionaries that is used for both querying and 'site mapping'
    /// </summary>
    // If you need to add a new site, this is where you do it
    private void buildDictionaries()
    {
        //The queryDict creats a dictionary that maps a user friendly name (Victor made this up) to the column name in the Oracle Database
        queryDict.Add("Time", "CHRONO");
        queryDict.Add("Turbine Name", "SATT3");
        queryDict.Add("Tag Name", "NAME");
        queryDict.Add("AlarmList", "LOGLIST");
        queryDict.Add("Tag Description", "TITLE");
        queryDict.Add("Value", "NVAL");
        queryDict.Add("Event", "EVTTITLE");
        queryDict.Add("Username", "USERNAME");
        queryDict.Add("Domain", "SATT1");
        queryDict.Add("Nature", "SATT2");
        queryDict.Add("Station", "STATION");

        //This is where you add a new site. Although this method is for creating new dictionaries, this is actually creating a List within a factory method call.
        //I really need to name this method better...
        SiteFactory.createNewSite("BB1-SV-UCC1", 45, new string[] { "Baffin Bay 1", "Baffin Bay 2" }, new string[] { "BAFI1", "BAFI2" });
        SiteFactory.createNewSite("BB1-SV-UCC2", 95, new string[] { "Baffin Bay 1", "Baffin Bay 2" }, new string[] { "BAFI1", "BAFI2" });
        SiteFactory.createNewSite("BN1-SV-UCC1", 30, new string[] { "Barton 1", "Barton 2" }, new string[] { "BART1", "BART2" });
        SiteFactory.createNewSite("BN1-SV-UCC2", 80, new string[] { "Barton 1", "Barton 2" }, new string[] { "BART1", "BART2" });
        SiteFactory.createNewSite("BC1-SV-UCC1", 25, new string[] { "Barton Chapel" }, new string[] { "BARTC" });
        SiteFactory.createNewSite("BC1-SV-UCC2", 75, new string[] { "Barton Chapel" }, new string[] { "BARTC" });
        SiteFactory.createNewSite("BH1-SV-UCC1", 4, new string[] { "Big Horn 1", "Big Horn 2" }, new string[] { "BIGHO", "BIGH2" });
        SiteFactory.createNewSite("BH1-SV-UCC2", 54, new string[] { "Big Horn 1", "Big Horn 2" }, new string[] { "BIGHO", "BIGH2" });
        SiteFactory.createNewSite("BL1-SV-UCC1", 41, new string[] { "Blue Creek" }, new string[] { "BLUEC" });
        SiteFactory.createNewSite("BL1-SV-UCC2", 91, new string[] { "Blue Creek" }, new string[] { "BLUEC" });
        SiteFactory.createNewSite("BR2-SV-UCC1", 33, new string[] { "Buffalo Ridge 2" }, new string[] { "BUFA2" });
        SiteFactory.createNewSite("BR2-SV-UCC2", 83, new string[] { "Buffalo Ridge 2" }, new string[] { "BUFA2" });
        SiteFactory.createNewSite("CMCORE01", 22, new string[] { "Casselman" }, new string[] { "CASSE" });
        SiteFactory.createNewSite("CM1-SV-UCC2", 72, new string[] { "Casselman" }, new string[] { "CASSE" });
        SiteFactory.createNewSite("CR1-SV-UCC1", 26, new string[] { "Cayuga Ridge" }, new string[] { "CRIDG" });
        SiteFactory.createNewSite("CR1-SV-UCC2", 76, new string[] { "Cayuga Ridge" }, new string[] { "CRIDG" });
        SiteFactory.createNewSite("CG1-SV-UCC1", 11, new string[] { "Colorado Green" }, new string[] { "COGRE" });
        SiteFactory.createNewSite("CG1-SV-UCC2", 61, new string[] { "Colorado Green" }, new string[] { "COGRE" });
        SiteFactory.createNewSite("CC1CORE01", 40, new string[] { "Copper Crossing" }, new string[] { "COPER" });
        SiteFactory.createNewSite("CO1-SV-UCC1", 57, new string[] { "Coyote Ridge" }, new string[] { "COYOT" });
        SiteFactory.createNewSite("CO1-SV-UCC2", 107, new string[] { "Coyote Ridge" }, new string[] { "COYOT" });
        SiteFactory.createNewSite("DE1-SV-UCC1", 50, new string[] { "Deerfield" }, new string[] { "DEERF" });
        SiteFactory.createNewSite("DE1-SV-UCC2", 100, new string[] { "Deerfield" }, new string[] { "DEERF" });
        SiteFactory.createNewSite("DW1-SV-UCC1", 47, new string[] { "Desert Wind" }, new string[] { "DESER" });
        SiteFactory.createNewSite("DW1-SV-UCC2", 97, new string[] { "Desert Wind" }, new string[] { "DESER" });
        SiteFactory.createNewSite("DI2-SV-UCC1", 9, new string[] { "Dillion" }, new string[] { "DILON" });
        SiteFactory.createNewSite("DI2-SV-UCC2", 59, new string[] { "Dillion" }, new string[] { "DILON" });
        SiteFactory.createNewSite("DLCORE01", 10, new string[] { "Dry Lake 1" }, new string[] { "DRYLA" });
        SiteFactory.createNewSite("DL2-SV-UCC1", 32, new string[] { "Dry Lake 2" }, new string[] { "DRYL2" });
        SiteFactory.createNewSite("DL2-SV-UCC2", 82, new string[] { "Dry Lake 2" }, new string[] { "DRYL2" });
        SiteFactory.createNewSite("EL1-SV-UCC1", 254, new string[] { "El Cabo" }, new string[] { "ELCAB" });
        SiteFactory.createNewSite("EL1-SV-UCC1", 48, new string[] { "El Cabo" }, new string[] { "ELCAB" });
        SiteFactory.createNewSite("EL1-SV-UCC2", 98, new string[] { "El Cabo" }, new string[] { "ELCAB" });
        SiteFactory.createNewSite("ER1-SV-UCC1", 12, new string[] { "Elk River" }, new string[] { "ELKRI" });
        SiteFactory.createNewSite("ER1-SV-UCC2", 62, new string[] { "Elk River" }, new string[] { "ELKRI" });
        SiteFactory.createNewSite("EC2CORE01", 34, new string[] { "Elm Creek 2" }, new string[] { "ELMC2" });
        SiteFactory.createNewSite("EC2CORE02", 84, new string[] { "Elm Creek 2" }, new string[] { "ELMC2" });
        SiteFactory.createNewSite("FA1-SV-UCC1", 16, new string[] { "Farmers City" }, new string[] { "FARME" });
        SiteFactory.createNewSite("FA1-SV-UCC2", 66, new string[] { "Farmers City" }, new string[] { "FARME" });
        SiteFactory.createNewSite("PT1-SV-FE01A", 201, new string[] { "FE01A" }, new string[] { "FE01" });
        SiteFactory.createNewSite("PT1-SV-FE01C", 202, new string[] { "FE01C" }, new string[] { "FE01" });
        SiteFactory.createNewSite("ACC-SV-FE01D", 227, new string[] { "FE01D" }, new string[] { "FE01" });
        SiteFactory.createNewSite("PT1-SV-FE02A", 203, new string[] { "FE02A" }, new string[] { "FE02" });
        SiteFactory.createNewSite("PT1-SV-FE02C", 204, new string[] { "FE02C" }, new string[] { "FE02" });
        SiteFactory.createNewSite("ACC-SV-FE02D", 228, new string[] { "FE02D" }, new string[] { "FE02" });
        SiteFactory.createNewSite("PT1-SV-FE03A", 205, new string[] { "FE03A" }, new string[] { "FE03" });
        SiteFactory.createNewSite("PT1-SV-FE03C", 206, new string[] { "FE03C" }, new string[] { "FE03" });
        SiteFactory.createNewSite("ACC-SV-FE03D", 229, new string[] { "FE03D" }, new string[] { "FE03" });
        SiteFactory.createNewSite("PT1-SV-FE04A", 207, new string[] { "FE04A" }, new string[] { "FE04" });
        SiteFactory.createNewSite("PT1-SV-FE04C", 208, new string[] { "FE04C" }, new string[] { "FE04" });
        SiteFactory.createNewSite("ACC-SV-FE04D", 230, new string[] { "FE04D" }, new string[] { "FE04" });
        SiteFactory.createNewSite("PT1-SV-FE05A", 209, new string[] { "FE05A" }, new string[] { "FE05" });
        SiteFactory.createNewSite("PT1-SV-FE05C", 210, new string[] { "FE05C" }, new string[] { "FE05" });
        SiteFactory.createNewSite("ACC-SV-FE05D", 231, new string[] { "FE05D" }, new string[] { "FE05" });
        SiteFactory.createNewSite("PT1-SV-FE06A", 211, new string[] { "FE06A" }, new string[] { "FE06" });
        SiteFactory.createNewSite("PT1-SV-FE06C", 212, new string[] { "FE06C" }, new string[] { "FE06" });
        SiteFactory.createNewSite("ACC-SV-FE06D", 232, new string[] { "FE06D" }, new string[] { "FE06" });
        SiteFactory.createNewSite("PT1-SV-FE07A", 213, new string[] { "FE07A" }, new string[] { "FE07" });
        SiteFactory.createNewSite("PT1-SV-FE07C", 214, new string[] { "FE07C" }, new string[] { "FE07" });
        SiteFactory.createNewSite("ACC-SV-FE07D", 233, new string[] { "FE07D" }, new string[] { "FE07" });
        SiteFactory.createNewSite("PT1-SV-FE08A", 215, new string[] { "FE08A" }, new string[] { "FE08" });
        SiteFactory.createNewSite("PT1-SV-FE08C", 216, new string[] { "FE08C" }, new string[] { "FE08" });
        SiteFactory.createNewSite("ACC-SV-FE08D", 234, new string[] { "FE08D" }, new string[] { "FE08" });
        SiteFactory.createNewSite("PT1-SV-FE09A", 217, new string[] { "FE09A" }, new string[] { "FE09" });
        SiteFactory.createNewSite("PT1-SV-FE09C", 218, new string[] { "FE09C" }, new string[] { "FE09" });
        SiteFactory.createNewSite("ACC-SV-FE09D", 235, new string[] { "FE09D" }, new string[] { "FE09" });
        SiteFactory.createNewSite("PT1-SV-FE10A", 219, new string[] { "FE10A" }, new string[] { "FE10" });
        SiteFactory.createNewSite("PT1-SV-FE10C", 220, new string[] { "FE10C" }, new string[] { "FE10" });
        SiteFactory.createNewSite("ACC-SV-FE10D", 236, new string[] { "FE10D" }, new string[] { "FE10" });
        SiteFactory.createNewSite("PT1-SV-FE11A", 221, new string[] { "FE11A" }, new string[] { "FE11" });
        SiteFactory.createNewSite("PT1-SV-FE11C", 222, new string[] { "FE11C" }, new string[] { "FE11" });
        SiteFactory.createNewSite("ACC-SV-FE11D", 237, new string[] { "FE11D" }, new string[] { "FE11" });
        SiteFactory.createNewSite("PT1-SV-FE12A", 223, new string[] { "FE12A" }, new string[] { "FE12" });
        SiteFactory.createNewSite("PT1-SV-FE12C", 224, new string[] { "FE12C" }, new string[] { "FE12" });
        SiteFactory.createNewSite("ACC-SV-FE12D", 238, new string[] { "FE12D" }, new string[] { "FE12" });
        SiteFactory.createNewSite("PT1-SV-FE13A", 225, new string[] { "FE13A" }, new string[] { "FE13" });
        SiteFactory.createNewSite("PT1-SV-FE14A", 151, new string[] { "FE14A" }, new string[] { "FE14" });
        SiteFactory.createNewSite("PT1-SV-FE14C", 152, new string[] { "FE14C" }, new string[] { "FE14" });
        SiteFactory.createNewSite("ACC-SV-FE14D", 177, new string[] { "FE14D" }, new string[] { "FE14" });
        SiteFactory.createNewSite("PT1-SV-FE15A", 153, new string[] { "FE15A" }, new string[] { "FE15" });
        SiteFactory.createNewSite("PT1-SV-FE15C", 154, new string[] { "FE15C" }, new string[] { "FE15" });
        SiteFactory.createNewSite("ACC-SV-FE15D", 178, new string[] { "FE15D" }, new string[] { "FE15" });
        SiteFactory.createNewSite("PT1-SV-FE16A", 155, new string[] { "FE16A" }, new string[] { "FE16" });
        SiteFactory.createNewSite("PT1-SV-FE16C", 156, new string[] { "FE16C" }, new string[] { "FE16" });
        SiteFactory.createNewSite("ACC-SV-FE16D", 179, new string[] { "FE16D" }, new string[] { "FE16" });
        SiteFactory.createNewSite("PT1-SV-FE17A", 157, new string[] { "FE17A" }, new string[] { "FE17" });
        SiteFactory.createNewSite("PT1-SV-FE17C", 158, new string[] { "FE17C" }, new string[] { "FE17" });
        SiteFactory.createNewSite("ACC-SV-FE17D", 180, new string[] { "FE17D" }, new string[] { "FE17" });
        SiteFactory.createNewSite("PT1-SV-FE18A", 333, new string[] { "FE18A" }, new string[] { "FE18" });
        SiteFactory.createNewSite("PT1-SV-FE18C", 333, new string[] { "FE18C" }, new string[] { "FE18" });
        SiteFactory.createNewSite("ACC-SV-FE18D", 333, new string[] { "FE18D" }, new string[] { "FE18" });
        SiteFactory.createNewSite("PT1-SV-FE19A", 333, new string[] { "FE19A" }, new string[] { "FE19" });
        SiteFactory.createNewSite("ACC-SV-FE19D", 333, new string[] { "FE19A" }, new string[] { "FE19" });
        SiteFactory.createNewSite("PT1-SV-FE19C", 333, new string[] { "FE19C" }, new string[] { "FE19" });
        SiteFactory.createNewSite("FCCORE01", 14, new string[] { "Flying Cloud" }, new string[] { "FLYCO" });
        SiteFactory.createNewSite("FC1-SV-UCC2", 64, new string[] { "Flying Cloud" }, new string[] { "FLYCO" });
        SiteFactory.createNewSite("GA1-SV-UCC1", 49, new string[] { "Gala" }, new string[] { "GALA1" });
        SiteFactory.createNewSite("GA1-SV-UCC2", 99, new string[] { "Gala" }, new string[] { "GALA1" });
        SiteFactory.createNewSite("GR1-SV-UCC1", 39, new string[] { "Groton" }, new string[] { "GROTO" });
        SiteFactory.createNewSite("GR1-SV-UCC2", 89, new string[] { "Groton" }, new string[] { "GROTO" });
        SiteFactory.createNewSite("HS1-SV-UCC1", 29, new string[] { "Hardscrabble" }, new string[] { "SCRAB" });
        SiteFactory.createNewSite("HS1-SV-UCC2", 79, new string[] { "Hardscrabble" }, new string[] { "SCRAB" });
        SiteFactory.createNewSite("HA1-SV-UCC1", 5, new string[] { "Hay Canyon " }, new string[] { "HAYCA" });
        SiteFactory.createNewSite("HO1-SV-UCC1", 38, new string[] { "Hoosac" }, new string[] { "HOOSA" });
        SiteFactory.createNewSite("HO1-SV-UCC2", 88, new string[] { "Hoosac" }, new string[] { "HOOSA" });
        SiteFactory.createNewSite("JC1-SV-UCC1", 27, new string[] { "Juniper Canyon" }, new string[] { "JUNCA" });
        SiteFactory.createNewSite("JC1-SV-UCC2", 77, new string[] { "Juniper Canyon" }, new string[] { "JUNCA" });
        SiteFactory.createNewSite("KA1-SV-UCC1", 56, new string[] { "Karakawa" }, new string[] { "KARAN" });
        SiteFactory.createNewSite("KA1-SV-UCC2", 106, new string[] { "Karakawa" }, new string[] { "KARAN" });
        SiteFactory.createNewSite("KM1-SV-UCC1", 46, new string[] { "Klamath Falls" }, new string[] { "KLAMA" });
        SiteFactory.createNewSite("KM1-SV-UCC2", 96, new string[] { "Klamath Falls" }, new string[] { "KLAMA" });
        SiteFactory.createNewSite("KL1-SV-UCC1", 3, new string[] { "Klondike", "Klondike 2", "Klondike Mitsubishi", "Klondike 3A", "Klondike 3GE", "Klondike Siemens" }, new string[] { "KLON1", "KLON2", "KLONM", "KLONA", "KLONG", "KLONS" });
        SiteFactory.createNewSite("KL1-SV-UCC2", 53, new string[] { "Klondike", "Klondike 2", "Klondike Mitsubishi", "Klondike 3A", "Klondike 3GE", "Klondike Siemens" }, new string[] { "KLON1", "KLON2", "KLONM", "KLONA", "KLONG", "KLONS" });
        SiteFactory.createNewSite("LJA-SV-UCC1", 28, new string[] { "Leaning Juniper 2 A" }, new string[] { "LEJUN" });
        SiteFactory.createNewSite("LJB-SV-UCC1", 31, new string[] { "Leaning Juniper 2 B" }, new string[] { "LEJU2" });
        SiteFactory.createNewSite("LJB-SV-UCC2", 81, new string[] { "Leaning Juniper 2 B" }, new string[] { "LEJU2" });
        SiteFactory.createNewSite("LP1-SV-UCC1", 17, new string[] { "Lempster" }, new string[] { "LEMPS" });
        SiteFactory.createNewSite("LP1-SV-UCC2", 67, new string[] { "Lempster" }, new string[] { "LEMPS" });
        SiteFactory.createNewSite("LR2-SV-UCC2", 68, new string[] { "Locus Ridge 1&2" }, new string[] { "LRID1", "LRID2" });
        SiteFactory.createNewSite("LR2-SV-UCC1", 18, new string[] { "Locus Ridge 2" }, new string[] { "LRID1", "LRID2" });
        SiteFactory.createNewSite("MZ1-SV-UCC1", 35, new string[] { "Manzana" }, new string[] { "MANZA" });
        SiteFactory.createNewSite("MZ1-SV-UCC2", 85, new string[] { "Manzana" }, new string[] { "MANZA" });
        SiteFactory.createNewSite("MR1-SV-UCC1", 23, new string[] { "MapleRidge 1" }, new string[] { "MRIDG", "MRID2", "MRID3" });
        SiteFactory.createNewSite("MD1-SV-UCC1", 1, new string[] { "Minndakota", "Buffalo Ridge" }, new string[] { "MINND", "BUFAL" });
        SiteFactory.createNewSite("MDCORE02", 51, new string[] { "Minndakota", "Buffalo Ridge" }, new string[] { "MINND", "BUFAL" });
        SiteFactory.createNewSite("MG1-SV-UCC1", 55, new string[] { "Montague" }, new string[] { "MONTA" });
        SiteFactory.createNewSite("MG1-SV-UCC2", 105, new string[] { "Montague" }, new string[] { "MONTA" });
        SiteFactory.createNewSite("MO1-SV-UCC1", 20, new string[] { "Moraine" }, new string[] { "MORA1", "MORA2" });
        SiteFactory.createNewSite("MO1-SV-UCC2", 70, new string[] { "Moraine" }, new string[] { "MORA1", "MORA2" });
        SiteFactory.createNewSite("MV3CORE01", 37, new string[] { "Mountain View 3" }, new string[] { "MV3" });
        SiteFactory.createNewSite("NH1-SV-UCC1", 42, new string[] { "New Harvest" }, new string[] { "NEWHA" });
        SiteFactory.createNewSite("NHCORE02", 92, new string[] { "New Harvest" }, new string[] { "NEWHA" });
        SiteFactory.createNewSite("OC1-SV-UCC1", 58, new string[] { "Ottercreek" }, new string[] { "OTTER" });
        SiteFactory.createNewSite("OC1-SV-UCC2", 108, new string[] { "Ottercreek" }, new string[] { "OTTER" });
        SiteFactory.createNewSite("PA1-SV-UCC1", 54, new string[] { "Patriot" }, new string[] { "PATRI" });
        SiteFactory.createNewSite("PA1-SV-UCC2", 104, new string[] { "Patriot" }, new string[] { "PATRI" });
        SiteFactory.createNewSite("PB1-SV-UCC1", 6, new string[] { "Pebble Spring" }, new string[] { "PESPR" });
        SiteFactory.createNewSite("PS1-SV-UCC1", 24, new string[] { "Penascal 1", "Penascal 2", "Penascal 3" }, new string[] { "PENE1", "PENE2", "PENE3" });
        SiteFactory.createNewSite("PS1-SV-UCC2", 74, new string[] { "Penascal 1", "Penascal 2", "Penascal 3" }, new string[] { "PENE1", "PENE2", "PENE3" });
        SiteFactory.createNewSite("PH1-SV-UCC1", 19, new string[] { "Providence Heights" }, new string[] { "PROVH" });
        SiteFactory.createNewSite("PH1-SV-UCC2", 69, new string[] { "Providence Heights" }, new string[] { "PROVH" });
        SiteFactory.createNewSite("RUCORE01", 21, new string[] { "Rugby" }, new string[] { "RUGBY" });
        SiteFactory.createNewSite("SA1-SV-UCC1", 44, new string[] { "San Luis" }, new string[] { "SLUIS" });
        SiteFactory.createNewSite("SHCORE01", 8, new string[] { "Shiloh" }, new string[] { "SHILO" });
        SiteFactory.createNewSite("SH1-SV-UCC2", 58, new string[] { "Shiloh" }, new string[] { "SHILO" });
        SiteFactory.createNewSite("SC1-SV-UCC1", 43, new string[] { "South Chestnut" }, new string[] { "SCHES" });
        SiteFactory.createNewSite("SC1-SV-UCC2", 93, new string[] { "South Chestnut" }, new string[] { "SCHES" });
        SiteFactory.createNewSite("SP1-SV-UCC1", 7, new string[] { "Star Point" }, new string[] { "STPOI" });
        SiteFactory.createNewSite("SP1-SV-UCC2", 57, new string[] { "Star Point" }, new string[] { "STPOI" });
        SiteFactory.createNewSite("TI2-SV-UCC1", 36, new string[] { "Top of Iowa 2" }, new string[] { "TOPIO" });
        SiteFactory.createNewSite("TI2-SV-UCC2", 86, new string[] { "Top of Iowa 2" }, new string[] { "TOPIO" });
        SiteFactory.createNewSite("TR1-SV-UCC1", 13, new string[] { "Trimont", "Elm Creek 1" }, new string[] { "TRIMO", "ELMCR" });
        SiteFactory.createNewSite("TR1-SV-UCC2", 63, new string[] { "Trimont", "ELm Creek 1" }, new string[] { "TRIMO", "ELMCR" });
        SiteFactory.createNewSite("TU1-SV-UCC1", 51, new string[] { "Tule" }, new string[] { "TULE1" });
        SiteFactory.createNewSite("TU1-SV-UCC2", 101, new string[] { "Tule" }, new string[] { "TULE1" });
        SiteFactory.createNewSite("TB1-SV-UCC1", 15, new string[] { "Twin Buttes 1" }, new string[] { "TWINB" });
        SiteFactory.createNewSite("TB1-SV-UCC2", 65, new string[] { "Twin Buttes 1" }, new string[] { "TWINB" });
        SiteFactory.createNewSite("TB2-SV-UCC1", 52, new string[] { "Twin Buttes 2" }, new string[] { "TWIN2" });
        SiteFactory.createNewSite("TB2-SV-UCC2", 102, new string[] { "Twin Buttes 2" }, new string[] { "TWIN2" });
        SiteFactory.createNewSite("WB1-SV-UCC1", 2, new string[] { "Winnebago" }, new string[] { "WINNE" });
        SiteFactory.createNewSite("WB1-SV-UCC2", 52, new string[] { "Winnebago" }, new string[] { "WINNE" });
        SiteFactory.createNewSite("KL1-WYEAST-UCC", 250, new string[] { "Wyeast" }, new string[] { "WYEAS" });


        //Create new  filters
        SiteFactory.createNewFilter("WindNode", "LOGLIST = 'ALMFE16' AND SATT1 = 'CORE'");
        SiteFactory.createNewFilter("RAS", "LOGLIST='RAS'");
        SiteFactory.createNewFilter("FrontVue", "LOGLIST='ALMFE16' AND STATION =0");
    }
    /// <summary>
    /// Clear the SiteFactory Dictionary and the queryDictionary objects created in buildDictionaries
    /// </summary>
    private void clearDictionaries()
    {
        queryDict.Clear();
        SiteFactory.clearDictionaries();
    }
    private DataTable applyParameters()
    {
        DataTable myDT = new DataTable(); //Temporary table to bind ListView to.

        string searchOption = coreSearchOption.SelectedValue;

        //Build the dictionaries 
        buildDictionaries();

        //Build the plsql command.
        string oraParams = "";
        //string oraQry = "SELECT * FROM ALARMS";

        string oraQry = "SELECT /*+PARALLEL(10)*/ ";
        foreach (KeyValuePair<string, string> queryEntry in queryDict)
        {
            oraQry += queryEntry.Value + " AS \"" + queryEntry.Key + "\",";
        }
        //remove the last comma from the string and then stap
        oraQry = oraQry.TrimEnd(',') + " FROM HIS.ALARMS";

        string oraOrd = " ORDER BY CHRONO DESC";
        OracleCommand oraCmd = new OracleCommand();


        //For use with time
        long epochStart = 0;
        long epochEnd = 0;

        //PARAMETER: CHRONO.START
        if (!txtParamDTStart.Text.Equals(""))
        {
            DateTime dtStart;
            if (!DateTime.TryParse(txtParamDTStart.Text, out dtStart))
            {
                lblAlarms.Text = "Invalid Start Date!";
            }
            else
            {
                epochStart = ((long)(dtStart.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
                Console.WriteLine("EpochStart: " + epochStart);
                oraParams += (oraParams.Equals("") ? "" : " AND ") + "CHRONO >= :dtStart";
                oraCmd.Parameters.Add(new OracleParameter("dtStart", epochStart));
            }
        }

        //PARAMETER: CHRONO.END
        if (!txtParamDTEnd.Text.Equals(""))
        {
            DateTime dtEnd;
            if (!DateTime.TryParse(txtParamDTEnd.Text, out dtEnd))
            {
                lblAlarms.Text = "Invalid End Date!";
            }
            else
            {
                //double epochEnd = (double)1000 * ((int)(dtEnd - new DateTime(1970, 1, 1)).TotalSeconds);
                epochEnd = ((long)(dtEnd.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "CHRONO <= :dtEnd";
                oraCmd.Parameters.Add(new OracleParameter("dtEnd", epochEnd));
            }
        }

        //perform a time check to see if the dtSTart and dtEnd exceeds 60 days or 5.184e+9 milliseconds
        //If it exceeds it, then display error and inform user  

        if ((!txtParamDTEnd.Text.Equals("")) && (!txtParamDTStart.Text.Equals("")))
        {
            if (epochEnd - epochStart > TWO_MONTH_TIME_LIMIT)
            {
                lblAlarms.Text = "Invalid Range. Range must be within two months";
                return null;
            }
        }

        //PARAMETER: LOGLIST
        if (!txtParamLogList.Text.Equals(""))
        {
            //Get the name of the 'site' or 'filter' from the user. This is in the txtParamLogList text field
            string logList = txtParamLogList.Text;

            //If it is a filter, then apply filter conditions
            if (SiteFactory.getFilters().ContainsKey(logList))
            {
                oraParams += (oraParams.Equals("") ? "" : " AND ") + SiteFactory.getFilters()[logList];
            }
            //otherwise, it is a site and should be handled as such
            else
            {
                //Get the site prefixe(s) from the site name
                string prefix = SiteFactory.getPrefixFromSiteName(logList);

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "LOGLIST = :loglist";
                oraCmd.Parameters.Add(new OracleParameter("loglist", "ALM" + prefix));
            }
        }

        //PARAMETER: NAME
        if (!txtParamName.Text.Equals(""))
        {
            // If the user selected to search by tag name 
            if (coreSearchOption.Equals(NAME_OPTION))
            {
                oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(NAME) LIKE lower(:tagname)";
                oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
            }
            // If the user decided to search by description 
            else
            {

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(TITLE) LIKE lower(:tagname)";
                oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
            }
        }


        //Finish constructing parameters and queries.
        if (oraParams != null)
            if (!oraParams.Equals(""))
                oraParams = " WHERE " + oraParams;
        oraCmd.CommandText = oraQry + oraParams + oraOrd;
        //Response.Write(oraCmd); return;

        //Spit out debug of commands and parameters.
        lblDebug.Text += "\n<br />" + oraCmd.CommandText;
        foreach (OracleParameter p in oraCmd.Parameters)
        {
            lblDebug.Text += "\n<br />" + p.ParameterName + ": " + p.Value;
        }

        //Build the connection string.
        string oraCstr = ORALCE_SOURCE;

        myDT = oraQueryTable(oraCmd, oraCstr);

        clearDictionaries();
        return postTableQueryEdits(myDT);
    }

    /// <summary>
    /// This function updates the time of the DataTable after queried from Oracle and update the  time to PT and the Station to  h
    /// human readable format
    /// </summary>
    /// <param name="myDT"></param>
    /// <returns>A modified DataTable with changes to Time and changes to Station </returns>
    private DataTable postTableQueryEdits(DataTable myDT)
    {
        DataTable newTable = myDT.Clone();
        string TIME_COL = "Time";
        string STATION_COL = "Station";
        string DOMAIN_COL = "Domain";
        string ALARM_LIST_COL = "AlarmList";

        int stationIndex = newTable.Columns.IndexOf(STATION_COL);
        newTable.Columns.Remove(STATION_COL);
        DataColumn newStationCol = newTable.Columns.Add(STATION_COL);
        newStationCol.SetOrdinal(stationIndex);

        int timeIndex = newTable.Columns.IndexOf(TIME_COL);
        newTable.Columns.Remove(TIME_COL);
        DataColumn newTimeCol = newTable.Columns.Add(TIME_COL);
        newTimeCol.SetOrdinal(timeIndex);

        Random random = new Random();
        foreach (DataRow row in myDT.Rows)
        {
            DataRow newRow = newTable.Rows.Add();
            //The station number should be changed so that the STATION should be a human readable value and not just a number.
            //The time should be in Local time and not UTC
            // If it is anything else, just ignore it
            newRow.BeginEdit();
            foreach (DataColumn col in newTable.Columns)
            {
                //If Time. This should be in local pacific  time
                if (col == newTimeCol)
                {
                    Object temp = fmtDateTime(epochToDateTime(row[col.ColumnName]).ToLocalTime());
                    newRow.SetField(col, temp);
                }

                //If Station. This should be in a machine name format
                else if (col == newStationCol)
                {
                    //Get the Site prefix from the Domain Column. Note that not all alarm will have a Domain or a nature
                    string sitePrefix = row[DOMAIN_COL].ToString();

                    //Get the site ID from the STATION Column
                    int siteID = Convert.ToInt16(row[STATION_COL]);

                    //Get the Alarm List column from the Alarm Column. This is only used for matching the Front Ends.
                    string alarmListName = row[ALARM_LIST_COL].ToString();

                    string temp = "";
                    temp = SiteFactory.getComputerName(siteID);
                    newRow.SetField(col, temp);
                }
                else
                {
                    newRow.SetField(col, row[col.ColumnName]);
                }
            }
            newRow.AcceptChanges();
        }
        //Drop the alarms list column (as Victor doesn't want it
        newTable.Columns.Remove(ALARM_LIST_COL);
        return newTable;

    }

    private enum FilterMode { Exclude = 0, Include = 1 };


    private DataTable sqlQueryTable(SqlCommand sqlCmd, string connectionString)
    {
        SqlConnection sqlConn = new SqlConnection(connectionString);
        sqlCmd.Connection = sqlConn;
        DataTable retDT = new DataTable();

        try
        {
            sqlConn.Open();
            SqlDataAdapter oraDA = new SqlDataAdapter(sqlCmd);
            oraDA.Fill(retDT);
            sqlConn.Close();
        }
        catch (Exception e)
        {
            retDT = null;
            throw e;
        }
        finally
        {
            sqlConn.Dispose();
            sqlConn = null;
        }

        return retDT;
    }

    private Object oraQueryScalar(OracleCommand oraCmd, string connectionString)
    {
        OracleConnection oraConn = new OracleConnection(connectionString);
        oraCmd.Connection = oraConn;
        Object retVal;

        Console.WriteLine(connectionString);
        try
        {
            oraConn.Open();
            retVal = oraCmd.ExecuteScalar();
            oraConn.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            retVal = null;
            throw e;
        }
        finally
        {
            oraConn.Dispose();
            oraConn = null;
        }

        return retVal;
    }

    private DataTable oraQueryTable(OracleCommand oraCmd, string connectionString)
    {
        OracleConnection oraConn = new OracleConnection(connectionString);
        oraCmd.Connection = new OracleConnection(connectionString);
        DataTable retDT = new DataTable();

        try
        {
            oraConn.Open();
            OracleDataAdapter oraDA = new OracleDataAdapter(oraCmd);
            oraDA.Fill(retDT);
            oraConn.Close();
        }
        catch (Exception e)
        {
            retDT = null;
            throw e;
        }
        finally
        {
            oraConn.Dispose();
            oraConn = null;
        }

        return retDT;
    }

    protected DateTime epochToDateTime(Object epoch)
    {
        double dblTemp;

        switch (epoch.GetType().FullName)
        {
            case "System.Double":
                dblTemp = (Double)epoch;
                break;
            case "System.String":
                if (!Double.TryParse((String)epoch, out dblTemp))
                    throw new InvalidCastException("Unable to cast epoch to double. Type was: " + epoch.GetType().FullName + ", value was: " + epoch.ToString());
                break;
            default:
                throw new InvalidCastException("Unable to cast epoch to double. Type was: " + epoch.GetType().FullName + ", value was: " + epoch.ToString());
                break;
        }
        // if (!Double.TryParse(epoch, out dblTemp))
        //    throw new InvalidCastException("Unable to cast epoch to double.");
        DateTime retVal = new DateTime(1970, 1, 1);
        retVal = retVal.AddMilliseconds(dblTemp).ToLocalTime();
        return retVal;
    }

    /// <summary>
    /// Resets all parameters. Used on first time page load or when user wants to clear everything
    /// </summary>
    private void resetParameters()
    {
        //Clear all Text Fields
        clearTextFields();
        //Local Time format
        txtParamDTEnd.Text = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");

        //Rebuild the dictionaries
        clearDictionaries();
        buildDictionaries();

        //Clear the ListView and rebuild it.
        ListView_Alarms.DataSource = "";
        ListView_Alarms.DataBind();
        List<String> siteList = new List<string>();
        foreach (SiteStructure site in SiteFactory.getSiteList())
        {
            if (!siteList.Contains(site.siteName))
                siteList.Add(site.siteName);
        }

        //Add the filters
        foreach (KeyValuePair<string, string> filter in SiteFactory.getFilters())
        {
            siteList.Add(filter.Key);
        }

        siteList.Sort();

        //Rebind the siteList to the dropdown list
        SiteDropDownList.DataSource = siteList;
        //Set the txtParamLogList textfield to the first item in the siteList
        txtParamLogList.Text = siteList[0];
        SiteDropDownList.DataBind();
    }

    protected string fmtDateTime(Object inDate)
    {
        if (inDate.GetType().FullName != "System.DateTime")
            return "INVALID TYPE:" + inDate.GetType().FullName;
        else
            return ((DateTime)inDate).ToString("yyyy-MM-dd HH:mm:ss");
    }

    protected string fmtDate(Object inDate)
    {
        if (inDate.GetType().FullName != "System.DateTime")
            return "INVALID TYPE:" + inDate.GetType().FullName;
        else
            return ((DateTime)inDate).ToString("yyyy-MM-dd");
    }

    private void linqDebug(string msg)
    {
        //debug(msg);
    }

    private void debug(string msg)
    {
        lblDebug.Text += msg;
    }

    private bool regexLike(string input, string search)
    {
        string regstr = (String)search;
        regstr = Regex.Escape(regstr);
        regstr = regstr.Replace("\\*", ".*");
        regstr = regstr.Replace("%", ".*");
        regstr = regstr.Replace("_", ".");
        regstr = "^" + regstr + "$";
        //"^" + Regex.Escape((String)search).Replace("*", ".*").Replace("%", ".*").Replace("_", ".") + "$";
        Regex regex = new Regex(regstr, RegexOptions.IgnoreCase);
        //throw new Exception(input.ToString() + "<br />" + search.ToString() + "<br />" + regstr);
        return regex.IsMatch((String)input ?? string.Empty);
    }

    private void updateFilterGrid()
    {
        //if (!txtParamFilter.Text.Equals(""))
        // {
        //DataTable tblGrid;
        //string sqlQry = "SELECT * FROM CORE.dbo.Alarm_Filters WHERE Filter_Group = @Filter_Group;";
        //SqlParameter sqlParm = new SqlParameter("Filter_Group", txtParamFilter.Text);
        //SqlCommand sqlCmd = new SqlCommand();
        //sqlCmd.CommandText = sqlQry;
        //sqlCmd.Parameters.Add(sqlParm);
        //tblGrid = sqlQueryTable(sqlCmd, System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnectionString_PDXSQL03_EMSWEB"].ConnectionString);
        //Grid_FilterEditor.DataSource = tblGrid;

        Grid_FilterEditor.ClearPreviousDataSource();
        Grid_FilterEditor.DataSourceID = "SqlDataSource_FilterEditor";
        Grid_FilterEditor.DataBind();


        //if (txtParamFilter.Text.Length > 0)
        //{
        //    Obout.Grid.FilterCriteria criteria = new Obout.Grid.FilterCriteria();
        //    criteria.Option.Type = Obout.Grid.FilterOptionType.EqualTo;
        //    criteria.Value = txtParamFilter.Text;
        //    Grid_FilterEditor.Columns["Filter_Group"].FilterCriteria = criteria;
        //}
    }

    private void exportToCSV(DataTable dt, string filename)
    {
        // Pulls all data from a SQLDataSource and creates a tab-delimeted file
        // which can be loaded into Excel.

        Response.Clear();
        Response.Charset = "";
        Response.Buffer = true;
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("content-disposition", "attachment;filename=" + filename);

        // If you want to force the user to save the file, uncomment the following line.
        // Response.Cache.SetCacheability(HttpCacheability.NoCache)

        this.EnableViewState = false;


        string sep = "";
        foreach (DataColumn dc in dt.Columns)
        {
            Response.Write(sep + dc.ColumnName);
            sep = "\t";
        }
        Response.Write("\n");

        foreach (DataRow dr in dt.Rows)
        {
            sep = "";
            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {
                Response.Write(sep + dr[i].ToString());
                sep = "\t";
            }
            Response.Write("\n");
        }
        Response.End();
    }

    protected void btnShowFilterEditor_Click(object sender, EventArgs e) { updateFilterGrid(); Window_FilterEditor.VisibleOnLoad = true; }

    protected void ddlParamLogList_SelectedIndexChanged(object sender, EventArgs e) { txtParamLogList.Text = SiteDropDownList.SelectedValue; }

    protected void lbtDTStart_Click(object sender, EventArgs e)
    {

        String[] command = ((LinkButton)(sender)).CommandArgument.Split(',');
        String timePeriod = command[1];
        double time = Convert.ToInt16("-" + command[0]);
        if (timePeriod.Equals("m"))
            txtParamDTStart.Text = DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss");
        else
            txtParamDTStart.Text = DateTime.Now.AddHours(time).ToString("yyyy-MM-dd HH:mm:ss");

        localTimetoEpochTime(DateTime.Now);

    }

    protected DateTime localTimetoEpochTime(DateTime dateTime)
    {
        //Timespan  t =dateTime - new DateTime(1970, 1, 1);
        Boolean dayLightSaving = TimeZoneInfo.Local.IsDaylightSavingTime(dateTime);


        long dt = (long)(dateTime - epoch).TotalMilliseconds;

        Console.WriteLine(dt);

        if (dayLightSaving)
        {

        }
        else
        {

        }
        return DateTime.Now;
    }
}