<%@ Control Language="vb" Inherits="DNNStuff.Aggregator.EditTab" CodeBehind="EditTab.ascx.vb" AutoEventWireup="false" Explicit="True" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="CustomPropertiesViewer" Src="CustomPropertiesViewer.ascx" %>
<div class="dnnForm dnnClear">
    <div id="edittabtabs" class="tabslayout">
        <ul id="edittabtabs-nav" class="tabslayout">
            <li><a href="#edittab"><span>
                <%=Localization.GetString("TabCaption_EditTab", LocalResourceFile)%></span></a></li>
            <li><a href="#editadvanced"><span>
                <%=Localization.GetString("TabCaption_EditAdvanced", LocalResourceFile)%></span></a></li>
            <li><a href="#edittabsettings"><span>
                <%=Localization.GetString("TabCaption_EditTabSettings", LocalResourceFile)%></span></a></li>
            <li><a href="#help"><span>
                <%=Localization.GetString("TabCaption_Help", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="edittab">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblCaption" runat="server" ControlName="txtCaption" Suffix=":" Text="Caption" />
                    <asp:TextBox ID="txtCaption" runat="server" CssClass="NormalTextBox" TextMode="SingleLine" Columns="100"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblContent" runat="server" ControlName="txtContent" Suffix=":" Text="Content" />
                    <dnn:TextEditor ID="txtContent" runat="server" Width="100%" TextRenderMode="Text" HtmlEncode="False" DefaultMode="Rich" Height="300" ChooseMode="True" ChooseRender="True"></dnn:TextEditor>
                </div>
            </div>
            <div class="tab" id="editadvanced">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblLocale" runat="server" ControlName="cboLocale" Suffix=":" Text="Locale" />
                    <asp:DropDownList ID="cboLocale" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblPostback" runat="server" ControlName="chkPostback" Suffix=":" Text="Postback" />
                    <asp:CheckBox ID="chkPostback" runat="server" />
                </div>
            </div>
            <div class="tab" id="edittabsettings">
                <div class="dnnFormItem">
                    <uc:CustomPropertiesViewer ID="cpvMain" runat="server" />
                </div>
            </div>
            <div class="tab" id="help">
                <div>
                    <%=Localization.GetString("DocumentationHelp.Text", LocalResourceFile)%></div>
                <div id="divHelp" runat="server" />
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
        id: 'edittabtabs',
        persist: true
    });
</script>
