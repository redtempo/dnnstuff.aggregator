Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke

Namespace DNNStuff.Aggregator

    Public Class SqlDataProvider
        Inherits DataProvider

#Region "Private Members"
        Private Const ProviderType As String = "data"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString()

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

        Public ReadOnly Property ObjectPrefix() As String
            Get
                Return "DNNStuff_Aggregator_"
            End Get
        End Property
#End Region

#Region "Public Methods"
        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

        Public Overrides Function ListAggregator(ByVal ModuleId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "ListAggregator", ModuleId)
        End Function

        Public Overrides Function ListAggregatorModule(ByVal AggregatorId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "ListAggregatorModule", AggregatorId)
        End Function

        'tabs
        Public Overrides Function GetAggregatorTab(ByVal AggregatorId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetAggregatorTab", AggregatorId)
        End Function
        Public Overrides Function UpdateAggregatorTab(ByVal AggregatorId As Integer, ByVal ModuleId As Integer, ByVal Caption As String, ByVal Locale As String, ByVal HtmlText As String, ByVal Postback As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateAggregatorTab", AggregatorId, ModuleId, Caption, Locale, HtmlText, Postback), Integer)
        End Function
        Public Overrides Sub DeleteAggregatorTab(ByVal AggregatorId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "DeleteAggregatorTab", AggregatorId)
        End Sub
        Public Overrides Sub UpdateTabOrder(ByVal AggregatorId As Integer, ByVal Increment As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateTabOrder", AggregatorId, Increment)
        End Sub
        ' modules
        Public Overrides Function GetAggregatorModule(ByVal AggregatorModuleId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetAggregatorModule", AggregatorModuleId)
        End Function
        Public Overrides Function UpdateAggregatorModule(ByVal AggregatorModuleId As Integer, ByVal AggregatorId As Integer, ByVal TabModuleId As Integer, ByVal Locale As String, ByVal InsertBreak As Boolean, ByVal LoadEvent As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateAggregatorModule", AggregatorModuleId, AggregatorId, TabModuleId, Locale, InsertBreak, LoadEvent), Integer)
        End Function
        Public Overrides Sub DeleteAggregatorModule(ByVal AggregatorModuleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "DeleteAggregatorModule", AggregatorModuleId)
        End Sub
        Public Overrides Sub UpdateModuleOrder(ByVal AggregatorModuleId As Integer, ByVal Increment As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateModuleOrder", AggregatorModuleId, Increment)
        End Sub

        Public Overrides Function GetTabModules(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal ShowAllModules As Boolean, ByVal AggregatorModuleId As Integer, ByVal PortalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetTabModules", TabId, ModuleId, ShowAllModules, AggregatorModuleId, PortalId), IDataReader)
        End Function
        Public Overrides Function GetPageModules(ByVal TabId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetPageModules", TabId), IDataReader)
        End Function

        ' targets
        Public Overrides Function GetTargets(ByVal ModuleId As Integer) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetTargets", ModuleId)
        End Function

        Public Overrides Function UpdateTarget(ByVal AggregatorTargetId As Integer, ByVal ModuleId As Integer, ByVal AggregatorTabModuleId As Integer) As Integer
            Return SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateTarget", AggregatorTargetId, ModuleId, AggregatorTabModuleId)
        End Function

        Public Overrides Sub DeleteTarget(ByVal AggregatorTargetId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "DeleteTarget", AggregatorTargetId)
        End Sub

        Public Overrides Function GetAvailableTargets(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal AggregatorTargetId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetAvailableTargets", TabId, ModuleId, AggregatorTargetId), IDataReader)
        End Function

        ' localization support
        Public Overrides Function GetMMLinks(ByVal ModuleId As Integer, ByVal Locale As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, String.Format("SELECT LocaleText FROM {0}Delisoft_MMLinksTitleLocales WHERE ModuleId = {1} AND LocaleKey = '{2}'", ObjectQualifier, ModuleId, Locale)), IDataReader)
        End Function

        Public Overrides Function GetMLHTML(ByVal ModuleId As Integer, ByVal Locale As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, String.Format("SELECT localizedModuletitle FROM {0}Apollo_ModuleLocalization WHERE ModuleId = {1} AND Locale = '{2}'", ObjectQualifier, ModuleId, Locale)), IDataReader)
        End Function

        Public Overrides Function GetNUNTIO(ByVal ModuleId As Integer, ByVal Locale As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, String.Format("SELECT Title FROM {0}Nuntio_Content WHERE ModuleId = {1} and Locale = '{2}'", ObjectQualifier, ModuleId, Locale)), IDataReader)
        End Function

        ' custom properties
        Public Overrides Function GetProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String) As IDataReader
            Return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetProperties", ModuleId, AggregatorTabId, Skin, Theme)
        End Function

        Public Overrides Function UpdateProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String, ByVal Name As String, ByVal Value As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateProperties", ModuleId, AggregatorTabId, Skin, Theme, Name, Value), Integer)
        End Function

        ' scripts
        Public Overrides Function AddScript(ByVal portalId As Integer, ByVal scriptName As String, ByVal dontLoadScript As Boolean, ByVal internalScriptPath As String, ByVal loadHosted As Boolean, ByVal hostedScriptPath As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "AddScript", portalId, scriptName, dontLoadScript, internalScriptPath, loadHosted, hostedScriptPath), Integer)
        End Function

        Public Overrides Sub UpdateScript(ByVal portalId As Integer, ByVal scriptName As String, ByVal dontLoadScript As Boolean, ByVal internalScriptPath As String, ByVal loadHosted As Boolean, ByVal hostedScriptPath As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "UpdateScript", portalId, scriptName, dontLoadScript, internalScriptPath, loadHosted, hostedScriptPath)
        End Sub

        Public Overrides Sub DeleteScript(ByVal portalId As Integer, ByVal scriptName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "DeleteScript", portalId, scriptName)
        End Sub

        Public Overrides Function GetScript(ByVal portalId As Integer, ByVal scriptName As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier + ObjectPrefix & "GetScript", portalId, scriptName), IDataReader)
        End Function

        Public Overrides Function GetScripts(ByVal PortalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ObjectPrefix & "GetScripts", PortalId), IDataReader)
        End Function
#End Region

    End Class

End Namespace
