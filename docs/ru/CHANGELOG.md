# История изменений

Все значимые изменения VBImGuiDx9 документируются в этом файле.

Проект придерживается Semantic Versioning, где это применимо.

---

# [Unreleased]

Подготовка проекта к публичному релизу.

## Changed

### Project Structure

- Актуализирована структура solution.
- Добавлен отдельный `VBImGuiDx9.Native` helper project.
- `VBImGuiDx9.Sample` выделен в отдельный проект.
- Уточнена структура документации проекта.
- Документация переводится на русский и английский языки.
- `BLUEPRINT.md` удалён как отдельный документ проекта.

### Packaging

- Настроена упаковка библиотеки в NuGet package.
- Настроено формирование symbols package `.snupkg`.
- В основной package включается `VBImGuiDx9.Native.dll`.
- В package включается `README.md`.
- Настроена MIT license metadata.

### Validation

- Проверена сборка solution в `Release`.
- Проверена структура NuGet package.
- Проверено наличие:
  - `VBImGuiDx9.dll`;
  - `VBImGuiDx9.Native.dll`;
  - XML documentation;
  - `README.md`.

---

# [0.1.0] - 2026-08-08

## Added

### Project Foundation

- Создан solution VBImGuiDx9.
- Добавлен основной проект библиотеки `VBImGuiDx9`.
- Добавлен проект `VBImGuiDx9.Sample`.
- Сформирована начальная архитектура проекта.
- Определены соглашения по разработке.
- Добавлена общая конфигурация сборки.
- Добавлен `.editorconfig`.
- Включена генерация XML-документации.
- Включены строгие настройки компилятора Visual Basic.
- Включены .NET analyzers.

### Documentation

Добавлена начальная документация проекта:

- `README.md`
- `PROJECT_TREE.md`
- `ROADMAP.md`
- `CODING_STANDARD.md`
- `CONTRIBUTING.md`
- `API.md`
- `CHANGELOG.md`

### Contracts

Сформированы публичные graphics contracts.

Добавлены:

- `IGraphicsDevice`
- `IGraphicsContext`
- `IGraphicsResource`
- `IBuffer`
- `IVertexBuffer`
- `IIndexBuffer`
- `ITexture`
- `IRenderTarget`
- `IBlendState`
- `IRasterizerState`
- `IDepthStencilState`
- `ISamplerState`
- `ILogger`

Разделены обязанности graphics device и graphics context.

`IGraphicsDevice` отвечает за:

- информацию об устройстве;
- создание graphics context;
- создание vertex buffer;
- создание index buffer;
- создание texture.

`IGraphicsContext` отвечает за:

- начало кадра;
- завершение кадра;
- представление кадра;
- очистку текущего render target.

### Core

Добавлен первоначальный Core layer.

Добавлены:

- `VersionInfo`
- `DeviceOptions`
- `RendererOptions`
- `FrameStatistics`
- `RenderContext`
- `Renderer`

### Renderer

Реализован базовый lifecycle renderer:

```text
BeginFrame
    ↓
Clear
    ↓
rendering
    ↓
EndFrame
    ↓
Present
```

### Direct3D9 Backend

Добавлена реализация Direct3D9 backend.

Включены:

- `Dx9GraphicsDevice`
- `Dx9GraphicsContext`
- `Dx9ImGuiRenderer`
- `Dx9VertexBuffer`
- `Dx9IndexBuffer`
- `Dx9Texture`

### Native Helper

Добавлен проект `VBImGuiDx9.Native` для native/helper functionality, используемой Direct3D9 backend.

### Sample

Добавлено демонстрационное приложение `VBImGuiDx9.Sample`.

Sample включает демонстрацию:

- ImGui controls;
- окон;
- настроек;
- диагностики;
- шрифтов;
- Direct3D9 rendering.

### Fonts

Добавлена работа с TTF-шрифтами в Sample.

Поддерживается обнаружение шрифтов из `Assets/Fonts`.

### Build

- Целевой framework библиотеки: `.NET 9`.
- Sample использует `net9.0-windows`.
- Настроена генерация XML documentation.
- Настроена сборка native helper project.

---

# Versioning

Версии проекта следуют Semantic Versioning, когда это применимо:

```text
MAJOR.MINOR.PATCH
```

Изменения, находящиеся в разработке, документируются в `[Unreleased]`.
