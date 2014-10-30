<%@ Control Language="vb" CodeBehind="CopyAggregator.ascx.vb" AutoEventWireup="false"
    Explicit="True" Inherits="DNNStuff.Aggregator.CopyAggregator" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="CustomPropertiesViewer" Src="CustomPropertiesViewer.ascx" %>
<div>
    <%=Localization.GetString("DocumentationHelp.Text", LocalResourceFile)%></div>
<div class="dnnForm dnnClear">
    <table id="tblCopyAggregator" cellspacing="0" cellpadding="2" width="525" border="0" runat="server">
        <tr>
            <td width="25">
            </td>
            <td class="SubHead" width="150">
                <dnn:Label ID="plCopyPage" runat="server" ResourceKey="CopyModules" Suffix=":" HelpKey="CopyModulesHelp"
                    ControlName="cboCopyPage"></dnn:Label>
            </td>
            <td width="325">
                <asp:DropDownList ID="cboCopyPage" CssClass="NormalTextBox" runat="server" Width="300"
                    DataTextField="TabName" DataValueField="TabId" AutoPostBack="False">
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="rowModules" runat="server">
            <td width="25">
            </td>
            <td class="SubHead" colspan="2">
                <dnn:Label ID="plModules" runat="server" ResourceKey="CopyContent" Suffix=":" HelpKey="CopyContentHelp"
                    ControlName="grdModules"></dnn:Label>
                <br>
                <asp:DataGrid ID="grdModules" runat="server" DataKeyField="ModuleID" ShowHeader="false"
                    ItemStyle-CssClass="Normal" GridLines="None" BorderWidth="0px" BorderStyle="None"
                    AutoGenerateColumns="False" CellSpacing="0" CellPadding="0" Width="100%">
                    <Columns>
                        <asp:BoundColumn runat="server" DataField="TagData" DataFormatString="Tab {0:D}" ItemStyle-CssClass="NormalBold" />
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkModule" runat="server" CssClass="NormalTextBox" Checked="True" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:TextBox ID="txtCopyTitle" Width="150" runat="server" CssClass="NormalTextBox"
                                    Text='<%# Databinder.eval(Container.Dataitem,"ModuleTitle")%>'>
                                </asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:RadioButton ID="optCopy" runat="server" CssClass="NormalBold" GroupName="Copy" Checked='<%# DataBinder.Eval(Container.DataItem, "IsPortable") %>'
                                    Text="Copy" resourcekey="ModuleCopy" Enabled='<%# DataBinder.Eval(Container.DataItem, "IsPortable") %>'>
                                </asp:RadioButton>
                                <asp:RadioButton ID="optReference" runat="server" CssClass="NormalBold" GroupName="Copy" Checked='<%# Not DataBinder.Eval(Container.DataItem, "IsPortable") %>'
                                    Text="Reference" resourcekey="ModuleReference" Enabled='<%# DataBinder.Eval(Container.DataItem, "ModuleID") <> -1  %>'>
                                </asp:RadioButton>
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <input type="hidden" id="txtTabModuleId" runat="server" value='<%# Databinder.eval(Container.Dataitem,"TabModuleId")%>' />
                                <input type="hidden" id="txtTabId" runat="server" value='<%# Databinder.eval(Container.Dataitem,"TabId")%>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </td>
        </tr>
    </table>
    <ul class="dnnActions dnnClear">
	<li>
				<asp:LinkButton 
                    id="cmdCopyAggregator" runat="server" Text="Copy Aggregator" ResourceKey="cmdCopyAggregator" CssClass="dnnPrimaryAction" CausesValidation="True" />
					</li>
	<li>
    <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" ResourceKey="cmdCancel"
        CssClass="dnnSecondaryAction" CausesValidation="False" />
		</li>
</ul>
</div>

<asp:Panel ID="phResults" runat="server" HorizontalAlign="Left"></asp:Panel>
