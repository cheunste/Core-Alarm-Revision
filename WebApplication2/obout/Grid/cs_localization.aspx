<%@ Page Language="C#"%>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script language="C#" runat="server">
	Grid grid1 = new Grid();
		
	void Page_Load(object sender, EventArgs e)
	{		 								
		grid1.ID = "grid1";
		grid1.CallbackMode = true;
		grid1.Serialize = true;
		grid1.AutoGenerateColumns = false;		
		
		grid1.FolderStyle = "styles/grand_gray";
		grid1.FolderLocalization = "localization";
        grid1.AllowFiltering = true;

		// setting the event handlers
		grid1.InsertCommand += new Obout.Grid.Grid.EventHandler(InsertRecord);
		grid1.DeleteCommand += new Obout.Grid.Grid.EventHandler(DeleteRecord);
		grid1.UpdateCommand += new Obout.Grid.Grid.EventHandler(UpdateRecord);
		grid1.Rebind += new Obout.Grid.Grid.DefaultEventHandler(RebindGrid);

		// creating the columns
		Column oCol1 = new Column();
		oCol1.DataField = "OrderID";
		oCol1.ReadOnly = true;
		oCol1.HeaderText = "ORDER ID";
		oCol1.Width = "100";

		Column oCol2 = new Column();
		oCol2.DataField = "ShipName";
		oCol2.HeaderText = "NAME";
		oCol2.Width = "200";

		Column oCol3 = new Column();
		oCol3.DataField = "ShipCity";
		oCol3.HeaderText = "CITY";
		oCol3.Width = "150";

		Column oCol4 = new Column();
		oCol4.DataField = "ShipPostalCode";
		oCol4.HeaderText = "POSTAL CODE";
		oCol4.Width = "150";

		Column oCol5 = new Column();
		oCol5.DataField = "ShipCountry";
		oCol5.HeaderText = "COUNTRY";
		oCol5.Width = "150";

		Column oCol6 = new Column();
		oCol6.HeaderText = "";
		oCol6.Width = "175";
        oCol6.AllowEdit = true;
        oCol6.AllowDelete = true;

		// add the columns to the Columns collection of the grid
		grid1.Columns.Add(oCol1);
		grid1.Columns.Add(oCol2);
		grid1.Columns.Add(oCol3);
		grid1.Columns.Add(oCol4);
		grid1.Columns.Add(oCol5);
		grid1.Columns.Add(oCol6);

		// add the grid to the controls collection of the PlaceHolder
		phGrid1.Controls.Add(grid1);
		
		if (!Page.IsPostBack)
		{
			CreateGrid();
		}

		if (ddlLocalization.SelectedValue != null && ddlLocalization.SelectedValue != String.Empty)
		{
			if (ddlLocalization.SelectedValue != "__")
			{
				grid1.Language = ddlLocalization.SelectedValue;
			}
			else
			{
				grid1.Language = Obout.Grid.Library.AccessibleLanguage(Page, grid1.FolderLocalization);
			}
		}			
	 
	}

	void CreateGrid()
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));

		OleDbCommand myComm = new OleDbCommand("SELECT TOP 25 * FROM Orders ORDER BY OrderID DESC", myConn);
		myConn.Open();
		/*OleDbDataAdapter da = new OleDbDataAdapter();
		DataSet ds = new DataSet();
		da.SelectCommand = myComm;
		da.Fill(ds, "Orders");*/
		OleDbDataReader myReader = myComm.ExecuteReader();


		grid1.DataSource = myReader;
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

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> <html>
	<head>
		<title>obout ASP.NET Grid examples</title>
		<style type="text/css">
			.tdText {
				font:11px Verdana;
				color:#333333;
			}
			.option2{
				font:11px Verdana;
				color:#0033cc;				
				padding-left:4px;
				padding-right:4px;
			}
			a {
				font:11px Verdana;
				color:#315686;
				text-decoration:underline;
			}

			a:hover {
				color:crimson;
			}
		</style>		
	</head>
	<body>	
		<form runat="server">
			<br/>
			<span class="tdText"><b>ASP.NET Grid Localization</b></span>
			<br/>
			<br/>
			<asp:DropDownList runat="server" ID="ddlLocalization" CssClass="tdText" AutoPostBack="true">
				<asp:ListItem>Select a language ...</asp:ListItem>
		        <asp:ListItem Value="bg">Bulgarian</asp:ListItem>
		        <asp:ListItem Value="dk">Danish</asp:ListItem>
                <asp:ListItem Value="nl">Dutch</asp:ListItem>
				<asp:ListItem Value="en" Selected="true">English</asp:ListItem>
				<asp:ListItem Value="fr">French</asp:ListItem>
				<asp:ListItem Value="de">German</asp:ListItem>
				<asp:ListItem Value="gr">Greek</asp:ListItem>
                <asp:ListItem Value="he">Hebrew</asp:ListItem>
				<asp:ListItem Value="it">Italian</asp:ListItem>
				<asp:ListItem Value="jp">Japanese</asp:ListItem>
				<asp:ListItem Value="pl">Polish</asp:ListItem>
				<asp:ListItem Value="ro">Romanian</asp:ListItem>
				<asp:ListItem Value="ru">Russian</asp:ListItem>
				<asp:ListItem Value="sr">Serbian</asp:ListItem>
				<asp:ListItem Value="cn">Chinese - Sipmlified</asp:ListItem>
				<asp:ListItem Value="es">Spanish</asp:ListItem>
				<asp:ListItem Value="se">Swedish</asp:ListItem>
				<asp:ListItem Value="ct">Chinese - Traditional</asp:ListItem>
				<asp:ListItem Value="tr">Turkish</asp:ListItem>
				<asp:ListItem Value="fa">Persian</asp:ListItem>
				<asp:ListItem Value="__">Autodetection</asp:ListItem>
			</asp:DropDownList>
			<br />             
			<span class="tdText">Please scroll down the page to see How To set any language.</span>
			<br /><br />
			
			<asp:PlaceHolder ID="phGrid1" runat="server"></asp:PlaceHolder>	
			
			<br /><br />

			<span class="tdText">
				You can make the <b>ASP.NET Grid</b> support your native language.
				It can be done in a few steps.
				<ol>
				<li>
				Make a copy of <span style="color:blue">en.xml</span> file in <span style="color:blue">localization</span> folder.
				<br/>
				<br/>
				</li>
				<li>
				Rename the copy to <span style="color:blue">xx.xml</span> where <span style="color:blue">xx</span>
				is new supported language name (<span style="color:blue">de</span>, <span style="color:blue">es</span> and so on).
				<br/>
				<br/>
				</li>
				<li>
				Translate semantic fields in the file to selected language.
				<br/>
				<br />
				</li>
				</ol>
				Now you can set the <b>Language</b> property of <b>ASP.NET Grid</b> to the new language name.
				<br/>
				Example: Language = "es"
				<br />
				<br />
				See <nobr><a href='http://www.obout.com/grid/doc_grid_properties.aspx'>Grid properties</a> page for details.</nobr>
				<br/>
				<br />
				We ask you to send your new localization <b>XML</b> files using this <a href="http://www.obout.com/inc/support.aspx">page</a>.
				<br/>
				We will make your localized XML file available for everyone and give you credit for it.
				<br/>
				<br/>
				<br/>
				It is also possible to make the Grid autodetect the language.
			</span>
			
			<br /><br /><br />
		
			<a href="Default.aspx?type=CSHARP">« Back to examples</a>
		
		</form>
	</body>
</html>