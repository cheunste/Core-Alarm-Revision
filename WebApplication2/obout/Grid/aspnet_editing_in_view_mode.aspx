<%@ Page Language="C#"  Inherits="OboutInc.oboutAJAXPage" EnableEventValidation="false" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET"%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="oajax" Namespace="OboutInc" Assembly="obout_AJAXPage" %> 


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
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_data/Northwind.mdb"));

		OleDbCommand myComm = new OleDbCommand("SELECT * FROM Suppliers"/* WHERE CompanyName NOT LIKE \"%'%\""*/, myConn);
		myConn.Open();		
		OleDbDataReader myReader = myComm.ExecuteReader();

		grid1.DataSource = myReader;
		grid1.DataBind();

		myConn.Close();	
	}
	void DeleteRecord(object sender, GridRecordEventArgs e)
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_data/Northwind.mdb"));
		myConn.Open();        		

        OleDbCommand myComm = new OleDbCommand("DELETE FROM Suppliers WHERE SupplierID = @SupplierID", myConn);

        myComm.Parameters.Add("@SupplierID", OleDbType.Integer).Value = e.Record["SupplierID"];
        
        myComm.ExecuteNonQuery();
		myConn.Close();
	}
	void UpdateRecord(object sender, GridRecordEventArgs e)
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_data/Northwind.mdb"));
		myConn.Open();

        OleDbCommand myComm = new OleDbCommand("UPDATE Suppliers SET CompanyName = @CompanyName, Address = @Address, Country=@Country WHERE SupplierID = @SupplierID", myConn);

        myComm.Parameters.Add("@CompanyName", OleDbType.VarChar).Value = e.Record["CompanyName"];
        myComm.Parameters.Add("@Address", OleDbType.VarChar).Value = e.Record["Address"];
        myComm.Parameters.Add("@Country", OleDbType.VarChar).Value = e.Record["Country"];
        myComm.Parameters.Add("@SupplierID", OleDbType.Integer).Value = e.Record["SupplierID"];
		
        myComm.ExecuteNonQuery();
		myConn.Close();
	}
	void InsertRecord(object sender, GridRecordEventArgs e)
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_data/Northwind.mdb"));
		myConn.Open();

        OleDbCommand myComm = new OleDbCommand("INSERT INTO Suppliers (CompanyName, Address, Country) VALUES(@CompanyName, @Address, @Country)", myConn);

        myComm.Parameters.Add("@CompanyName", OleDbType.VarChar).Value = e.Record["CompanyName"];
        myComm.Parameters.Add("@Address", OleDbType.VarChar).Value = e.Record["Address"];
        myComm.Parameters.Add("@Country", OleDbType.VarChar).Value = e.Record["Country"];
		
        myComm.ExecuteNonQuery();
		myConn.Close();		
	}
	void RebindGrid(object sender, EventArgs e)
	{
		CreateGrid();
	}		

	public int UpdateRecord2(string SupplierID, string CompanyName, string Address, string Country)
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_data/Northwind.mdb"));
		myConn.Open();

        OleDbCommand myComm = new OleDbCommand("UPDATE Suppliers SET CompanyName = @CompanyName, Address = @Address, Country=@Country WHERE SupplierID = @SupplierID", myConn);

        myComm.Parameters.Add("@CompanyName", OleDbType.VarChar).Value = CompanyName;
        myComm.Parameters.Add("@Address", OleDbType.VarChar).Value = Address;
        myComm.Parameters.Add("@Country", OleDbType.VarChar).Value = Country;
        myComm.Parameters.Add("@SupplierID", OleDbType.Integer).Value = SupplierID;
		
        int ret = myComm.ExecuteNonQuery();
		myConn.Close();

		return ret;
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
		</style>
		<script type="text/javascript">		
		    function doSaveGrid(){
		        var tfArrays = document.getElementsByTagName("input");
		        var rowCount = 0;
		        for (var i=0; i< tfArrays.length; i++ ){
		            var tf = tfArrays[i];
		            
		            if ( tf.type=="text" && tf.id=="txtSupplierID" ){
		                // get txtSupplierID
		                var sSupplierID = tf.value;
		                
		                // get txtCompanyName
		                while ( i< tfArrays.length && tfArrays[i].id !="txtCompanyName" ) i++;
		                var sCompanyName = tfArrays[i].value;
		                
		                // get txtAddress
		                while ( i< tfArrays.length && tfArrays[i].id !="txtAddress" ) i++;
		                var sAddress = tfArrays[i].value;
		                
                        // get txtCountry
		                while ( i< tfArrays.length && tfArrays[i].id !="txtCountry" ) i++;
		                var sCountry = tfArrays[i].value;
		                		                
						try{

							if(typeof ob_post == "object")
							{
								// add the parameter needed on server-side
								// SupplierID, CompanyName, Address, Country will be a server-side argument for the UpdateRecord2 method
								
								ob_post.AddParam("SupplierID", sSupplierID);	
								ob_post.AddParam("CompanyName", sCompanyName);
								ob_post.AddParam("Address", sAddress);
								ob_post.AddParam("Country", sCountry);
								
								var result = ob_post.post(null, "UpdateRecord2");

								if (result <= 0)
								{
									alert("Update record error");
								}
							}
							else
							{
								alert("Please add obout_AJAXPage ASP.NET control to your page to use the AjaxPage");
							} 
						}catch (ex){
							alert(ex);
						}
	   	            }
		        }
		        grid1.refresh();
		        
		    }	
		    

		</script>
		
	</head>
	<body>
		<form runat="server">
		<br /><br />
		<span class="tdtext"><b>Editing records in the Grid view mode.</b></span>
		<br /><br />
		<span class="tdtext">
		Make some change in the text boxes then press <b>Save all</b> to update all records in the grid<br /> <br />
		</span>
		<obout:Grid id="grid1" runat="server" CallbackMode="true" Serialize="true" AllowRecordSelection="false"
			AllowAddingRecords="true" AllowColumnResizing="true" AutoGenerateColumns="false" 
			 FolderStyle="styles/premiere_blue" EnableTypeValidation="false"
			OnRebind="RebindGrid" OnInsertCommand="InsertRecord" OnDeleteCommand="DeleteRecord" OnUpdateCommand="UpdateRecord">
			<Columns>
				<obout:Column DataField="SupplierID" ReadOnly="true" HeaderText="ID" Width="45" runat="server">				
				    <TemplateSettings TemplateID="TmplSupplierID"/>
				</obout:Column>				
				<obout:Column DataField="CompanyName" HeaderText="Company Name" Width="250" runat="server">				
				    <TemplateSettings TemplateID="TmplCompanyName"/>
				</obout:Column>
				<obout:Column DataField="Address" HeaderText="Address" Width="225" runat="server">
				    <TemplateSettings TemplateID="TmplAddress"/>
				</obout:Column>				
				<obout:Column DataField="Country" HeaderText="Country" Width="100" runat="server">
				    <TemplateSettings TemplateID="TmplCountry"/>
				</obout:Column>				
				<obout:Column DataField="" HeaderText="" Width="125" AllowDelete="true" Visible="true" runat="server" />				
			</Columns>			
            <Templates>
                <obout:GridTemplate runat="server" ID="TmplSupplierID">
					<Template>
						<input type="text" id="txtSupplierID" class="ob_gEC" value="<%# Container.Value %>" disabled/>
					</Template>
				</obout:GridTemplate>
				<obout:GridTemplate runat="server" ID="TmplCompanyName">
					<Template>
						<input type="text" id="txtCompanyName" class="ob_gEC" value="<%# Container.Value %>"/>
					</Template>
				</obout:GridTemplate>
                <obout:GridTemplate runat="server" ID="TmplAddress">
					<Template>
						<input type="text" id="txtAddress" class="ob_gEC" value="<%# Container.Value %>"/>
					</Template>
				</obout:GridTemplate>
				<obout:GridTemplate runat="server" ID="TmplCountry">
					<Template>
						<input type="text" id="txtCountry" class="ob_gEC" value="<%# Container.Value %>"/>
					</Template>
				</obout:GridTemplate>
			</Templates>
		</obout:Grid>			
		<br>
		<input type="button" onclick="doSaveGrid();" value="Save all changes" class="tdtext">
		<br /><br /><br />
		
		<a href="Default.aspx?type=ASPNET">« Back to examples</a>
		
		</form>
	</body>
</html>


