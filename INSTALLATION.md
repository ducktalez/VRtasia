# Installation

> Audience: developers new to Unity and VR. Each step is explicit.
> Engine baseline: see [Architecture.md](Architecture.md).

## 1. Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| Windows 10/11 | 64-bit | Required for Quest Link |
| Git | latest | Repo clone |
| Unity Hub | latest | Manages Unity installs and projects |
| Unity Editor | **6000.4.7f1** (exact, pinned) | Engine — see `unity/VRtasia/ProjectSettings/ProjectVersion.txt` |
| Meta Quest Link / Meta Horizon app | latest | PCVR streaming to headset |
| (optional) Meta Quest Developer Hub | latest | Standalone APK deployment + logs |

### Required Unity Modules
When installing the Editor via Unity Hub, **check these modules**:
- ✅ Microsoft Visual Studio Community (or use Rider / VS Code separately)
- ✅ Android Build Support
  - ✅ OpenJDK
  - ✅ Android SDK & NDK Tools
- ✅ Windows Build Support (IL2CPP)
- ✅ Documentation (recommended)

## 2. Headset Setup (Meta Quest)

1. Install **Meta Horizon** desktop app on Windows.
2. Sign in with the Meta account linked to the headset.
3. Connect the headset via USB-C (Link cable) or Air Link (Wi-Fi 6).
4. In the headset: Settings → System → Quest Link → Launch Quest Link.
5. Verify the Meta Horizon app shows the headset as **Connected**.

## 3. Clone the Repo

```powershell
git clone <repo-url> VRtasia
cd VRtasia
```

## 4. Get the Unity Project

The Unity project is **already committed** under `unity/VRtasia/`. After cloning the repo, just open it from Unity Hub:

1. Open **Unity Hub** → tab *Projects* → **Add → Add project from disk**.
2. Select the folder `…/VRtasia/unity/VRtasia/`.
3. Unity Hub validates the version (must be **6000.4.7f1**); if missing, install it via **Installs → Install Editor → Archive → Download Archive**.
4. Click the project entry to open it. First import takes several minutes (Library/ rebuild).

### Variant: re-create the project from scratch (only if the unity/ folder is missing)

Use the bootstrap script:

```powershell
pwsh ./scripts/bootstrap-unity.ps1
```

The script invokes Unity headlessly (`-batchmode -createProject`) at the pinned version. It assumes Unity 6000.4.7f1 is installed via Unity Hub at the default path; override with `$env:UNITY_EDITOR_PATH` if needed. See [scripts/README.md](scripts/README.md).

## 5. XR Auto-Setup (runs automatically on first Editor open)

The XR packages are already in `Packages/manifest.json` and an Editor bootstrap script is committed. When you open the project for the first time:

1. Unity downloads the three XR packages (`com.unity.xr.management`, `com.unity.xr.openxr`, `com.unity.xr.interaction.toolkit`). This can take 2–5 minutes on first open.
2. After compilation, the script `Assets/Scripts/Editor/XRAutoSetup.cs` runs automatically.
3. It imports the **XRI Starter Assets** sample (triggers one more recompile — normal).
4. On the second recompile it creates `Assets/Scenes/HelloVR.unity` and opens it.
5. Check the Console for a message starting with `[VRtasia Setup] ✔ HelloVR scene saved`.

> **If nothing happens**: use menu **VRtasia → Run XR Setup** to trigger it manually.
> **To redo from scratch**: **VRtasia → Reset XR Setup Flags**, then **VRtasia → Run XR Setup**.

### Remaining manual XR Management steps (< 1 minute, three UI clicks)

These cannot be reliably automated and must be done once per machine:

1. **Edit → Project Settings → XR Plug-in Management**
   - **PC tab**: check ✅ **OpenXR**
   - **Android tab**: check ✅ **OpenXR**
2. **Both tabs** → click the **OpenXR** row → **(+) Add Interaction Profile**:
   - `Oculus Touch Controller Profile` (Quest 2)
   - `Meta Quest Touch Pro Controller Profile` (Quest 3 / Pro)
3. **Android → OpenXR → Rendering** → set **Render Mode: Single Pass Instanced**

## 6. Run the Hello-World

1. Put the headset on, ensure **Quest Link** is active.
2. In Unity, open `Assets/Scenes/HelloVR.unity`.
3. Press **Play**. The view should appear in-headset within a few seconds.
4. Look around, point controllers, grab the cube on the table, teleport.
5. The Console should log grab events from `HelloVRLogger`.

## 7. Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| Black screen in headset on Play | OpenXR runtime not Meta | Meta Horizon → Settings → General → Set as active OpenXR runtime |
| Player spawns underground | Tracking origin = Device | Set XR Origin → Tracking Origin Mode = **Floor** |
| Stutter / judder | Frame budget exceeded | Stats window → check Main Thread / GPU; lower URP quality |
| Controllers invisible | Sample assets not imported | Window → Package Manager → XR Interaction Toolkit → Samples → import *Starter Assets* |
| Input not working | Mixed action-based + device-based | Use only the action-based rig from the Starter Assets |

