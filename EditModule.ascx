<%@ Control Language="vb" Inherits="DNNStuff.Aggregator.EditModule" CodeBehind="EditModule.ascx.vb" AutoEventWireup="false" Explicit="True" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <div id="editmoduletabs" class="tabslayout">
        <ul id="editmoduletabs-nav" class="tabslayout">
            <li><a href="#editmodule"><span>
                <%=Localization.GetString("TabCaption_EditModule", LocalResourceFile)%></span></a></li>
            <li><a href="#help"><span>
                <%=Localization.GetString("TabCaption_Help", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="edittab">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTabModule" runat="server" ControlName="cboTabModule" Suffix=":" Text="Module" />
                    <asp:DropDownList ID="cboTabModule" runat="server" />
                    &nbsp;<asp:CheckBox ID="chkShowAllModules" runat="server" AutoPostBack="true" CssClass="NormalTextBox" />
                    <dnn:Label ID="lblShowAllModules" runat="server" ControlName="chkShowAllModules" Suffix=":" Text="Show modules from all pages" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblInsertBreak" runat="server" ControlName="chkInsertBreak" Suffix=":" Text="Insert Break" />
                    <asp:CheckBox ID="chkInsertBreak" runat="server" CssClass="NormalTextBox" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTab" runat="server" ControlName="cboTab" Suffix=":" Text="Tab" />
                    <asp:DropDownList ID="cboTab" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblLocale" runat="server" ControlName="cboLocale" Suffix=":" Text="Locale" />
                    <asp:DropDownList ID="cboLocale" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblLoadEvent" runat="server" ControlName="cboLoadEvent" Suffix=":" Text="LoadEvent" />
                    <asp:DropDownList ID="cboLoadEvent" runat="server" />
                </div>
            </div>
            <div class="tab" id="help">
                <div>
                    <%=Localization.GetString("DocumentationHelp.Text", LocalResourceFile)%></div>
            </div>
        </div>
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" Text="Update" resourcekey="cmdUpdate" CausesValidation="True" runat="server" CssClass="dnnPrimaryAction" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" Text="Cancel" resourcekey="cmdCancel" CausesValidation="False" runat="server" CssClass="dnnSecondaryAction" /></li>
    </ul>
</div>
<script type="text/javascript">
    var tabber1 = new Yetii({
        id: 'editmoduletabs',
        persist: true
    });
</script>
