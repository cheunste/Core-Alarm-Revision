<%@ Page language="VB"%>

<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script language="VB" runat="server">
	
	Sub Page_load(ByVal sender As Object, ByVal e As EventArgs)		        
		If Not Page.IsPostBack Then
            CreateGrid()
		End If
        
        Dim oTree As obout_ASPTreeView_2_NET.Tree = New obout_ASPTreeView_2_NET.Tree()
        oTree.AddRootNode("I'm the root node", true, Nothing)

        oTree.Add("root", "a0", "obout.com", true, Nothing, Nothing)
        oTree.Add("a0", "a0_0", "ASP TreeView", true, "Folder.gif"" onclick=""ob_t27(this)", Nothing)
        oTree.Add("a0", "a0_1", "Fast", true, Nothing, Nothing)
        oTree.Add("a0", "a0_2", "Easy", true, "page.gif", Nothing)

        oTree.FolderIcons = "../TreeView/tree2/icons"
        oTree.FolderScript = "../TreeView/custom_scripts/dnd_to_grid"
        oTree.FolderStyle = "../TreeView/tree2/style/xpBlue"
        oTree.SelectedEnable = true
        oTree.DragAndDropEnable = true
        oTree.Width = "250px"

        ASPTreeView.Text = oTree.HTML()
       
	End Sub	

	Sub CreateGrid()
	
		Dim myConn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"))

        Dim myComm As OleDbCommand = New OleDbCommand("SELECT TOP 50 * FROM Orders", myConn)
		myConn.Open()
        Dim da As OleDbDataAdapter = New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
		da.SelectCommand = myComm
		da.Fill(ds, "Orders")

        grid1.DataSource = ds
		grid1.DataBind()

        
		myConn.Close()
        
	End Sub 
	
    Function GetTemplateIcon(ByVal sFreight As String) As String
        sFreight = sFreight.Replace("$", "")
        If Double.Parse(sFreight) < 25 Then
            Return "../TreeView/tree2/icons/square_yellowS.gif"
        ElseIf Double.Parse(sFreight) < 50 Then
            Return "../TreeView/tree2/icons/square_greenS.gif"
        ElseIf Double.Parse(sFreight) < 100 Then
            Return "../TreeView/tree2/icons/square_blueS.gif"
        Else
            Return "../TreeView/tree2/icons/square_redS.gif"
        End If
    End Function
	</script>


<html>
<head>
    <script language="javascript" src="resources/custom_scripts/dndFromGridToTree.js"></script>
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
    <style type="text/css">
        .tdTextSmall {
		    font:9px Verdana;
		    color:#333333;
	    }
    </style>
    <style type="text/css">
			.tdText {
				font:11px Verdana;
				color:#333333;
			}
			.option2{
				font:11px Verdana;
				color:#0033cc;
				background-color___:#f6f9fc;
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
<body style="margin: 0px;">
<br />
<br />
		
	<form runat="server">					
		<table>
			<tr>
			    <td valign="top">
                    <asp:Literal runat="server" ID="ASPTreeView" EnableViewState="false"></asp:Literal>									
				</td>
				<td valign="top">
			        <obout:Grid id="grid1" EnableRecordHover="false" GenerateRecordIds="true"  PageSize="10"
			            runat="server" AllowMultiRecordSelection="false" KeepSelectedRecords="true" EnableTypeValidation="true"
			            AutoGenerateColumns="false" CallbackMode="true" Serialize="true" AllowAddingRecords="false"
			            FolderStyle="styles/grand_gray">
			            <ClientSideEvents OnClientCallback="attachDndToRecords" />
		                <Columns>                            
                            <obout:Column ID="Column6" DataField="Freight"  Width="75" Visible="true" HeaderText="NAME" runat="server">
                                <TemplateSettings TemplateID="tplImage" />
                            </obout:Column>
                            <obout:Column ID="Column2" DataField="ShipName"  Width="150" HeaderText="NAME" runat="server"/>			
                            <obout:Column ID="Column3" DataField="ShipCity" Width="150" HeaderText="CITY" runat="server" />
                            <obout:Column ID="Column4" DataField="ShipPostalCode" Width="125" HeaderText="POSTAL CODE" runat="server" />
                            <obout:Column ID="Column5" DataField="ShipCountry" Width="125" HeaderText="COUNTRY" runat="server" />                           
                            <obout:Column ID="Column1" DataField="OrderID" Visible="false" Width="100" ReadOnly="true" HeaderText="" runat="server"/>
                        </Columns> 	
                        <Templates>
                            <obout:GridTemplate runat="server" id="tplImage">
	                            <Template><img src="<%# GetTemplateIcon(Container.DataItem("Freight").ToString())  %>" alt="" /></Template>
	                        </obout:GridTemplate>
	                    </Templates>
			        </obout:Grid>	
			    </td>			    
			</tr>
		</table>
		
		<br /><br /><br />
		
		<a href="Default.aspx?type=VBNET">« Back to examples</a>
    </form>		
    <script type="text/javascript">
        attachDndToRecords();
    </script>
</body>
</html>