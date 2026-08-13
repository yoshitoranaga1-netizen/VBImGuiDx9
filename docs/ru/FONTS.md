# Шрифты

## Overview

VBImGuiDx9 предоставляет `FontService` для загрузки TTF-шрифтов и управления
вариантами размеров шрифта в Dear ImGui.

FontService автоматически обнаруживает TTF-файлы в каталоге:

Assets/Fonts

Например:

Assets/
└── Fonts/
    ├── Inter.ttf
    ├── Roboto.ttf
    └── Segoe UI.ttf

Имя файла без расширения используется как имя шрифта.

Inter.ttf
    ↓
Inter

Roboto.ttf
    ↓
Roboto

Segoe UI.ttf
    ↓
Segoe UI
FontService

FontService является частью Sample UI и отвечает за конфигурацию шрифтов
для Sample-приложения.

Он не владеет ImGui context или Direct3D9 renderer.

Основные зависимости:

FontService
    │
    ├── ImGuiContextManager
    │
    └── Dx9ImGuiRenderer

Context manager отвечает за ImGui context, а renderer — за создание
соответствующей Direct3D9 font texture.

Font discovery

При инициализации FontService ищет файлы:

*.ttf

в:

Assets/Fonts

Используется только верхний уровень каталога.

Подкаталоги не сканируются.

Если каталог отсутствует, FontService сообщает об ошибке.

Если TTF-файлы отсутствуют, инициализация также завершается ошибкой.

Font names

Имя шрифта определяется по имени файла.

Например:

Inter.ttf

становится:

Inter

Расширение .ttf не отображается в пользовательском интерфейсе.

Это позволяет автоматически добавлять новые шрифты без изменения кода.

Например, достаточно добавить:

Assets/Fonts/OpenSans.ttf

после чего OpenSans появится в Font Gallery.

Font sizes

Для каждого обнаруженного TTF создаются три варианта:

13 px
16 px
22 px

Например:

Inter
├── 13 px
├── 16 px
└── 22 px

Roboto
├── 13 px
├── 16 px
└── 22 px

Размеры являются частью текущей реализации Sample.

Они не означают, что ImGui или TTF ограничены только этими размерами.

При необходимости набор размеров может быть изменён в FontService.

Font keys

FontService хранит варианты шрифтов по составному ключу:

FontName + Size

Например:

Inter|13
Inter|16
Inter|22

Это позволяет получить конкретный вариант:

Dim font As ImFontPtr =
    fontService.GetFont(
        "Inter",
        16.0F)
Using a font

Полученный ImFontPtr используется через стандартный ImGui.NET API:

Dim font As ImFontPtr =
    fontService.GetFont(
        "Inter",
        16.0F)

ImGui.PushFont(font)

ImGui.Text(
    "Text rendered using Inter.")

ImGui.PopFont()

PushFont() должен иметь соответствующий PopFont().

Рекомендуемая структура:

ImGui.PushFont(font)

' UI rendered with selected font.

ImGui.PopFont()

Не следует оставлять font stack в изменённом состоянии между независимыми участками UI.

Font Atlas

Dear ImGui использует Font Atlas для хранения glyphs всех загруженных
шрифтов.

Общая схема:

TTF files
    │
    ▼
FontService
    │
    ▼
ImGui Font Atlas
    │
    ▼
RGBA32 pixel data
    │
    ▼
Direct3D9 texture

После построения atlas Direct3D9 renderer создаёт GPU texture.

Эта texture используется при отрисовке текста.

Font texture

Font texture является Direct3D9 resource.

Renderer:

получает pixel data Font Atlas;
создаёт IDirect3DTexture9;
копирует данные atlas в texture;
устанавливает texture ID в ImGui;
использует texture во время rendering.

Таким образом, пользовательский код не должен самостоятельно создавать
Direct3D9 texture для стандартных ImGui fonts.

Unicode and Cyrillic

FontService загружает диапазон кириллических glyphs при создании font variant.

Используется:

io.Fonts.GetGlyphRangesCyrillic()

и диапазон передаётся в:

io.Fonts.AddFontFromFileTTF(
    fontPath,
    fontSize,
    Nothing,
    glyphRanges)

Это позволяет ImGui включать кириллические glyphs в Font Atlas, если они
присутствуют в исходном TTF.

Important: glyph availability

GetGlyphRangesCyrillic() не добавляет отсутствующие символы в TTF.

Он только указывает ImGui, какие Unicode ranges необходимо попытаться
загрузить.

Например:

Roboto.ttf
    │
    ├── Latin      ✓
    └── Cyrillic   ✓

GetGlyphRangesCyrillic()
    ↓
Cyrillic glyphs loaded

Если TTF не содержит кириллицу:

SomeFont.ttf
    │
    ├── Latin      ✓
    └── Cyrillic   ✗

GetGlyphRangesCyrillic()
    ↓
Cyrillic glyphs unavailable

В таком случае отсутствие кириллицы является свойством самого файла шрифта,
а не ошибкой Direct3D9 renderer.

Example

Предположим, каталог содержит:

Assets/Fonts/

Inter.ttf
Roboto.ttf
Segoe UI.ttf

FontService создаёт:

Inter
├── 13
├── 16
└── 22

Roboto
├── 13
├── 16
└── 22

Segoe UI
├── 13
├── 16
└── 22

В Font Gallery пользователь получает ComboBox:

Inter
Roboto
Segoe UI

и выбор:

13 px
16 px
22 px
Font Gallery

Sample содержит специальную вкладку Fonts.

Она демонстрирует:

обнаружение TTF;
выбор шрифта;
выбор размера;
PushFont;
PopFont;
Latin text;
Cyrillic text;
Font Atlas information;
количество загруженных font variants.

Это одновременно является примером использования FontService.

Adding a new font

Для добавления нового шрифта достаточно положить TTF в:

Assets/Fonts

Например:

Assets/Fonts/

Inter.ttf
Roboto.ttf
Segoe UI.ttf
OpenSans.ttf

После запуска:

OpenSans

автоматически появится в Font Gallery.

Изменять FontService для каждого нового TTF не требуется.

Font licensing

Файл TTF является отдельным произведением и может иметь собственную лицензию.

Наличие файла в проекте не означает автоматически право на его
распространение.

Перед включением конкретного TTF в репозиторий или NuGet package необходимо
проверить:

лицензию;
разрешение на распространение;
требования к attribution;
требования к включению license file.

Поэтому библиотека не должна автоматически предполагать наличие какого-либо
конкретного коммерческого или системного шрифта.

Sample может использовать шрифты, распространение которых разрешено их
лицензией.

Current limitations

Текущая реализация FontService намеренно не предоставляет:

автоматический fallback между несколькими TTF;
автоматическое определение отсутствующих glyphs;
автоматическое объединение нескольких font families в один font;
загрузку системных шрифтов Windows;
WOFF/WOFF2;
variable-font configuration;
динамическую загрузку новых TTF без пересборки Font Atlas.

Эти возможности могут быть добавлены в будущих версиях.

Recommended usage

Для приложения рекомендуется:

подготовить набор TTF;
проверить их лицензии;
разместить их в Assets/Fonts;
инициализировать FontService;
получить нужный ImFontPtr;
использовать PushFont() / PopFont() только вокруг нужного UI.

Пример:

Dim font As ImFontPtr =
    fontService.GetFont(
        "Roboto",
        16.0F)

ImGui.PushFont(font)

ImGui.Begin("Example")

ImGui.Text(
    "Пример русского текста")

ImGui.End()

ImGui.PopFont()
Font lifecycle

Font Atlas должен существовать до использования соответствующих ImFontPtr.

При полном изменении набора fonts необходимо пересобрать Font Atlas и
соответствующую Direct3D9 font texture.

Концептуально:

Clear fonts
    ↓
Add TTF fonts
    ↓
Build Font Atlas
    ↓
Create DX9 font texture
    ↓
Use fonts

Нельзя изменять Font Atlas во время обычного rendering frame без понимания
последствий для связанных GPU resources.

Summary

Текущая система шрифтов предоставляет простой pipeline:

Assets/Fonts/*.ttf
        ↓
    FontService
        ↓
  13 / 16 / 22 px
        ↓
 ImGui Font Atlas
        ↓
 Direct3D9 Texture
        ↓
       GPU

Основной принцип:

Шрифт определяется содержимым TTF, а не именем файла.

Поэтому если конкретный TTF не содержит кириллицу, FontService не может
создать её автоматически.


> Примечание: этот документ описывает текущую реализацию FontService в Sample. Он не означает наличие font system в основной библиотеке как отдельного публичного сервиса.
