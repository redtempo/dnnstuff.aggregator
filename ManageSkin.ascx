<%@ Control Language="vb" CodeBehind="ManageSkin.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DNNStuff.Aggregator.ManageSkin" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <div id="skintabs" class="tabslayout">
        <ul id="skintabs-nav" class="tabslayout">
            <li><a href="#editskin"><span>
                <%=Localization.GetString("TabCaption_EditSkin", LocalResourceFile)%></span></a></li>
            <li><a href="#copyskin"><span>
                <%=Localization.GetString("TabCaption_CopySkin", LocalResourceFile)%></span></a></li>
            <li><a href="#help"><span>
                <%=Localization.GetString("TabCaption_Help", LocalResourceFile)%></span></a></li>
        </ul>
        <div class="tabs-container">
            <div class="tab" id="editskin">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEditTabSkin" CssClass="SubHead" runat="server" ControlName="cboEditTabSkin" Suffix=":" />
                    <asp:DropDownList ID="cboEditTabSkin" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEditTabTemplate" CssClass="SubHead" runat="server" ControlName="cboEditTabTemplate" Suffix=":" />
                    <asp:DropDownList ID="cboEditTabTemplate" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEditTabFile" CssClass="SubHead" runat="server" ControlName="cboEditTabFile" Suffix=":" />
                    <asp:DropDownList ID="cboEditTabFile" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSkinText" CssClass="SubHead" runat="server" ControlName="txtSkinText" Suffix=":" />
                    <asp:TextBox ID="txtSkinText" runat="server" Columns="80" Rows="15" TextMode="MultiLine" />
                </div>
                <div class="dnnFormItem">
                    <asp:LinkButton ID="cmdSaveFile" runat="server" BorderStyle="none" Text="Save File" ResourceKey="cmdSaveFile" CssClass="dnnPrimaryAction" CausesValidation="True" ValidationGroup="EditSkin" />
                    <asp:PlaceHolder ID="phEditSkinResults" runat="server" />
                </div>
                <div class="disclaimer">
                    <%=Localization.GetString("Disclaimer_EditSkin", LocalResourceFile)%></div>
            </div>
            <div class="tab" id="copyskin">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTabSkin" CssClass="SubHead" runat="server" ControlName="cboTabSkin" Suffix=":" />
                    <asp:DropDownList ID="cboTabSkin" runat="server" AutoPostBack="True" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTabTemplate" CssClass="SubHead" runat="server" ControlName="cboTabTemplate" Suffix=":" />
                    <asp:DropDownList ID="cboTabTemplate" runat="server" AutoPostBack="False" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblNewSkinName" CssClass="SubHead" runat="server" ControlName="txtNewSkinName" Suffix=":" />
                    <asp:TextBox ID="txtNewSkinName" runat="server" Columns="40" />
                    <asp:CustomValidator ID="vldNewSkinName" runat="server" ControlToValidate="txtNewSkinName" Display="Dynamic" ErrorMessage="<br />New skin name cannot contain spaces" ValidationGroup="CopySkin" />
                    <asp:RequiredFieldValidator ID="vldNewSkinNameRequired" runat="server" ControlToValidate="txtNewSkinName" Display="Dynamic" ErrorMessage="<br />New skin name is required" ValidationGroup="CopySkin" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblOverwrite" CssClass="SubHead" runat="server" ControlName="chkOverwrite" Suffix=":" />
                    <asp:CheckBox ID="chkOverwrite" runat="server"></asp:CheckBox>
                </div>
                <div class="dnnFormItem">
                    <asp:LinkButton ID="cmdCopySkin" runat="server" BorderStyle="none" Text="Copy Skin" ResourceKey="cmdCopySkin" CssClass="dnnPrimaryAction" CausesValidation="True" ValidationGroup="CopySkin" />
                    <asp:PlaceHolder ID="phCopySkinResults" runat="server" />
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
            <asp:LinkButton ID="cmdCancel" Text="Cancel" resourcekey="cmdCancel" CausesValidation="False" runat="server" CssClass="dnnPrimaryAction" /></li>
    </ul>
</div>
<script type="text/javascript">
    var tabber1 = new Yetii({
        id: 'skintabs',
        persist: true
    });
</script>
