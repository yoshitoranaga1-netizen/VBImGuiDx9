# Участие в разработке

Спасибо за интерес к VBImGuiDx9.

Проект развивается как библиотека для создания интерфейсов на VB.NET с использованием Dear ImGui, ImGui.NET и Direct3D9.

Перед изменениями рекомендуется ознакомиться с:

- `README.md`
- `docs/ARCHITECTURE.md`
- `docs/GETTING_STARTED.md`
- `docs/FONTS.md`
- `docs/RENDERING.md`

## Структура проекта

```text
VBImGuiDx9/
├── Backends/
│   └── Direct3D9/
├── Contracts/
├── Core/
├── Diagnostics/
├── docs/
├── Sample/
├── VBImGuiDx9.Native/
├── VBImGuiDx9.Sample/
├── Directory.Build.props
├── LICENSE
├── README.md
├── VBImGuiDx9.slnx
└── VBImGuiDx9.vbproj
```

### Core

Содержит код, связанный с lifecycle ImGui и управлением UI.

Изменения Core не должны зависеть от конкретной реализации Direct3D9, если такая зависимость не является частью существующего API.

### Contracts

Содержит графические абстракции и должен оставаться максимально независимым от конкретного backend.

### Backends

Содержит конкретные реализации графического API.

Текущий backend:

```text
Direct3D9
```

### Native

`VBImGuiDx9.Native` содержит helper functionality, необходимую Direct3D9 backend. В NuGet он поставляется как часть основной библиотеки.

### Sample

Sample является демонстрационным приложением и используется для демонстрации API, проверки изменений, визуального тестирования и примеров использования.

Код Sample не должен становиться обязательной зависимостью основной библиотеки.

## Среда разработки

Используются:

- Windows;
- .NET 9;
- Visual Studio;
- VB.NET;
- ImGui.NET;
- Vortice.Direct3D9.

Новый код должен сохранять:

```text
Option Strict On
Option Explicit On
Option Infer On
```

## Стиль

Используйте существующий стиль проекта. Предпочтительно:

```vb
Dim value As Integer =
    CalculateValue()
```

Публичные классы и методы должны иметь понятные имена и XML documentation.

## Обработка ошибок

Ошибки состояния должны сообщаться явно. Если ресурс отсутствует и операция невозможна, не следует молча продолжать выполнение.

Для ожидаемых временных состояний, например временно недействительного Direct3D9 display size, допустимо безопасно пропустить текущий frame.

## Владение ресурсами

Каждый компонент должен явно понимать, какими ресурсами он владеет.

Например:

```text
ImGuiContextManager
    └── ImGui context

Dx9ImGuiRenderer
    ├── Font texture
    ├── Vertex buffer
    └── Index buffer

Dx9GraphicsDevice
    └── Direct3D9 device
```

Компонент освобождает только принадлежащие ему ресурсы.

## IDisposable

Классы, владеющие unmanaged/native resources, должны корректно реализовывать `IDisposable`.

Lifecycle:

```text
Create
  ↓
Initialize
  ↓
Use
  ↓
Dispose
```

После `Dispose()` объект не должен использоваться.

## ImGui context и frame lifecycle

UI должен строиться только внутри активного ImGui frame:

```text
BeginFrame
    ↓
Build UI
    ↓
EndFrame
    ↓
Render DrawData
```

## Direct3D9

При изменении backend необходимо учитывать:

- device lost;
- device reset;
- resource lifetime;
- dynamic buffers;
- render state;
- scissor state;
- texture binding.

Изменения rendering pipeline необходимо проверять визуально через Sample.

## Fonts

При изменении FontService проверить:

- загрузку нескольких TTF;
- размеры 13/16/22 px;
- Latin;
- Cyrillic;
- Font Atlas;
- DX9 font texture.

Перед добавлением TTF в repository необходимо проверить его лицензию и право распространения.

## UI changes

Минимальная проверка:

```text
Application starts
    ↓
ImGui initializes
    ↓
Windows appear
    ↓
Controls respond
    ↓
Fonts render correctly
    ↓
Resize works
    ↓
Minimize / restore works
    ↓
Application closes cleanly
```

## Build verification

Перед отправкой изменений необходимо выполнить полный build.

Минимальное требование:

```text
0 errors
```

Warnings оцениваются отдельно.

Изменение не считается завершённым только потому, что изменённый файл компилируется отдельно.

## Regression testing

После изменения Core или Backend рекомендуется проверить:

- запуск Sample;
- основные окна;
- buttons;
- checkbox;
- sliders;
- combo;
- tabs;
- fonts;
- Cyrillic text;
- resize;
- minimize/restore;
- закрытие приложения.

Особое внимание — Direct3D9 reset после изменений renderer.

## Pull Requests

Pull Request должен содержать:

- краткое описание;
- причину изменения;
- информацию о проверке;
- ограничения, если они есть.

## Commit messages

Примеры:

```text
Add font service
Fix DX9 font texture upload
Improve window manager
Add rendering documentation
Prepare NuGet packaging
```

Не рекомендуется:

```text
fix
changes
update
test
stuff
```

## Breaking changes

Изменение публичного API рассматривается как потенциальный breaking change.

Перед удалением или переименованием public-типа необходимо проверить Sample и остальные проекты.

Несовместимые изменения должны быть явно отмечены в release notes.

## Dependencies

Перед добавлением зависимости оценить:

- необходимость;
- размер;
- лицензию;
- совместимость с .NET 9;
- влияние на NuGet package;
- наличие уже существующей функциональности.

## Documentation

Изменения public API должны сопровождаться обновлением документации.

Документация должна описывать фактически реализованное поведение, а не планы.

## Release process

```text
Build
  ↓
Sample
  ↓
Documentation
  ↓
Package
  ↓
NuGet validation
  ↓
GitHub release
```

NuGet package должен содержать только необходимые библиотечные assemblies, dependencies и metadata. Sample остаётся в repository, но не должен становиться частью runtime package.

## License

Перед release необходимо учитывать лицензию проекта и лицензии:

- ImGui;
- ImGui.NET;
- Vortice;
- других dependencies;
- распространяемых TTF.

## Final checklist

Перед merge:

- [ ] Build succeeds
- [ ] 0 errors
- [ ] No accidental debug files
- [ ] Sample starts
- [ ] Main UI works
- [ ] Fonts work
- [ ] Cyrillic works where supported by TTF
- [ ] Resize works
- [ ] Device reset path checked
- [ ] Resources disposed correctly
- [ ] Documentation updated
- [ ] Public API reviewed
- [ ] Dependencies reviewed

Перед release:

- [ ] Version updated
- [ ] NuGet metadata checked
- [ ] README checked
- [ ] LICENSE checked
- [ ] Dependency licenses checked
- [ ] Package contents checked
- [ ] Sample excluded from runtime package
- [ ] NuGet package tested
- [ ] GitHub release prepared
