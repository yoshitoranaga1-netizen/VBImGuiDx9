Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Numerics
Imports ImGuiNET

Namespace VBImGuiDx9.Sample.UI

    ''' <summary>
    ''' Interactive Dear ImGui controls gallery used by the Sample project.
    ''' Demonstrates controls, layout and their VB.NET usage.
    ''' </summary>
    Public NotInheritable Class ImGuiDemoWindow

        Private _showSource As Boolean = True
        Private ReadOnly _fontService As FontService

        Private _useDemoFont As Boolean = False

        Private _basicCheckbox As Boolean = True
        Private _basicRadio As Integer = 0
        Private _basicProgress As Single = 0.72F

        Private _sliderInt As Integer = 50
        Private _sliderFloat As Single = 0.5F
        Private _dragInt As Integer = 25
        Private _dragFloat As Single = 2.5F
        Private _angle As Single = 45.0F

        Private _comboIndex As Integer = 0
        Private _listIndex As Integer = 0
        Private _selectableA As Boolean
        Private _selectableB As Boolean
        Private _selectableC As Boolean

        Private _fontNameIndex As Integer = 0
        Private _fontSizeIndex As Integer = 1

        Private ReadOnly _comboItems As String() = {
                "Direct3D9",
                "OpenGL",
                "Vulkan",
                "Direct3D11"
            }

        Private _inputText As String = "Hello ImGui.NET"
        Private _inputInt As Integer = 42
        Private _inputFloat As Single = 3.14F
        Private _inputMultiline As String =
            "This is a multiline input." &
            Environment.NewLine &
            "It is edited directly by ImGui."

        Private _appearanceScale As Single = 1.0F
        Private _windowRounding As Single = 6.0F
        Private _frameRounding As Single = 4.0F
        Private _frameBorderSize As Single = 1.0F
        Private _itemSpacing As Single = 8.0F

        Private _appearanceDark As Boolean = True
        Private _appearanceLight As Boolean = False

        Private _buttonClicks As Integer

        Public Sub New(
                    fontService As FontService)

            If fontService Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(fontService))
            End If

            _fontService = fontService

        End Sub

        Public Sub Render()

            ImGui.Text("Dear ImGui Controls Gallery")
            ImGui.Text(
                "Interactive examples for VB.NET + ImGui.NET.")

            ImGui.Separator()

            ImGui.Checkbox(
                "Show source examples",
                _showSource)

            ImGui.Spacing()

            If ImGui.BeginTabBar("ImGuiDemoTabs") Then

                If ImGui.BeginTabItem("Basic") Then
                    RenderBasic()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Input") Then
                    RenderInput()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Sliders") Then
                    RenderSliders()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Selection") Then
                    RenderSelection()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Containers") Then
                    RenderContainers()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Layout") Then
                    RenderLayout()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Appearance") Then
                    RenderAppearance()
                    ImGui.EndTabItem()
                End If

                If ImGui.BeginTabItem("Fonts") Then
                    RenderFonts()
                    ImGui.EndTabItem()
                End If

                ImGui.EndTabBar()

            End If

        End Sub

        Private Sub RenderBasic()

            ImGui.Text("Basic Controls")
            ImGui.Separator()

            ImGui.Text("Buttons")

            If ImGui.Button("Click me") Then
                _buttonClicks += 1
            End If

            ImGui.SameLine()

            ImGui.Text(
                "Clicks: " &
                _buttonClicks.ToString())

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##BasicButton") Then

                    ImGui.Text(
                        "If ImGui.Button(""Click me"") Then")

                    ImGui.Text(
                        "    _buttonClicks += 1")

                    ImGui.Text(
                        "End If")

                    ImGui.TreePop()

                End If

            End If

            ImGui.Spacing()

            ImGui.Text("Selection")

            ImGui.Checkbox(
                "Enable feature",
                _basicCheckbox)

            ImGui.RadioButton(
                "Option A",
                _basicRadio,
                0)

            ImGui.RadioButton(
                "Option B",
                _basicRadio,
                1)

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##BasicSelection") Then

                    ImGui.Text(
                        "ImGui.Checkbox(""Enable feature"", _basicCheckbox)")

                    ImGui.Text(
                        "ImGui.RadioButton(""Option A"", _basicRadio, 0)")

                    ImGui.Text(
                        "ImGui.RadioButton(""Option B"", _basicRadio, 1)")

                    ImGui.TreePop()

                End If

            End If

            ImGui.Spacing()

            ImGui.Text("Progress")

            ImGui.ProgressBar(
                    _basicProgress,
                    New Vector2(-1.0F, 0.0F),
                    "72%")

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##BasicProgress") Then

                    ImGui.Text(
                        "ImGui.ProgressBar(_basicProgress, -1.0F, ""72%"")")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderFonts()

            ImGui.Text("Fonts")
            ImGui.Separator()

            ImGui.Text(
        "Interactive font gallery and font atlas demonstration.")

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Primary font
            ' ------------------------------------------------------------

            ImGui.Text("Font")

            Dim fontNames As String() =
        _fontService.GetFontNames()

            If fontNames.Length = 0 Then

                ImGui.TextDisabled(
            "No fonts are available.")

                Return

            End If

            If _fontNameIndex < 0 OrElse
       _fontNameIndex >= fontNames.Length Then

                _fontNameIndex = 0

            End If

            ImGui.Combo(
                    "##PrimaryFont",
                    _fontNameIndex,
                    fontNames,
                    fontNames.Length)

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Font size
            ' ------------------------------------------------------------

            ImGui.Text("Size")

            If ImGui.Button(
                     "13 px##FontGallerySize") Then

                _fontSizeIndex = 0

            End If

            ImGui.SameLine()

            If ImGui.Button(
                    "16 px##FontGallerySize") Then

                _fontSizeIndex = 1

            End If

            ImGui.SameLine()

            If ImGui.Button(
                    "22 px##FontGallerySize") Then

                _fontSizeIndex = 2

            End If

            Dim fontSize As Single =
                _fontService.GetFontSizeValue(
                    _fontSizeIndex)

            ImGui.SameLine()

            ImGui.Text(
                    "Current: " &
                    fontSize.ToString("0") &
                    " px")

            ImGui.Spacing()
            ImGui.Separator()
            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Selected font
            ' ------------------------------------------------------------

            Dim selectedFontName As String =
                        fontNames(_fontNameIndex)

            Dim selectedFont As ImFontPtr =
                        _fontService.GetFont(
                            selectedFontName,
                            fontSize)

            ' ------------------------------------------------------------
            ' Preview
            ' ------------------------------------------------------------

            ImGui.Text("Preview")

            ImGui.Separator()

            ImGui.PushFont(
                    selectedFont)

            ImGui.Text("Latin")

            ImGui.Text(
                    "The quick brown fox jumps over the lazy dog.")

            ImGui.Text(
                    "1234567890 !@#$%^&*()")

            ImGui.Text(
                    "VB.NET + ImGui.NET + Direct3D9")

            ImGui.Text("Cyrillic")

            ImGui.Text(
                    "Съешь ещё этих мягких французских булок, да выпей чаю.")

            ImGui.PopFont()

            ImGui.Spacing()
            ImGui.Separator()
            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Selected font information
            ' ------------------------------------------------------------

            ImGui.Text("Selected font")

            ImGui.BulletText(
                        "Name: " &
                        selectedFontName)

            ImGui.BulletText(
                        "Size: " &
                        fontSize.ToString("0") &
                        " px")

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Font atlas information
            ' ------------------------------------------------------------

            ImGui.Text("Font atlas information")

            Dim io As ImGuiIOPtr =
        ImGui.GetIO()

            ImGui.BulletText(
                    "Font files: " &
                    _fontService.FontFileCount.ToString())

            ImGui.BulletText(
                    "Font variants: " &
                    _fontService.FontVariantCount.ToString())

            ImGui.BulletText(
                        "Atlas fonts: " &
                        io.Fonts.Fonts.Size.ToString())

            ImGui.BulletText(
                    "Font atlas: available")

            ImGui.BulletText(
                    "Font texture: " &
                    io.Fonts.TexID.ToString())

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Source example
            ' ------------------------------------------------------------

            If _showSource Then

                If ImGui.TreeNode(
            "VB.NET source##FontGallery") Then

                    ImGui.Text(
            "Dim font = fontService.GetFont(selectedFontName, fontSize)")

                    ImGui.Text(
                "ImGui.PushFont(font)")

                    ImGui.Text(
                "ImGui.Text(""Preview text"")")

                    ImGui.Text(
                "ImGui.PopFont()")

                    ImGui.TreePop()

                End If

            End If

        End Sub


        Private Sub RenderAppearance()

            ImGui.Text("Appearance")
            ImGui.Separator()

            ImGui.Text(
        "Live editing of the Dear ImGui style.")

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Presets
            ' ------------------------------------------------------------

            ImGui.Text("Presets")

            If ImGui.Button(
        "Dark") Then

                ImGui.StyleColorsDark()

                _appearanceDark = True
                _appearanceLight = False

            End If

            ImGui.SameLine()

            If ImGui.Button(
        "Light") Then

                ImGui.StyleColorsLight()

                _appearanceDark = False
                _appearanceLight = True

            End If

            ImGui.SameLine()

            If ImGui.Button(
        "Classic") Then

                ImGui.StyleColorsClassic()

                _appearanceDark = False
                _appearanceLight = False

            End If

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Style
            ' ------------------------------------------------------------

            Dim style As ImGuiStylePtr =
        ImGui.GetStyle()

            ImGui.Text("Geometry")

            ImGui.SliderFloat(
        "Window rounding",
        _windowRounding,
        0.0F,
        20.0F)

            ImGui.SliderFloat(
        "Frame rounding",
        _frameRounding,
        0.0F,
        20.0F)

            ImGui.SliderFloat(
        "Frame border",
        _frameBorderSize,
        0.0F,
        3.0F)

            ImGui.SliderFloat(
        "Item spacing",
        _itemSpacing,
        0.0F,
        20.0F)

            style.WindowRounding =
        _windowRounding

            style.FrameRounding =
        _frameRounding

            style.FrameBorderSize =
        _frameBorderSize

            style.ItemSpacing =
        New System.Numerics.Vector2(
            _itemSpacing,
            _itemSpacing)

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Preview
            ' ------------------------------------------------------------

            ImGui.Text("Preview")
            ImGui.Separator()

            ImGui.Button(
        "Example button")

            ImGui.SameLine()

            ImGui.Checkbox(
        "Example checkbox",
        _basicCheckbox)

            ImGui.ProgressBar(
        _basicProgress,
        New System.Numerics.Vector2(
            -1.0F,
            0.0F),
        "72%")

            ImGui.Spacing()

            If _showSource Then

                If ImGui.TreeNode(
            "VB.NET examples##Appearance") Then

                    ImGui.Text(
                "Dim style As ImGuiStylePtr = ImGui.GetStyle()")

                    ImGui.Text(
                "style.WindowRounding = value")

                    ImGui.Text(
                "style.FrameRounding = value")

                    ImGui.Text(
                "style.FrameBorderSize = value")

                    ImGui.Text(
                "style.ItemSpacing = New Vector2(x, y)")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderLayout()

            ImGui.Text("Layout")
            ImGui.Separator()

            ImGui.Text(
        "This section demonstrates how controls can be arranged " &
        "into structured UI regions.")

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Two-column layout
            ' ------------------------------------------------------------

            If ImGui.BeginTable(
        "LayoutPanels",
        2,
        ImGuiTableFlags.BordersInnerV Or
        ImGuiTableFlags.Resizable) Then

                ImGui.TableNextColumn()

                ImGui.Text("Left panel")
                ImGui.Separator()

                If ImGui.Button(
            "Action A##Layout") Then

                    _buttonClicks += 1

                End If

                If ImGui.Button(
            "Action B##Layout") Then

                    _buttonClicks += 1

                End If

                ImGui.Text(
            "Actions: " &
            _buttonClicks.ToString())

                ImGui.TableNextColumn()

                ImGui.Text("Right panel")
                ImGui.Separator()

                ImGui.Text("Progress")

                ImGui.ProgressBar(
            _basicProgress,
            New System.Numerics.Vector2(
                -1.0F,
                0.0F),
            "72%")

                ImGui.Spacing()

                ImGui.Text("Status")

                ImGui.BulletText("Device: Ready")
                ImGui.BulletText("Renderer: Ready")
                ImGui.BulletText("ImGui: Ready")

                ImGui.EndTable()

            End If

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' SameLine example
            ' ------------------------------------------------------------

            ImGui.Text("SameLine")

            If ImGui.Button(
        "One##SameLine") Then
            End If

            ImGui.SameLine()

            If ImGui.Button(
        "Two##SameLine") Then
            End If

            ImGui.SameLine()

            If ImGui.Button(
        "Three##SameLine") Then
            End If

            ImGui.Spacing()

            ' ------------------------------------------------------------
            ' Resource table
            ' ------------------------------------------------------------

            ImGui.Text("Resource table")

            If ImGui.BeginTable(
        "ResourceTable",
        3,
        ImGuiTableFlags.Borders Or
        ImGuiTableFlags.RowBg Or
        ImGuiTableFlags.Resizable Or
        ImGuiTableFlags.SizingStretchProp) Then

                ImGui.TableSetupColumn("Name")
                ImGui.TableSetupColumn("Type")
                ImGui.TableSetupColumn("Status")

                ImGui.TableHeadersRow()

                RenderResourceRow(
            "Vertex Buffer",
            "GPU Resource",
            "Ready")

                RenderResourceRow(
            "Index Buffer",
            "GPU Resource",
            "Ready")

                RenderResourceRow(
            "Font Texture",
            "Texture",
            "Ready")

                RenderResourceRow(
            "Render Target",
            "Surface",
            "Ready")

                ImGui.EndTable()

            End If

            If _showSource Then

                ImGui.Spacing()

                If ImGui.TreeNode(
            "VB.NET examples##Layout") Then

                    ImGui.Text(
                "ImGui.BeginTable(""LayoutPanels"", 2, flags)")

                    ImGui.Text(
                "ImGui.TableNextColumn()")

                    ImGui.Text(
                "ImGui.SameLine()")

                    ImGui.Text(
                "ImGui.BeginTable(""ResourceTable"", 3, flags)")

                    ImGui.Text(
                "ImGui.TableSetupColumn(""Name"")")

                    ImGui.Text(
                "ImGui.TableHeadersRow()")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderResourceRow(
                    name As String,
                    resourceType As String,
                    status As String)

            ImGui.TableNextRow()

            ImGui.TableNextColumn()
            ImGui.Text(name)

            ImGui.TableNextColumn()
            ImGui.Text(resourceType)

            ImGui.TableNextColumn()
            ImGui.Text(status)

        End Sub

        Private Sub RenderContainers()

            ImGui.Text("Containers")
            ImGui.Separator()

            If ImGui.CollapsingHeader(
        "Collapsing Header") Then

                ImGui.Text(
            "Content inside a collapsible section.")

                If ImGui.Button(
            "Action##ContainerButton") Then

                    _buttonClicks += 1

                End If

                ImGui.SameLine()

                ImGui.Text(
            "Clicks: " &
            _buttonClicks.ToString())

            End If

            ImGui.Spacing()

            If ImGui.TreeNode(
        "Direct3D9##ContainerTree") Then

                ImGui.Text("Device")

                If ImGui.TreeNode(
            "Graphics Context##ContainerContext") Then

                    ImGui.BulletText("BeginFrame")
                    ImGui.BulletText("EndFrame")
                    ImGui.BulletText("Present")

                    ImGui.TreePop()

                End If

                If ImGui.TreeNode(
            "ImGui Renderer##ContainerRenderer") Then

                    ImGui.BulletText("Vertex Buffer")
                    ImGui.BulletText("Index Buffer")
                    ImGui.BulletText("Font Texture")

                    ImGui.TreePop()

                End If

                ImGui.TreePop()

            End If

            ImGui.Spacing()

            If ImGui.BeginChild(
                "ContainerChild",
                New System.Numerics.Vector2(
                    0.0F,
                    140.0F),
                ImGuiChildFlags.Borders) Then

                ImGui.Text("Child region")
                ImGui.Separator()

                ImGui.TextWrapped(
                    "This is an independent scrolling region " &
                    "inside the parent window.")

                ImGui.BulletText("Useful for long content")
                ImGui.BulletText("Has its own scrolling")
                ImGui.BulletText("Can contain normal ImGui controls")

                ImGui.EndChild()

            End If

            If _showSource Then

                ImGui.Spacing()

                If ImGui.TreeNode(
            "VB.NET examples##Containers") Then

                    ImGui.Text(
                "ImGui.CollapsingHeader(""Collapsing Header"")")

                    ImGui.Text(
                "ImGui.TreeNode(""Direct3D9"")")

                    ImGui.Text(
                "ImGui.TreePop()")

                    ImGui.Text(
                "ImGui.BeginChild(""ContainerChild"", size, ImGuiChildFlags.Borders)")

                    ImGui.Text(
                "ImGui.EndChild()")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderSelection()

            ImGui.Text("Selection Controls")
            ImGui.Separator()

            ImGui.Text("Combo")

            ImGui.Combo(
                "Renderer",
                _comboIndex,
                _comboItems,
                _comboItems.Length)

            ImGui.Spacing()

            ImGui.Text("Selectable")

            If ImGui.Selectable(
                "Direct3D9##SelectableA") Then

                _selectableA = Not _selectableA

            End If

            ImGui.SameLine()
            ImGui.Text(
                If(_selectableA, "Selected", "Not selected"))
            If ImGui.Selectable(
                 "ImGui.NET##SelectableB") Then

                _selectableB = Not _selectableB

            End If

            ImGui.SameLine()

            ImGui.Text(
                If(_selectableB, "Selected", "Not selected"))

            If ImGui.Selectable(
                "VB.NET##SelectableC") Then

                _selectableC = Not _selectableC

            End If

            ImGui.SameLine()

            ImGui.Text(
                If(_selectableC, "Selected", "Not selected"))

            ImGui.Spacing()

            ImGui.Text("List")

            Dim listItems As String() = {
                    "Vertex Buffer",
                    "Index Buffer",
                    "Texture",
                    "Render Target"
                }

            ImGui.ListBox(
                    "Resources",
                    _listIndex,
                    listItems,
                    listItems.Length,
                    4)

            If _showSource Then

                ImGui.Spacing()

                If ImGui.TreeNode(
            "VB.NET examples##Selection") Then

                    ImGui.Text(
                "ImGui.Combo(""Renderer"", _comboIndex, items, items.Length)")

                    ImGui.Text(
                "ImGui.Selectable(""Direct3D9"", selected)")

                    ImGui.Text(
                "ImGui.ListBox(""Resources"", _listIndex, items, items.Length, 4)")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderSliders()

            ImGui.Text("Sliders and Drag Controls")
            ImGui.Separator()

            If ImGui.BeginTable(
        "SliderTable",
        2) Then

                ImGui.TableNextColumn()

                ImGui.Text("Integer")

                ImGui.SliderInt(
            "Value##SliderInt",
            _sliderInt,
            0,
            100)

                ImGui.Text(
            "Value: " &
            _sliderInt.ToString())

                ImGui.TableNextColumn()

                ImGui.Text("Float")

                ImGui.SliderFloat(
            "Value##SliderFloat",
            _sliderFloat,
            0.0F,
            1.0F)

                ImGui.Text(
            "Value: " &
            _sliderFloat.ToString("0.00"))

                ImGui.TableNextColumn()

                ImGui.Text("Drag Integer")

                ImGui.DragInt(
            "Value##DragInt",
            _dragInt,
            1.0F,
            0,
            100)

                ImGui.TableNextColumn()

                ImGui.Text("Drag Float")

                ImGui.DragFloat(
            "Value##DragFloat",
            _dragFloat,
            0.1F,
            0.0F,
            10.0F)

                ImGui.TableNextColumn()

                ImGui.Text("Angle")

                ImGui.SliderAngle(
            "Angle##SliderAngle",
            _angle,
            -180.0F,
            180.0F)

                ImGui.Text(
            "Angle: " &
            _angle.ToString("0.0") &
            "°")

                ImGui.EndTable()

            End If

            If _showSource Then

                ImGui.Spacing()

                If ImGui.TreeNode(
            "VB.NET examples##Sliders") Then

                    ImGui.Text(
                "ImGui.SliderInt(""Value"", _sliderInt, 0, 100)")

                    ImGui.Text(
                "ImGui.SliderFloat(""Value"", _sliderFloat, 0.0F, 1.0F)")

                    ImGui.Text(
                "ImGui.DragInt(""Value"", _dragInt, 1.0F, 0, 100)")

                    ImGui.Text(
                "ImGui.DragFloat(""Value"", _dragFloat, 0.1F, 0.0F, 10.0F)")

                    ImGui.Text(
                "ImGui.SliderAngle(""Angle"", _angle, -180.0F, 180.0F)")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderInput()

            ImGui.Text("Input Controls")
            ImGui.Separator()

            ImGui.Text("Text")

            ImGui.InputText(
                "Name",
                _inputText,
                256)

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##InputText") Then

                    ImGui.Text(
                        "ImGui.InputText(""Name"", _inputText, 256)")

                    ImGui.TreePop()

                End If

            End If

            ImGui.Spacing()

            ImGui.Text("Numeric")

            ImGui.InputInt(
                "Integer",
                _inputInt)

            ImGui.InputFloat(
                "Float",
                _inputFloat)

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##InputNumeric") Then

                    ImGui.Text(
                        "ImGui.InputInt(""Integer"", _inputInt)")

                    ImGui.Text(
                        "ImGui.InputFloat(""Float"", _inputFloat)")

                    ImGui.TreePop()

                End If

            End If

            ImGui.Spacing()

            ImGui.Text("Multiline")

            ImGui.InputTextMultiline(
                        "##MultilineInput",
                        _inputMultiline,
                        2048,
                        New Vector2(-1.0F, 100.0F))

            If _showSource Then

                If ImGui.TreeNode(
                    "VB.NET source##InputMultiline") Then

                    ImGui.Text(
                        "ImGui.InputTextMultiline(")

                    ImGui.Text(
                        "    ""##MultilineInput"",")

                    ImGui.Text(
                        "    _inputMultiline,")

                    ImGui.Text(
                        "    2048,")

                    ImGui.Text(
                        "    New Vector2(-1.0F, 100.0F))")

                    ImGui.TreePop()

                End If

            End If

        End Sub

        Private Sub RenderNotImplemented(
            category As String)

            ImGui.Text(category)
            ImGui.Separator()

            ImGui.Text(
                "This section will be implemented next.")

        End Sub

    End Class

End Namespace