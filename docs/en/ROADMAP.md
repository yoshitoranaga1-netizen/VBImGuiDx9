# VBImGuiDx9 ROADMAP — Current State

**Last updated:** 2026-08-13

---

# 1. DX9 / ImGui Stability

## 1.1 Frame lifecycle

- [x] Basic DX9 frame lifecycle.
- [x] ImGui `NewFrame` / `Render` / `DrawData` lifecycle.
- [x] Correct `Clear`.
- [x] Correct ScissorTest handling before `Clear`.
- [x] `Present`.
- [x] Frame lifecycle protection against repeated `Begin` / `End`.
- [x] Dispose / lifetime of core components.

## 1.2 Resize / Minimize / Restore

- [x] Resize / BackBuffer changes.
- [x] Minimize / Restore.
- [x] Protection against zero window size.
- [x] Projection protection against zero dimensions.
- [x] Renderer protection against `ImDrawData.DisplaySize <= 0`.
- [x] Device Lost / Device Not Reset recovery.
- [x] Recreation of ImGui DX9 resources after Reset.
- [x] MainForm minimize / restore verified.

## 1.3 Remaining validation

- [ ] Full Alt+Tab verification on the user's real system.
- [ ] Final review of diagnostic DX9 methods.
- [ ] Final Dispose / lifetime verification after all architectural changes are complete.

---

# 2. ImGui Window Manager

- [x] Multiple logical ImGui windows in one ImGui context.
- [x] Window registration by stable ID.
- [x] Independent window movement.
- [x] Independent window resizing.
- [x] `Visible` management.
- [x] Window closing through X.
- [x] `Visible` synchronization after closing through X.
- [x] Visibility control through Settings.
- [x] Window management through `ImGuiWindowManager`.
- [x] Independent render callbacks.
- [x] `Collapsed` state.
- [x] `Insert` hotkey for opening / closing Settings.
- [x] User UI moved from MainForm into separate logical windows.
- [x] Old `RenderTestWindow` removed.
- [x] Position / size / collapsed state persisted through Dear ImGui `imgui.ini`.
- [x] Position / size / collapsed state restored after restart.

## Architectural rule

The Window Manager:

- does not manage DX9;
- does not manage `ImGui.NewFrame` / `ImGui.Render`;
- does not create the ImGui context;
- operates inside one ImGui context.

---

# 3. UI Architecture

Current flow:

```text
MainForm
    ↓
ImGui input / frame orchestration
    ↓
ImGuiFrameController
    ↓
ImGuiWindowManager
    ↓
logical ImGui windows
    ↓
ImGui DrawData
    ↓
Dx9ImGuiRenderer
    ↓
DX9
```

## Completed

- [x] MainForm no longer contains the complete layout of all windows.
- [x] MainWindow separated.
- [x] SettingsWindow separated.
- [x] DebugWindow separated.
- [x] SampleWindowSet handles window composition.
- [x] Window Manager handles registration and lifecycle of logical windows.
- [x] Settings controls Main / Debug through the Window Manager.
- [x] Settings opens / closes through `Insert`.
- [x] One ImGui context is used by the current MainForm.

## Next UI stage

- [ ] Define one application-level UI state mechanism.
- [ ] Define a global menu / toolbar.
- [ ] Extend the hotkey system only when a real need appears.
- [ ] Do not create a separate HotkeyManager until multiple independent hotkeys require it.

---

# 4. Performance / Profiling

## Profiling infrastructure

- [x] `FrameProfiler`.
- [x] Input measurement.
- [x] Device measurement.
- [x] ImGui Build measurement.
- [x] ImGui Render measurement.
- [x] DX9 Draw measurement.
- [x] Present measurement.
- [x] Other calculation.
- [x] Rolling sample window.
- [x] P50.
- [x] P95.
- [x] P99.
- [x] Min.
- [x] Max.
- [x] Average frame time.
- [x] Average FPS.

## Confirmed baseline

Current confirmed baseline:

- P50 ≈ 4 ms
- P95 ≈ 5 ms
- P99 ≈ 6 ms
- Min ≈ 3 ms
- Max ≈ 6 ms
- FPS ≈ 215–235

Rare external hitches up to approximately 111 ms were observed under normal system load and do not appear in P95 / P99.

## Optimization

- [ ] Measure allocations / frame.
- [ ] Measure vertex / index upload separately.
- [ ] Check actual render-state changes.
- [ ] Check temporary allocations in hot paths.
- [ ] Optimize only after a measured bottleneck appears.
- [ ] Do not optimize for theoretical gains without measurements.

---

# 5. DX9 Multithreading

## Verified

`D3DCREATE_MULTITHREADED` is not a worker / job architecture.

Experiment performed with:

```text
EnableMultithreading = True
EnableMultithreading = False
```

Result:

- P50 ≈ 4 ms
- P95 ≈ 5 ms
- P99 ≈ 6 ms
- Min ≈ 3 ms
- Max ≈ 6 ms
- no measurable difference.

## Decision

- [x] Check the effect of the D3D9 multithreaded flag.
- [x] Disable the multithreaded flag in the current baseline.
- [x] Do not use the D3D9 multithreaded flag as a replacement for worker threads.

## Worker threads

Multithreading is **not a current task**.

Return to it only if a real CPU-heavy operation appears.

Conditions:

- [ ] A CPU-heavy operation is identified.
- [ ] Its execution time is measured.
- [ ] Its impact on frame time is demonstrated.
- [ ] Immutable / shared data is defined.
- [ ] Worker / job abstraction is designed.
- [ ] Results can be safely transferred to the UI thread.
- [ ] No ImGui API calls from worker threads.
- [ ] No DX9 Reset / Present from worker threads.
- [ ] Real performance gain is measured.

Target flow:

```text
Worker
    ↓
immutable result
    ↓
UI thread
    ↓
ImGui
    ↓
DrawData
    ↓
DX9
    ↓
Present
```

---

# 6. Native Multi-Window

Current multiple ImGui windows are **not multiple native HWNDs**.

This remains a separate future stage.

- [ ] Determine whether multiple HWNDs are actually required.
- [ ] Design a Native Window Manager.
- [ ] Define ownership of the DX9 device / context.
- [ ] Define render targets / backbuffers.
- [ ] Define resize / reset ownership.
- [ ] Implement only after ownership has been designed.

Do not start implementation merely to have multiple native windows.

---

# 7. Core / Backend Architecture

## Core

- [x] `VersionInfo`.
- [x] `DeviceOptions`.
- [x] `RendererOptions`.
- [x] `FrameStatistics`.
- [x] `RenderContext`.
- [x] `Renderer`.
- [x] `ImGuiContextManager`.
- [x] `ImGuiFrameController`.
- [x] `ImGuiWindowManager`.
- [x] `ImGuiWindowState`.

## Contracts

- [x] `IGraphicsDevice`.
- [x] `IGraphicsContext`.
- [x] `IGraphicsResource`.
- [x] `IBuffer`.
- [x] `IVertexBuffer`.
- [x] `IIndexBuffer`.
- [x] `ITexture`.
- [x] `IRenderTarget`.
- [x] `ILogger`.

## Direct3D9 backend

- [x] Graphics Device.
- [x] Graphics Context.
- [x] ImGui Renderer.
- [x] Vertex Buffer.
- [x] Index Buffer.
- [x] Texture.
- [x] Device reset / recovery.

---

# 8. Font System

- [x] TTF discovery from `Assets\Fonts`.
- [x] Font name derived from the TTF filename.
- [x] 13 px font variants.
- [x] 16 px font variants.
- [x] 22 px font variants.
- [x] Cyrillic glyph range support.
- [x] Font Gallery in the Sample.
- [x] Font selection by name and size.
- [x] Latin / Cyrillic preview.
- [x] Font Atlas diagnostics.
- [x] No special built-in `DemoFont`.
- [x] Inter Cyrillic verified.
- [x] Roboto Cyrillic verified.

## Current limitations

- [ ] Automatic font fallback is not implemented.
- [ ] System-font discovery is not implemented.
- [ ] WOFF / WOFF2 support is not implemented.
- [ ] Variable-font configuration is not implemented.

These are limitations / possible future features, not current library capabilities.

---

# 9. Documentation

## Structure

- [x] Documentation directory created.
- [x] Russian documentation moved to `docs\ru`.
- [x] English documentation moved to `docs\en`.
- [x] Duplicate `docs\README.md` removed.
- [x] Architecture English version prepared.
- [x] Getting Started English version prepared.
- [x] Rendering English version prepared.
- [ ] Fonts English version.
- [ ] API English version.
- [ ] Contributing English version.
- [ ] Coding Standard English version.
- [ ] Project Tree English version.
- [ ] Roadmap English version.
- [ ] Changelog English version.
- [ ] Blueprint English version.
- [ ] Final RU / EN consistency review.
- [ ] Documentation navigation page.

---

# 10. NuGet Packaging

## Completed

- [x] NuGet package metadata.
- [x] Package ID: `VBImGuiDx9`.
- [x] MIT license metadata.
- [x] README included in package.
- [x] XML documentation generation.
- [x] Symbol package (`.snupkg`) enabled.
- [x] Native helper assembly included in the main package.
- [x] NuGet dependencies configured.
- [x] `.nupkg` successfully generated.
- [x] `.snupkg` successfully generated.
- [x] Package contents inspected.
- [x] `README.md` present in package.
- [x] `VBImGuiDx9.dll` present.
- [x] `VBImGuiDx9.Native.dll` present.
- [x] XML documentation present.

## Remaining

- [ ] Final package content review.
- [ ] Verify package installation from a local NuGet source.
- [ ] Verify package consumption from a clean test project.
- [ ] Final version decision.
- [ ] Final NuGet metadata review.
- [ ] Publish package to NuGet.org.

---

# 11. GitHub

## Repository preparation

- [x] Git repository initialized.
- [x] `.gitignore` configured.
- [x] `.vs` excluded.
- [x] `bin` / `obj` excluded.
- [x] user / solution-user files excluded.
- [x] package artifacts excluded from Git.
- [x] Library / Native / Sample projects placed in one repository.
- [x] Relative project references updated.
- [x] Release solution build verified after repository restructuring.
- [x] LICENSE added.

## Current repository structure

```text
VBImGuiDx9/
├── VBImGuiDx9.vbproj
├── VBImGuiDx9.Native/
├── VBImGuiDx9.Sample/
├── Backends/
├── Contracts/
├── Core/
├── Diagnostics/
├── Sample/
├── docs/
├── Directory.Build.props
├── LICENSE
├── README.md
├── .gitignore
└── VBImGuiDx9.slnx
```

## Remaining

- [ ] Final repository file review.
- [ ] First Git commit.
- [ ] Create GitHub repository.
- [ ] Add remote.
- [ ] Push initial repository.
- [ ] Verify GitHub rendering of README / documentation.
- [ ] Create initial release tag.
- [ ] Create GitHub Release.

---

# 12. Final Release

The final release should be performed only after all previous stages are complete.

Checklist:

- [ ] Clean Release build.
- [ ] No compiler errors.
- [ ] No unintended compiler warnings.
- [ ] Sample starts correctly.
- [ ] Resize / minimize / restore verified.
- [ ] Device reset verified.
- [ ] Font system verified.
- [ ] Documentation complete in Russian and English.
- [ ] NuGet package rebuilt.
- [ ] `.nupkg` contents verified.
- [ ] `.snupkg` contents verified.
- [ ] Package tested from a clean project.
- [ ] GitHub repository verified.
- [ ] GitHub Release created.
- [ ] NuGet package published.

---

# 13. Current Project Status

The project has reached the **pre-release / packaging stage**.

Current state:

```text
DX9 renderer
    ↓
stable

ImGui lifecycle
    ↓
stable

Window Manager
    ↓
stable

Font System
    ↓
working

Performance profiling
    ↓
baseline established

NuGet packaging
    ↓
package successfully builds

Git repository
    ↓
initialized and structurally prepared

Documentation
    ↓
RU/EN migration in progress

GitHub
    ↓
not yet published

NuGet.org
    ↓
not yet published
```

The current priority is **not adding new rendering functionality**.

The priority is:

```text
Documentation
    ↓
Repository audit
    ↓
Git first commit
    ↓
GitHub
    ↓
Final package verification
    ↓
NuGet
    ↓
Release
```

New architectural or performance features should be postponed unless they are required to fix a verified issue or release blocker.
