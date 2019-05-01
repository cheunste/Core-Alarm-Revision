<%@ Page Language="C#" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 

<script type="text/C#" runat="server">
    Grid grid1 = new Grid();
    DetailGrid grid2 = new DetailGrid();

    void Page_load(object sender, EventArgs e)
    {
        // Creating grid1
        grid1.ID = "grid1";
        grid1.CallbackMode = true;
        grid1.Serialize = false;
        grid1.AutoGenerateColumns = false;
        grid1.AllowAddingRecords = true;
        grid1.DataSourceID = "sds1";
        grid1.PageSize = 5;
        grid1.AllowFiltering = false;

        grid1.MasterDetailSettings.LoadingMode = DetailGridLoadingMode.OnCallback;
        
        grid1.InsertCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        grid1.DeleteCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        grid1.UpdateCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        
        grid1.ClientSideEvents.OnClientInsert = "onEmptyCommand";
        grid1.ClientSideEvents.OnClientUpdate = "onEmptyCommand";
        grid1.ClientSideEvents.OnClientDelete = "onEmptyCommand";

        // creating the columns
        Column oCol1 = new Column();
        oCol1.DataField = "CustomerID";
        oCol1.HeaderText = "CUSTOMER ID";

        Column oCol2 = new Column();
        oCol2.DataField = "CompanyName";
        oCol2.HeaderText = "COMPANY NAME";
        oCol2.Width = "285";

        Column oCol3 = new Column();
        oCol3.DataField = "ContactName";
        oCol3.HeaderText = "CONTACT NAME";

        Column oCol4 = new Column();
        oCol4.DataField = "Country";
        oCol4.HeaderText = "COUNTRY";

        Column oCol5 = new Column();
        oCol5.DataField = "";
        oCol5.AllowEdit = true;
        oCol5.AllowDelete = true;
        oCol5.Width = "140";

        // add the columns to the Columns collection of the grid
        grid1.Columns.Add(oCol1);
        grid1.Columns.Add(oCol2);
        grid1.Columns.Add(oCol3);
        grid1.Columns.Add(oCol4);
        grid1.Columns.Add(oCol5);


        // Creating grid2
        grid2.ID = "grid2";
        grid2.CallbackMode = true;
        grid2.Serialize = false;
        grid2.AutoGenerateColumns = false;
        grid2.AllowAddingRecords = true;
        grid2.ShowFooter = true;
        grid2.AllowPageSizeSelection = true;
        grid2.AllowPaging = false;
        grid2.DataSourceID = "sds2";
        grid2.ForeignKeys = "CustomerID";
        grid2.PageSize = 5;

        grid2.MasterDetailSettings.LoadingMode = DetailGridLoadingMode.OnCallback;
        
        grid2.InsertCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        grid2.DeleteCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        grid2.UpdateCommand += new Obout.Grid.Grid.EventHandler(EmptyCommand);
        
        grid2.ClientSideEvents.OnClientInsert = "onEmptyCommand";
        grid2.ClientSideEvents.OnClientUpdate = "onEmptyCommand";
        grid2.ClientSideEvents.OnClientDelete = "onEmptyCommand";


        // creating the columns
        Column oCol2_1 = new Column();
        oCol2_1.DataField = "CustomerID";
        oCol2_1.HeaderText = "CUSTOMER ID";
        oCol2_1.Visible = false;
        oCol2_1.ReadOnly = true;

        Column oCol2_2 = new Column();
        oCol2_2.DataField = "OrderID";
        oCol2_2.HeaderText = "ORDER ID";
        oCol2_2.Visible = false;
        oCol2_1.ReadOnly = true;

        Column oCol2_3 = new Column();
        oCol2_3.DataField = "ShipName";
        oCol2_3.HeaderText = "SHIP NAME";
        oCol2_3.Width = "250";

        Column oCol2_4 = new Column();
        oCol2_4.DataField = "ShipCity";
        oCol2_4.HeaderText = "SHIP CITY";
        oCol2_4.Width = "200";

        Column oCol2_5 = new Column();
        oCol2_5.DataField = "ShipCountry";
        oCol2_5.HeaderText = "SHIP COUNTRY";
        oCol2_5.DataFormatString = "{0:C2}";
        oCol2_5.Width = "200";

        Column oCol2_6 = new Column();
        oCol2_6.DataField = "";
        oCol2_6.AllowEdit = true;
        oCol2_6.AllowDelete = true;
        oCol2_6.Width = "210";
        oCol2_6.Align = "center";

        // add the columns to the Columns collection of the grid
        grid2.Columns.Add(oCol2_1);
        grid2.Columns.Add(oCol2_2);
        grid2.Columns.Add(oCol2_3);
        grid2.Columns.Add(oCol2_4);
        grid2.Columns.Add(oCol2_5);
        grid2.Columns.Add(oCol2_6);

        grid1.DetailGrids.Add(grid2);


        // add the grid to the controls collection of the PlaceHolder
        phGrid1.Controls.Add(grid1);
    }
    
    protected void EmptyCommand(object sender, GridRecordEventArgs e)
    {
    }
</script>

<html>
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
		<script type="text/javascript">
		    function onEmptyCommand(record) {
		        alert('The operation was successful. For this example, the database has not been updated.');
		    }
		</script>
	</head>
	<body>	
		<form runat="server">					
		<br />
		<span class="tdText"><b>ASP.NET Grid - Add / edit / delete at runtime</b></span>		
		
		<br /><br />
		
		<asp:PlaceHolder ID="phGrid1" runat="server"></asp:PlaceHolder>
		
		<br /><br />				
		
		<span class="tdText">
		    The DetailGrids can use all the features that are available for regular grids (e.g. adding, editing, deleting, sorting, paging, etc.)
		</span>
				
		<asp:SqlDataSource runat="server" ID="sds1" ProviderName="System.Data.OleDb"
		    ConnectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Northwind.mdb;"
		    SelectCommand="SELECT TOP 15 * FROM [Customers]">		    
		 </asp:SqlDataSource>
		 
		<asp:SqlDataSource runat="server" ID="sds2" SelectCommand="SELECT * FROM [Orders] WHERE CustomerID = ?"
		 ConnectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Northwind.mdb;" ProviderName="System.Data.OleDb">
		    <SelectParameters>
                <asp:Parameter Name="CustomerID" Type="String" />
            </SelectParameters>
		</asp:SqlDataSource>
		
		<asp:SqlDataSource runat="server" ID="sds3" SelectCommand="SELECT * FROM [Order Details] WHERE OrderID = ?"
		 ConnectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Northwind.mdb;" ProviderName="System.Data.OleDb">
		    <SelectParameters>
                <asp:Parameter Name="OrderID" Type="Int32" />
            </SelectParameters>
		</asp:SqlDataSource>
		
		<br /><br /><br />
		
		<a href="Default.aspx?type=CSHARP">� Back to examples</a>
		
		</form>
	</body>
</html>