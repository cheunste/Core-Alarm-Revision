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

    public SiteStructure(string computerName, int nodeNumber, string siteName, string[] sitePrefix)
    {
        this.computerName = computerName;
        this.nodeNumber = nodeNumber;
        this.siteName = siteName;
        this.sitePrefix = sitePrefix;

    }

    public string computerName { get; set; }
    public int nodeNumber { get; set; }
    public string siteName { get; set; }
    public string[] sitePrefix { get; set; }
}

public static class SiteFactory
{
    private static List<SiteStructure> siteList = new List<SiteStructure>();
    private static Dictionary<int, string> siteCacheDict = new Dictionary<int, string>();
    private static Dictionary<string, string[]> sitePrefixDict = new Dictionary<string, string[]>();
    private static Dictionary<string, string> specialFilter = new Dictionary<string, string>();

    public static void createNewSite(string computerName, int nodeNumber, string siteName, string[] sitePrefix)
    {

        SiteStructure s = new SiteStructure(computerName, nodeNumber, siteName, sitePrefix);
        //Oddly enough, I can't adccess Dictionary.TryAdd() method
        try
        {
            sitePrefixDict.Add(siteName, sitePrefix);
        }
        catch (Exception e)
        {
            //Ignore exception
        }
        siteList.Add(s);
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
        catch(Exception e)
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
    public static string[] getPrefixFromSiteName(string siteName)
    {
        string[] sitePrefixes = { };
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
            if (site.sitePrefix == null || ((site.nodeNumber == nodeNumber) && (Array.IndexOf(site.sitePrefix, sitePrefix) > -1)))
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
        SiteFactory.createNewSite("BB1-SV-UCC1", 45, "Baffin Bay", new string[] { "BAFI1", "BAFI2" });
        SiteFactory.createNewSite("BB1-SV-UCC2", 95, "Baffin Bay", new string[] { "BAFI1", "BAFI2" });
        SiteFactory.createNewSite("BN1-SV-UCC1", 30, "Barton", new string[] { "BART1", "BART2" });
        SiteFactory.createNewSite("BN1-SV-UCC2", 80, "Barton", new string[] { "BART1", "BART2" });
        SiteFactory.createNewSite("BC1-SV-UCC1", 25, "Barton Chapel", new string[] { "BARTC" });
        SiteFactory.createNewSite("BC1-SV-UCC2", 75, "Barton Chapel", new string[] { "BARTC" });
        SiteFactory.createNewSite("BH1-SV-UCC1", 4, "Big Horn 1", new string[] { "BIGHO", "BIGH2" });
        SiteFactory.createNewSite("BH1-SV-UCC2", 54, "Big Horn 1", new string[] { "BIGHO", "BIGH2" });
        SiteFactory.createNewSite("BL1-SV-UCC1", 41, "Blue Creek", new string[] { "BLUEC" });
        SiteFactory.createNewSite("BL1-SV-UCC2", 91, "Blue Creek", new string[] { "BLUEC" });
        SiteFactory.createNewSite("BR2-SV-UCC1", 33, "Buffalo Ridge 2", new string[] { "BUFA2" });
        SiteFactory.createNewSite("BR2-SV-UCC2", 83, "Buffalo Ridge 2", new string[] { "BUFA2" });
        SiteFactory.createNewSite("CMCORE01", 22, "Casselman", new string[] { "CASSE" });
        SiteFactory.createNewSite("CM1-SV-UCC2", 72, "Casselman", new string[] { "CASSE" });
        SiteFactory.createNewSite("CR1-SV-UCC1", 26, "Cayuga Ridge", new string[] { "CRIDG" });
        SiteFactory.createNewSite("CR1-SV-UCC2", 76, "Cayuga Ridge", new string[] { "CRIDG" });
        SiteFactory.createNewSite("CG1-SV-UCC1", 11, "Colorado Green", new string[] { "COGRE" });
        SiteFactory.createNewSite("CG1-SV-UCC2", 61, "Colorado Green", new string[] { "COGRE" });
        SiteFactory.createNewSite("CC1CORE01", 40, "Copper Crossing", new string[] { "COPER" });
        SiteFactory.createNewSite("CO1-SV-UCC1", 57, "Coyote Ridge", new string[] { "COYOT" });
        SiteFactory.createNewSite("CO1-SV-UCC2", 107, "Coyote Ridge", new string[] { "COYOT" });
        SiteFactory.createNewSite("DE1-SV-UCC1", 50, "Deerfield", new string[] { "DEERF" });
        SiteFactory.createNewSite("DE1-SV-UCC2", 100, "Deerfield", new string[] { "DEERF" });
        SiteFactory.createNewSite("DW1-SV-UCC1", 47, "Desert Wind", new string[] { "DESER" });
        SiteFactory.createNewSite("DW1-SV-UCC2", 97, "Desert Wind", new string[] { "DESER" });
        SiteFactory.createNewSite("DI2-SV-UCC1", 9, "Dillion", new string[] { "DILON" });
        SiteFactory.createNewSite("DI2-SV-UCC2", 59, "Dillion", new string[] { "DILON" });
        SiteFactory.createNewSite("DLCORE01", 10, "Dry Lake 1", new string[] { "DRYLA" });
        SiteFactory.createNewSite("DL2-SV-UCC1", 32, "Dry Lake 2", new string[] { "DRYL2" });
        SiteFactory.createNewSite("DL2-SV-UCC2", 82, "Dry Lake 2", new string[] { "DRYL2" });
        SiteFactory.createNewSite("EL1-SV-UCC1", 254, "El Cabo", new string[] { "ELCAB" });
        SiteFactory.createNewSite("EL1-SV-UCC1", 48, "El Cabo", new string[] { "ELCAB" });
        SiteFactory.createNewSite("EL1-SV-UCC2", 98, "El Cabo", new string[] { "ELCAB" });
        SiteFactory.createNewSite("ER1-SV-UCC1", 12, "Elk River", new string[] { "ELKRI" });
        SiteFactory.createNewSite("ER1-SV-UCC2", 62, "Elk River", new string[] { "ELKRI" });
        SiteFactory.createNewSite("EC2CORE01", 34, "Elm Creek 2", new string[] { "ELMC2" });
        SiteFactory.createNewSite("EC2CORE02", 84, "Elm Creek 2", new string[] { "ELMC2" });
        SiteFactory.createNewSite("FA1-SV-UCC1", 16, "Farmers City", new string[] { "FARME" });
        SiteFactory.createNewSite("FA1-SV-UCC2", 66, "Farmers City", new string[] { "FARME" });
        SiteFactory.createNewSite("PT1-SV-FE01A", 201, "FE01A", new string[] { "FE01" });
        SiteFactory.createNewSite("PT1-SV-FE01C", 202, "FE01C", new string[] { "FE01" });
        SiteFactory.createNewSite("ACC-SV-FE01D", 227, "FE01D", new string[] { "FE01" });
        SiteFactory.createNewSite("PT1-SV-FE02A", 203, "FE02A", new string[] { "FE02" });
        SiteFactory.createNewSite("PT1-SV-FE02C", 204, "FE02C", new string[] { "FE02" });
        SiteFactory.createNewSite("ACC-SV-FE02D", 228, "FE02D", new string[] { "FE02" });
        SiteFactory.createNewSite("PT1-SV-FE03A", 205, "FE03A", new string[] { "FE03" });
        SiteFactory.createNewSite("PT1-SV-FE03C", 206, "FE03C", new string[] { "FE03" });
        SiteFactory.createNewSite("ACC-SV-FE03D", 229, "FE03D", new string[] { "FE03" });
        SiteFactory.createNewSite("PT1-SV-FE04A", 207, "FE04A", new string[] { "FE04" });
        SiteFactory.createNewSite("PT1-SV-FE04C", 208, "FE04C", new string[] { "FE04" });
        SiteFactory.createNewSite("ACC-SV-FE04D", 230, "FE04D", new string[] { "FE04" });
        SiteFactory.createNewSite("PT1-SV-FE05A", 209, "FE05A", new string[] { "FE05" });
        SiteFactory.createNewSite("PT1-SV-FE05C", 210, "FE05C", new string[] { "FE05" });
        SiteFactory.createNewSite("ACC-SV-FE05D", 231, "FE05D", new string[] { "FE05" });
        SiteFactory.createNewSite("PT1-SV-FE06A", 211, "FE06A", new string[] { "FE06" });
        SiteFactory.createNewSite("PT1-SV-FE06C", 212, "FE06C", new string[] { "FE06" });
        SiteFactory.createNewSite("ACC-SV-FE06D", 232, "FE06D", new string[] { "FE06" });
        SiteFactory.createNewSite("PT1-SV-FE07A", 213, "FE07A", new string[] { "FE07" });
        SiteFactory.createNewSite("PT1-SV-FE07C", 214, "FE07C", new string[] { "FE07" });
        SiteFactory.createNewSite("ACC-SV-FE07D", 233, "FE07D", new string[] { "FE07" });
        SiteFactory.createNewSite("PT1-SV-FE08A", 215, "FE08A", new string[] { "FE08" });
        SiteFactory.createNewSite("PT1-SV-FE08C", 216, "FE08C", new string[] { "FE08" });
        SiteFactory.createNewSite("ACC-SV-FE08D", 234, "FE08D", new string[] { "FE08" });
        SiteFactory.createNewSite("PT1-SV-FE09A", 217, "FE09A", new string[] { "FE09" });
        SiteFactory.createNewSite("PT1-SV-FE09C", 218, "FE09C", new string[] { "FE09" });
        SiteFactory.createNewSite("ACC-SV-FE09D", 235, "FE09D", new string[] { "FE09" });
        SiteFactory.createNewSite("PT1-SV-FE10A", 219, "FE10A", new string[] { "FE10" });
        SiteFactory.createNewSite("PT1-SV-FE10C", 220, "FE10C", new string[] { "FE10" });
        SiteFactory.createNewSite("ACC-SV-FE10D", 236, "FE10D", new string[] { "FE10" });
        SiteFactory.createNewSite("PT1-SV-FE11A", 221, "FE11A", new string[] { "FE11" });
        SiteFactory.createNewSite("PT1-SV-FE11C", 222, "FE11C", new string[] { "FE11" });
        SiteFactory.createNewSite("ACC-SV-FE11D", 237, "FE11D", new string[] { "FE11" });
        SiteFactory.createNewSite("PT1-SV-FE12A", 223, "FE12A", new string[] { "FE12" });
        SiteFactory.createNewSite("PT1-SV-FE12C", 224, "FE12C", new string[] { "FE12" });
        SiteFactory.createNewSite("ACC-SV-FE12D", 238, "FE12D", new string[] { "FE12" });
        SiteFactory.createNewSite("PT1-SV-FE13A", 225, "FE13A", new string[] { "FE13" });
        SiteFactory.createNewSite("PT1-SV-FE14A", 151, "FE14A", new string[] { "FE14" });
        SiteFactory.createNewSite("PT1-SV-FE14C", 152, "FE14C", new string[] { "FE14" });
        SiteFactory.createNewSite("ACC-SV-FE14D", 177, "FE14D", new string[] { "FE14" });
        SiteFactory.createNewSite("PT1-SV-FE15A", 153, "FE15A", new string[] { "FE15" });
        SiteFactory.createNewSite("PT1-SV-FE15C", 154, "FE15C", new string[] { "FE15" });
        SiteFactory.createNewSite("ACC-SV-FE15D", 178, "FE15D", new string[] { "FE15" });
        SiteFactory.createNewSite("PT1-SV-FE16A", 155, "FE16A", new string[] { "FE16" });
        SiteFactory.createNewSite("PT1-SV-FE16C", 156, "FE16C", new string[] { "FE16" });
        SiteFactory.createNewSite("ACC-SV-FE16D", 179, "FE16D", new string[] { "FE16" });
        SiteFactory.createNewSite("PT1-SV-FE17A", 157, "FE17A", new string[] { "FE17" });
        SiteFactory.createNewSite("PT1-SV-FE17C", 158, "FE17C", new string[] { "FE17" });
        SiteFactory.createNewSite("ACC-SV-FE17D", 180, "FE17D", new string[] { "FE17" });
        SiteFactory.createNewSite("PT1-SV-FE18A", 333, "FE18A", new string[] { "FE18" });
        SiteFactory.createNewSite("PT1-SV-FE18C", 333, "FE18C", new string[] { "FE18" });
        SiteFactory.createNewSite("ACC-SV-FE18D", 333, "FE18D", new string[] { "FE18" });
        SiteFactory.createNewSite("PT1-SV-FE19A", 333, "FE19A", new string[] { "FE19" });
        SiteFactory.createNewSite("ACC-SV-FE19D", 333, "FE19A", new string[] { "FE19" });
        SiteFactory.createNewSite("PT1-SV-FE19C", 333, "FE19C", new string[] { "FE19" });
        SiteFactory.createNewSite("FCCORE01", 14, "Flying Cloud", new string[] { "FLYCO" });
        SiteFactory.createNewSite("FC1-SV-UCC2", 64, "Flying Cloud", new string[] { "FLYCO" });
        SiteFactory.createNewSite("GA1-SV-UCC1", 49, "Gala", new string[] { "GALA1" });
        SiteFactory.createNewSite("GA1-SV-UCC2", 99, "Gala", new string[] { "GALA1" });
        SiteFactory.createNewSite("GR1-SV-UCC1", 39, "Groton", new string[] { "GROTO" });
        SiteFactory.createNewSite("GR1-SV-UCC2", 89, "Groton", new string[] { "GROTO" });
        SiteFactory.createNewSite("HS1-SV-UCC1", 29, "Hardscrabble", new string[] { "SCRAB" });
        SiteFactory.createNewSite("HS1-SV-UCC2", 79, "Hardscrabble", new string[] { "SCRAB" });
        SiteFactory.createNewSite("HA1-SV-UCC1", 5, "Hay Canyon ", new string[] { "HAYCA" });
        SiteFactory.createNewSite("HO1-SV-UCC1", 38, "Hoosac", new string[] { "HOOSA" });
        SiteFactory.createNewSite("HO1-SV-UCC2", 88, "Hoosac", new string[] { "HOOSA" });
        SiteFactory.createNewSite("JC1-SV-UCC1", 27, "Juniper Canyon", new string[] { "JUNCA" });
        SiteFactory.createNewSite("JC1-SV-UCC2", 77, "Juniper Canyon", new string[] { "JUNCA" });
        SiteFactory.createNewSite("KA1-SV-UCC1", 56, "Karakawa", new string[] { "KARAN" });
        SiteFactory.createNewSite("KA1-SV-UCC2", 106, "Karakawa", new string[] { "KARAN" });
        SiteFactory.createNewSite("KM1-SV-UCC1", 46, "Klamath Falls", new string[] { "KLAMA" });
        SiteFactory.createNewSite("KM1-SV-UCC2", 96, "Klamath Falls", new string[] { "KLAMA" });
        SiteFactory.createNewSite("KL1-SV-UCC1", 3, "Klondike", new string[] { "KLON1", "KLON2", "KLONM", "KLONA", "KLONG", "KLONS" });
        SiteFactory.createNewSite("KL1-SV-UCC2", 53, "Klondike", new string[] { "KLON1", "KLON2", "KLONM", "KLONA", "KLONG", "KLONS" });
        SiteFactory.createNewSite("LJA-SV-UCC1", 28, "Leaning Juniper 2 A", new string[] { "LEJUN" });
        SiteFactory.createNewSite("LJB-SV-UCC1", 31, "Leaning Juniper 2 B", new string[] { "LEJU2" });
        SiteFactory.createNewSite("LJB-SV-UCC2", 81, "Leaning Juniper 2 B", new string[] { "LEJU2" });
        SiteFactory.createNewSite("LP1-SV-UCC1", 17, "Lempster", new string[] { "LEMPS" });
        SiteFactory.createNewSite("LP1-SV-UCC2", 67, "Lempster", new string[] { "LEMPS" });
        SiteFactory.createNewSite("LR2-SV-UCC2", 68, "Locus Ridge 1&2", new string[] { "LRID1", "LRID2" });
        SiteFactory.createNewSite("LR2-SV-UCC1", 18, "Locus Ridge 2", new string[] { "LRID1", "LRID2" });
        SiteFactory.createNewSite("MZ1-SV-UCC1", 35, "Manzana", new string[] { "MANZA" });
        SiteFactory.createNewSite("MZ1-SV-UCC2", 85, "Manzana", new string[] { "MANZA" });
        SiteFactory.createNewSite("MR1-SV-UCC1", 23, "MapleRidge", new string[] { "MRIDG", "MRID2", "MRID3" });
        SiteFactory.createNewSite("MD1-SV-UCC1", 1, "Minndakota/Buffalo Ridge", new string[] { "MINND", "BUFAL" });
        SiteFactory.createNewSite("MDCORE02", 51, "Minndakota/Buffalo Ridge", new string[] { "MINND", "BUFAL" });
        SiteFactory.createNewSite("MG1-SV-UCC1", 55, "Montague", new string[] { "MONTA" });
        SiteFactory.createNewSite("MG1-SV-UCC2", 105, "Montague", new string[] { "MONTA" });
        SiteFactory.createNewSite("MO1-SV-UCC1", 20, "Moraine", new string[] { "MORA1", "MORA2" });
        SiteFactory.createNewSite("MO1-SV-UCC2", 70, "Moraine", new string[] { "MORA1", "MORA2" });
        SiteFactory.createNewSite("MV3CORE01", 37, "Mountain View 3", new string[] { "MV3" });
        SiteFactory.createNewSite("NH1-SV-UCC1", 42, "New Harvest", new string[] { "NEWHA" });
        SiteFactory.createNewSite("NHCORE02", 92, "New Harvest", new string[] { "NEWHA" });
        SiteFactory.createNewSite("OC1-SV-UCC1", 58, "Ottercreek", new string[] { "OTTER" });
        SiteFactory.createNewSite("OC1-SV-UCC2", 108, "Ottercreek", new string[] { "OTTER" });
        SiteFactory.createNewSite("PA1-SV-UCC1", 54, "Patriot", new string[] { "PATRI" });
        SiteFactory.createNewSite("PA1-SV-UCC2", 104, "Patriot", new string[] { "PATRI" });
        SiteFactory.createNewSite("PB1-SV-UCC1", 6, "Pebble Spring", new string[] { "PESPR" });
        SiteFactory.createNewSite("PS1-SV-UCC1", 24, "Penascal", new string[] { "PENE1", "PENE2", "PENE3" });
        SiteFactory.createNewSite("PS1-SV-UCC2", 74, "Penascal", new string[] { "PENE1", "PENE2", "PENE3" });
        SiteFactory.createNewSite("PH1-SV-UCC1", 19, "Providence Heights", new string[] { "PROVH" });
        SiteFactory.createNewSite("PH1-SV-UCC2", 69, "Providence Heights", new string[] { "PROVH" });
        SiteFactory.createNewSite("RUCORE01", 21, "Rugby", new string[] { "RUGBY" });
        SiteFactory.createNewSite("SA1-SV-UCC1", 44, "San Luis", new string[] { "SLUIS" });
        SiteFactory.createNewSite("SHCORE01", 8, "Shiloh", new string[] { "SHILO" });
        SiteFactory.createNewSite("SH1-SV-UCC2", 58, "Shiloh", new string[] { "SHILO" });
        SiteFactory.createNewSite("SC1-SV-UCC1", 43, "South Chestnut", new string[] { "SCHES" });
        SiteFactory.createNewSite("SC1-SV-UCC2", 93, "South Chestnut", new string[] { "SCHES" });
        SiteFactory.createNewSite("SP1-SV-UCC1", 7, "Star Point", new string[] { "STPOI" });
        SiteFactory.createNewSite("SP1-SV-UCC2", 57, "Star Point", new string[] { "STPOI" });
        SiteFactory.createNewSite("TI2-SV-UCC1", 36, "Top of Iowa 2", new string[] { "TOPIO" });
        SiteFactory.createNewSite("TI2-SV-UCC2", 86, "Top of Iowa 2", new string[] { "TOPIO" });
        SiteFactory.createNewSite("TR1-SV-UCC1", 13, "Trimont", new string[] { "TRIMO", "ELMCR" });
        SiteFactory.createNewSite("TR1-SV-UCC2", 63, "Trimont", new string[] { "TRIMO", "ELMCR" });
        SiteFactory.createNewSite("TU1-SV-UCC1", 51, "Tule", new string[] { "TULE1" });
        SiteFactory.createNewSite("TU1-SV-UCC2", 101, "Tule", new string[] { "TULE1" });
        SiteFactory.createNewSite("TB1-SV-UCC1", 15, "Twin Buttes 1", new string[] { "TWINB" });
        SiteFactory.createNewSite("TB1-SV-UCC2", 65, "Twin Buttes 1", new string[] { "TWINB" });
        SiteFactory.createNewSite("TB2-SV-UCC1", 52, "Twin Buttes 2", new string[] { "TWIN2" });
        SiteFactory.createNewSite("TB2-SV-UCC2", 102, "Twin Buttes 2", new string[] { "TWIN2" });
        SiteFactory.createNewSite("WB1-SV-UCC1", 2, "Winnebago", new string[] { "WINNE" });
        SiteFactory.createNewSite("WB1-SV-UCC2", 52, "Winnebago", new string[] { "WINNE" });
        SiteFactory.createNewSite("KL1-WYEAST-UCC", 250, "Wyeast", new string[] { "WYEAS" });


        //Create new  filters
        SiteFactory.createNewFilter("WindNode", "LOGLIST = 'ALMFE16' AND SATT1 = 'CORE'");
        SiteFactory.createNewFilter("RAS", "LOGLIST='ALMFE16' AND SATT1='CORE'");
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
        long epochStart=0;
        long epochEnd=0;

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

        if ((!txtParamDTEnd.Text.Equals("")) && (!txtParamDTStart.Text.Equals(""))){

            if(epochEnd-epochStart > TWO_MONTH_TIME_LIMIT)
            {
                lblAlarms.Text = "Invalid Range. Range must be within two months";
                return null;
            }

        }

        
        

        //PARAMETER: PROJECT
        //if (!txtParamProject.Text.Equals(""))
        //{
        //    oraParams += (oraParams.Equals("") ? "" : " AND ") + "PROJECT = :project";
        //    oraCmd.Parameters.Add(new OracleParameter("project", txtParamProject.Text));
        //    oraCtCmd.Parameters.Add(new OracleParameter("project", txtParamProject.Text));
        //}

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
                string[] prefix = SiteFactory.getPrefixFromSiteName(logList);

                //For single sites (Copper X-ing, Wyeast, etc.)
                if (prefix.Length == 1)
                {
                    oraParams += (oraParams.Equals("") ? "" : " AND ") + "LOGLIST = :loglist";
                    oraCmd.Parameters.Add(new OracleParameter("loglist", "ALM" + prefix[0]));
                }
                //For sites with multiple sub sites (Klondike [KA,K3, etc], Baffin Bay [BB1,BB2], etc.)
                //In this particular case, it is a bad idea to use Oracle cmd substitution. You're better off "hard coding it"
                else
                {
                    //Hard code the first site...this is unavoidable and then loop through the rest. Use skip to skip the first element
                    oraParams += (oraParams.Equals("") ? "" : " AND ") + "(LOGLIST = " + "'ALM" + prefix[0] + "'";
                    foreach (string subSite in prefix.Skip(1))
                    {
                        oraParams += " OR LOGLIST = " + "'ALM" + subSite + "'";
                    }
                    oraParams += ")";
                }
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

            //oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(NAME) LIKE lower(:tagname)";
            //oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
        }

        //PARAMETER: SATT3
        //if (!txtParamSATT3.Text.Equals(""))
        //{
        //    oraParams += (oraParams.Equals("") ? "" : " AND ") + "SATT3 = :satt3";
        //    oraCmd.Parameters.Add(new OracleParameter("satt3", txtParamSATT3.Text));
        //    oraCtCmd.Parameters.Add(new OracleParameter("satt3", txtParamSATT3.Text));
        //}

        //PARAMETER: EVTTYPE

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
        //string oraCstr = "Data Source=ORAPDX;User ID=HIS;Password=HIS;";
        string oraCstr = "Data Source=PT1-SV-ORACLE02/core.ppmems.us;User ID=HIS;Password=HIS;";

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

    private DataTable applyFilter(DataTable inTable, string filterGroup, FilterMode filterMode = FilterMode.Exclude)
    {

        int filterCount = 0;

        DataTable retTable = null;
        try
        {
            //Query filters.
            //DataTable tblFilters = sqlQueryTable(new SqlCommand("SELECT * FROM CORE.dbo.Alarm_Filters WHERE Filter_Group = '" + txtParamFilter.Text + "';"), System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnectionString_PDXSQL03_EMSWEB"].ConnectionString);
            DataTable tblFilters = sqlQueryTable(new SqlCommand("SELECT * FROM CORE.dbo.Alarm_Filters WHERE Filter_Group = '" + "';"), System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnectionString_PDXSQL03_EMSWEB"].ConnectionString);

            debug("<br />Filters applied: ");
            foreach (DataRow row in tblFilters.Rows)
            {
                debug(row.Field<int>("Filter_Id").ToString() + ",");
            }

            //Name tables.
            tblFilters.TableName = "Alarm_Filters";
            inTable.TableName = "Alarms";

            //Create DataSet and add in Tables.
            DataSet tempSet = new DataSet();
            tempSet.Tables.Add(inTable);
            tempSet.Tables.Add(tblFilters);

            //Execute a Linq query to apply the filters stored in SQL against the resultset from Oracle.
            //http://stackoverflow.com/questions/10855/linq-query-on-a-datatable
            //http://www.hookedonlinq.com/JoinOperator.ashx
            var linqAlarms = inTable.AsEnumerable();
            var linqFilters = tblFilters.AsEnumerable();
            //var alarmResults =
            //    from a in linqAlarms
            //    join f in linqFilters on new { a.Field<int>("x"), a.Field<int>("y") } equals new { f.Field<int>("x"), f.Field<int>("x") }
            //    select myRow;

            //Return the DataTable from the IEnumerable<DataRow>
            //retTable = alarmResults.CopyToDataTable<DataRow>();

            //Iterate each alarm, use LINQ to query the alarm filters to filter out junk.
            //TODO: This doesn't work... unfortunately, SqlMethods.Like only works against a Table<> on SQL. So we need to build our own (yay~!~) LIKE method.
            for (int row = tempSet.Tables["Alarms"].Rows.Count; row > 0; row--)
            {

                DataRow thisRow = tempSet.Tables["Alarms"].Rows[row - 1];


                var fltResults =
                    from f in linqFilters
                    where
                        (
                               //TEXTUAL COMPARATORS
                               //linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("UNITNAME"), f.Field<Object>("Filter_Field_UnitName"))
                               linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("PROJECT"), f.Field<Object>("Filter_Field_Project"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("UNITNAME"), f.Field<Object>("Filter_Field_UnitName"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("VARTYPE"), f.Field<Object>("Filter_Field_VarType"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("NAME"), f.Field<Object>("Filter_Field_Name"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("LOGLIST"), f.Field<Object>("Filter_Field_LogList"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("SATT1"), f.Field<Object>("Filter_Field_SATT1"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("SATT2"), f.Field<Object>("Filter_Field_SATT2"))
                            && linqMatch(tempSet.Tables["Alarms"].Rows[row - 1].Field<Object>("SATT3"), f.Field<Object>("Filter_Field_SATT3"))
                            //NUMERIC COMPARATORS
                            && linqMatch(thisRow.Field<Object>("PRIORITY"), f.Field<Object>("Filter_Field_Priority"))
                            && linqMatch(thisRow.Field<Object>("STATION"), f.Field<Object>("Filter_Field_Station"))
                            && linqMatch(thisRow.Field<Object>("TSTYPE"), f.Field<Object>("Filter_Field_TSType"))
                            && linqMatch(thisRow.Field<Object>("EVTTYPE"), f.Field<Object>("Filter_Field_EvtType"))
                            && linqMatch(thisRow.Field<Object>("NVAL"), f.Field<Object>("Filter_Field_NVal"))

                        )
                    /* Delete if:
                     * -Matches Filter AND Filter_Exclude=1
                     * -Does not match filter AND Filter_Exclude=0
                     */
                    //group f by 1 into g
                    //group f by f.Field<bool>("Filter_Exclude") into g
                    //orderby g descending
                    //select new { Count = g.Count() };
                    select f;

                if (fltResults.Count() > 0) //fltResults.Count() > 0)
                {
                    if (filterMode == FilterMode.Exclude)
                    {
                        tempSet.Tables["Alarms"].Rows[row - 1].Delete();
                        filterCount++;
                    }
                }
                else
                {
                    if (filterMode == FilterMode.Include)
                    {
                        tempSet.Tables["Alarms"].Rows[row - 1].Delete();
                        filterCount++;
                    }
                }

                //throw new Exception(fltResults.GetType().FullName);

                //if (fltResults.Count() > 0)

                //DataRow[] dr = tempSet.Tables["Alarm_Filters"].Select("1=1 AND 1=1");
                //if (dr.Count() > 0)
                //{
                //    if (filterMode == FilterMode.Exclude)
                //    {
                //        tempSet.Tables["Alarms"].Rows[row-1].Delete();
                //        filterCount++;
                //    }
                //}
                //else
                //{
                //    if (filterMode == FilterMode.Include)
                //    {
                //        tempSet.Tables["Alarms"].Rows[row-1].Delete();
                //        filterCount++;
                //    }
                //}
            }

            retTable = tempSet.Tables["Alarms"];

        }
        catch (Exception e)
        {
            lblDebug.Text += "<br />" + e.ToString().Replace("\n", "<br />");
        }
        finally
        {
            lblDebug.Text += "<br />inTable rows:" + inTable.Rows.Count + "<br />deleted rows:" + filterCount.ToString();
        }


        return retTable;
    }

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
        //UTC Time format
        //txtParamDTEnd.Text = DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
        //txtParamDTStart.Text = DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");

        clearTextFields();
        //Local Time format
        txtParamDTEnd.Text = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");

        txtParamLogList.Text = "Baffin Bay";
        //txtParamDTStart.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //txtParamDTEnd.Text = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
        //Set defaults (yesterday) for calendar controls.
        //calDateStart.SelectedDate = DateTime.Today.AddDays(-1);
        //calDateEnd.SelectedDate = DateTime.Today.AddDays(-1);

        //Clear out the site.
        //ddlSite.SelectedValue = null;

        //Clear out the person.
        //txtPerson.Text = "";

        //Rebuild the dictionaries
        clearDictionaries();
        buildDictionaries();

        //Clear the ListView
        ListView_Alarms.DataSource = "";
        ListView_Alarms.DataBind();
        List<String> siteList = new List<string>();
        foreach (SiteStructure site in SiteFactory.getSiteList())
        {
            if (!siteList.Contains(site.siteName))
                siteList.Add(site.siteName);
        }

        //Add the filters
        foreach(KeyValuePair<string,string> filter in SiteFactory.getFilters())
        {
            siteList.Add(filter.Key);
        }

        siteList.Sort();

        //Rebind the siteList to the dropdown list
        SiteDropDownList.DataSource = siteList;
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

    protected bool Like(string text, string search)
    {
        if (search == null)
            return true;
        else
        {
            Regex regex = new Regex(string.Format("^{0}$", Regex.Escape(text).Replace("*", ".*").Replace("%", ".*").Replace("_", ".")), RegexOptions.IgnoreCase);
            return regex.IsMatch(text ?? string.Empty);
        }
    }

    protected bool linqMatch(object input, object search)
    {
        if (input == null)
        {
            //input null -- how does this happen?!
            linqDebug("<br />input=null");
            if (search == null)
            {
                linqDebug("; search=null.a");
                return true;
            }
            else
            {
                switch (search.GetType().FullName)
                {
                    case "System.DBNull":
                        linqDebug("; search=DBNull");
                        return true;
                    case "System.String":
                        linqDebug("; search=String:" + (String)search);
                        if (((String)search).Equals("null"))
                        {
                            linqDebug(" - DELETE");
                            return true;
                        }
                        else
                        {
                            linqDebug(" - ignore");
                            return false;
                        }
                        return true;
                    default:
                        linqDebug("; search=UNK TYPE:" + search.GetType().FullName);
                        return true;
                }
            }
        }
        else
        {
            switch (input.GetType().FullName)
            {
                case "System.DBNull":
                    //input DBNull -- if search is valid string, check for DBNull value
                    linqDebug("<br />input=DBNull");
                    if (search == null) { linqDebug("; search=null.b"); return true; }
                    else
                    {
                        switch (search.GetType().FullName)
                        {
                            case "System.DBNull":
                                linqDebug("; search=DBNull");
                                return true;
                            case "System.String":
                                linqDebug("; search=String:" + ((String)search));
                                return true;
                            default:
                                linqDebug("; search=UNK TYPE:" + search.GetType().FullName);
                                return true;
                        }
                    }
                case "System.String":
                    //input valid String -- if search is valid string, check
                    linqDebug("<br />input=String:" + (String)input);
                    if (search == null) { linqDebug("; search=null.c - pass"); return true; }
                    else
                    {
                        switch (search.GetType().FullName)
                        {
                            case "System.DBNull":
                                linqDebug("; search=DBNull");
                                return true;
                            case "System.String":
                                linqDebug("; search=String:" + ((String)search));
                                if (regexLike((String)input, (String)search))
                                {
                                    linqDebug(" - DELETE");
                                    return true;
                                }
                                else
                                {
                                    linqDebug(" - ignore");
                                    return false;
                                }
                            default:
                                linqDebug("; search=UNK TYPE:" + search.GetType().FullName);
                                return true;
                        }
                    }
                case "System.Double":
                    linqDebug("<br />input=Double:" + ((Double)input).ToString());
                    //input valid Double -- if search is valid string, check
                    if (search == null) { linqDebug("; search=null.d - pass"); return true; }
                    else
                    {
                        switch (search.GetType().FullName)
                        {
                            case "System.DBNull":
                                linqDebug("; search=DBNull");
                                return true;
                            case "System.String":
                                linqDebug("; search=String:" + ((String)search));
                                if (((Double)input).ToString().Equals((String)search))
                                {
                                    linqDebug(" - DELETE");
                                    return true;
                                }
                                else
                                {
                                    linqDebug(" - ignore");
                                    return false;
                                }
                            default:
                                linqDebug("; search=UNK TYPE:" + search.GetType().FullName);
                                return true;
                        }
                    }
                default:
                    linqDebug("<br />input=UNK TYPE:" + search.GetType().FullName);
                    if (search == null) { linqDebug("; search=null.e"); return true; }
                    else
                    {
                        switch (search.GetType().FullName)
                        {
                            case "System.DBNull":
                                linqDebug("; search=DBNull");
                                return true;
                            case "System.String":
                                linqDebug("; search=String:" + ((String)search));
                                return true;
                            default:
                                linqDebug("; search=UNK TYPE:" + search.GetType().FullName);
                                return true;
                        }
                    }
            }

        }
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

    //protected void ddlParamProject_SelectedIndexChanged(object sender, EventArgs e) { txtParamProject.Text = ddlParamProject.SelectedValue; }
    //protected void ddlParamLogList_SelectedIndexChanged(object sender, EventArgs e) { txtParamLogList.Text = ddlParamLogList.SelectedValue; }
    protected void ddlParamLogList_SelectedIndexChanged(object sender, EventArgs e) { txtParamLogList.Text = SiteDropDownList.SelectedValue; }
    //protected void ddlParamFilter_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilter.Text = ddlParamFilter.SelectedValue; }
    //protected void ddlParamFilter_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilter.Text = ddlParamFilter.SelectedValue; updateFilterGrid(); }
    //protected void ddlParamFilterMode_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilterMode.Text = ddlParamFilterMode.SelectedValue; }

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