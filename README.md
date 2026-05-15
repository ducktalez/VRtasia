# VRtasia
VR zombie-shooter prototype — single-player, Meta Quest, built in Unity.

## Status
| Field | Value |
|---|---|
| Phase | Bootstrap (docs done, Unity project pending manual creation) |
| Engine | **Unity 6 LTS** + C# + URP |
| XR Stack | **OpenXR** + XR Interaction Toolkit (+ Meta XR SDK Core) |
| Target Headset | **Meta Quest** (PCVR via Link first, Standalone later) |
| Backend | None (single-player) |

## Quick Start
> Full setup: [INSTALLATION.md](INSTALLATION.md) · Architecture: [Architecture.md](Architecture.md)

1. Install Unity Hub + Unity 6 LTS (with Android Build Support).
2. Create the Unity project once via Unity Hub at `unity/VRtasia/` — see [INSTALLATION.md §4](INSTALLATION.md).
3. Ask the coding agent to run the **XR setup pass** (see [docs/implementation-plan.md](docs/implementation-plan.md) → *XR Setup Checklist*).
4. Open `Assets/Scenes/HelloVR.unity`, put the headset on, press **Play**.

### PyCharm helpers
| ▶ Configuration | Action |
|---|---|
| `Open Docs` | Serves the project docs locally on port 8080 |

Unity itself is launched from Unity Hub, not PyCharm.

## Repository Layout
```
VRtasia/
├── unity/VRtasia/             # Unity project (created manually, see INSTALLATION.md)
├── docs/
│   └── implementation-plan.md
├── .github/
│   ├── copilot-instructions.md
│   └── instructions/
├── .run/                      # PyCharm utility run configurations
├── Architecture.md
├── INSTALLATION.md
├── ideas.md
└── README.md
```

## Documentation
| Document | Purpose |
|---|---|
| [Architecture.md](Architecture.md) | Engine + XR decisions, comfort/perf targets, pitfalls |
| [docs/implementation-plan.md](docs/implementation-plan.md) | Roadmap, backlog, design discussions, XR setup checklist |
| [INSTALLATION.md](INSTALLATION.md) | Step-by-step environment setup |
| [ideas.md](ideas.md) | Raw brainstorm backlog |
| [.github/copilot-instructions.md](.github/copilot-instructions.md) | AI coding guidance |

## Current Focus
- [ ] Create the empty Unity project at `unity/VRtasia/` (manual, one-time)
- [ ] Run the XR setup pass (agent)
- [ ] Verify Hello-World scene in headset (grab cube + teleport)
- [ ] Decide D1 (locomotion) — see implementation plan
