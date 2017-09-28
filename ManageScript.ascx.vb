Option Strict On
Option Explicit On 

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports Dotnetnuke.Services.Localization
Imports System.Collections.Generic

Namespace DNNStuff.Aggregator

    Partial Class ManageScript
        Inherits Entities.Modules.PortalModuleBase

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            MyBase.HelpURL = "http://www.dnnstuff.com/"
        End Sub

#End Region

#Region " Page Level"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

                If Page.IsPostBack = False Then
                    LoadSettings()
                End If

            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                ReturnToPage()
            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                UpdateSettings()
                ReturnToPage()
            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub
        Private Sub ReturnToPage()
            Entities.Modules.ModuleController.SynchronizeModule(ModuleId)

            ' clear the tab cache
            DataCache.ClearModuleCache(TabId)

            ' Redirect back to the portal home page
            Response.Redirect(NavigateURL(), True)

        End Sub
#End Region

#Region " Settings"
        Private Sub UpdateSettings()
            Dim ctrl As New AggregatorController

            Dim jquery As ScriptInfo = ctrl.GetScript(PortalId, "JQUERY")
            ' jquery
            With jquery
                .PortalId = PortalId
                .ScriptName = "JQUERY"
                .DontLoadScript = chkLoadScript1.Checked
                .LoadHosted = chkUseHostedScript1.Checked
                .HostedScriptPath = txtHostedScript1.Text
            End With
            ctrl.UpdateScript(jquery)

            'jqueryui
            Dim jqueryui As ScriptInfo = ctrl.GetScript(PortalId, "JQUERYUI")
            With jqueryui
                .PortalId = PortalId
                .ScriptName = "JQUERYUI"
                .DontLoadScript = chkLoadScript2.Checked
                .LoadHosted = chkUseHostedScript2.Checked
                .HostedScriptPath = txtHostedScript2.Text
            End With
            ctrl.UpdateScript(jqueryui)

        End Sub

        Private Sub LoadSettings()
            Dim ctrl As New AggregatorController

            ' jquery
            Dim jquery As ScriptInfo = ctrl.GetScript(PortalId, "JQUERY")
            chkLoadScript1.Checked = jquery.DontLoadScript
            chkUseHostedScript1.Checked = jquery.LoadHosted
            txtHostedScript1.Text = jquery.HostedScriptPath

            ' jquery ui
            Dim jqueryui As ScriptInfo = ctrl.GetScript(PortalId, "JQUERYUI")
            chkLoadScript2.Checked = jqueryui.DontLoadScript
            chkUseHostedScript2.Checked = jqueryui.LoadHosted
            txtHostedScript2.Text = jqueryui.HostedScriptPath

        End Sub

#End Region

#Region " Validation"
#End Region


    End Class


End Namespace