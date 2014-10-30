<%@ Control Language="vb" Inherits="DNNStuff.Aggregator.UploadSkin" CodeBehind="UploadSkin.ascx.vb" AutoEventWireup="false" Explicit="True" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<div class="dnnForm dnnClear">
    <div id="uploadtabs" class="tabslayout">
        <ul id="uploadtabs-nav" class="tabslayout">
            <li><a href="#uploadskin"><span>
                <%=Localization.GetString("TabCaption_UploadSkin", LocalResourceFile)%></span></a></li>
            <li><a href="#help"><span>
                <%=Localization.GetString("TabCaption_Help", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="uploadskin">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblBrowse" runat="server" ControlName="cmdBrowse" Suffix=":" Text="Browse Files" />
                    <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server" />&nbsp;&nbsp;
                    <dnn:CommandButton ID="cmdAdd" runat="server" CssClass="dnnPrimaryAction" Text="Upload New Skin" ResourceKey="cmdAdd" ImageUrl="~/images/save.gif" />
                    <br />
                    <asp:Label ID="lblMessage" runat="server" CssClass="Normal" Width="500px" EnableViewState="False"></asp:Label>
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
            <asp:LinkButton ID="cmdReturn" runat="server" BorderStyle="none" Text="Return" ResourceKey="cmdReturn" CssClass="dnnPrimaryAction" CausesValidation="False" />
        </li>
    </ul>
</div>

<br />
<table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table" runat="server" visible="False">
    <tr>
        <td>
            <asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle">Resource Upload Logs</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:PlaceHolder ID="phPaLogs" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <dnn:CommandButton ID="cmdReturn2" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" ResourceKey="cmdReturn" />
        </td>
    </tr>
</table>
<script type="text/javascript">
    var tabber1 = new Yetii({
        id: 'uploadtabs',
        persist: true
    });
</script>
