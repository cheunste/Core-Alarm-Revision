<%@ Page Language="C#" ASPCOMPAT="TRUE"%>
<%@ Import Namespace="obout_ASPTreeView_2_NET" %>

<!-- For sub tree do NOT put any HTML tags above and below code -->

<script language="C#" runat="server">
	void Page_Load(object sender, EventArgs e) {
		obout_ASPTreeView_2_NET.Tree oTree = new obout_ASPTreeView_2_NET.Tree();

		// These 3 lines prevent from browser caching. They are optional.
		// Useful when your data changes frequently. 
		Response.AddHeader("pragma","no-cache");
		Response.AddHeader("cache-control","private");
		Response.CacheControl = "no-cache";
		
		// For non-English characters. See MSDN for your language settings. 
		//Response.CodePage = 1252;
		//Response.CharSet = "windows-1252";

		// IMPORTANT:  For loaded SubTree set to TRUE.
		oTree.SubTree = true;
		
		oTree.Add("root", "a_11", "<div style=\"height: 18px;\"><nobr>Albert Yap</nobr></div>", true, null, null);
		oTree.Add("a_11", "a_12", "<div style=\"height: 18px;\"><nobr>1st half of Jan</nobr></div>", true, "page.gif", null);
		oTree.Add("a_11", "a_13", "<div style=\"height: 18px;\"><nobr>2nd half of Jan</nobr></div>", true, "page.gif", null);
		oTree.Add("a_11", "a_14", "<div style=\"height: 18px;\"><nobr>1st half of Feb</nobr></div>", true, "page.gif", null);
		oTree.Add("a_11", "a_15", "<div style=\"height: 18px;\"><nobr>2nd half of Feb</nobr></div>", true, "page.gif", null);

		oTree.Add("root", "a_16", "<div style=\"height: 18px;\"><nobr>Mike Baker</nobr></div>", true, null, null);
		oTree.Add("a_16", "a_17", "<div style=\"height: 18px;\"><nobr>1st half of Jan</nobr></div>", true, "page.gif", null);
		oTree.Add("a_16", "a_18", "<div style=\"height: 18px;\"><nobr>2nd half of Jan</nobr></div>", true, "page.gif", null);
		oTree.Add("a_16", "a_19", "<div style=\"height: 18px;\"><nobr>1st half of Feb</nobr></div>", true, "page.gif", null);
		oTree.Add("a_16", "a_20", "<div style=\"height: 18px;\"><nobr>2nd half of Feb</nobr></div>", true, "page.gif", null);

		oTree.FolderStyle = "../TreeView/tree2/style/Classic";
		oTree.FolderIcons = "../TreeView/tree2/icons";
		Response.Write(oTree.HTML());
}
</script>

<!-- For sub tree do NOT put any HTML tags above and below code -->
