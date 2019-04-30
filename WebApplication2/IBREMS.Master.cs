//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//
//namespace WebApplication2
//{
//    public partial class SiteMaster : MasterPage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class IBREMS : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Master_User_Identity.Text = Page.User.Identity.Name;
        //Master_User_PNumber.Text = WOS.Utilities.getPNumberFromIdentityName(Page.User.Identity.Name);
        //Master_User_Impersonating.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //Master_Accessed_Time.Text = System.DateTime.Now.ToString();
        //Master_Accessed_URL.Text = Request.Url.ToString();
    }
}
