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


// This is a class that's used to essentually map a computer name, with a node number, a sitename, etc.
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

/// <summary>
/// This is a inner class used to create a "site" object. 
/// </summary>
/*
 * This is a class used to create SiteStructure objects, but ended up doing much more than that because of site prefix and filter requirements 
 * 
 * Contains methods for creating a site as well as methods to set/get the content in the dictionaries members
 *  
 */
public static class SiteFactory
{
    //These are private members to this class. 
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

            //Oddly enough, I can't adccess Dictionary.TryAdd() method. Going to settle with an alternative
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
    private string TURBINE_OPTION = "turbine";
    private long TWO_MONTH_TIME_LIMIT = 5184000000;
    //This is the Oracle DB password. It needs to be changed here
    private string ORALCE_SOURCE = "Data Source=PT1-SV-ORACLE02/core.ppmems.us;User ID=HIS;Password=HIS;";

    //The queryDict is a Dictionary that stores the Header (used to display to user) and the query name (used for the SQL portion)
    private Dictionary<string, string> queryDict = new Dictionary<string, string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        // if Page is being rendered for the first time
        if (Page.IsPostBack)
        {
            //no operation
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
    /// <summary>
    /// function called when user clicks the search button 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    /// <summary>
    /// Function called when user clicks on the save to csv function
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lnkParametersCSV_Click(object sender, EventArgs e)
    {
        DataTable myDT = applyParameters();
        if (myDT != null)
        {
            string fileName = "OracleAlarms_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-HHmmss");
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

        /*----Lots of Other sites removed for convience sake----*/

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
    /// <summary>
    /// Private function that applies the parameters from the user and forms an Oracle SQL requests. 
    /// Then it submits it to the Oracle DB
    /// </summary>
    /// <returns></returns>
    private DataTable applyParameters()
    {
        DataTable myDT = new DataTable(); //Temporary table to bind ListView to.

        string searchOption = coreSearchOption.SelectedValue;

        //Build the dictionaries 
        buildDictionaries();

        //Build the plsql command.
        string oraParams = "";
        string tableName = "HIS.ALARMS";

        //Use 4 threads
        string oraQry = "SELECT /*+PARALLEL(4)*/ ";
        foreach (KeyValuePair<string, string> queryEntry in queryDict)
        {
            oraQry += queryEntry.Value + " AS \"" + queryEntry.Key + "\",";
        }
        oraQry = oraQry.TrimEnd(',') + " FROM " + tableName;

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
                string prefix = SiteFactory.getPrefixFromSiteName(logList);

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "LOGLIST = :loglist";
                oraCmd.Parameters.Add(new OracleParameter("loglist", "ALM" + prefix));
            }
        }

        //PARAMETER: NAME
        if (!txtParamName.Text.Equals(""))
        {
            // If the user selected to search by tag name 
            if (coreSearchOption.SelectedValue.Equals(NAME_OPTION))
            {
                oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(NAME) LIKE lower(:tagname)";
                //oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
            }
            //If the user sleected to search by Turbine Name
            else if (coreSearchOption.SelectedValue.Equals(TURBINE_OPTION))
            {

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(SATT3) LIKE lower(:tagname)";
                //oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
            }
            // If the user decided to search by description 
            else
            {

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(TITLE) LIKE lower(:tagname)";
                //oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
            }
                oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
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

        //Make hte call to Oracle
        myDT = oraQueryTable(oraCmd, oraCstr);

        clearDictionaries();

        return postTableQueryEdits(myDT);
        //return myDT;
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

        //Make a copy of the station id column
        int stationIndex = newTable.Columns.IndexOf(STATION_COL);
        newTable.Columns.Remove(STATION_COL);
        DataColumn newStationCol = newTable.Columns.Add(STATION_COL);
        newStationCol.SetOrdinal(stationIndex);

        //Make a copy of the time column
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
    
        Grid_FilterEditor.ClearPreviousDataSource();
        Grid_FilterEditor.DataSourceID = "SqlDataSource_FilterEditor";
        Grid_FilterEditor.DataBind();
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