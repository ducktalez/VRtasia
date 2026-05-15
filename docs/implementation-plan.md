# Implementation Plan

> Roadmap, backlog, and design discussions for VRtasia.
> Baseline architecture: [../Architecture.md](../Architecture.md) · Setup: [../INSTALLATION.md](../INSTALLATION.md)

## Roadmap

| Phase | Goal | Status |
|---|---|---|
| 0 — Repo baseline | Docs, decisions, .gitignore, run configs | ✅ Done |
| 1 — Unity project bootstrap | Empty URP project committed to `unity/VRtasia/` (Unity 6000.4.7f1) | ✅ Done |
| 2 — XR setup | OpenXR + XRI + Meta XR packages, settings, sample import | ⏳ Agent step (see §XR Setup Checklist) |
| 3 — Hello-World scene | Grab cube + teleport works in-headset | ⏳ Blocked by phase 2 |
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

## XR Setup Checklist (agent runs after phase 1)

When the Unity project exists at `unity/VRtasia/`, the agent must:

1. **Edit `Packages/manifest.json`** — add dependencies:
   ```json
   "com.unity.xr.openxr": "1.13.0",
   "com.unity.xr.interaction.toolkit": "3.0.7",
   "com.unity.xr.management": "4.5.0"
   ```
   (Pin to versions present in the chosen Unity 6 LTS — agent looks up actual latest at execution time.)
2. **Meta XR SDK Core** — install via Unity Asset Store package or scoped registry; record exact source in this file.
3. **Edit `ProjectSettings/XRSettings.asset` + `XRGeneralSettings.asset`** — enable OpenXR loader for *Standalone (PC)* and *Android*.
4. **Edit `ProjectSettings/OpenXRSettings.asset`** — enable interaction profile *Meta Quest Touch Plus* (and *Touch Pro* for Quest Pro), set Render Mode = *Single Pass Instanced*.
5. **Import sample**: XRI → *Starter Assets* and *XR Device Simulator*.
6. **Create `Assets/Scenes/HelloVR.unity`** with: XR Origin (XR Rig) prefab from Starter Assets, ground plane (Teleportation Area), table, grabbable cube (`XRGrabInteractable` + `Rigidbody`), directional light.
7. **Create `Assets/Scripts/HelloVRLogger.cs`** — logs `Select Entered` / `Select Exited` from the cube's interactable.
8. **Set Tracking Origin Mode = Floor** on XR Origin.
9. **Verify** by entering Play mode (developer with headset).

## Done

- [x] Baseline docs created (`Architecture.md`, `INSTALLATION.md`, `ideas.md`, this file)
- [x] Engine direction fixed: Unity 6 LTS + C# + OpenXR + XRI + Meta Quest
- [x] `.gitignore` covers Unity outputs

