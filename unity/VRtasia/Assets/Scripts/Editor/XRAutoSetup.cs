using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
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
    ///   Cycle 1 — Imports XRI "Starter Assets" + "XR Device Simulator" samples → triggers domain reload
    ///   Cycle 2 — Creates Assets/Scenes/HelloVR.unity (includes XR Device Simulator for no-headset testing)
    ///
    /// Re-run manually: menu  VRtasia ▶ Run XR Setup
    /// Reset flags:     menu  VRtasia ▶ Reset XR Setup Flags
    /// Add simulator:   menu  VRtasia ▶ Add Device Simulator to Scene
    /// </summary>
    [InitializeOnLoad]
    static class XRAutoSetup
    {
        // EditorPrefs keys — survives domain reloads, cleared only via Reset menu
        const string k_SamplesKey  = "VRtasia.XRI.SamplesImported";
        const string k_SceneKey    = "VRtasia.HelloVR.SceneCreated";
        const string k_PkgId       = "com.unity.xr.interaction.toolkit";
        const string k_ScenePath   = "Assets/Scenes/HelloVR.unity";

        // Both samples are imported in a single pass (before domain reload fires)
        static readonly string[] k_RequiredSamples = { "Starter Assets", "XR Device Simulator" };

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

        [MenuItem("VRtasia/Add Device Simulator to Scene")]
        static void AddSimulatorMenu()
        {
            var go = TryInstantiateDeviceSimulatorPrefab();
            if (go != null)
            {
                go.AddComponent<SimulatorEditorOnly>();
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(scene);
                Debug.Log("[VRtasia Setup] ✔ Device Simulator added and scene saved.");
            }
            else
            {
                Debug.LogWarning("[VRtasia Setup] XR Device Simulator prefab not found.\n" +
                                 "Import it via Window > Package Manager > XR Interaction Toolkit > " +
                                 "Samples > XR Device Simulator > Import, then retry.");
            }
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
                    Debug.Log("[VRtasia Setup] XRI package not yet available; retrying after package install.");
                    return;
                }

                var available = Sample.FindByPackage(pkgInfo.name, pkgInfo.version).ToList();
                bool anyImported = false;

                foreach (var sampleName in k_RequiredSamples)
                {
                    var sample = available.FirstOrDefault(s => s.displayName == sampleName);
                    if (string.IsNullOrEmpty(sample.displayName))
                    {
                        Debug.LogWarning($"[VRtasia Setup] Sample '{sampleName}' not found in {k_PkgId}@{pkgInfo.version} — skipping.");
                        continue;
                    }
                    if (sample.isImported) continue;

                    Debug.Log($"[VRtasia Setup] Importing '{sampleName}'…");
                    sample.Import(Sample.ImportOptions.HideImportWindow);
                    anyImported = true;
                }

                // Set flag before the domain reload triggered by Import() fires.
                EditorPrefs.SetBool(k_SamplesKey, true);

                if (!anyImported)
                {
                    // All samples already present — go straight to scene creation.
                    Debug.Log("[VRtasia Setup] XRI samples already imported.");
                    CreateHelloVRScene();
                }
                // else: domain reload → Run() fires again → k_SamplesKey == true → CreateHelloVRScene()
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[VRtasia Setup] Sample import error: {e.Message}\n" +
                                 "Import manually: Window > Package Manager > XR Interaction Toolkit > Samples.");
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

                // XR Device Simulator (Editor-only — disabled in builds) ────────
                var simGO = TryInstantiateDeviceSimulatorPrefab();
                if (simGO != null)
                    simGO.AddComponent<SimulatorEditorOnly>();
                else
                    Debug.LogWarning("[VRtasia Setup] XR Device Simulator prefab not found. " +
                                     "Add it later via VRtasia > Add Device Simulator to Scene.");

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
        /// Finds and instantiates the XR Device Simulator prefab from the XRI sample.
        /// Returns null if the sample has not been imported yet.
        /// </summary>
        static GameObject TryInstantiateDeviceSimulatorPrefab()
        {
            var guids = AssetDatabase.FindAssets("XR Device Simulator t:Prefab", new[] { "Assets/Samples" });
            if (guids.Length == 0) return null;

            var path   = AssetDatabase.GUIDToAssetPath(guids[0]);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return null;

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Debug.Log($"[VRtasia Setup] XR Device Simulator instantiated from: {path}");
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

