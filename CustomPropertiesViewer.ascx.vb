Imports System.Xml
Imports System.Collections.Generic

Namespace DNNStuff.Aggregator

    Partial Public Class CustomPropertiesViewer
        Inherits System.Web.UI.UserControl

#Region " Public Properties"
        Private _properties As Dictionary(Of String, CustomProperty)
        Private _description As String

        Public Property Properties() As Dictionary(Of String, CustomProperty)
            Get
                Return _properties
            End Get
            Set(ByVal value As Dictionary(Of String, CustomProperty))
                _properties = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            lblDescription.Text = Description

            LoadProperties()
        End Sub

        ''' <summary>
        ''' Capture settings from controls and save to properties dictionary
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub SetProperties()
            For Each prop As CustomProperty In Properties.Values
                Select Case prop.Type
                    Case PropertyType.String
                        Dim tb As TextBox = DirectCast(FindControl("tb" & prop.Name), TextBox)
                        If tb IsNot Nothing Then
                            prop.Value = tb.Text
                        End If
                    Case PropertyType.Html
                        Dim tb As TextBox = DirectCast(FindControl("tb" & prop.Name), TextBox)
                        If tb IsNot Nothing Then
                            prop.Value = Server.HtmlEncode(tb.Text)
                        End If
                    Case PropertyType.Boolean
                        Dim cb As CheckBox = DirectCast(FindControl("cb" & prop.Name), CheckBox)
                        If cb IsNot Nothing Then
                            prop.Value = cb.Checked.ToString.ToLower
                        End If
                    Case PropertyType.Choice, PropertyType.Directory, PropertyType.Files
                        Dim ddl As DropDownList = DirectCast(FindControl("ddl" & prop.Name), DropDownList)
                        If ddl IsNot Nothing Then
                            prop.Value = ddl.SelectedValue
                        End If
                End Select
            Next
        End Sub

        Private Sub LoadProperties()

            If Properties IsNot Nothing Then
                If Properties.Count > 0 Then

                    For Each prop As CustomProperty In Properties.Values
                        Dim trProp As HtmlTableRow = New HtmlTableRow
                        Dim tdCaption As HtmlTableCell = New HtmlTableCell
                        Dim tdPrompt As HtmlTableCell = New HtmlTableCell

                        ' style rows, cells
                        trProp.VAlign = "Top"
                        tdCaption.VAlign = "Top"
                        tdPrompt.Align = "Left"

                        ' add label
                        Dim l As New DotNetNuke.UI.WebControls.PropertyLabelControl
                        l.ID = prop.Name & "_Help"
                        l.Caption = prop.Caption & " :"
                        l.Font.Bold = True
                        If prop.Help.Length > 0 Then
                            l.HelpText = "<div class=""Help"">" & prop.Help & "</div>"
                        End If

                        Dim prompt As Control = Nothing
                        ' prompt
                        Select Case prop.Type
                            Case PropertyType.String
                                Dim tb As New TextBox
                                tb.ID = "tb" & prop.Name
                                tb.Columns = Convert.ToInt32(prop.Columns)
                                tb.Rows = Convert.ToInt32(prop.Rows)
                                If tb.Rows > 1 Then tb.TextMode = TextBoxMode.MultiLine
                                tb.Text = prop.Value
                                prompt = tb
                            Case PropertyType.Html
                                Dim tb As New TextBox
                                tb.ID = "tb" & prop.Name
                                tb.Columns = Convert.ToInt32(prop.Columns)
                                tb.Rows = Convert.ToInt32(prop.Rows)
                                If tb.Rows > 1 Then tb.TextMode = TextBoxMode.MultiLine
                                tb.Text = Server.HtmlDecode(prop.Value)
                                prompt = tb
                            Case PropertyType.Boolean
                                Dim cb As New CheckBox
                                cb.ID = "cb" & prop.Name
                                Boolean.TryParse(prop.Value, cb.Checked)
                                prompt = cb
                            Case PropertyType.Choice
                                Dim ddl As New DropDownList
                                ddl.ID = "ddl" & prop.Name
                                Dim li As ListItem
                                For Each ch As CustomPropertyChoice In prop.Choices
                                    li = New ListItem(ch.Caption, ch.Value)
                                    li.Selected = (li.Value = prop.Value)
                                    ddl.Items.Add(li)
                                Next
                                prompt = ddl
                            Case PropertyType.Directory
                                Dim ddl As New DropDownList
                                ddl.ID = "ddl" & prop.Name
                                Dim dir As New IO.DirectoryInfo(MapPath(prop.Directory))
                                Dim li As ListItem
                                For Each subdir As IO.DirectoryInfo In dir.GetDirectories
                                    If Not subdir.Name.StartsWith("_") Then
                                        li = New ListItem(StrConv(subdir.Name.Replace("-", " "), VbStrConv.ProperCase), subdir.Name)
                                        li.Selected = (li.Value = prop.Value)
                                        ddl.Items.Add(li)
                                    End If
                                Next
                                prompt = ddl
                            Case PropertyType.Files
                                Dim ddl As New DropDownList
                                ddl.ID = "ddl" & prop.Name
                                Dim dir As New IO.DirectoryInfo(MapPath(prop.Directory))
                                Dim li As ListItem
                                For Each subfile As IO.FileInfo In dir.GetFiles
                                    If Not subfile.Name.StartsWith("_") Then
                                        li = New ListItem(subfile.Name, subfile.Name)
                                        li.Selected = (li.Value = prop.Value)
                                        ddl.Items.Add(li)
                                    End If
                                Next
                                prompt = ddl
                        End Select
                        l.EditControl = prompt

                        Dim div As New Panel()
                        div.CssClass = "dnnFormItem"
                        div.Controls.Add(l)
                        div.Controls.Add(prompt)
                        pnlProperties.Controls.Add(div)

                    Next
                Else
                    LoadNoProperties()
                End If
            Else
                LoadNoProperties()
            End If

        End Sub
        Private Sub LoadNoProperties()
            Dim trProp As HtmlTableRow = New HtmlTableRow
            Dim tdCaption As HtmlTableCell = New HtmlTableCell

            Dim l As New LiteralControl(DotNetNuke.Services.Localization.Localization.GetString("NoCustomSettings", ResolveUrl("App_LocalResources/Aggregator")))
            tdCaption.Controls.Add(l)

            trProp.Cells.Add(tdCaption)
            tblProperties.Rows.Add(trProp)

        End Sub
    End Class
End Namespace
