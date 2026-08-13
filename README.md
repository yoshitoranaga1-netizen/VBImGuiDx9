VBImGuiDx9

VBImGuiDx9 is a VB.NET library for building graphical user interfaces with Dear ImGui / ImGui.NET and Direct3D9.

The project separates UI logic, the Dear ImGui lifecycle, and the Direct3D9 backend, so application UI code does not need to manage the low-level graphics device directly.

Features

Dear ImGui context management

Complete frame lifecycle

Direct3D9 backend

Vertex/index buffer management

ImGui Font Atlas and DX9 font texture

Support for application-provided TTF fonts

Support for Cyrillic when the selected TTF contains the required glyphs

Management of multiple ImGui windows

Persistent window position and size state

Direct3D9 device lost/reset handling

VB.NET API with Option Strict On

Sample application demonstrating the library

Architecture

VBImGuiDx9
│
├── Contracts
│
├── Core
│   ├── ImGuiContextManager
│   ├── ImGuiFrameController
│   └── ImGuiWindows
│       ├── ImGuiWindowManager
│       └── ImGuiWindowState
│
├── Backends
│   └── Direct3D9
│       ├── Dx9GraphicsDevice
│       ├── Dx9GraphicsContext
│       └── Dx9ImGuiRenderer
│
└── VBImGuiDx9.Sample
    ├── UI
    └── Diagnostics

ImGuiContextManager is responsible for creating and managing the Dear ImGui context and does not directly depend on Direct3D9.

Direct3D9 is implemented as a separate backend. Dx9GraphicsDevice implements IGraphicsDevice, while the backend also contains a dedicated renderer for uploading and drawing ImGui draw data.

Creating a UI

After initializing ImGui, applications can create regular ImGui interfaces directly from VB.NET:

ImGui.Begin("My Window")

ImGui.Text("Hello from VB.NET")

If ImGui.Button("Click me") Then
    ' Button clicked.
End If

ImGui.End()

Standard ImGui.NET controls can be used as usual:

ImGui.Checkbox(
    "Enable feature",
    enabled)

ImGui.SliderFloat(
    "Value",
    value,
    0.0F,
    100.0F)

ImGui.ProgressBar(
    progress,
    New Vector2(-1.0F, 0.0F),
    "Progress")

The Sample application contains examples including Button, Checkbox, RadioButton, and ProgressBar.

Window Management

ImGuiWindowManager allows applications to register multiple independent windows and manage their state.

A window can store:

identifier

title

visibility

position

size

collapsed state

rendering callback

During RenderAll(), the manager invokes the registered window renderer and updates its current geometry.

Example:

Dim state As New ImGuiWindowState(
    "settings",
    "Settings")

state.Position =
    New Vector2(100.0F, 100.0F)

state.Size =
    New Vector2(500.0F, 400.0F)

windowManager.Register(
    state,
    AddressOf RenderSettings)

The window renderer itself:

Private Sub RenderSettings()

    ImGui.Text("Application Settings")

    ImGui.Separator()

    ' Controls...

End Sub

Fonts

VBImGuiDx9 does not ship TTF fonts in the NuGet package.

Applications are responsible for providing the fonts they want to use. A typical application layout can be:

MyApplication
└── Assets
    └── Fonts
        └── MyFont.ttf

The application may use ImGui.NET directly or implement its own font-loading service.

The selected TTF must contain the glyphs required by the application. For Cyrillic UI, the font must contain the required Cyrillic glyphs and the application must include the corresponding glyph range when building the Font Atlas.

VBImGuiDx9 does not redistribute system or third-party TTF fonts.

Direct3D9 Device Reset

Direct3D9 can lose the device, for example when the window state changes or during other device-related events.

The backend provides device status detection:

Operational
DeviceLost
DeviceNotReset
DriverInternalError
Unknown

and provides TryReset() for device recovery.

ImGui DX9 resources are invalidated before a reset and restored afterwards.

During resize/minimize/restore, the renderer also avoids processing draw data when the display dimensions or framebuffer scale are invalid.

Sample

The Sample application serves as:

a demonstration application

a set of working examples

a backend validation environment

practical API documentation

For example, MainWindow demonstrates a basic window, button, checkbox, and slider.

SampleWindowSet groups the application windows and registers them with ImGuiWindowManager.

Documentation

Detailed documentation is available in two languages:

docs/
├── en/
└── ru/

The English documentation is the primary documentation for the public GitHub repository. The Russian documentation is provided as an additional localized version.

Important documents include:

API

Architecture

Getting Started

Rendering

Fonts

Contributing

Coding Standard

Changelog

Roadmap

Requirements

.NET 9

Windows

Direct3D9

ImGui.NET 1.91.6.1

Vortice.Direct3D9 3.8.3

Vortice.Mathematics 2.1.1

Installation

Install the NuGet package:

dotnet add package VBImGuiDx9

The package includes the main VBImGuiDx9 assembly and its native helper assembly.

License

VBImGuiDx9 is released under the MIT License.

See LICENSE for the full license text.