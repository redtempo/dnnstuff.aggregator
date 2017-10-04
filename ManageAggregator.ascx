<%@ Control Language="vb" CodeBehind="ManageAggregator.ascx.vb" AutoEventWireup="false"
    Explicit="True" Inherits="DNNStuff.Aggregator.ManageAggregator" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="CustomPropertiesViewer" Src="CustomPropertiesViewer.ascx" %>
<div class="dnnForm dnnClear">
    <div id="editsettings" class="tabslayout">
        <ul id="editsettings-nav" class="tabslayout">
            <li><a href="#edittabs"><span>
                <%=Localization.GetString("TabCaption_EditTabs", LocalResourceFile)%></span></a></li>
            <li><a href="#editstyle"><span>
                <%=Localization.GetString("TabCaption_EditStyle", LocalResourceFile)%></span></a></li>
            <li><a href="#editskinsettings"><span>
                <%=Localization.GetString("TabCaption_EditSkinSettings", LocalResourceFile)%></span></a></li>
            <li><a href="#edittargets"><span>
                <%=Localization.GetString("TabCaption_EditTargets", LocalResourceFile)%></span></a></li>
            <li><a href="#editrss"><span>
                <%=Localization.GetString("TabCaption_EditRSS", LocalResourceFile)%></span></a></li>
            <li><a href="#edittools"><span>
                <%=Localization.GetString("TabCaption_EditTools", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="edittabs">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTabs" CssClass="SubHead" runat="server" ControlName="cmdAddTab"
                        Suffix=":" />
                    <asp:LinkButton ID="cmdAddModule" Text="Add New Module" ResourceKey="cmdAddModule"
                        CausesValidation="False" runat="server" CssClass="CommandButton" BorderStyle="none" />&nbsp;|&nbsp;
                    <asp:LinkButton ID="cmdAddTab" Text="Add New Tab" ResourceKey="cmdAddTab" CausesValidation="False"
                        runat="server" CssClass="CommandButton" BorderStyle="none" />&nbsp;|&nbsp;
                    <asp:LinkButton ID="cmdAddAllModules" Text="Add All Page Modules" ResourceKey="cmdAddAllModules"
                        CausesValidation="False" runat="server" CssClass="CommandButton" BorderStyle="none" />
                </div>
                <div class="dnnFormItem">
                    <asp:DataList ID="dlTabs" runat="server" CssClass="dnnMaxLabelWidth">
                        <HeaderTemplate>
                            <table width="100%" cellpadding="0" cellspacing="0" border="1" rules="none" frame="box">
                                <tr style="background: silver">
                                    <td style="width: 100px">
                                        &nbsp;
                                    </td>
                                    <td style="width: 700px">
                                        <strong>
                                            <%=Localization.GetString("dlTabs.Header.Caption", LocalResourceFile)%></strong>
                                    </td>
                                    <td style="width: 16px">
                                        &nbsp;
                                    </td>
                                    <td style="width: 16px">
                                        &nbsp;
                                    </td>
                                    <td style="width: 100px; text-align: center">
                                        <strong>
                                            <%=Localization.GetString("dlTabs.Header.Link", LocalResourceFile)%></strong>
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr style="background: #fff">
                                <td valign="top">
                                    <asp:ImageButton ID="cmdEditTab" runat="server" CausesValidation="false" CommandName="edit"
                                        ImageUrl="~/images/edit.gif" ImageAlign="AbsMiddle" AlternateText="" ResourceKey="cmdEditTab.AlternateText">
                                    </asp:ImageButton>
                                    <asp:ImageButton ID="cmdDeleteTab" runat="server" CausesValidation="false" CommandName="delete"
                                        ImageUrl="~/images/delete.gif" ImageAlign="AbsMiddle" AlternateText="" ResourceKey="cmdDeleteTab.AlternateText">
                                    </asp:ImageButton>
                                </td>
                                <td valign="top">
                                    <asp:Label runat="server" Text='<%# Server.HTMLEncode(DataBinder.Eval(Container.DataItem,"Caption")) %>'
                                        ID="lblCaption" />
                                    <asp:Label runat="server" Font-Size="Smaller" Text='<%# MaxTrim(Server.HTMLEncode(DataBinder.Eval(Container.DataItem,"HtmlText")),"<br />&nbsp;&nbsp;&nbsp;&nbsp;{0}...",80) %>'
                                        ID="lblHtmlText" />
                                </td>
                                <td valign="top">
                                    <asp:ImageButton ID="cmdMoveTabUp" runat="server" CausesValidation="false" CommandName="moveup"
                                        CommandArgument="up" ImageUrl="~/images/up.gif" ResourceKey="cmdMoveTabUp.AlternateText"
                                        AlternateText=""></asp:ImageButton>
                                </td>
                                <td valign="top">
                                    <asp:ImageButton ID="cmdMoveTabDown" runat="server" CausesValidation="false" CommandName="movedown"
                                        CommandArgument="down" ImageUrl="~/images/dn.gif" ResourceKey="cmdMoveTabDown.AlternateText"
                                        AlternateText=""></asp:ImageButton>
                                </td>
                                <td style="text-align: center" valign="top">
                                    <asp:HyperLink ID="cmdUrl" runat="server" Text="Link" Title='This link can be used to select this tab from another page'
                                        NavigateUrl='<%# LinkPath(Container.ItemIndex) %>'></asp:HyperLink>
                                </td>
                            </tr>
                            <tr runat="server" id="trTabModules">
                                <td>
                                    &nbsp;
                                </td>
                                <td colspan="4" valign="top">
                                    <asp:DataList ID="dlTabModules" runat="server" OnItemCommand="dlTabModules_ItemCommand"
                                        OnItemCreated="dlTabModules_ItemCreated">
                                        <HeaderTemplate>
                                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td style="width: 100px">
                                                    <asp:ImageButton ID="cmdEditTabModule" runat="server" CausesValidation="false" CommandName="edit"
                                                        ImageUrl="~/images/edit.gif" ImageAlign="AbsMiddle" AlternateText="" ResourceKey="cmdEditTabModule.AlternateText">
                                                    </asp:ImageButton>
                                                    <asp:ImageButton ID="cmdDeleteTabModule" runat="server" CausesValidation="false"
                                                        CommandName="delete" ImageUrl="~/images/delete.gif" ImageAlign="AbsMiddle" AlternateText=""
                                                        ResourceKey="cmdDeleteTabModule.AlternateText"></asp:ImageButton>
                                                </td>
                                                <td style="width: 500px">
                                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ModuleTitle") %>'
                                                        ID="Label6" />
                                                </td>
                                                <td align="right" style="width: 40px">
                                                    <b title="Use this token to insert the module into the html/text of the tab">[MOD<asp:Label
                                                        runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ModuleId") %>' ID="Label1" />]</b>
                                                </td>
                                                <td style="width: 20px">
                                                    <asp:ImageButton ID="cmdEditTabModuleSettings" runat="server" Visible="false" CausesValidation="false"
                                                        CommandName="editsettings" ImageUrl="~/images/action_settings.gif" ImageAlign="AbsMiddle"
                                                        AlternateText="" ResourceKey="cmdEditTabModuleSettings.AlternateText"></asp:ImageButton>
                                                </td>
                                                <td valign="top">
                                                    <asp:ImageButton ID="cmdMoveModuleUp" runat="server" CausesValidation="false" CommandName="moveup"
                                                        CommandArgument="up" ImageUrl="~/images/up.gif" ResourceKey="cmdMoveModuleUp.AlternateText"
                                                        AlternateText=""></asp:ImageButton>
                                                </td>
                                                <td valign="top">
                                                    <asp:ImageButton ID="cmdMoveModuleDown" runat="server" CausesValidation="false" CommandName="movedown"
                                                        CommandArgument="down" ImageUrl="~/images/dn.gif" ResourceKey="cmdMoveModuleDown.AlternateText"
                                                        AlternateText=""></asp:ImageButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:DataList>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:DataList>
                </div>
            </div>
            <div class="tab" id="editstyle">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblCssTheme" CssClass="SubHead" runat="server" ControlName="cboTabSkin"
                        Suffix=":" />
                    <asp:DropDownList ID="cboTabSkin" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTabTemplate" CssClass="SubHead" runat="server" ControlName="cboTabTemplate"
                        Suffix=":" />
                    <asp:DropDownList ID="cboTabTemplate" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblWidth" runat="server" CssClass="SubHead" ControlName="txtWidth"
                        Suffix=":" />
                    <asp:TextBox ID="txtWidth" runat="server" Columns="5"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblHeight" runat="server" CssClass="SubHead" ControlName="txtHeight"
                        Suffix=":" />
                    <asp:TextBox ID="txtHeight" runat="server" Columns="5"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblAutoHide" runat="server" CssClass="SubHead" ControlName="chkAutoHide"
                        Suffix=":" />
                    <asp:CheckBox ID="chkHideTitles" Text="" runat="server" CssClass="Normal" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblShowPrevNext" runat="server" CssClass="SubHead" ControlName="chkShowPrevNext"
                        Suffix=":" />
                    <asp:CheckBox ID="chkShowPrevNext" Text="" runat="server" CssClass="Normal" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblHideSingleTab" runat="server" CssClass="SubHead" ControlName="chkHideSingleTab"
                        Suffix=":" />
                    <asp:CheckBox ID="chkHideSingleTab" Text="" runat="server" CssClass="Normal" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblActiveHover" runat="server" CssClass="SubHead" ControlName="chkActiveHover"
                        Suffix=":" />
                    <asp:CheckBox ID="chkActiveHover" Text="" runat="server" CssClass="Normal" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblActiveHoverDelay" runat="server" CssClass="SubHead" ControlName="txtActiveHoverDelay"
                        Suffix=":" />
                    <asp:TextBox ID="txtActiveHoverDelay" runat="server" Columns="5"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblHideTabs" runat="server" CssClass="SubHead" ControlName="chkHideTabs"
                        Suffix=":" />
                    <asp:CheckBox ID="chkHideTabs" Text="" runat="server" CssClass="Normal" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDefaultTab" runat="server" CssClass="SubHead" ControlName="txtDefaultTab"
                        Suffix=":" />
                    <asp:TextBox ID="txtDefaultTab" runat="server" Columns="5"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRememberLastOpenTab" runat="server" CssClass="SubHead" ControlName="chkRememberLastOpenTab"
                        Suffix=":" />
                    <asp:CheckBox ID="chkRememberLastOpenTab" Text="" runat="server" CssClass="Normal" />
                </div>
            </div>
            <div class="tab" id="editskinsettings">
                <div class="dnnFormItem">
                    <%=Localization.GetString("TabHelp_EditSkinSettings", LocalResourceFile)%>
                </div>
                <div id="skinsettingstabs" class="tabslayout">
                    <ul id="skinsettingstabs-nav" class="tabslayout">
                        <li><a href="#skinsettingstabs-edit"><span>
                            <%=Localization.GetString("TabCaption_SkinSettings_Edit", LocalResourceFile)%></span></a></li>
                        <li><a href="#skinsettingstabs-help"><span>
                            <%=Localization.GetString("TabCaption_SkinSettings_Help", LocalResourceFile)%></span></a></li>
                    </ul>
                    <div class="tabs-container">
                        <div class="tab-nested" id="skinsettingstabs-edit">
                            <div class="dnnFormItem">
                                <uc:CustomPropertiesViewer ID="cpvMain" runat="server" />
                            </div>
                            <div class="tab-nested" id="skinsettingstabs-help">
                                <div class="dnnFormItem">
                                    <asp:Label ID="lblCustomPropertyHelp" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab" id="edittargets">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTargets" CssClass="SubHead" runat="server" ControlName="cmdAddTarget"
                        Suffix=":" />
                    <asp:LinkButton ID="cmdAddTarget" Text="Add Target" ResourceKey="cmdAddTarget" CausesValidation="False"
                        runat="server" class="CommandButton" BorderStyle="none" Enabled="True" />
                    <br />
                    <asp:DataGrid ID="grdTargets" runat="server" CssClass="Normal" OnCancelCommand="grdTargets_CancelEdit"
                        OnUpdateCommand="grdTargets_Update" OnEditCommand="grdTargets_Edit" OnDeleteCommand="grdTargets_Delete"
                        OnItemDataBound="grdTargets_Item_Bound" AutoGenerateColumns="False" Border="0"
                        GridLines="Horizontal" DataKeyField="AggregatorTargetId">
                        <Columns>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:ImageButton ID="cmdEditTarget" runat="server" CausesValidation="false" CommandName="Edit"
                                        ImageUrl="~/images/edit.gif" AlternateText="" ResourceKey="cmdEditTarget.AlternateText">
                                    </asp:ImageButton>&nbsp;
                                    <asp:ImageButton ID="cmdDeleteTarget" runat="server" CausesValidation="false" CommandName="Delete"
                                        ImageUrl="~/images/delete.gif" AlternateText="" ResourceKey="cmdDeleteTarget.AlternateText">
                                    </asp:ImageButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="cmdSaveTarget" runat="server" CausesValidation="false" CommandName="Update"
                                        ImageUrl="~/images/save.gif" AlternateText="" ResourceKey="cmdSaveTarget.AlternateText">
                                    </asp:ImageButton>&nbsp;
                                    <asp:ImageButton ID="cmdCancelTarget" runat="server" CausesValidation="false" CommandName="Cancel"
                                        ImageUrl="~/images/cancel.gif" AlternateText="" ResourceKey="cmdCancelTarget.AlternateText">
                                    </asp:ImageButton>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Target">
                                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                                <ItemStyle CssClass="Normal" Width="150px"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TargetTitle") %>'
                                        ID="Label2" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="cboTarget" runat="server" CssClass="NormalTextBox">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </div>
            <div class="tab" id="editrss">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSUrl" CssClass="SubHead" runat="server" ControlName="txtRSSUrl"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSUrl" runat="server" Columns="80" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSMaxItems" CssClass="SubHead" runat="server" ControlName="txtRSSMaxItems"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSMaxItems" runat="server" CssClass="NormalTextBox" Columns="4" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSCacheTime" CssClass="SubHead" runat="server" ControlName="txtRSSCacheTime"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSCacheTime" runat="server" CssClass="NormalTextBox" Columns="4" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSTabCaption" CssClass="SubHead" runat="server" ControlName="txtRSSTabCaption"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSTabCaption" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSUsername" CssClass="SubHead" runat="server" ControlName="txtRSSUsername"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSUsername" runat="server" autocomplete="off" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblRSSPassword" CssClass="SubHead" runat="server" ControlName="txtRSSPassword"
                        Suffix=":" />
                    <asp:TextBox ID="txtRSSPassword" runat="server" TextMode="password" autocomplete="off" />
                </div>
            </div>
            <div class="tab" id="edittools">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblQuickTabs" runat="server" ControlName="txtQuickTabs" Suffix=":" />
					<asp:LinkButton ID="cmdQuickTabsAdd" runat="server" Text="Add Tabs" ResourceKey="cmdQuickTabsAdd" /><br />
                    <asp:TextBox ID="txtQuickTabs" runat="server" TextMode="MultiLine" Rows="8" Columns="80" />
				</div>
            </div>
        </div>
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" Text="Update" resourcekey="cmdUpdate" CausesValidation="True"
                runat="server" CssClass="dnnPrimaryAction" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" Text="Cancel" resourcekey="cmdCancel" CausesValidation="False"
                runat="server" CssClass="dnnSecondaryAction" /></li>
    </ul>
</div>
<script type="text/javascript">
    var tabber1 = new Yetii({
        id: 'editsettings',
        persist: true
    });

    var tabber2 = new Yetii({
        id: 'skinsettingstabs',
        persist: false,
        tabclass: 'tab-nested'
    });
    
</script>
