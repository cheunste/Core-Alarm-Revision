<%@ Page Language="C#" Inherits="OboutInc.oboutAJAXPage"  %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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

		OleDbCommand myComm = new OleDbCommand("SELECT * FROM Suppliers", myConn);
		myConn.Open();		
		OleDbDataReader myReader = myComm.ExecuteReader();

		grid1.DataSource = myReader;
		grid1.DataBind();

        myReader.Close();

        myComm = new OleDbCommand("Select * From Suppliers Order By SupplierID DESC", myConn);

        myReader = myComm.ExecuteReader();
        
        grid2.DataSource = myReader;
        grid2.DataBind();
        
		myConn.Close();	
	}

    public int insertRecord(string CompanyName)
    {   
        return 1;
    }
	
	void RebindGrid(object sender, EventArgs e)
	{
		CreateGrid();
	}			
	</script>		

<html>
	<head runat="server">
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
			
            /* for node text */
            td.ob_t2 {
	            color:navy;
	            FONT: 8pt Tahoma; 
	            vertical-align:middle; 
	            border:none; 
	            background-color:transparent;
	            padding:2px;
            }
		</style>	
		<script language="javascript" src="resources/custom_scripts/dndFromGridToGrid.js"></script>
	<script type="text/javascript">
        
	    function attachDndToRecords() {
	        // this function attaches the drag and drop feature to each record from the Grid
	        var sRecordsIds = grid1.getRecordsIds();
	        var arrRecordsIds = sRecordsIds.split(",");
	        for(var i=0; i<arrRecordsIds.length; i++) {	        
	            ob_attachDragAndDrop(document.getElementById(arrRecordsIds[i]));
	        }
	    }
    </script>		
	</head>
	<body>
		<form runat="server">
		<br />
		<span class="tdText"><b>ASP.NET Grid - D-nD to Grid</b></span>		
		
		<br /><br />
		<table class="tdText">
		<tr>
		    <td><b style="color:crimson">Grid Source</b></td>
		    <td></td>
		    <td><b style="color:crimson">Grid Destination</b></td>
		</tr>
		<tr>
		    <td valign="top">
		    <obout:Grid id="grid1" EnableRecordHover="false" GenerateRecordIds="true"  PageSize="10"
		 runat="server" AllowMultiRecordSelection="false" KeepSelectedRecords="true" EnableTypeValidation="true"  OnRebind="RebindGrid" 
		  AutoGenerateColumns="false" CallbackMode="true" Serialize="true" AllowAddingRecords="false"
		  FolderStyle="../grid/styles/black_glass" AllowFiltering="false" AllowManualPaging="false" ShowFooter="false">
		  <ClientSideEvents  OnClientCallback="attachDndToRecords"/>
			<Columns>
				<obout:Column ID="Column1" DataField="SupplierID" ReadOnly="true" HeaderText="ID" Width="45" runat="server"/>				
				<obout:Column ID="Column2" DataField="CompanyName" HeaderText="Company Name" Width="250" runat="server"/>				
				<%--<obout:Column DataField="Address" HeaderText="Address" Width="225" runat="server" />
				<obout:Column DataField="Country" HeaderText="Country" Width="100" runat="server" />--%>
			</Columns>			
		</obout:Grid>    
		    </td>
		    <td style="width: 10px">
		    </td>
		    <td valign="top">
		    <obout:Grid id="grid2" EnableRecordHover="false" GenerateRecordIds="true"  PageSize="10"
		 runat="server" AllowMultiRecordSelection="false" KeepSelectedRecords="true" EnableTypeValidation="true"  OnRebind="RebindGrid" 
		  AutoGenerateColumns="false" CallbackMode="true" Serialize="true" AllowAddingRecords="false"
		  FolderStyle="../grid/styles/black_glass" AllowFiltering="false" AllowManualPaging="false" ShowFooter="false">
		  <ClientSideEvents  OnClientCallback="attachDndToRecords"/>
			<Columns>
				<obout:Column ID="Column3" DataField="SupplierID" ReadOnly="true" HeaderText="ID" Width="45" runat="server"/>				
				<obout:Column ID="Column4" DataField="CompanyName" HeaderText="Company Name" Width="250" runat="server"/>				
				<%--<obout:Column DataField="Address" HeaderText="Address" Width="225" runat="server" />
				<obout:Column DataField="Country" HeaderText="Country" Width="100" runat="server" />--%>
			</Columns>			
		</obout:Grid> 
		    </td>
		</tr>
		
		</table>
		<br /><br /><br />
		
		<a href="Default.aspx?type=ASPNET">« Back to examples</a>
		</form>
	</body>
</html>
<script type="text/javascript">
function init(){
	attachDndToRecords();
}

function addLoadEvent(func) {
  var oldonload = window.onload;
  if (typeof window.onload != 'function') {
	window.onload = func;
  } else {
	window.onload = function() {
	  if (oldonload) {
		oldonload();
	  }
	  func();
	}
  }

}

	    var grid1id = 'grid1_ob_grid1MainContainer';
        var grid2id = 'grid2_ob_grid2MainContainer';

addLoadEvent(init);

</script>


