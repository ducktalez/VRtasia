using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace VRtasia.Editor
{
    /// <summary>
    /// One-time XR bootstrap. Runs automatically on every Editor start via [InitializeOnLoad],
    /// but each step is guarded so it only executes once.
    ///
    /// Flow (two domain-reload cycles):
    ///   Cycle 1 — Imports XRI "Starter Assets" sample  → triggers domain reload
    ///   Cycle 2 — Creates Assets/Scenes/HelloVR.unity
    ///
    /// Re-run manually: menu  VRtasia ▶ Run XR Setup
    /// Reset flags:     menu  VRtasia ▶ Reset XR Setup Flags
    /// </summary>
    [InitializeOnLoad]
    static class XRAutoSetup
    {
        // EditorPrefs keys — survives domain reloads, cleared only via Reset menu
        const string k_SamplesKey  = "VRtasia.XRI.SamplesImported";
        const string k_SceneKey    = "VRtasia.HelloVR.SceneCreated";
        const string k_PkgId       = "com.unity.xr.interaction.toolkit";
        const string k_SampleName  = "Starter Assets";
        const string k_ScenePath   = "Assets/Scenes/HelloVR.unity";

        static XRAutoSetup() => EditorApplication.delayCall += Run;

        // ── Menu items ───────────────────────────────────────────────────────

        [MenuItem("VRtasia/Run XR Setup")]
        static void RunMenu() => Run();

        [MenuItem("VRtasia/Reset XR Setup Flags")]
        static void ResetFlags()
        {
            EditorPrefs.DeleteKey(k_SamplesKey);
            EditorPrefs.DeleteKey(k_SceneKey);
            Debug.Log("[VRtasia Setup] Flags reset. Run 'VRtasia > Run XR Setup' to redo.");
        }

        // ── Main entry point ─────────────────────────────────────────────────

        static void Run()
        {
            // Step 1: ensure XRI Starter Assets sample is present
            if (!EditorPrefs.GetBool(k_SamplesKey, false))
            {
                ImportStarterAssets();
                // CreateHelloVRScene() will execute after the domain reload
                // that is triggered by the sample import.
                return;
            }

            // Step 2: create HelloVR scene if missing
            if (!EditorPrefs.GetBool(k_SceneKey, false) ||
                !File.Exists(Path.Combine(Application.dataPath, "Scenes", "HelloVR.unity")))
            {
                CreateHelloVRScene();
            }
        }

        // ── Step 1: Sample import ─────────────────────────────────────────────

        static void ImportStarterAssets()
        {
            try
            {
                var pkgInfo = PackageInfo.FindForPackageName(k_PkgId);
                if (pkgInfo == null)
                {
                    // Package not downloaded yet — will retry on next domain reload
                    Debug.Log("[VRtasia Setup] XRI package not yet available; retrying after package install.");
                    return;
                }

                var samples = Sample.FindByPackage(pkgInfo.name, pkgInfo.version);
                var starter = samples.FirstOrDefault(s => s.displayName == k_SampleName);

                if (string.IsNullOrEmpty(starter.displayName))
                {
                    Debug.LogWarning($"[VRtasia Setup] Sample '{k_SampleName}' not found in {k_PkgId}@{pkgInfo.version}.");
                    // Proceed to scene creation anyway (will use manual fallback rig)
                    EditorPrefs.SetBool(k_SamplesKey, true);
                    CreateHelloVRScene();
                    return;
                }

                if (starter.isImported)
                {
                    Debug.Log("[VRtasia Setup] XRI Starter Assets already imported.");
                    EditorPrefs.SetBool(k_SamplesKey, true);
                    CreateHelloVRScene();
                    return;
                }

                // Set flag BEFORE import — import triggers a domain reload and
                // code after Import() might not execute.
                EditorPrefs.SetBool(k_SamplesKey, true);
                Debug.Log("[VRtasia Setup] Importing XRI Starter Assets… Unity will recompile.");
                starter.Import(Sample.ImportOptions.HideImportWindow);
                // Domain reload → Run() fires again → Step 2 executes
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[VRtasia Setup] Sample import failed: {e.Message}\n" +
                                 "You can import it manually: Window > Package Manager > " +
                                 "XR Interaction Toolkit > Samples > Starter Assets > Import.");
                // Don't block scene creation
                EditorPrefs.SetBool(k_SamplesKey, true);
            }
        }

        // ── Step 2: HelloVR scene creation ───────────────────────────────────

        static void CreateHelloVRScene()
        {
            try
            {
                Debug.Log("[VRtasia Setup] Creating HelloVR scene…");

                if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                    AssetDatabase.CreateFolder("Assets", "Scenes");

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                // Lighting ────────────────────────────────────────────────────
                var sunGO = new GameObject("Directional Light");
                var sun   = sunGO.AddComponent<Light>();
                sun.type      = LightType.Directional;
                sun.color     = new Color(1f, 0.96f, 0.84f);
                sun.intensity = 1.2f;
                sunGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

                // XR Interaction Manager ──────────────────────────────────────
                var xrimGO = new GameObject("XR Interaction Manager");
                xrimGO.AddComponent<XRInteractionManager>();

                // XR Origin  (use starter-assets prefab when available) ────────
                var originGO = TryInstantiateXROriginPrefab() ?? BuildXROriginFallback();

                // Floor — Teleportation Area ──────────────────────────────────
                var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Floor";
                floor.transform.localScale = new Vector3(3f, 1f, 3f);
                floor.AddComponent<TeleportationArea>();

                // Table ───────────────────────────────────────────────────────
                var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
                table.name = "Table";
                table.transform.position   = new Vector3(0f, 0.4f, 1.5f);
                table.transform.localScale = new Vector3(1f, 0.8f, 0.6f);

                // Grabbable Cube ──────────────────────────────────────────────
                var cubeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeGO.name = "Grab Cube";
                cubeGO.transform.position   = new Vector3(0f, 0.865f, 1.5f);
                cubeGO.transform.localScale = Vector3.one * 0.2f;
                var rb = cubeGO.AddComponent<Rigidbody>();
                rb.mass = 0.5f;
                cubeGO.AddComponent<XRGrabInteractable>();
                cubeGO.AddComponent<HelloVRLogger>();

                // Save ────────────────────────────────────────────────────────
                EditorSceneManager.SaveScene(scene, k_ScenePath);
                EditorPrefs.SetBool(k_SceneKey, true);

                // Register as build scene 0 ───────────────────────────────────
                var buildScenes = EditorBuildSettings.scenes.ToList();
                if (buildScenes.All(s => s.path != k_ScenePath))
                {
                    buildScenes.Insert(0, new EditorBuildSettingsScene(k_ScenePath, true));
                    EditorBuildSettings.scenes = buildScenes.ToArray();
                }

                Debug.Log($"[VRtasia Setup] ✔ HelloVR scene saved → '{k_ScenePath}'.");
                PrintManualNextSteps();
                EditorSceneManager.OpenScene(k_ScenePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRtasia Setup] Scene creation failed: {e}");
            }
        }

        // ── XR Origin helpers ────────────────────────────────────────────────

        /// <summary>
        /// Tries to instantiate the "Complete XR Origin Set Up" prefab from XRI Starter Assets.
        /// Falls back to "XR Origin (XR Rig)" if the full prefab is not found.
        /// </summary>
        static GameObject TryInstantiateXROriginPrefab()
        {
            // Search in Samples folder first to avoid false-positive matches
            string[] searchFolders = { "Assets/Samples" };
            var guids = AssetDatabase.FindAssets("Complete XR Origin Set Up t:Prefab", searchFolders);
            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets("XR Origin (XR Rig) t:Prefab", searchFolders);

            if (guids.Length == 0) return null;

            var path   = AssetDatabase.GUIDToAssetPath(guids[0]);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return null;

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.position = Vector3.zero;
            Debug.Log($"[VRtasia Setup] XR Origin instantiated from prefab: {path}");
            return go;
        }

        /// <summary>
        /// Minimal XR Origin when the Starter Assets prefab is not yet available.
        /// Controllers will be missing — re-run setup after importing Starter Assets.
        /// </summary>
        static GameObject BuildXROriginFallback()
        {
            Debug.LogWarning("[VRtasia Setup] XRI prefab not found — building minimal XR Origin.\n" +
                             "Re-run 'VRtasia > Run XR Setup' after samples are imported for full controller support.");

            var originGO = new GameObject("XR Origin");
            var xrOrigin = originGO.AddComponent<XROrigin>();
            xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;

            var camOffset = new GameObject("Camera Offset");
            camOffset.transform.SetParent(originGO.transform, false);
            xrOrigin.CameraFloorOffsetObject = camOffset;

            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.transform.SetParent(camOffset.transform, false);
            xrOrigin.Camera = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();

            originGO.AddComponent<TeleportationProvider>();

            return originGO;
        }

        // ── Guidance ─────────────────────────────────────────────────────────

        static void PrintManualNextSteps()
        {
            Debug.Log(
                "[VRtasia Setup] ─── 3 REMAINING MANUAL STEPS (< 1 minute) ───\n" +
                "\n" +
                "1. Edit > Project Settings > XR Plug-in Management\n" +
                "      PC tab      → ✓ OpenXR\n" +
                "      Android tab → ✓ OpenXR\n" +
                "\n" +
                "2. Both tabs > OpenXR sub-page > (+) Add Interaction Profile:\n" +
                "      • Oculus Touch Controller Profile         (Quest 2)\n" +
                "      • Meta Quest Touch Pro Controller Profile (Quest 3 / Pro)\n" +
                "\n" +
                "3. Android > OpenXR > Render Mode → Single Pass Instanced\n" +
                "\n" +
                "Then: connect Quest via Link, press Play in HelloVR scene.\n" +
                "─────────────────────────────────────────────────────────────");
        }
    }
}

