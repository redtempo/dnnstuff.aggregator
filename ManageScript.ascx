<%@ Control Language="vb" CodeBehind="ManageScript.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DNNStuff.Aggregator.ManageScript" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <div id="scripttabs" class="tabslayout">
        <ul id="scripttabs-nav" class="tabslayout">
            <li><a href="#editscript1"><span>
                <%=Localization.GetString("TabCaption_EditScript1", LocalResourceFile)%></span></a></li>
            <li><a href="#editscript2"><span>
                <%=Localization.GetString("TabCaption_EditScript2", LocalResourceFile)%></span></a></li>
            <li><a href="#help"><span>
                <%=Localization.GetString("TabCaption_Help", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="editscript">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblLoadScript1" runat="server" ControlName="chkLoadScript1" Suffix=":" Text="Load Script1" />
                    <asp:CheckBox ID="chkLoadScript1" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblUseHostedScript1" runat="server" ControlName="chkUseHostedScript1" Suffix=":" Text="Use Hosted Script1" />
                    <asp:CheckBox ID="chkUseHostedScript1" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblHostedScript1" runat="server" ControlName="txtHostedScript1" Suffix=":" Text="Hosted Script1 Url" />
                    <asp:TextBox ID="txtHostedScript1" runat="server" CssClass="NormalTextBox" TextMode="SingleLine" Columns="80"></asp:TextBox>
                </div>
            </div>
            <div class="tab" id="editscript2">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblLoadScript2" runat="server" ControlName="chkLoadScript2" Suffix=":" Text="Load Script2" />
                    <asp:CheckBox ID="chkLoadScript2" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblUseHostedScript2" runat="server" ControlName="chkUseHostedScript2" Suffix=":" Text="Use Hosted Script2" />
                    <asp:CheckBox ID="chkUseHostedScript2" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblHostedScript2" runat="server" ControlName="txtHostedScript2" Suffix=":" Text="Hosted Script2 Url" />
                    <asp:TextBox ID="txtHostedScript2" runat="server" CssClass="NormalTextBox" TextMode="SingleLine" Columns="80"></asp:TextBox>
                </div>
            </div>
            <div class="tab" id="help">
                <div>
                    <%=Localization.GetString("DocumentationHelp.Text", LocalResourceFile)%></div>
            </div>
        </div>
    </div>
    <div class="disclaimer">
        <%=Localization.GetString("Disclaimer", LocalResourceFile)%>
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
        id: 'scripttabs',
        persist: true
    });
</script>
