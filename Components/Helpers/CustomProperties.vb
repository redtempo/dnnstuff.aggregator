Imports System.Xml
Imports System.Xml.Serialization
Imports System.Collections.Generic

Namespace DNNStuff.Aggregator

    Public Enum PropertyType As Integer
        [String] = 0
        Choice = 1
        [Boolean] = 2
        Directory = 3
        Files = 4
        Html = 5
    End Enum

    <Serializable(), XmlRoot("settings")> _
    Public Class CustomProperties
        Private _internalProperties As New List(Of CustomProperty)
        Private _properties As New Dictionary(Of String, CustomProperty)

        Private _internalTabProperties As New List(Of CustomProperty)
        Private _tabproperties As New Dictionary(Of String, CustomProperty)

        Private _description As String = ""
        <XmlElement("description", GetType(System.String))> _
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Private _help As String = ""
        <XmlElement("help", GetType(System.String))> _
        Public Property Help() As String
            Get
                Return _help
            End Get
            Set(ByVal value As String)
                _help = value
            End Set
        End Property

        <XmlIgnore()> _
        Public ReadOnly Property Properties() As Dictionary(Of String, CustomProperty)
            Get
                _properties.Clear()
                For Each cp As CustomProperty In _internalProperties
                    _properties.Add(cp.Name, cp)
                Next
                Return _properties
            End Get
        End Property

        <XmlArray("properties"), XmlArrayItem("property", GetType(CustomProperty))> _
        Public Property InternalProperties() As List(Of CustomProperty)
            Get
                Return _internalProperties
            End Get
            Set(ByVal value As List(Of CustomProperty))
                _internalProperties = value
            End Set
        End Property

        <XmlIgnore()> _
        Public ReadOnly Property TabProperties() As Dictionary(Of String, CustomProperty)
            Get
                _tabproperties.Clear()
                For Each cp As CustomProperty In _internalTabProperties
                    _tabproperties.Add(cp.Name, cp)
                Next
                Return _tabproperties
            End Get
        End Property

        <XmlArray("tabproperties"), XmlArrayItem("property", GetType(CustomProperty))> _
        Public Property InternalTabProperties() As List(Of CustomProperty)
            Get
                Return _internalTabProperties
            End Get
            Set(ByVal value As List(Of CustomProperty))
                _internalTabProperties = value
            End Set
        End Property

        Shared Shadows Function Load(ByVal filename As String) As CustomProperties
            Dim serializer As XmlSerializer = New XmlSerializer(GetType(CustomProperties))
            Using reader As New IO.StreamReader(filename)
                Return DirectCast(serializer.Deserialize(reader), CustomProperties)
            End Using
            Return New CustomProperties
        End Function

        Public Sub Save(ByVal filename As String)
            Dim serializer As XmlSerializer = New XmlSerializer(GetType(CustomProperties))
            Using writer As New IO.StreamWriter(filename)
                serializer.Serialize(writer, Me)
            End Using
        End Sub
    End Class

    <Serializable()> _
    Public Class CustomProperty

        Private _value As String = ""
        <XmlIgnore(), XmlAttribute("value")> _
        Public Property Value() As String
            Get
                If _value.Length > 0 Then
                    Return _value
                End If
                Return _default
            End Get
            Set(ByVal value As String)

                _value = value
            End Set
        End Property

        Private _name As String = ""
        <XmlAttribute("name")> _
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Private _type As PropertyType = PropertyType.String
        <XmlAttribute("type")> _
        Public Property Type() As PropertyType
            Get
                Return _type
            End Get
            Set(ByVal value As PropertyType)
                _type = value
            End Set
        End Property

        Private _caption As String = ""
        <XmlAttribute("caption")> _
        Public Property Caption() As String
            Get
                Return _caption
            End Get
            Set(ByVal value As String)
                _caption = value
            End Set
        End Property

        Private _default As String = ""
        <XmlAttribute("default")> _
        Public Property [Default]() As String
            Get
                Return _default
            End Get
            Set(ByVal value As String)
                _default = value
            End Set
        End Property

        Private _directory As String = ""
        <XmlAttribute("directory")> _
        Public Property Directory() As String
            Get
                Return _directory
            End Get
            Set(ByVal value As String)
                _directory = value
            End Set
        End Property

        Private _help As String = ""
        <XmlElement("help")> _
        Public Property Help() As String
            Get
                Return _help
            End Get
            Set(ByVal value As String)
                _help = value
            End Set
        End Property

        Private _columns As String = ""
        <XmlAttribute("columns")> _
        Public Property Columns() As String
            Get
                If _columns.Length > 0 Then
                    Return _columns
                End If
                Return "40"
            End Get
            Set(ByVal value As String)
                _columns = value
            End Set
        End Property

        Private _rows As String = ""
        <XmlAttribute("rows")> _
        Public Property Rows() As String
            Get
                If _rows.Length > 0 Then
                    Return _rows
                End If
                Return "1"
            End Get
            Set(ByVal value As String)
                _rows = value
            End Set
        End Property

        Private _choices As List(Of CustomPropertyChoice) = Nothing
        <XmlArray("choices"), XmlArrayItem("choice", GetType(CustomPropertyChoice))> _
        Public Property Choices() As List(Of CustomPropertyChoice)
            Get
                Return _choices
            End Get
            Set(ByVal value As List(Of CustomPropertyChoice))
                _choices = value
            End Set
        End Property
    End Class

    <Serializable()> _
    Public Class CustomPropertyChoice
        Public Sub New(ByVal caption As String, ByVal value As String)
            _caption = caption
            _value = value
        End Sub
        Public Sub New()

        End Sub
        Private _value As String = ""
        <XmlAttribute("value")> _
        Public Property Value() As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

        Private _caption As String = ""
        <XmlAttribute("caption")> _
        Public Property Caption() As String
            Get
                If _caption.Length > 0 Then
                    Return _caption
                End If
                Return _value
            End Get
            Set(ByVal value As String)
                _caption = value
            End Set
        End Property

    End Class

End Namespace
