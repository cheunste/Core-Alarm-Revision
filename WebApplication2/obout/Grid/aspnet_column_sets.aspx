<%@ Page Language="C#" %>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script language="C#" runat="server">	
    void Page_load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            CreateGrid();
        }
    }

    void CreateGrid()
    {
        OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));

        OleDbCommand myComm = new OleDbCommand("SELECT TOP 25 * FROM Orders ORDER BY OrderID DESC", myConn);
        myConn.Open();

        OleDbDataAdapter da = new OleDbDataAdapter();
        DataSet ds = new DataSet();
        da.SelectCommand = myComm;
        da.Fill(ds, "Orders");


        grid1.DataSource = ds.Tables[0];
        grid1.DataBind();

        myConn.Close();
    }

    void DeleteRecord(object sender, GridRecordEventArgs e)
    {
        OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));
        myConn.Open();

        OleDbCommand myComm = new OleDbCommand("DELETE FROM Orders WHERE OrderID = @OrderID", myConn);

        myComm.Parameters.Add("@OrderID", OleDbType.Integer).Value = e.Record["OrderID"];

        myComm.ExecuteNonQuery();
        myConn.Close();
    }
    void UpdateRecord(object sender, GridRecordEventArgs e)
    {
        OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));
        myConn.Open();

        OleDbCommand myComm = new OleDbCommand("UPDATE Orders SET ShipName = @ShipName, ShipCity = @ShipCity, ShipPostalCode=@ShipPostalCode, ShipCountry = @ShipCountry WHERE OrderID = @OrderID", myConn);

        myComm.Parameters.Add("@ShipName", OleDbType.VarChar).Value = e.Record["ShipName"];
        myComm.Parameters.Add("@ShipCity", OleDbType.VarChar).Value = e.Record["ShipCity"];
        myComm.Parameters.Add("@ShipPostalCode", OleDbType.VarChar).Value = e.Record["ShipPostalCode"];
        myComm.Parameters.Add("@ShipCountry", OleDbType.VarChar).Value = e.Record["ShipCountry"];
        myComm.Parameters.Add("@OrderID", OleDbType.Integer).Value = e.Record["OrderID"];

        myComm.ExecuteNonQuery();
        myConn.Close();
    }
    void InsertRecord(object sender, GridRecordEventArgs e)
    {
        OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));
        myConn.Open();

        OleDbCommand myComm = new OleDbCommand("INSERT INTO Orders (ShipName, ShipCity, ShipPostalCode, ShipCountry) VALUES(@ShipName, @ShipCity, @ShipPostalCode, @ShipCountry)", myConn);

        myComm.Parameters.Add("@ShipName", OleDbType.VarChar).Value = e.Record["ShipName"];
        myComm.Parameters.Add("@ShipCity", OleDbType.VarChar).Value = e.Record["ShipCity"];
        myComm.Parameters.Add("@ShipPostalCode", OleDbType.VarChar).Value = e.Record["ShipPostalCode"];
        myComm.Parameters.Add("@ShipCountry", OleDbType.VarChar).Value = e.Record["ShipCountry"];

        myComm.ExecuteNonQuery();
        myConn.Close();
    }
    void RebindGrid(object sender, EventArgs e)
    {
        CreateGrid();
    }
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>obout ASP.NET Grid examples</title>
    <style type="text/css">
        .tdText
        {
            font: 11px Verdana;
            color: #333333;
        }
        .option2
        {
            font: 11px Verdana;
            color: #0033cc;
            padding-left: 4px;
            padding-right: 4px;
        }
        a
        {
            font: 11px Verdana;
            color: #315686;
            text-decoration: underline;
        }
        a:hover
        {
            color: crimson;
        }
        .ob_MHcell
        {
            color: #FFFFFF;
            cursor: pointer;
            font-family: Verdana;
            font-size: 13px;
            font-weight: bold;
            height: 20px;
            padding-bottom: 3px;
        }
        .ob_MHTCell
        {
            border-color: -moz-use-text-color transparent -moz-use-text-color #CCCCD3;
            border-right:solid 1px #CCCCD3;
            padding-right: 0;
        }
        .ob_MHTbl
        {
            background-image:url(styles/grand_gray/header_without_grouping.gif);
            background-color:#C4C6D1;          
            background-repeat: repeat-x;
            border-left: 0px solid transparent;
            border-right: 0px solid transparent;
        }
    </style>

    <script type="text/javascript">

        window.onload = function() {

            var newRow = new AddMultiColumnHeader("ob_gridMultiColumnHeader");

            newRow.AddCell("colum 1 & 2", 304, "#C4C6D1", "styles/grand_gray/header_without_grouping.gif");
            newRow.AddCell("column 3 & 4 & 5", 454, "#C4C6D1", "styles/grand_gray/header_without_grouping.gif");

        }



        function AddMultiColumnHeader(id) {

            this.ID = id;

            var hRow = createElement("div", "ob_gHContWG");

            var hRTbl = createTable("ob_MHTbl", null);
            hRTbl.id = this.ID;  
            hRow.appendChild(hRTbl);

            var hContainer = document.getElementById("grid1_ob_grid1HeaderContainer");
            hContainer.parentNode.insertBefore(hRow, hContainer);

            this.AddCell = function AddMColHCell(hContainer, hText, width, bgcolor, bgImage) {

                var headerRow = document.getElementById(this.ID);

                headerRow.style.cssText = "border-collapse: collapse;";

                var hTCell = createElement("td", "ob_MHTCell");
                var hTDCell = createElement("div", "ob_MHcell");

                hTDCell.appendChild(document.createTextNode(hText));

                hTCell.style.cssText = "width:" + width + "px; background-image:url(" + bgImage + ");background-color:" + bgcolor + ";";
                hTDCell.style.cssText = "text-align:center;";

                hTCell.appendChild(hTDCell);

                var hTRC = headerRow.firstChild.firstChild.lastChild;
                headerRow.firstChild.firstChild.insertBefore(hTCell, hTRC);

            }

        }

        function createTable(className, checkEmpty) {
            var tblHeader = createElement("table", className);
            tblHeader.setAttribute("cellspacing", "0");
            tblHeader.setAttribute("cellpadding", "0");
            tblHeader.setAttribute("border", "0");

            tblHeader.setAttribute("width", "100%");

            if (checkEmpty == null) {

                var tbdy = document.createElement("tbody");

                var tRow = createElement("tr","ob_gHR");
                tRow.appendChild(createElement("td","ob_gCH"));
                tbdy.appendChild(tRow);
                tblHeader.appendChild(tbdy);

            }

            return tblHeader;
        }


        function createElement(elm, className) {
            var newElem = document.createElement(elm);
            newElem.setAttribute((document.all ? 'className' : 'class'), className);
            return newElem;
        }
       	
    </script>

</head>
<body>
    <form id="Form1" runat="server">
    <br />
    <span class="tdText"><b>ASP.NET Grid - Multiple header rows</b></span>
    <br />
    <br />
    <obout:Grid ID="grid1" runat="server" CallbackMode="true" Serialize="true" AutoGenerateColumns="false"
        FolderStyle="styles/grand_gray" AllowFiltering="true" OnRebind="RebindGrid" OnInsertCommand="InsertRecord"
        OnDeleteCommand="DeleteRecord" OnUpdateCommand="UpdateRecord">
        <Columns>
            <obout:Column ID="Column1" DataField="OrderID" ReadOnly="true" HeaderText="ORDER ID"
                Width="100" runat="server" />
            <obout:Column ID="Column2" DataField="ShipName" HeaderText="NAME" Width="200" runat="server" />
            <obout:Column ID="Column3" DataField="ShipCity" HeaderText="CITY" Width="150" runat="server" />
            <obout:Column ID="Column4" DataField="ShipPostalCode" HeaderText="POSTAL CODE" Width="150"
                runat="server" />
            <obout:Column ID="Column5" DataField="ShipCountry" HeaderText="COUNTRY" Width="150"
                runat="server" />
            <obout:Column ID="Column6" HeaderText="EDIT" AllowEdit="true" AllowDelete="true"
                Width="125" runat="server" />
        </Columns>
    </obout:Grid>
    <br />
    
    <br />
    <br />
    <a href="Default.aspx?type=ASPNET">« Back to examples</a>
    </form>
</body>
</html>
