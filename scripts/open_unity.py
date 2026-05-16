"""
Launches the Unity Editor with the VRtasia project.

Used by the PyCharm run configuration "Open Unity Editor".
Respects the UNITY_EDITOR_PATH environment variable; falls back to the
default Unity Hub install path for the pinned version (6000.4.7f1).
"""

import os
import pathlib
import subprocess
import sys

UNITY_VERSION = "6000.4.7f1"
DEFAULT_EDITOR = pathlib.Path(
    rf"C:\Program Files\Unity\Hub\Editor\{UNITY_VERSION}\Editor\Unity.exe"
)
PROJECT_DIR = pathlib.Path(__file__).parent.parent / "unity" / "VRtasia"


def find_editor() -> pathlib.Path:
    env_override = os.environ.get("UNITY_EDITOR_PATH")
    if env_override:
        return pathlib.Path(env_override)
    return DEFAULT_EDITOR


def main() -> None:
    editor = find_editor()

    if not editor.exists():
        print(
            f"[open_unity] Unity Editor not found at:\n  {editor}\n\n"
            f"Fix: install Unity {UNITY_VERSION} via Unity Hub, or set the\n"
            f"  UNITY_EDITOR_PATH environment variable to the full Unity.exe path.",
            file=sys.stderr,
        )
        sys.exit(1)

    if not PROJECT_DIR.exists():
        print(
            f"[open_unity] Project directory not found:\n  {PROJECT_DIR}\n\n"
            "Did you clone the full repo and are you running from the repo root?",
            file=sys.stderr,
        )
        sys.exit(1)

    print(f"[open_unity] Launching Unity {UNITY_VERSION}")
    print(f"[open_unity] Project : {PROJECT_DIR}")
    print(f"[open_unity] Editor  : {editor}")

    # Popen (non-blocking) — Unity runs independently; PyCharm terminal stays clean.
    subprocess.Popen(
        [str(editor), "-projectPath", str(PROJECT_DIR)],
        creationflags=subprocess.DETACHED_PROCESS | subprocess.CREATE_NEW_PROCESS_GROUP,
    )

    print("[open_unity] Unity is starting — this takes ~30 s on first open after a Library/ rebuild.")


if __name__ == "__main__":
    main()

