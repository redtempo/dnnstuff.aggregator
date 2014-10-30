'***************************************************************************/
'* DataProvider.vb
'*
'* COPYRIGHT (c) 2004 by DNNStuff
'* ALL RIGHTS RESERVED.
'*
'* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'* TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'* DEALINGS IN THE SOFTWARE.
'*************/
Option Strict On
Option Explicit On 

Imports System
Imports System.Web.Caching
Imports System.Reflection
Imports DotNetNuke

Namespace DNNStuff.Aggregator

    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DNNStuff.Aggregator", "DNNStuff.Aggregator"), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Abstract methods"

        Public MustOverride Function ListAggregator(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function ListAggregatorModule(ByVal AggregatorTabId As Integer) As IDataReader

        'tabs
        Public MustOverride Function GetAggregatorTab(ByVal AggregatorTabId As Integer) As IDataReader
        Public MustOverride Function UpdateAggregatorTab(ByVal AggregatorTabId As Integer, ByVal ModuleId As Integer, ByVal Caption As String, ByVal Locale As String, ByVal HtmlText As String, ByVal Postback As Boolean) As Integer
        Public MustOverride Sub DeleteAggregatorTab(ByVal AggregatorTabId As Integer)
        Public MustOverride Sub UpdateTabOrder(ByVal AggregatorTabId As Integer, ByVal Increment As Integer)

        'modules
        Public MustOverride Function GetAggregatorModule(ByVal AggregatorTabId As Integer) As IDataReader
        Public MustOverride Function UpdateAggregatorModule(ByVal AggregatorModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal TabModuleId As Integer, ByVal Locale As String, ByVal InsertBreak As Boolean, ByVal LoadEvent As Integer) As Integer
        Public MustOverride Sub DeleteAggregatorModule(ByVal AggregatorModuleId As Integer)
        Public MustOverride Sub UpdateModuleOrder(ByVal AggregatorModuleId As Integer, ByVal Increment As Integer)

        Public MustOverride Function GetTabModules(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal ShowAllModules As Boolean, ByVal AggregatorModuleId As Integer, ByVal PortalId As Integer) As IDataReader
        Public MustOverride Function GetPageModules(ByVal TabId As Integer) As IDataReader

        'targets
        Public MustOverride Function GetTargets(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function UpdateTarget(ByVal AggregatorTargetId As Integer, ByVal ModuleId As Integer, ByVal TargetModuleId As Integer) As Integer
        Public MustOverride Sub DeleteTarget(ByVal AggregatorTargetId As Integer)
        Public MustOverride Function GetAvailableTargets(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal AggregatorTargetId As Integer) As IDataReader

        'localization support for other modules
        Public MustOverride Function GetMMLinks(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader
        Public MustOverride Function GetMLHTML(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader
        Public MustOverride Function GetNUNTIO(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader

        'properties
        Public MustOverride Function GetProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String) As IDataReader
        Public MustOverride Function UpdateProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String, ByVal Name As String, ByVal Value As String) As Integer

        ' script
        Public MustOverride Function AddScript(ByVal portalId As Integer, ByVal scriptName As String, ByVal dontLoadScript As Boolean, ByVal internalScriptPath As String, ByVal loadHosted As Boolean, ByVal hostedScriptPath As String) As Integer
        Public MustOverride Sub UpdateScript(ByVal portalId As Integer, ByVal scriptName As String, ByVal dontLoadScript As Boolean, ByVal internalScriptPath As String, ByVal loadHosted As Boolean, ByVal hostedScriptPath As String)
        Public MustOverride Sub DeleteScript(ByVal portalId As Integer, ByVal scriptName As String)
        Public MustOverride Function GetScript(ByVal portalId As Integer, ByVal scriptName As String) As IDataReader
        Public MustOverride Function GetScripts(ByVal portalId As Integer) As IDataReader

#End Region

    End Class

End Namespace
