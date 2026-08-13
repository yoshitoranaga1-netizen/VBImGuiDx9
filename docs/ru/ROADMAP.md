# VBImGuiDx9 ROADMAP — актуальное состояние

Дата обновления: 2026-08-11

---

# 1. DX9 / ImGui Stability

## 1.1 Frame lifecycle

- [x] Базовый DX9 frame lifecycle.
- [x] ImGui NewFrame / Render / DrawData lifecycle.
- [x] Корректный Clear.
- [x] Корректная обработка ScissorTest перед Clear.
- [x] Present.
- [x] Защита frame lifecycle от повторного Begin/End.
- [x] Dispose/lifetime базовых компонентов.

## 1.2 Resize / Minimize / Restore

- [x] Resize / изменение BackBuffer.
- [x] Minimize / Restore.
- [x] Защита от нулевого размера окна.
- [x] Защита projection от нулевых размеров.
- [x] Защита renderer от ImDrawData.DisplaySize <= 0.
- [x] Device Lost / Device Not Reset recovery.
- [x] Пересоздание ImGui DX9 resources после Reset.
- [x] Проверено сворачивание и восстановление MainForm.

## 1.3 Осталось

- [ ] Полная проверка Alt+Tab на реальной системе пользователя.
- [ ] Финальная ревизия диагностических DX9 методов.
- [ ] Финальная проверка Dispose/lifetime после завершения всех архитектурных изменений.

---

# 2. ImGui Window Manager

- [x] Несколько логических ImGui-окон в одном ImGui context.
- [x] Регистрация окон по стабильному ID.
- [x] Независимое перемещение окон.
- [x] Независимое изменение размеров.
- [x] Управление Visible.
- [x] Закрытие окна через X.
- [x] Синхронизация Visible после закрытия через X.
- [x] Управление видимостью через Settings.
- [x] Управление окнами через ImGuiWindowManager.
- [x] Независимые Render callbacks.
- [x] Состояние Collapsed.
- [x] Hotkey Insert для открытия/закрытия Settings.
- [x] Реальный пользовательский UI перенесён из MainForm в отдельные окна.
- [x] Старый RenderTestWindow удалён.
- [x] Позиция/размер/Collapsed сохраняются штатным ImGui imgui.ini.
- [x] Положение/размер/Collapsed восстанавливаются после перезапуска.

## Архитектурное правило

Window Manager:

- не управляет DX9;
- не управляет ImGui NewFrame/Render;
- не создаёт ImGui context;
- работает внутри одного ImGui context.

---

# 3. UI Architecture

Текущая схема:

MainForm
→ ImGui input/frame orchestration
→ ImGuiFrameController
→ ImGuiWindowManager
→ logical ImGui windows
→ ImGui DrawData
→ Dx9ImGuiRenderer
→ DX9

## Выполнено

- [x] MainForm не содержит непосредственную разметку всех окон.
- [x] MainWindow отделён.
- [x] SettingsWindow отделён.
- [x] DebugWindow отделён.
- [x] SampleWindowSet отвечает за композицию окон.
- [x] Window Manager отвечает за регистрацию и lifecycle логических окон.
- [x] Settings управляет Main/Debug через Window Manager.
- [x] Settings открывается/закрывается через Insert.
- [x] Один ImGui context используется текущим MainForm.

## Следующий UI этап

- [ ] Определить единый механизм application-level UI state.
- [ ] Определить глобальное меню / toolbar.
- [ ] Расширить систему hotkeys только при появлении реальной необходимости.
- [ ] Не создавать отдельный HotkeyManager до появления нескольких независимых hotkeys.

---

# 4. Performance / Profiling

## Profiling infrastructure

- [x] FrameProfiler.
- [x] Измерение Input.
- [x] Измерение Device.
- [x] Измерение ImGui Build.
- [x] Измерение ImGui Render.
- [x] Измерение DX9 Draw.
- [x] Измерение Present.
- [x] Расчёт Other.
- [x] Rolling sample window.
- [x] P50.
- [x] P95.
- [x] P99.
- [x] Min.
- [x] Max.
- [x] Average frame time.
- [x] Average FPS.

## Baseline

Текущий подтверждённый baseline:

- P50 ≈ 4 ms
- P95 ≈ 5 ms
- P99 ≈ 6 ms
- Min ≈ 3 ms
- Max ≈ 6 ms
- FPS ≈ 215–235

Редкие внешние hitch до ~111 ms наблюдались при обычной системной нагрузке и не проявляются в P95/P99.

## Optimization

- [ ] Измерить allocations/frame.
- [ ] Измерить vertex/index upload отдельно.
- [ ] Проверить реальные state changes.
- [ ] Проверить временные allocations в горячих путях.
- [ ] Оптимизировать только после появления измеренного bottleneck.
- [ ] Не выполнять оптимизацию ради теоретического выигрыша.

---

# 5. DX9 Multithreading

## Проверено

`D3DCREATE_MULTITHREADED` не является worker/job архитектурой.

Проведён эксперимент:

EnableMultithreading = True
и
EnableMultithreading = False

Результат:

- P50 ≈ 4 ms
- P95 ≈ 5 ms
- P99 ≈ 6 ms
- Min ≈ 3 ms
- Max ≈ 6 ms
- измеримой разницы нет.

## Решение

- [x] Проверить влияние D3D9 Multithreaded flag.
- [x] Отключить Multithreaded flag в текущем baseline.
- [x] Не использовать D3D9 multithread flag как замену worker threads.

## Worker threads

Многопоточность НЕ является текущей задачей.

Вернуться к ней только при появлении реальной CPU-heavy операции.

Условия:

- [ ] Найдена CPU-heavy операция.
- [ ] Измерено её время.
- [ ] Доказано, что она влияет на frame time.
- [ ] Определены immutable/shared data.
- [ ] Worker/job abstraction.
- [ ] Безопасная передача результатов в UI thread.
- [ ] Никаких ImGui API из worker threads.
- [ ] Никаких DX9 Reset/Present из worker threads.
- [ ] Измерен реальный выигрыш.

Целевая схема:

Worker
→ immutable result
→ UI thread
→ ImGui
→ DrawData
→ DX9
→ Present

---

# 6. Native Multi-Window

Текущие несколько ImGui-окон НЕ являются несколькими native HWND.

Это отдельный будущий этап.

- [ ] Определить реальную необходимость нескольких HWND.
- [ ] Спроектировать Native Window Manager.
- [ ] Определить ownership DX9 device/context.
- [ ] Определить render targets/backbuffers.
- [ ] Определить resize/reset ownership.
- [ ] Реализовать только после завершения проектирования ownership.

Не начинать реализацию только ради наличия нескольких окон.

---

# 7. Core / Backend Architecture

## Core

- [x] VersionInfo.
- [x] DeviceOptions.
- [x] RendererOptions.
- [x] FrameStatistics.
- [x] RenderContext.
- [x] Renderer.
- [x] ImGuiContextManager.
- [x] ImGuiFrameController.
- [x] ImGuiWindowManager.
- [x] ImGuiWindowState.

## Contracts

- [x] IGraphicsDevice.
- [x] IGraphicsContext.
- [x] IGraphicsResource.
- [x] IBuffer.
- [x] IVertexBuffer.
- [x] IIndexBuffer.
- [x] ITexture.
- [x] IRenderTarget.
- [x] ILogger.

## Direct3D9 backend

- [x] Graphics Device.
- [x] Graphics Context.
- [x] ImGui Renderer.
- [x] Vertex Buffer.
- [x] Index Buffer.
- [x] Texture.
- [x] Device reset/recovery.

---

# 8. Technical Debt / Cleanup

- [ ] Проверить дублирующиеся Imports.
- [ ] Проверить namespace collisions.
- [ ] Проверить старые `VBImGuiDx9.VBImGuiDx9.*` namespace references.
- [ ] Проверить дублирование RenderContext / Renderer API.
- [ ] Проверить актуальность FrameStatistics.
- [ ] Оптимизировать вычисление percentile в FrameProfiler.
- [ ] Удалить действительно неиспользуемые методы после проверки ссылок.
- [ ] Финальная проверка Dispose/lifetime.
- [ ] Финальная проверка документации.

Важно:

Не удалять API только на основании визуального сходства.
Сначала проверить реальные references.

---

# 9. Persistence

- [x] Позиция окон сохраняется через Dear ImGui.
- [x] Размер окон сохраняется через Dear ImGui.
- [x] Collapsed state сохраняется через Dear ImGui.
- [x] Состояние восстанавливается после перезапуска.
- [x] Используется штатный imgui.ini.

Собственная JSON-система WindowStateStore НЕ требуется.

---

# 10. Current Baselines

## Baseline A — DX9 Stable

[x] Resize
[x] Minimize / Restore
[x] Device Reset
[x] ImGui resource recreation
[x] MainForm lifecycle

## Baseline B — ImGui Multi-Window

[x] Main
[x] Settings
[x] Debug
[x] Independent positioning
[x] Independent sizing
[x] Visible state
[x] Collapsed state
[x] imgui.ini persistence
[x] Settings visibility control
[x] Insert hotkey

## Baseline C — UI Architecture

[x] MainForm
→ ImGuiFrameController
→ ImGuiWindowManager
→ logical windows
→ DrawData
→ DX9 renderer

## Baseline D — Performance

[x] Profiler
[x] P50/P95/P99
[x] Frame baseline
[x] D3D9 Multithreaded experiment

Current result:

~4 ms P50
~5 ms P95
~6 ms P99
~215–235 FPS

## Target E — Application UI

[ ] Global application UI state
[ ] Global menu / toolbar
[ ] Additional application functionality

## Target F — Worker Threads

[ ] Only after a measured CPU bottleneck appears.

## Target G — Native Multi-Window

[ ] Only after a real requirement for multiple HWND appears.
