<%@ Page Language="VB" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

<script runat="server" Language="VB">
    Dim grid1 As Obout.Grid.Grid = New Grid()
    
    Sub Page_load(ByVal sender As Object, ByVal e As EventArgs )
        grid1.ID = "grid1"
        grid1.CallbackMode = true
        grid1.Serialize = false
        grid1.AutoGenerateColumns = false
        grid1.FolderStyle = "styles/grand_gray"
        grid1.AllowAddingRecords = false
        grid1.DataSourceID = "sds1"

        ' creating the columns
        Dim oCol1 As Column = New Column()
        oCol1.DataField = "OrderID"
        oCol1.ReadOnly = true
        oCol1.HeaderText = "ORDER ID"
        oCol1.Width = "125"
        oCol1.Visible = false

        Dim oCol2 As Column = New Column()
        oCol2.DataField = "OrderDate"
        oCol2.HeaderText = "ORDER DATE"       
        oCol2.DataFormatString = "{0:MM/dd/yyyy}"
        oCol2.ApplyFormatInEditMode = true

        Dim oCol3 As Column = New Column()
        oCol3.DataField = "ShippedDate"
        oCol3.HeaderText = "SHIPPED DATE"
        oCol3.DataFormatString = "{0:dd-MMM-yyyy}"
        oCol3.ApplyFormatInEditMode = true

        Dim oCol4 As Column = New Column()
        oCol4.DataField = "RequiredDate"
        oCol4.HeaderText = "REQUIRED DATE"
        oCol4.DataFormatString = "{0:MMMM dd, yyyy}"
        oCol4.ApplyFormatInEditMode = true

        Dim oCol5 As Column = New Column()
        oCol5.DataField = "Freight"
        oCol5.HeaderText = "FREIGHT"
        oCol5.Width = "150"
        oCol5.DataFormatString = "{0:C2}"
        oCol5.ApplyFormatInEditMode = true

        Dim oCol6 As Column = New Column()
        oCol6.DataField = ""
        oCol6.Width = "125"        
        oCol6.AllowEdit = true

        ' add the columns to the Columns collection of the grid
        grid1.Columns.Add(oCol1)
        grid1.Columns.Add(oCol2)
        grid1.Columns.Add(oCol3)
        grid1.Columns.Add(oCol4)
        grid1.Columns.Add(oCol5)
        grid1.Columns.Add(oCol6)        
        
        Dim i As Integer
        For i = 0 To grid1.Columns.Count - 1
            grid1.Columns(i).ApplyFormatInEditMode = chkSwitchFormatting.Checked
        Next i

        ' add the grid to the controls collection of the PlaceHolder
        phGrid1.Controls.Add(grid1)
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
			
	</head>
	<body>	
		<form runat="server">					
		<br />
		<span class="tdText"><b>ASP.NET Grid - Formatting data in edit mode using the ApplyFormatInEditMode & DataFormatString properties</b></span>		
		
		<br /><br />
		
		<asp:CheckBox runat="server" ID="chkSwitchFormatting" CssClass="tdText" AutoPostBack="true" Checked="true" Text="Apply formatting in edit mode" />
		
		<br /><br />
		
		<asp:PlaceHolder ID="phGrid1" runat="server"></asp:PlaceHolder>		
		
		<br />
		
		<span class="tdText">
		    If <b>ApplyFormatInEditMode</b> is set to <span class="option2">true</span>, the formatting specified with the <b>DataFormatString</b> property will be applied in edit mode as well.
		</span>
				
		<asp:SqlDataSource runat="server" ID="sds1" SelectCommand="SELECT TOP 25 * FROM Orders ORDER BY OrderID DESC"
		 ConnectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Northwind.mdb;" ProviderName="System.Data.OleDb"></asp:SqlDataSource>
		
		<br /><br /><br />
		
		<a href="Default.aspx?type=VBNET">� Back to examples</a>
		
		</form>
	</body>
</html>