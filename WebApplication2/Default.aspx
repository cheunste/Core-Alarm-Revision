<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/IBREMS.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="alarms_Default" %>

<%@ Register Assembly="obout_Window_NET" Namespace="OboutInc.Window" TagPrefix="owd" %>
<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="obout" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <h2>CORE Alarms</h2>

        <table class="tblParameters">
        <thead>
            <tr>
                <th>Parameter</th>
                <th>Value</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
<%--            <tr><td>Start Day</td>
                <td>
                    <ASP:TextBox runat="server" id="txtDateStart" />
                    <obout:Calendar ID="calDateStart" runat="server" DatePickerMode="True" TextBoxId="txtDateStart" DatePickerImagePath="/_images/calicon.gif" />
                </td>
                <td>From midnight.</td></tr>
            <tr><td>End Day</td>
                <td><ASP:TextBox runat="server" id="txtDateEnd" />
                    <obout:Calendar ID="calDateEnd" runat="server" DatePickerMode="True" TextBoxId="txtDateEnd" DatePickerImagePath="/_images/calicon.gif" /></td>
                <td>Until 11:59:59.999</td></tr>--%>
<%--            <tr><td>Site</td>     
                <td><asp:DropDownList runat="server" ID="ddlSite" DataSourceID="SqlDataSource_List_Sites" DataTextField="Transaction_Site" DataValueField="Transaction_Site" AppendDataBoundItems="true">
                        <asp:ListItem Text="(All Sites)" Value="" />
                    </asp:DropDownList>
                    <asp:SqlDataSource runat="server" ID="SqlDataSource_List_Sites"
                        ConnectionString="<%$ ConnectionStrings:ConnectionString_PDXSQL08_EMSWEB %>"
                        SelectCommandType="StoredProcedure" SelectCommand="uspPSPSites"
                    ></asp:SqlDataSource></td>
                <td></td></tr>--%>
            <tr><th>Time Range<br /><span style="font-weight:normal; font-size:small; font-style:italic;">(Pacific Time)</span></th>
                <td colspan="1">
                    <asp:TextBox runat="server" ID="txtParamDTStart" Width="140" />
                    to
                    <asp:TextBox runat="server" ID="txtParamDTEnd" Width="140" />
                    <br />
                    <div class="sidenote">
                        <b>Set start time to (NOW-</b>
                        <asp:LinkButton runat="server" Text="1m"  OnCommand="lbtDTStart_Click" CommandArgument="1,m"  /> |
                        <asp:LinkButton runat="server" Text="10m"  onclick="lbtDTStart_Click" CommandArgument="10,m" /> |
                        <asp:LinkButton runat="server" Text="30m"  onclick="lbtDTStart_Click" CommandArgument="30,m" /> |
                        <asp:LinkButton runat="server" Text="1h"  onclick="lbtDTStart_Click" CommandArgument="1,h" /> |
                        <asp:LinkButton runat="server" Text="3h"  onclick="lbtDTStart_Click" CommandArgument="3,h" /> |
                        <asp:LinkButton runat="server" Text="6h"  onclick="lbtDTStart_Click" CommandArgument="6,h" /> | <b>)</b>
                    </div>
                </td><td>Start/end time in Pacific Time.</td></tr>
            <tr><th>Alarm List</th>
                <td>
                    <asp:TextBox runat="server" ID="txtParamLogList" />

                    <asp:DropDownList
                        id="SiteDropDownList"
                        AutoPostBack="true"
                        runat="server"
                        OnSelectedIndexChanged="ddlParamLogList_SelectedIndexChanged" 
                        >


                    </asp:DropDownList>

                    <!-- Old -->
                    <%--<asp:DropDownList runat="server" ID="ddlParamLogList" OnSelectedIndexChanged="ddlParamLogList_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Text="(All)" Value="" Selected="True" />
                        <asp:ListItem Text="ALMBART1" />    
                        <asp:ListItem Text="ALMBART2" />    
                        <asp:ListItem Text="ALMBARTC" />    
                        <asp:ListItem Text="ALMBIGH2" />    
                        <asp:ListItem Text="ALMBIGHO" />    
                        <asp:ListItem Text="ALMBUFA2" />    
                        <asp:ListItem Text="ALMBUFAL" />    
                        <asp:ListItem Text="ALMCASSE" />    
                        <asp:ListItem Text="ALMCOGRE" />    
                        <asp:ListItem Text="ALMCRIDG" />    
                        <asp:ListItem Text="ALMDILON" />    
                        <asp:ListItem Text="ALMDRYL2" />    
                        <asp:ListItem Text="ALMDRYLA" />    
                        <asp:ListItem Text="ALMELKRI" />    
                        <asp:ListItem Text="ALMELMC2" />    
                        <asp:ListItem Text="ALMELMCR" />    
                        <asp:ListItem Text="ALMFARME" />    
                        <asp:ListItem Text="ALMFE01" />     
                        <asp:ListItem Text="ALMFE02" />     
                        <asp:ListItem Text="ALMFE03" />     
                        <asp:ListItem Text="ALMFE04" />     
                        <asp:ListItem Text="ALMFE05" />     
                        <asp:ListItem Text="ALMFE06" />     
                        <asp:ListItem Text="ALMFE07" />     
                        <asp:ListItem Text="ALMFE08" />     
                        <asp:ListItem Text="ALMFLYCO" />    
                        <asp:ListItem Text="ALMHAYCA" />    
                        <asp:ListItem Text="ALMJUNCA" />    
                        <asp:ListItem Text="ALMKLON1" />    
                        <asp:ListItem Text="ALMKLON2" />    
                        <asp:ListItem Text="ALMKLONA" />    
                        <asp:ListItem Text="ALMKLONG" />    
                        <asp:ListItem Text="ALMKLONS" />    
                        <asp:ListItem Text="ALMLEJU2" />    
                        <asp:ListItem Text="ALMLEJUN" />    
                        <asp:ListItem Text="ALMLEMPS" />    
                        <asp:ListItem Text="ALMLRID1" />    
                        <asp:ListItem Text="ALMLRID2" />    
                        <asp:ListItem Text="ALMMINND" />    
                        <asp:ListItem Text="ALMMORA1" />    
                        <asp:ListItem Text="ALMMORA2" />    
                        <asp:ListItem Text="ALMMRID1" />    
                        <asp:ListItem Text="ALMMRID2" />    
                        <asp:ListItem Text="ALMMRIDG" />    
                        <asp:ListItem Text="ALMPENE1" />    
                        <asp:ListItem Text="ALMPENE2" />    
                        <asp:ListItem Text="ALMPENE3" />    
                        <asp:ListItem Text="ALMPESPR" />    
                        <asp:ListItem Text="ALMPROVH" />    
                        <asp:ListItem Text="ALMRUGBY" />    
                        <asp:ListItem Text="ALMSCRAB" />    
                        <asp:ListItem Text="ALMSHILO" />    
                        <asp:ListItem Text="ALMSTPOI" />    
                        <asp:ListItem Text="ALMTOPIO" />    
                        <asp:ListItem Text="ALMTRIMO" />    
                        <asp:ListItem Text="ALMTWINB" />    
                        <asp:ListItem Text="ALMWINNE" />    
                        <asp:ListItem Text="BARTC" />       
                        <asp:ListItem Text="PENE1" />       
                        <asp:ListItem Text="PENE2" />       
                        <asp:ListItem Text="PENE3" /> 
                    </asp:DropDownList>--%>
                </td>
                <td>List of alarms. (e.g., plant)</td></tr>
            <tr><th>Tag</th>
                <td>
                    <asp:TextBox runat="server" ID="txtParamName" Width="250"    />
                    <br />
                    <asp:RadioButtonList ID="coreSearchOption" runat="server">
                        <asp:ListItem Text="Search by Tag Name" Value="name" Selected="True"/>
                        <asp:ListItem Text="Search by Tag Desc" Value="desc" />
                    </asp:RadioButtonList>

                </td>
                <td>
                    Uses SQL-style syntax: % for multi-character wildcard, _ for single-character wildcard.<br />
                    This parameter is very processor-intensive, so use it sparingly. <br />
                    <b>Search by Tag Desc MUST use wildcard %</b>
                    <br />
                    Ex: <b>'BIGHO.%'</b> Search for all CORE tags that start w/ 'BIGHO' such as BIGHO.A099.Gen.IntVenGenAct <br />
                    Ex: <b>'%.ST.%'</b> Search for <u>all substation tags in the fleet</u>. <br />
                    Ex: <b>'%HUB%'</b> If used with <u>Search by Tag Desc</u>, this will find out all tag(s) with 'HUB' in the description<br />

                </td></tr>
          
        </tbody>
        <tfoot>
            <tr>
                <td colspan="3">
                    <asp:LinkButton ID="lnkParametersApply" runat="server" onclick="lnkParametersApply_Click">Search</asp:LinkButton>
                    |
                    <asp:LinkButton ID="lnkParametersCSV" runat="server" onclick="lnkParametersCSV_Click">Export to Excel</asp:LinkButton>
                    |
                    <asp:LinkButton ID="lnkParametersReset" runat="server" onclick="lnkParametersReset_Click">Reset Parameters</asp:LinkButton>
                </td>
            </tr>
        </tfoot>
    </table>
    
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
        ErrorMessage="Invalid Start Date."
        EnableClientScript="true"
        ControlToValidate="txtParamDTStart"
        ValidationExpression="^[0-9]{4}-(((0[13578]|(10|12))-(0[1-9]|[1-2][0-9]|3[0-1]))|(02-(0[1-9]|[1-2][0-9]))|((0[469]|11)-(0[1-9]|[1-2][0-9]|30))) (([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$"
        Font-Bold="true"
        Display="Dynamic"
        ForeColor="Red"
        SetFocusOnError="true"
    ></asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
        ErrorMessage="Invalid End Date."
        EnableClientScript="true"
        ControlToValidate="txtParamDTEnd"
        ValidationExpression="^[0-9]{4}-(((0[13578]|(10|12))-(0[1-9]|[1-2][0-9]|3[0-1]))|(02-(0[1-9]|[1-2][0-9]))|((0[469]|11)-(0[1-9]|[1-2][0-9]|30))) (([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$"
        Font-Bold="true"
        Display="Dynamic"
        ForeColor="Red"
        SetFocusOnError="true"
    ></asp:RegularExpressionValidator>

    <asp:Label runat="server" ID="lblAlarms" Font-Bold="true" />
    <asp:ListView runat="server" ID="ListView_Alarms" >
        <LayoutTemplate>
            <table class="tblResults">
                <tr>
                    <th>Time</th>
                    <th >Turbine Name</th>
                    <th>Tag Name</th>
                    <th>Tag Description</th>
                    <th class="hide">Date</th>
                    <th class="hide">Unit Name</th>
                    <th class="hide">Var Type</th>
                    <th class="hide">Priority</th>
                    <th >Value</th>
                    <th class="hide">T Val</th>
                    <th class="hide">Evt Type</th>
                    <th >Event </th>
                    <th class="hide">Evt Text</th>
                    <th class="hide">Comp Inf</th>
                    <th>User Name</th>
                    <th class="hide">User Note</th>
                    <th class="hide">TS Type</th>
                    <td class="hide">TS Val</th>
                    <th class="hide">BATT</th>
                    <th >Domain</th>
                    <th >Nature</th>
                    <th class="hide">CDATT 8</th>
                    <th >Station</th>

                </tr>
                <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td class="nowrap"><%# Eval("Time")%></td>
<%--                <td><%# Eval("PROJECT") %></td>
                <td><%# Eval("LOGLIST") %></td>--%>
                <td class="nowrap"><%# Eval("Turbine Name") %></td>
                <td class="nowrap"><%# Eval("Tag Name") %></td>
                <td class="nowrap"><%# Eval("Tag Description") %></td>
<%--                <td><%# Eval("UNITNAME") %></td>
                <td><%# Eval("VARTYPE") %></td>
                <td><%# Eval("PRIORITY") == null ? "NULL" : (Eval("PRIORITY").GetType().Name == "DBNull" ? "null" : Eval("PRIORITY")) %></td>--%>
                <td><%# Eval("Value") %></td>
               <%-- <td><%# Eval("TVAL") %></td>
                <td><%# Eval("EVTTYPE") %></td>
                   --%>
                <td class="nowrap"><%# Eval("Event") %></td>
                <%-- %><td><%# Eval("EVTTXT") %></td>--%>
                <%--<td><%# Eval("COMPINF") %></td>--%>
                <td><%# Eval("Username") %></td>
                <%--<td><%# Eval("USERNOTE") %></td>
                <td><%# Eval("TSTYPE") %></td>
                <td><%# Eval("BATT") %></td> --%>
                <td><%# Eval("Domain") %></td>
                <td><%# Eval("Nature") %></td>
                <%-- %><td><%# Eval("CDATT8") %></td>--%>
                <td><%# Eval("Station") %></td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>NoData</EmptyDataTemplate>
        <EmptyItemTemplate>NoItem</EmptyItemTemplate>
    </asp:ListView>
    
    <asp:SqlDataSource runat="server" ID="SqlDataSource_Alarms"
        ConnectionString="<%$ ConnectionStrings:ConnectionString_ORAPDX %>" 
        SelectCommand="
            SELECT *
            FROM ALARMS
            WHERE
                [CHRONO] &gt; 1295400535000
            --DECLARE @epochStart bigint;
            --SET @epochStart = DATEDIFF(ss,'19700101',DateAdd(hour,8,DateAdd(minute,-1,GetDate()))) * CONVERT(bigint,1000);
            --PRINT @epochStart;
            --SELECT * FROM ORAPDX..HIS.ALARMS WHERE [CHRONO] > @epochStart;
        "
    >

    </asp:SqlDataSource>

        <owd:Window ID="Window_FilterEditor" runat="server"
            StyleFolder="/obout/Window/wdstyles/blue"
            IsModal="True"
            VisibleOnLoad="false"
            Overflow="HIDDEN"
            MinWidth="802"
            MinHeight="570"
            OnClientOpen="this.screenCenter();"
            Title="Filter Editor"
        >
            <div class="sidenote" style="color:Black; padding-left: 10px;">
                <b>Instructions:</b>
                <ul style="padding:0 0 0 5; margin:0;">
                <li>FILTER NAME is the filter group that appears in the drop-down-list in the Alarms page parameters. Keep it short.</li>
                <li>All other fields are 'search' fields:
                    <ul style="padding:0 0 0 5;">
                        <li>If a filter in the selected Filter Group (and 'Exclude Matching' is chosen) matches an alarm, that alarm will not be displayed.</li>
                        <li>Special characters include % and * (any sequence of characters), _ (any single character), and the word 'null' (no data)</li>
                        <li>e.g., a filter %.TurbinaOk will match only tags ending in .TurbinaOk, e.g. MINDD.A064.TurbinaOk.</li>
                        <li>However, if you specify MINDD.A064, it will not match MINDD.A064.TurbinaOk because there is no % or * character indicating 'any characters after MINDD.A064'.</li>
                    </ul>
                </li>
                </ul>
            </div>

            <obout:Grid ID="Grid_FilterEditor" runat="server"
                DataSourceID="SqlDataSource_FilterEditor"
                AutoGenerateColumns="false"
                AllowColumnResizing="true"
                AllowPaging="false"
                AllowFiltering="true"
                AllowAddingRecords="true"
                PageSize="0"
                Height="400"
                FolderStyle="/obout/Grid/styles/style_5"
            >
                <LocalizationSettings UpdateLink="Save" />
                <ScrollingSettings ScrollWidth="800" />
                <Columns>
                    <obout:Column AllowEdit="true" AllowDelete="true" HeaderText="Edit" Width="100" runat="server" />
                    <obout:Column DataField="Filter_Id"             HeaderText="Id"             Visible="false" />
                    <obout:Column DataField="Filter_Exclude"        HeaderText=""               Visible="false" />
                    <obout:Column DataField="Filter_Creator"        HeaderText="Creator"        Visible="false" ReadOnly="true" InsertVisible="false" />
                    <obout:Column DataField="Filter_Created"        HeaderText="Created"        Visible="false" ReadOnly="true" InsertVisible="false" />
                    <obout:Column DataField="Filter_Group"          HeaderText="FILTER NAME"    ControlStyle-Font-Bold="true" ItemStyle-Font-Bold="true" />
                    <obout:Column DataField="Filter_Field_Project"  HeaderText="Project"     />
                    <obout:Column DataField="Filter_Field_UnitName" HeaderText="Unit Name"   />
                    <obout:Column DataField="Filter_Field_VarType"  HeaderText="Var Type"    />
                    <obout:Column DataField="Filter_Field_Name"     HeaderText="Tag Name"    />
                    <obout:Column DataField="Filter_Field_Priority" HeaderText="Priority"    />
                    <obout:Column DataField="Filter_Field_NVal"     HeaderText="N Val"       />
                    <obout:Column DataField="Filter_Field_LogList"  HeaderText="T Val"       />
                    <obout:Column DataField="Filter_Field_EvtType"  HeaderText="Evt Type"    />
                    <obout:Column DataField="Filter_Field_TSType"   HeaderText="TS Type"     />
                    <obout:Column DataField="Filter_Field_SATT1"    HeaderText="SATT 1"      />
                    <obout:Column DataField="Filter_Field_SATT2"    HeaderText="SATT 2"      />
                    <obout:Column DataField="Filter_Field_SATT3"    HeaderText="SATT 3"      />
                    <obout:Column DataField="Filter_Field_Station"  HeaderText="Station"     />
                </Columns>
            </obout:Grid>

            <asp:SqlDataSource runat="server" ID="SqlDataSource_FilterEditor"
                ConnectionString="<%$ ConnectionStrings:ConnectionString_PDXSQL03_EMSWEB %>"
                ProviderName="System.Data.SqlClient"
                SelectCommand="SELECT * FROM CORE.dbo.Alarm_Filters ORDER BY Filter_Group;"
                UpdateCommand="UPDATE CORE.dbo.Alarm_Filters
                                SET
                                      Filter_Group          = @Filter_Group
                                    , Filter_Exclude        = @Filter_Exclude
                                    , Filter_Field_Project  = @Filter_Field_Project
                                    , Filter_Field_UnitName = @Filter_Field_UnitName
                                    , Filter_Field_VarType  = @Filter_Field_VarType
                                    , Filter_Field_Name     = @Filter_Field_Name   
                                    , Filter_Field_Priority = @Filter_Field_Priority
                                    , Filter_Field_NVal     = @Filter_Field_NVal   
                                    , Filter_Field_LogList  = @Filter_Field_LogList
                                    , Filter_Field_EvtType  = @Filter_Field_EvtType
                                    , Filter_Field_TSType   = @Filter_Field_TSType 
                                    , Filter_Field_SATT1    = @Filter_Field_SATT1  
                                    , Filter_Field_SATT2    = @Filter_Field_SATT2  
                                    , Filter_Field_SATT3    = @Filter_Field_SATT3  
                                    , Filter_Field_Station  = @Filter_Field_Station
                                WHERE Filter_Id = @Filter_Id
                    "
                    DeleteCommand="DELETE FROM CORE.dbo.Alarm_Filters WHERE Filter_Id = @Filter_Id;"
                    InsertCommand="INSERT CORE.dbo.Alarm_Filters
                                    (
                                          Filter_Group         
                                        , Filter_Exclude       
                                        , Filter_Field_Project 
                                        , Filter_Field_UnitName
                                        , Filter_Field_VarType 
                                        , Filter_Field_Name    
                                        , Filter_Field_Priority
                                        , Filter_Field_NVal    
                                        , Filter_Field_LogList 
                                        , Filter_Field_EvtType 
                                        , Filter_Field_TSType  
                                        , Filter_Field_SATT1   
                                        , Filter_Field_SATT2   
                                        , Filter_Field_SATT3   
                                        , Filter_Field_Station 
                                    )
                                    VALUES
                                    (
                                          @Filter_Group
                                        , @Filter_Exclude
                                        , @Filter_Field_Project
                                        , @Filter_Field_UnitName
                                        , @Filter_Field_VarType
                                        , @Filter_Field_Name   
                                        , @Filter_Field_Priority
                                        , @Filter_Field_NVal   
                                        , @Filter_Field_LogList
                                        , @Filter_Field_EvtType
                                        , @Filter_Field_TSType 
                                        , @Filter_Field_SATT1  
                                        , @Filter_Field_SATT2  
                                        , @Filter_Field_SATT3  
                                        , @Filter_Field_Station
                                    )
                    "
            >
            </asp:SqlDataSource>
        </owd:Window>

    <asp:Label runat="server" ID="lblDebug" ForeColor="White" />
</asp:Content>
