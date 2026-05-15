# scripts

Helper scripts for repo bootstrap. Not required for daily development.

| Script | Purpose | When to run |
|---|---|---|
| `bootstrap-unity.ps1` | Re-create the Unity project at `unity/VRtasia/` headlessly using the pinned Editor version | Only if `unity/VRtasia/` is missing or you want a clean reset |

## bootstrap-unity.ps1

Creates the URP-based Unity project at the path expected by all docs.

### Prerequisites
- Unity Hub installed
- Unity Editor **6000.4.7f1** installed via Unity Hub (with Android Build Support module)

### Usage

```powershell
# from repo root, in PowerShell 7+ (pwsh) or Windows PowerShell
./scripts/bootstrap-unity.ps1
```

### Options

| Env var | Default | Purpose |
|---|---|---|
| `UNITY_EDITOR_PATH` | `C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe` | Override Editor path |
| `VRTASIA_UNITY_DIR` | `<repo>/unity/VRtasia` | Override target project directory |

### What it does

1. Resolves Unity Editor path; aborts if not found.
2. Refuses to overwrite a non-empty target directory unless `-Force` is passed.
3. Runs `Unity.exe -batchmode -createProject <path> -quit -nographics` to scaffold a default 3D project.
4. Prints next-step hint pointing to [INSTALLATION.md §5](../INSTALLATION.md) for the XR setup pass.

### Caveats

- Unity's `-createProject` produces a **plain 3D** template, not URP. The repo's committed project was created via Unity Hub with the *Universal 3D* template; if you re-bootstrap, you'll need to convert to URP manually (Window → Package Manager → URP → Install + create render pipeline asset).
- For most users, **don't run this** — just clone and open the existing `unity/VRtasia/` folder.

