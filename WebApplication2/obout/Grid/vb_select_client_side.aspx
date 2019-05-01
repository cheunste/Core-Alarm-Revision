<%@ Page Language="VB" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script language="VB" runat="server">	
	Dim grid1 As Obout.Grid.Grid = New Grid()
	
	Sub Page_load(ByVal sender As Object, ByVal e As EventArgs )
		grid1.ID = "grid1"
		grid1.CallbackMode = True
		grid1.Serialize = True
		grid1.AutoGenerateColumns = False
		
        grid1.FolderStyle = "styles/premiere_blue"

		' setting the event handlers
		AddHandler CType(grid1, Grid).InsertCommand, AddressOf InsertRecord
		AddHandler CType(grid1, Grid).DeleteCommand, AddressOf DeleteRecord
		AddHandler CType(grid1, Grid).UpdateCommand, AddressOf UpdateRecord
		AddHandler CType(grid1, Grid).Rebind, AddressOf RebindGrid
		
		' creating the columns
		Dim oCol1 As Column = new Column()
		oCol1.DataField = "OrderID"
		oCol1.ReadOnly = True
		oCol1.HeaderText = "ORDER ID"
		oCol1.Width = "100"

		Dim oCol2 As Column = new Column()
		oCol2.DataField = "ShipName"
		oCol2.HeaderText = "NAME"
		oCol2.Width = "200"

		Dim oCol3 As Column = new Column()
		oCol3.DataField = "ShipCity"
		oCol3.HeaderText = "CITY"
		oCol3.Width = "150"

		Dim oCol4 As Column = new Column()
		oCol4.DataField = "ShipPostalCode"
		oCol4.HeaderText = "POSTAL CODE"
		oCol4.Width = "150"

		Dim oCol5 As Column = new Column()
		oCol5.DataField = "ShipCountry"
		oCol5.HeaderText = "COUNTRY"
		oCol5.Width = "150"

        Dim oCol6 As Column = new Column()
        oCol6.DataField = "OrderDate"
        oCol6.HeaderText = "ORDER DATE"
        oCol6.Width = "175"        
        
        Dim oCol7 As Column = new Column()
        oCol7.HeaderText = "EDIT"
        oCol7.Width = "125"
        oCol7.AllowEdit = True
        oCol7.AllowDelete = True

		' add the columns to the Columns collection of the grid
		grid1.Columns.Add(oCol1)
		grid1.Columns.Add(oCol2)
		grid1.Columns.Add(oCol3)
		grid1.Columns.Add(oCol4)
		grid1.Columns.Add(oCol5)
        grid1.Columns.Add(oCol6)
        grid1.Columns.Add(oCol7)
		
		' add the grid to the controls collection of the PlaceHolder
		phGrid1.Controls.Add(grid1)
				
		If Not Page.IsPostBack Then
			BindGrid()			
		End If
	End Sub

	Sub BindGrid()
		Dim myConn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"))

		Dim myComm As OleDbCommand = New OleDbCommand("SELECT TOP 25 * FROM Orders ORDER BY OrderID DESC", myConn)
		myConn.Open()
		'Dim da = new OleDbDataAdapter()
		'Dim ds = new DataSet()
		'da.SelectCommand = myComm
		'da.Fill(ds, "Orders")
		Dim myReader As OleDbDataReader = myComm.ExecuteReader()


		grid1.DataSource = myReader
		grid1.DataBind()

		myConn.Close()
	End Sub
		
    Sub DeleteRecord(ByVal sender As Object, ByVal e As GridRecordEventArgs)
        Dim myConn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"))
        myConn.Open()
        
        Dim myComm As OleDbCommand = New OleDbCommand("DELETE FROM Orders WHERE OrderID = @OrderID", myConn)
        
        myComm.Parameters.Add("@OrderID", OleDbType.Integer).Value = e.Record("OrderID")
        
        myComm.ExecuteNonQuery()
        myConn.Close()
    End Sub
	
    Sub UpdateRecord(ByVal sender As Object, ByVal e As GridRecordEventArgs)
        Dim myConn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"))
        myConn.Open()
        
        Dim myComm As OleDbCommand = New OleDbCommand("UPDATE Orders SET ShipName = @ShipName, ShipCity = @ShipCity, ShipPostalCode=@ShipPostalCode, ShipCountry = @ShipCountry, OrderDate=@OrderDate WHERE OrderID = @OrderID", myConn)

        myComm.Parameters.Add("@ShipName", OleDbType.VarChar).Value = e.Record("ShipName")
        myComm.Parameters.Add("@ShipCity", OleDbType.VarChar).Value = e.Record("ShipCity")
        myComm.Parameters.Add("@ShipPostalCode", OleDbType.VarChar).Value = e.Record("ShipPostalCode")
        myComm.Parameters.Add("@ShipCountry", OleDbType.VarChar).Value = e.Record("ShipCountry")
        myComm.Parameters.Add("@OrderDate", OleDbType.Date).Value = e.Record("OrderDate")
        myComm.Parameters.Add("@OrderID", OleDbType.Integer).Value = e.Record("OrderID")
        
        myComm.ExecuteNonQuery()
        myConn.Close()
    End Sub
	
    Sub InsertRecord(ByVal sender As Object, ByVal e As GridRecordEventArgs)
        Dim myConn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"))
        myConn.Open()
		
        Dim myComm As OleDbCommand = New OleDbCommand("INSERT INTO Orders (ShipName, ShipCity, ShipPostalCode, ShipCountry, OrderDate) VALUES(@ShipName, @ShipCity, @ShipPostalCode, @ShipCountry, @OrderDate)", myConn)
		
        myComm.Parameters.Add("@ShipName", OleDbType.VarChar).Value = e.Record("ShipName")
        myComm.Parameters.Add("@ShipCity", OleDbType.VarChar).Value = e.Record("ShipCity")
        myComm.Parameters.Add("@ShipPostalCode", OleDbType.VarChar).Value = e.Record("ShipPostalCode")
        myComm.Parameters.Add("@ShipCountry", OleDbType.VarChar).Value = e.Record("ShipCountry")
        myComm.Parameters.Add("@OrderDate", OleDbType.Date).Value = e.Record("OrderDate")
  
        myComm.ExecuteNonQuery()
        myConn.Close()
    End Sub
	
	Sub RebindGrid(ByVal sender As Object, ByVal e As EventArgs)
		BindGrid()
	End Sub
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
		<script type="text/javascript">
		function selectRecordFromGrid() {
		    var iIndex = document.getElementById("txtIndex").value;
		    if(iIndex != "") {
		        grid1.selectRecord(iIndex);
		    }
		}
		function deselectRecordFromGrid() {
		    var iIndex = document.getElementById("txtIndex").value;
		    if(iIndex != "") {
		        grid1.deselectRecord(iIndex);
		    }
		}
		
		</script>		
	</head>
	<body>	
		<form runat="server">
					
		<br />
		<span class="tdText"><b>ASP.NET Grid - Select Records on the Client-Side</b></span>
		<br /><br />
		<span class="tdText">
		    Enter the index of a record <input type="text" id="txtIndex" value="" class="tdText" size="5" />
		    <input type="button" onclick="selectRecordFromGrid()" value="Select" class="tdText" />
		    <input type="button" onclick="deselectRecordFromGrid()" value="Deselect" class="tdText" />
		</span>
		<br /><br />		
		
		<asp:PlaceHolder ID="phGrid1" runat="server"></asp:PlaceHolder>
					
		<br /><br /><br />
		
		<span class="tdText">
		    Use the <b>selectRecord</b> and <b>deselectRecord</b> methods to select/deselect records on the client-side.
		    <br /><br />
		    <span class="option2">
                grid1.selectRecord(0);
		        <br />
                &#160;grid1.deselectRecord(4);
		    </span>		    
		</span>
		
		<br /><br /><br />
		
		<a href="Default.aspx?type=VBNET">� Back to examples</a>
		
		</form>
	</body>
</html>