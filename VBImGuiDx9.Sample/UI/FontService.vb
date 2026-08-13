Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports ImGuiNET
Imports VBImGuiDx9.VBImGuiDx9.Backends.Direct3D9
Imports VBImGuiDx9.VBImGuiDx9.Core

Namespace VBImGuiDx9.Sample.UI

    ''' <summary>
    ''' Manages application fonts and the Dear ImGui font atlas.
    '''
    ''' All TTF files located in Assets\Fonts are discovered
    ''' automatically. Each font is loaded in several predefined
    ''' sizes so that the UI can switch fonts without rebuilding
    ''' the atlas during a frame.
    ''' </summary>
    Public NotInheritable Class FontService

        Private Const SmallFontSize As Single = 13.0F
        Private Const DefaultFontSize As Single = 16.0F
        Private Const LargeFontSize As Single = 22.0F

        Private ReadOnly _contextManager As ImGuiContextManager
        Private ReadOnly _renderer As Dx9ImGuiRenderer

        Private ReadOnly _fonts As New Dictionary(Of String, ImFontPtr)(
            StringComparer.OrdinalIgnoreCase)

        Private ReadOnly _fontPaths As New Dictionary(Of String, String)(
            StringComparer.OrdinalIgnoreCase)

        Private _defaultFontName As String

        Private _initialized As Boolean
        Private _disposed As Boolean

        Public ReadOnly Property DefaultFontName As String
            Get
                ThrowIfDisposed()
                Return _defaultFontName
            End Get
        End Property
        Public Function GetDefaultFont(
    size As Single) As ImFontPtr

            ThrowIfDisposed()

            Return GetFont(
        _defaultFontName,
        size)

        End Function

        Public Sub New(
            contextManager As ImGuiContextManager,
            renderer As Dx9ImGuiRenderer)

            If contextManager Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(contextManager))
            End If

            If renderer Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(renderer))
            End If

            _contextManager = contextManager
            _renderer = renderer

        End Sub

        Public ReadOnly Property IsAvailable As Boolean
            Get
                Return Not _disposed AndAlso
                       _contextManager.IsInitialized AndAlso
                       _renderer.IsInitialized
            End Get
        End Property

        Public ReadOnly Property IsInitialized As Boolean
            Get
                Return _initialized AndAlso
                       Not _disposed
            End Get
        End Property

        ''' <summary>
        ''' Gets all discovered font names.
        ''' </summary>
        Public ReadOnly Property FontNames As IReadOnlyCollection(Of String)
            Get
                ThrowIfDisposed()

                Return _fontPaths.Keys
            End Get
        End Property

        ''' <summary>
        ''' Gets the number of discovered TTF files.
        ''' </summary>
        Public ReadOnly Property FontFileCount As Integer
            Get
                ThrowIfDisposed()

                Return _fontPaths.Count
            End Get
        End Property

        ''' <summary>
        ''' Gets the total number of font variants stored
        ''' in the ImGui font atlas.
        ''' </summary>
        Public ReadOnly Property FontVariantCount As Integer
            Get
                ThrowIfDisposed()

                Return _fonts.Count
            End Get
        End Property

        ''' <summary>
        ''' Initializes all bundled TTF fonts.
        ''' </summary>
        Public Sub Initialize()

            ThrowIfDisposed()

            If _initialized Then
                Return
            End If

            If Not _contextManager.IsInitialized Then
                Throw New InvalidOperationException(
                    "ImGui context is not initialized.")
            End If

            If Not _renderer.IsInitialized Then
                Throw New InvalidOperationException(
                    "Dx9 ImGui renderer is not initialized.")
            End If

            Dim fontsDirectory As String =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "Fonts")

            If Not Directory.Exists(fontsDirectory) Then

                fontsDirectory =
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Sample",
                        "Assets",
                        "Fonts")

            End If

            If Not Directory.Exists(fontsDirectory) Then

                Throw New DirectoryNotFoundException(
                    "Font directory was not found. " &
                    "Checked:" &
                    Environment.NewLine &
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Assets",
                        "Fonts") &
                    Environment.NewLine &
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Sample",
                        "Assets",
                        "Fonts"))

            End If

            Dim fontFiles As String() =
                Directory.GetFiles(
                    fontsDirectory,
                    "*.ttf",
                    SearchOption.TopDirectoryOnly)

            If fontFiles.Length = 0 Then

                Throw New FileNotFoundException(
                    "No TTF fonts were found in: " &
                    fontsDirectory)

            End If

            Array.Sort(
                fontFiles,
                StringComparer.OrdinalIgnoreCase)

            Dim io As ImGuiIOPtr =
                ImGui.GetIO()

            io.Fonts.Clear()

            _fonts.Clear()
            _fontPaths.Clear()

            _defaultFontName = Nothing


            For Each fontPath As String In fontFiles

                Dim fontName As String =
        Path.GetFileNameWithoutExtension(
            fontPath)

                If _fontPaths.ContainsKey(fontName) Then
                    Continue For
                End If

                _fontPaths(fontName) = fontPath

                If String.IsNullOrEmpty(_defaultFontName) Then
                    _defaultFontName = fontName
                End If

                AddFontVariant(
                        io,
                        fontName,
                        fontPath,
                        SmallFontSize)

                AddFontVariant(
                        io,
                        fontName,
                        fontPath,
                        DefaultFontSize)

                AddFontVariant(
                        io,
                        fontName,
                        fontPath,
                        LargeFontSize)

            Next

            If _fonts.Count = 0 Then
                Throw New InvalidOperationException(
                    "No fonts could be loaded.")
            End If

            _contextManager.RebuildFontAtlas()

            _renderer.RebuildFontTexture()

            _initialized = True

        End Sub

        Private Sub AddFontVariant(
                    io As ImGuiIOPtr,
                    fontName As String,
                    fontPath As String,
                    fontSize As Single)

            Dim glyphRanges As IntPtr =
        io.Fonts.GetGlyphRangesCyrillic()

            Dim font As ImFontPtr =
        io.Fonts.AddFontFromFileTTF(
            fontPath,
            fontSize,
            Nothing,
            glyphRanges)

            Dim key As String =
        CreateFontKey(
            fontName,
            fontSize)

            _fonts(key) = font

        End Sub

        Private Shared Function CreateFontKey(
            fontName As String,
            fontSize As Single) As String

            Return fontName &
                   "|" &
                   fontSize.ToString(
                       CultureInfo.InvariantCulture)

        End Function

        Public Function ContainsFont(
            name As String) As Boolean

            ThrowIfDisposed()

            If String.IsNullOrWhiteSpace(name) Then
                Return False
            End If

            Return _fontPaths.ContainsKey(name)

        End Function

        ''' <summary>
        ''' Gets a font variant by font name and size.
        ''' </summary>
        Public Function GetFont(
            name As String,
            size As Single) As ImFontPtr

            ThrowIfDisposed()

            If String.IsNullOrWhiteSpace(name) Then
                Throw New ArgumentException(
                    "Font name cannot be empty.",
                    NameOf(name))
            End If

            If size <= 0.0F Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(size))
            End If

            Dim key As String =
                CreateFontKey(
                    name,
                    size)

            Dim font As ImFontPtr

            If Not _fonts.TryGetValue(
                key,
                font) Then

                Throw New KeyNotFoundException(
                    "Font variant was not found: " &
                    name &
                    " / " &
                    size.ToString(
                        CultureInfo.InvariantCulture) &
                    " px")

            End If

            Return font

        End Function

        Public ReadOnly Property DefaultFont As ImFontPtr
            Get
                ThrowIfDisposed()

                Return GetFont(
                    _defaultFontName,
                    DefaultFontSize)
            End Get
        End Property

        Public ReadOnly Property SmallFont As ImFontPtr
            Get
                ThrowIfDisposed()

                Return GetFont(
                    _defaultFontName,
                    SmallFontSize)
            End Get
        End Property

        Public ReadOnly Property LargeFont As ImFontPtr
            Get
                ThrowIfDisposed()

                Return GetFont(
                    _defaultFontName,
                    LargeFontSize)
            End Get
        End Property

        Public Function GetFontCount() As Integer

            ThrowIfDisposed()

            If Not _contextManager.IsInitialized Then
                Throw New InvalidOperationException(
                    "ImGui context is not initialized.")
            End If

            Return ImGui.GetIO().Fonts.Fonts.Size

        End Function

        Public Function GetFontNames() As String()

            ThrowIfDisposed()

            Dim result As String() =
        New String(_fontPaths.Count - 1) {}

            _fontPaths.Keys.CopyTo(
                    result,
                    0)

            Array.Sort(
                    result,
                    StringComparer.OrdinalIgnoreCase)

            Return result

        End Function

        Public Function GetFontSizeNames() As String()

            ThrowIfDisposed()

            Return New String() {
                "13 px",
                "16 px",
                "22 px"
            }

        End Function

        Public Function GetFontSizeValue(
            index As Integer) As Single

            ThrowIfDisposed()

            Select Case index

                Case 0
                    Return SmallFontSize

                Case 1
                    Return DefaultFontSize

                Case 2
                    Return LargeFontSize

                Case Else
                    Throw New ArgumentOutOfRangeException(
                        NameOf(index))

            End Select

        End Function

        Public Sub Rebuild()

            ThrowIfDisposed()

            If Not IsAvailable Then
                Throw New InvalidOperationException(
                    "Font service is not available.")
            End If

            _contextManager.RebuildFontAtlas()

            _renderer.RebuildFontTexture()

        End Sub

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(FontService))
            End If

        End Sub

        Public Sub Dispose()

            If _disposed Then
                Return
            End If

            _fonts.Clear()
            _fontPaths.Clear()

            _defaultFontName = Nothing

            _initialized = False
            _disposed = True

        End Sub

    End Class

End Namespace