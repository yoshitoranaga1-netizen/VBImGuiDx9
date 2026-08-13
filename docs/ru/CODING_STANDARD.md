# Стандарт кодирования

## 1. Общие правила

Все исходные файлы проекта должны соответствовать этому документу.

## 2. Компилятор

Во всех проектах используются:

- `Option Strict On`
- `Option Explicit On`
- `Option Infer On`

## 3. Структура файла

Каждый VB-файл начинается с:

```vb
Option Strict On
Option Explicit On
Option Infer On
```

Затем идут `Imports`, после чего `Namespace`.

## 4. Структура класса

Классы оформляются последовательно:

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

Не следует добавлять пустые или неиспользуемые секции без необходимости.

## 5. Именование

### Классы

Используется PascalCase:

```text
Renderer
GraphicsState
```

### Интерфейсы

Имя всегда начинается с `I`:

```text
IGraphicsDevice
```

### Private-поля

Начинаются с `_`:

```text
_device
_context
_vertexBuffer
```

### Константы

Используется PascalCase:

```text
MaxVertices
```

## 6. XML-документация

Public и Friend-типы должны иметь XML-документацию.

Public-методы должны иметь как минимум:

- `summary`;
- `param`, если есть параметры;
- `returns`, если метод возвращает значение.

## 7. Exceptions

Нельзя использовать пустой `Catch`.

Допустимо:

```vb
Catch ex As Exception
    Logger.Log(ex)
End Catch
```

Если исключение действительно можно безопасно игнорировать, причина должна быть понятна из контекста.

## 8. IDisposable

Класс, владеющий графическим, unmanaged или native ресурсом, должен корректно реализовывать `IDisposable`.

## 9. Shared

`Shared` используется только там, где это оправдано архитектурой, например для:

- factory;
- constants;
- extension methods.

Не следует использовать `Shared` для скрытого глобального состояния.

## 10. Архитектура

Зависимости между архитектурными блоками не должны нарушаться.

Core должен использовать Contracts там, где достаточно абстракции, а Direct3D9-специфичная реализация должна находиться в backend.

