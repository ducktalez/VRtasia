# Copilot Instructions — VRtasia
> Keep this file **short** — only what cannot be discovered from source code.
> Full details: `docs/Architecture.md` · tasks: `docs/implementation-plan.md`
## What is this?
VRtasia is a VR game project. The current phase is **concept and prototype definition** — the engine and platform are not yet fixed. The immediate goal is a narrow, technically credible prototype, not a full game platform.
## Development Phase Policy
The repository is in the **exploratory setup phase**. Do not scaffold broadly.
- Prefer technical spikes and validated decisions over full game scaffolding
- Do not assume a final engine without an explicit task saying so
- OpenXR-compatible direction is preferred to keep platform choices open
- **Backend, online features, multiplayer, analytics**: deferred unless they are core to the prototype
- Mark speculative decisions with `# TODO: decide`
## Architecture Direction
Until changed by an explicit decision:
- **Engine**: TBD (Unity/C# or Godot/GDScript/C# are the primary candidates)
- **XR Runtime**: OpenXR-compatible preferred; keep Unity-specific / SteamVR-specific code behind abstraction layers
- **Scripting**: depends on engine choice
- **Backend**: none by default — add only if gameplay requires it
- See `.github/instructions/docs.instructions.md` for documentation conventions
## Prototype Guidance
- One narrow interaction loop first
- Make that loop testable and observable
- Capture comfort, locomotion, and framerate constraints early in `docs/Architecture.md`
- Separate gameplay ideas (`ideas.md`) from planned work (`docs/implementation-plan.md`)
## Working Behaviour
- **Proactive code review**: Report any **bugs**, **code smells**, or **questionable patterns** found along the way — even if unrelated to the current task. Include a brief suggestion.
- **Don't silently fix ambiguous findings**: Only fix if **unambiguously wrong** (missing import, typo, off-by-one). If intent is unclear or a comment suggests ongoing work — **ask first** or add a `# TODO`.
- **Open tasks → implementation plan**: Add TODOs to `docs/implementation-plan.md`, not to source code.
- **Design discussions → implementation plan**: Architecture and engine questions go into the **Design Discussions** section of `docs/implementation-plan.md`.
- **Raise concerns**: If an approach seems risky, fragile, or architecturally wrong — voice it explicitly before implementing.
## Maintaining These Instructions
- New engine decision, platform choice, or interaction convention → update `docs/Architecture.md` and this file
- New tasks → add to `docs/implementation-plan.md`
- New pitfalls → add to `docs/Architecture.md` (Pitfalls section)
- Keep instructions **lean**
