# VRtasia
VR zombie-shooter prototype — single-player, Meta Quest, built in Unity.

## Status
| Field | Value |
|---|---|
| Phase | XR setup done — verify Hello-World in headset |
| Engine | **Unity 6000.4.7f1** + C# + URP |
| XR Stack | **OpenXR** + XR Interaction Toolkit 3 |
| Target Headset | **Meta Quest** (PCVR via Link first, Standalone later) |
| Backend | None (single-player) |

## Quick Start
> Full setup: [INSTALLATION.md](INSTALLATION.md) · Architecture: [Architecture.md](Architecture.md)

1. Install **Unity Hub** + Unity **6000.4.7f1** (Android Build Support module required).
2. Clone repo → **Unity Hub → Add project from disk** → select `unity/VRtasia/`.
3. On first open Unity downloads XR packages and runs the auto-setup script — see [INSTALLATION.md §5](INSTALLATION.md).
4. Do the **3 manual XR Management clicks** described in INSTALLATION.md §5.
5. Connect Quest via **Quest Link**, open `Assets/Scenes/HelloVR.unity`, press **▶ Play**.  
   **No headset?** Press Play anyway — the XR Device Simulator lets you test with mouse + keyboard. See [INSTALLATION.md §8](INSTALLATION.md).

### PyCharm helpers
| ▶ Configuration | Action |
|---|---|
| `Open Unity Editor` | Launches Unity 6000.4.7f1 with the VRtasia project |
| `Open Docs` | Serves project docs locally on port 8080 |

> **Tip (JetBrains IDEs):** the config names above are clickable in the IDE's Markdown preview — they link directly to the run configuration.

## Repository Layout
```
VRtasia/
├── unity/VRtasia/             # Unity project (Unity 6000.4.7f1, URP)
│   ├── Assets/Scripts/        # Runtime C# scripts
│   ├── Assets/Scripts/Editor/ # Editor-only tools (XRAutoSetup.cs)
│   ├── Assets/Scenes/         # HelloVR.unity (auto-created on first open)
│   ├── Packages/              # manifest.json + packages-lock.json
│   └── ProjectSettings/
├── docs/
│   └── implementation-plan.md
├── scripts/                   # Repo-level helper scripts
│   ├── open_unity.py          # Launched by "Open Unity Editor" run config
│   └── bootstrap-unity.ps1   # Disaster-recovery project re-creation
├── .github/
├── .run/                      # PyCharm run configurations
├── Architecture.md
├── INSTALLATION.md
├── ideas.md
└── README.md
```

## Documentation
| Document | Purpose |
|---|---|
| [Architecture.md](Architecture.md) | Engine + XR decisions, comfort/perf targets, pitfalls |
| [docs/implementation-plan.md](docs/implementation-plan.md) | Roadmap, backlog, design discussions |
| [docs/meeting-01-agenda.md](docs/meeting-01-agenda.md) | Kickoff-Treffen — Agenda, Entscheidungspunkte, Demo-Ablauf |
| [INSTALLATION.md](INSTALLATION.md) | Step-by-step environment setup |
| [ideas.md](ideas.md) | Raw brainstorm backlog |
| [.github/copilot-instructions.md](.github/copilot-instructions.md) | AI coding guidance |

## Current Focus
- [ ] Open project in Unity, let XR auto-setup run (see INSTALLATION.md §5)
- [ ] Do the 3 manual XR Management clicks
- [ ] Verify HelloVR scene in-headset: umsehen, Würfel greifen, teleportieren
- [ ] Decide D1 (locomotion) — see implementation plan
