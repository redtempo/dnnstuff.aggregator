Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Cache

Namespace DNNStuff.Aggregator
    Public Class Template
        Private _ms As ModuleSettings = Nothing
        Private _basePath As String = ""
        Private _resourcePath As String = ""

        Public Sub New(ByVal ms As ModuleSettings, ByVal basePath As String, ByVal resourcePath As String)
            _ms = ms
            _basePath = basePath
            _resourcePath = resourcePath
        End Sub

#Region " Properties"

        Public ReadOnly Property LayoutTemplate() As String
            Get
                Return GetTemplate("Layout", _ms.TabTemplate)
            End Get
        End Property

        Public ReadOnly Property TabStripTemplate() As String
            Get
                If _ms.HideTabs Then
                    Return "<div style=""display:none"" class=""[SKIN]_TabStrip"">[TABS][PREVNEXT]</div>"
                End If
                Return GetTemplate("TabStrip", _ms.TabTemplate)
            End Get
        End Property

        Public ReadOnly Property TabTemplate() As String
            Get
                Return GetTemplate("Tab", _ms.TabTemplate)
            End Get
        End Property

        Public ReadOnly Property TabPageTemplate() As String
            Get
                Return GetTemplate("TabPage", _ms.TabTemplate)
            End Get
        End Property

        Public ReadOnly Property PagingTemplate() As String
            Get
                Return GetTemplate("Paging", _ms.TabTemplate)
            End Get
        End Property


        Public ReadOnly Property PagingItemTemplate() As String
            Get
                Return GetTemplate("PagingItem", _ms.TabTemplate)
            End Get
        End Property

        Public ReadOnly Property RSSContentTemplate() As String
            Get
                Return GetTemplate("RSSContent", _ms.TabTemplate)
            End Get
        End Property

#End Region

#Region " Derived Properties"
        Public ReadOnly Property PagingInTemplates() As Boolean
            Get
                Return LayoutTemplate.Contains("[PAGING]") OrElse TabStripTemplate.Contains("[PAGING]") OrElse TabTemplate.Contains("[PAGING]") OrElse TabPageTemplate.Contains("[PAGING]")
            End Get
        End Property
#End Region

        Private Function GetTemplate(ByVal sectionName As String, ByVal templateName As String) As String
            ' get html file from resources/templates

            Dim cacheKey As String = _ms.TabTheme & "_" & templateName & "_" & sectionName

            Dim sectionHtml As String = CType(DataCache.GetCache(cacheKey), String)

            If (sectionHtml Is Nothing) Then
                Dim sr As System.IO.StreamReader = Nothing
                Dim skinpath As String = IO.Path.Combine(_basePath, sectionName & ".html")

                ' read from file system
                sectionHtml = GetFileContents(skinpath)
                If sectionHtml.Length > 0 Then
                    DataCache.SetCache(cacheKey, sectionHtml, New DNNCacheDependency(skinpath))
                Else
                    Dim resourcepath As String = IO.Path.Combine(IO.Path.Combine(_resourcePath, "Templates"), sectionName & ".html")
                    sectionHtml = GetFileContents(resourcepath)
                    DataCache.SetCache(cacheKey, sectionHtml, New DNNCacheDependency(resourcepath))
                End If
            End If
            Return sectionHtml


        End Function

        Public Function GetScript(ByVal sectionName As String, ByVal scope As String, ByVal useFallback As Boolean) As String
            ' get script file from resources/scripts
            Dim cacheKey As String = _ms.TabTheme & "_" & sectionName & "_" & scope

            Dim sectionHtml As String = CType(DataCache.GetCache(cacheKey), String)

            If (sectionHtml Is Nothing) Then
                Dim sr As System.IO.StreamReader = Nothing
                Dim skinfolder As IO.DirectoryInfo = New IO.DirectoryInfo(_basePath)
                Dim skinpath As String = ""

                Select Case scope.ToLower
                    Case "template"
                        skinpath = IO.Path.Combine(skinfolder.FullName, sectionName & ".txt")
                    Case "skin"
                        skinpath = IO.Path.Combine(skinfolder.Parent.FullName, sectionName & ".txt")
                    Case "resource"
                        skinpath = IO.Path.Combine(IO.Path.Combine(_resourcePath, "Scripts"), sectionName & ".txt")
                    Case Else
                        skinpath = IO.Path.Combine(_basePath, sectionName & ".txt")
                End Select

                ' read from file system
                sectionHtml = GetFileContents(skinpath)
                If sectionHtml.Length > 0 Then
                    DataCache.SetCache(cacheKey, sectionHtml, New DNNCacheDependency(skinpath))
                Else
                    If useFallback Then
                        ' grab from global resource path
                        Dim resourcepath As String = IO.Path.Combine(IO.Path.Combine(_resourcePath, "Scripts"), sectionName & ".txt")
                        sectionHtml = GetFileContents(resourcepath)
                        DataCache.SetCache(cacheKey, sectionHtml, New DNNCacheDependency(resourcepath))
                    End If
                End If
            End If
            Return sectionHtml

        End Function

        Private Function GetFileContents(ByVal filename As String) As String
            ' read contents of file and return empty if nothing
            Dim result As String = ""

            Dim sr As System.IO.StreamReader = Nothing
            ' read from file system
            If System.IO.File.Exists(filename) Then
                Try
                    sr = New System.IO.StreamReader(filename)
                    result = sr.ReadToEnd()
                Catch
                    result = ""
                Finally
                    If Not sr Is Nothing Then sr.Close()
                End Try
            End If
            Return result
        End Function
    End Class

End Namespace
