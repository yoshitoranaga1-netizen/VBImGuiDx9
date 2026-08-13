# Coding Standard

## 1. General

All source files in the project must follow this document.

## 2. Compiler

All projects use:

- `Option Strict On`
- `Option Explicit On`
- `Option Infer On`

## 3. File Layout

Every VB file starts with:

```vb
Option Strict On
Option Explicit On
Option Infer On
```

Then `Imports`, followed by `Namespace`.

## 4. Class Layout

Classes follow a consistent structure:

```vb
Option Strict On
Option Explicit On
Option Infer On

Imports ...

Namespace ...

    ''' <summary>
    ''' ...
    ''' </summary>
    Public NotInheritable Class Example

#Region "Fields"
#End Region

#Region "Constructors"
#End Region

#Region "Properties"
#End Region

#Region "Public Methods"
#End Region

#Region "Protected Methods"
#End Region

#Region "Private Methods"
#End Region

#Region "Dispose"
#End Region

    End Class

End Namespace
```

Do not add empty or unused sections without a reason.

## 5. Naming

### Classes

Use PascalCase:

```text
Renderer
GraphicsState
```

### Interfaces

Names always start with `I`:

```text
IGraphicsDevice
```

### Private fields

Start with `_`:

```text
_device
_context
_vertexBuffer
```

### Constants

Use PascalCase:

```text
MaxVertices
```

## 6. XML Documentation

Public and Friend types must have XML documentation.

Public methods should have:

- `summary`;
- `param`, when parameters exist;
- `returns`, when applicable.

## 7. Exceptions

Do not use empty `Catch` blocks.

Allowed:

```vb
Catch ex As Exception
    Logger.Log(ex)
End Catch
```

If an exception is intentionally ignored, the reason should be clear from the context.

## 8. IDisposable

A class that owns graphics, unmanaged, or native resources must correctly implement `IDisposable`.

## 9. Shared

Use `Shared` only where justified by the architecture, for example:

- factories;
- constants;
- extension methods.

Do not use `Shared` for hidden global state.

## 10. Architecture

Architectural dependency rules must not be violated.

Core should use Contracts where an abstraction is sufficient, while Direct3D9-specific implementation belongs in the backend.
