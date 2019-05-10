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

public partial class alarms_Default : System.Web.UI.Page
{

    private DataTable dtAlarms;
    private DateTime epoch = new DateTime(1970, 1, 1);
    //The queryDict is a Dictionary that stores the Header (used to display to user) and the query name (used for the SQL portion)
    private Dictionary<string,string> queryDict  = new Dictionary<string,string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            List<String> siteList = new List<string>();
            siteList.Add("");
            SiteDropDownList.DataSource = null;

           
        }
        if (Page.IsPostBack)
        {
            //noop
             //Add the  headers and the query columns to the dictionary
            //queryDict.Add("Time", "CHRONO");
            //queryDict.Add("Project", "PROJECT");
            //queryDict.Add("Alarm List","LOGLIST");
            //queryDict.Add("SATT 3", "SATT3");
            //queryDict.Add("Tag Name","NAME");
            //queryDict.Add("Description","TITLE");
            //queryDict.Add("Unit Name", "UNITNAME");
            //queryDict.Add("Var Type", "VARTYPE");
            //queryDict.Add("Priority", "PRIORITY");
            //queryDict.Add("Value", "NVAL");
            //queryDict.Add("T Val", "TVAL");
            //queryDict.Add("Evt Type", "EVTTYPE");
            //queryDict.Add("Evt Title", "EVTITLE");
            //queryDict.Add("Evt Text", "EVTTXT");
            //queryDict.Add("Comp Inf","COMPINF");
            //queryDict.Add("Username", "USERNAME");
            //queryDict.Add("User Note", "USERNOTE");
            //queryDict.Add("TS Type","TSTYPE");
            //queryDict.Add("S Val", "SVAL");
            //queryDict.Add("BATT","BATT");
            //queryDict.Add("SATT 1", "SATT1");
            //queryDict.Add("SATT 2", "SATT2");
            //queryDict.Add("CDATT 8", "CDATT8");
            //queryDict.Add("Station", "STATION");

            queryDict.Add("Time", "CHRONO");
            queryDict.Add("Turbine Name", "SATT3");
            queryDict.Add("Tag Name","NAME");
            queryDict.Add("Tag Description","TITLE");
            queryDict.Add("Value", "NVAL");
            queryDict.Add("Event", "EVTTITLE");
            queryDict.Add("Username", "USERNAME");
            queryDict.Add("Domain", "SATT1");
            queryDict.Add("Nature", "SATT2");
            queryDict.Add("Station", "STATION");
        }
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
        lblAlarms.Text = "Alarms Returned: " + ListView_Alarms.Items.Count;
        if (myDT != null)
            myDT.Dispose();
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
    private DataTable applyParameters()
    {
        DataTable myDT = new DataTable(); //Temporary table to bind ListView to.

        //Build the plsql command.
        string oraParams = "";
        //string oraQry = "SELECT * FROM ALARMS";

        //New Query in construction. Todo: Finsih this shit
        string oraQry = "SELECT /*+PARALLEL(10)*/ ";
        foreach(KeyValuePair<string,string> queryEntry in queryDict)
        {
            oraQry += queryEntry.Value+" AS \""+queryEntry.Key+"\",";
        }
        //remove the last comma from the string and then stap
        oraQry=oraQry.TrimEnd(',')+" FROM HIS.ALARMS";

        string oraOrd = " ORDER BY CHRONO DESC";
        OracleCommand oraCmd = new OracleCommand();

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
                //double epochStart = (double)1000 * ((int)(dtStart - new DateTime(1970, 1, 1)).TotalSeconds);
                //long epochStart = ((long)(dtStart - new DateTime(1970, 1, 1)).TotalMilliseconds);
                long epochStart = ((long)(dtStart.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
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
                long epochEnd = ((long)(dtEnd.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);

                oraParams += (oraParams.Equals("") ? "" : " AND ") + "CHRONO <= :dtEnd";
                oraCmd.Parameters.Add(new OracleParameter("dtEnd", epochEnd));
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
            oraParams += (oraParams.Equals("") ? "" : " AND ") + "LOGLIST = :loglist";
            oraCmd.Parameters.Add(new OracleParameter("loglist", txtParamLogList.Text));
        }

        //PARAMETER: NAME
        if (!txtParamName.Text.Equals(""))
        {
            oraParams += (oraParams.Equals("") ? "" : " AND ") + "lower(NAME) LIKE lower(:tagname)";
            oraCmd.Parameters.Add(new OracleParameter("tagname", txtParamName.Text));
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


        return postQueryTabl(myDT);
    }

    /// <summary>
    /// This function updates the time of the DataTable after queried from Oracle and update the  time to PT and the Station to  h
    /// human readable format
    /// </summary>
    /// <param name="myDT"></param>
    /// <returns>A modified DataTable with changes to Time and changes to Station </returns>
    private DataTable postQueryTabl(DataTable myDT)
    {
        DataTable newTable = myDT.Clone();
        string TIME_COL = "Time";
        string STATION_COL = "Station";

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
            foreach(DataColumn col in newTable.Columns)
            {
                //If Time. This should be in local pacific  time
                if(col == newTimeCol)
                {
                    Object temp = fmtDateTime(epochToDateTime(row[col.ColumnName]).ToLocalTime());
                    newRow.SetField(col, temp);
                }

                //If Station. This should be in a machine name format
                else if (col ==  newStationCol)
                {
                    string temp = random.Next(500, 900).ToString();
                    newRow.SetField(col, temp);

                    //newRow.SetField(col, row[col.ColumnName]);
                }
                else
                {
                    newRow.SetField(col, row[col.ColumnName]);
                }
            }
            newRow.AcceptChanges();
        }
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

    private void resetParameters()
    {
        //UTC Time format
        //txtParamDTEnd.Text = DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
        //txtParamDTStart.Text = DateTime.UtcNow.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");

       //Local Time format
        txtParamDTEnd.Text = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
        //txtParamDTStart.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //txtParamDTEnd.Text = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
        //Set defaults (yesterday) for calendar controls.
        //calDateStart.SelectedDate = DateTime.Today.AddDays(-1);
        //calDateEnd.SelectedDate = DateTime.Today.AddDays(-1);

        //Clear out the site.
        //ddlSite.SelectedValue = null;

        //Clear out the person.
        //txtPerson.Text = "";
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
    protected void ddlParamLogList_SelectedIndexChanged(object sender, EventArgs e) { txtParamLogList.Text = ddlParamLogList.SelectedValue; }
    //protected void ddlParamFilter_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilter.Text = ddlParamFilter.SelectedValue; }
    //protected void ddlParamFilter_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilter.Text = ddlParamFilter.SelectedValue; updateFilterGrid(); }
    //protected void ddlParamFilterMode_SelectedIndexChanged(object sender, EventArgs e) { txtParamFilterMode.Text = ddlParamFilterMode.SelectedValue; }

    protected void lbtDTStart_Click(object sender, EventArgs e) {

        String[] command = ((LinkButton)(sender)).CommandArgument.Split(',');
        String timePeriod = command[1];
        double time = Convert.ToInt16("-"+command[0]);
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