<%@ Page Language="C#" %>
<%@ Register TagPrefix="owd" Namespace="OboutInc.Window" Assembly="obout_Window_NET"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Dialog Position</title>

<script runat="server">
    void Page_Load(object o, EventArgs e)
    {
        switch(ddPosition.SelectedValue)
        {
            case "0": myDialog.Position = Positions.CUSTOM; break;
            case "1": myDialog.Position = Positions.SCREEN_CENTER; break;                            
        }
         
    }
</script>
</head>
<body style="font-family:Tahoma;font-size:10pt;">
            <a href="Default.aspx?div=aspnet">� Back to examples</a>
	        <br /><br /><br /><br />
        <br />
        
        <form id="Form1" runat="Server">
        Select the type of Position:
        <asp:DropDownList ID="ddPosition" runat="Server" AutoPostBack="true">
            <asp:ListItem Value="0">CUSTOM</asp:ListItem>
            <asp:ListItem Value="1">SCREEN_CENTER</asp:ListItem>            
        </asp:DropDownList>
        Open Dialog and try scrolling the page.
        <br /><br />
        
        
            <input type="Button" value="Open" onclick="myDialog.Open()" />
        
        <br />
        <!--Create Hello World Dialog -->
        <owd:Dialog ID="myDialog" runat="server" Height="115" Top="200" Left="300" StyleFolder="wdstyles/default" Title="Obout Dialog" Width="200" VisibleOnLoad="false" zIndex="10">
            <center>
                <br />
                Hello World
                <br /><br />
                <input type="Button" value="OK" onclick="myDialog.Close()" />
                
            </center>
        </owd:Dialog>
        </form>
              <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	                
	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        <br /><br /><br /><br />
	        	        <br /><br /><br /><br />
	        <br /><br /><br /><br />  
        
        
</body>
</html>
