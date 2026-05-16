# Implementation Plan

> Roadmap, backlog, and design discussions for VRtasia.
> Baseline architecture: [../Architecture.md](../Architecture.md) · Setup: [../INSTALLATION.md](../INSTALLATION.md)

## Roadmap

| Phase | Goal | Status |
|---|---|---|
| 0 — Repo baseline | Docs, decisions, .gitignore, run configs | ✅ Done |
| 1 — Unity project bootstrap | Empty URP project committed to `unity/VRtasia/` (Unity 6000.4.7f1) | ✅ Done |
| 2 — XR setup | XR packages in manifest, `XRAutoSetup.cs` auto-configures on first open | ✅ Done (3 UI clicks remain — see INSTALLATION.md §5) |
| 3 — Hello-World scene | `HelloVR.unity` auto-created by setup script; grab cube + teleport | ✅ Done (verify in-headset) |
| 4 — Shooting spike | Raycast pistol grabbed from holster | 📋 Backlog |
| 5 — Static zombie target | Hittable dummy with damage states | 📋 Backlog |
| 6 — Zombie NPC | NavMesh-driven walker that damages player | 📋 Backlog |

## Backlog

### Now (after Hello-World runs)
- [ ] Verify framerate ≥ 90 Hz on PCVR Link with empty scene
- [ ] Decide D1 (locomotion) — see Design Discussions
- [ ] Add holster anchor on XR Origin and a pistol prefab (cube proxy first)
- [ ] Raycast-shoot script with muzzle flash + audio stub

### Soon
- [ ] Static zombie dummy: capsule + ragdoll fall on death
- [ ] Damage component + hit feedback (haptics on controller)
- [ ] NavMesh bake + single walking zombie

### Later
- [ ] Wave spawner
- [ ] Magazine reload mechanic (two-handed)
- [ ] Standalone Quest APK build pipeline (after D2)
- [ ] Git LFS for binary assets (models, audio)

## Design Discussions

### D1 — Locomotion model
- **Options**: Teleport + Snap-Turn (default) · Smooth locomotion (opt-in) · Both via comfort menu
- **Trade-off**: Smooth fits a shooter feel but excludes motion-sensitive players
- **Status**: Open. Recommendation: ship Teleport for Hello-World, add Smooth as opt-in before phase 4
- **Decision**: `TBD`

### D2 — Build pipeline target
- **Options**: PCVR-only (Quest Link) · Standalone APK · Both
- **Trade-off**: Standalone needs aggressive optimisation early; PCVR is forgiving
- **Status**: Open. Recommendation: PCVR-only through phase 5, then evaluate
- **Decision**: `TBD`

### D3 — Hand representation
- **Options**: Controller meshes · Stylised hand meshes · Hand-tracking
- **Status**: Open. Recommendation: controller meshes from XRI Starter Assets; revisit after phase 6
- **Decision**: `TBD`

### D4 — Asset pipeline & version control
- **Options**: Plain Git · Git + LFS · Unity Version Control (Plastic SCM)
- **Trade-off**: LFS sufficient for small teams; Plastic better for large binary churn
- **Status**: Open. Recommendation: plain Git until first 3D model lands, then add LFS
- **Decision**: `TBD`

## XR Setup — what's automated vs manual

### Automated (runs on first Editor open via `XRAutoSetup.cs`)
- XR packages in `manifest.json`: `com.unity.xr.management 4.5.0`, `com.unity.xr.openxr 1.13.1`, `com.unity.xr.interaction.toolkit 3.0.7`
- Import XRI *Starter Assets* sample (needed for pre-configured controller prefabs)
- Create `Assets/Scenes/HelloVR.unity`: full XR rig from starter prefab, floor with `TeleportationArea`, table, grabbable cube with `XRGrabInteractable` + `HelloVRLogger`
- Register HelloVR as build scene 0

### Manual (once per developer machine, < 1 min)
See [INSTALLATION.md §5](../INSTALLATION.md) for exact UI steps:
1. Project Settings → XR Plug-in Management → enable **OpenXR** (PC + Android)
2. Add interaction profiles: *Oculus Touch Controller Profile*, *Meta Quest Touch Pro Controller Profile*
3. Android → Render Mode: **Single Pass Instanced**

## Done

- [x] Baseline docs created (`Architecture.md`, `INSTALLATION.md`, `ideas.md`, this file)
- [x] Engine direction fixed: Unity 6000.4.7f1 + C# + URP + OpenXR + XRI + Meta Quest
- [x] `.gitignore` and `.gitattributes` cover Unity outputs
- [x] XR packages added to `manifest.json`
- [x] `Assets/Scripts/HelloVRLogger.cs` created
- [x] `Assets/Scripts/Editor/XRAutoSetup.cs` created (auto-imports sample + creates scene)

