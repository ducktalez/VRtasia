# Architecture

> Status: **Baseline established.** Engine and XR stack chosen; gameplay specifics still `TBD`.
> Companion docs: [docs/implementation-plan.md](docs/implementation-plan.md) · [INSTALLATION.md](INSTALLATION.md) · [ideas.md](ideas.md)

## Confirmed Decisions

| Topic | Decision | Notes |
|---|---|---|
| Engine | **Unity 6000.4.7f1** | Pinned. All team members must install this exact version via Unity Hub |
| Language | **C#** | |
| Render Pipeline | **Universal Render Pipeline (URP)** | Quest-friendly, default for Meta XR |
| XR Runtime | **OpenXR** | via Unity XR Plug-in Management |
| Interaction Layer | **XR Interaction Toolkit (XRI)** | Action-based input |
| Vendor SDK | **Meta XR SDK (Core)** installed but optional | Reserved for Passthrough / Hand-Tracking later |
| Target Platform | **Meta Quest** | PCVR via Quest Link first; Standalone APK later |
| Multiplayer / Backend | **None** | Deferred — single-player prototype |

## Open Decisions

Tracked as Design Discussions in [docs/implementation-plan.md](docs/implementation-plan.md):
- **D1** — Locomotion: Teleport (default) vs Smooth
- **D2** — Build pipeline: Standalone APK vs PCVR-only for prototype phase
- **D3** — Hand model: Controller visuals vs hand-tracked meshes

## Comfort & Performance Targets

| Metric | Target | Rationale |
|---|---|---|
| Framerate (PCVR/Link) | 90 Hz stable | Below 90 Hz on Quest Link causes judder |
| Framerate (Standalone) | 72 Hz minimum, 90 Hz goal | Quest 2/3 default refresh |
| Locomotion default | **Teleport + Snap-Turn** | Lowest motion-sickness risk for newcomers |
| Smooth locomotion | Opt-in only | Behind a comfort toggle |
| Vignette on movement | Enabled by default | Reduces nausea |
| Frametime budget | 11 ms (90 Hz) / 13.8 ms (72 Hz) | CPU + GPU combined |

## Component Breakdown (planned)

```
Scene "HelloVR"
├── XR Origin (Action-based)        # Camera rig + controllers
│   ├── Camera Offset
│   │   ├── Main Camera             # XRCamera
│   │   ├── Left Controller         # XRController + Ray Interactor
│   │   └── Right Controller        # XRController + Direct Interactor
├── Environment
│   ├── Floor (Plane + Teleport Area)
│   └── Table
└── Interactables
    └── Cube (RigidBody + XRGrabInteractable)
```

## Prototype Interaction Loop (target)

1. Player stands inside playspace
2. Zombie walks toward player
3. Player draws pistol (grab from holster on belt)
4. Player aims and shoots (trigger → raycast)
5. Zombie takes damage / falls
6. Loop repeats with increasing pressure

> **Hello-World scope** is intentionally narrower: only step 0 — *spawn into scene, look around, grab a cube, teleport*. Weapons + zombies come after D1 is decided.

## Pitfalls

- **Quest Link framerate**: dropping below target Hz causes immediate motion sickness. Profile early.
- **Single Pass Instanced rendering** must be enabled for Quest performance — easy to forget.
- **Action-based input** (new XRI) is mandatory; do not mix with deprecated device-based input.
- **Tracking origin**: must be set to *Floor* for room-scale; *Device* causes the player to spawn underground.
- **Garbage collection spikes** on Quest standalone — avoid per-frame allocations in gameplay scripts.
- **Scene reload during Play in Editor** can leave OpenXR in a broken state — restart Editor if input dies.
- **Git + Unity**: never commit `Library/`. Always commit `ProjectSettings/` and `Packages/manifest.json` + `packages-lock.json`.

## Repository Layout (current)

```
VRtasia/
├── unity/VRtasia/             # Unity project (created via Unity Hub — see INSTALLATION.md)
│   ├── Assets/
│   ├── Packages/
│   └── ProjectSettings/
├── docs/
│   └── implementation-plan.md
├── .github/
│   ├── copilot-instructions.md
│   └── instructions/
├── Architecture.md
├── INSTALLATION.md
├── ideas.md
└── README.md
```


