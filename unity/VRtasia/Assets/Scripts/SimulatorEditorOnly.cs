using UnityEngine;

namespace VRtasia
{
    /// <summary>
    /// Disables this GameObject in non-Editor builds.
    /// Attached automatically to the XR Device Simulator root by XRAutoSetup
    /// so that the simulator is never included in shipped builds.
    /// </summary>
    public class SimulatorEditorOnly : MonoBehaviour
    {
        void Awake()
        {
#if !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }
    }
}

