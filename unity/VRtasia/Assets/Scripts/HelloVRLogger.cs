using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRtasia
{
    /// <summary>
    /// Logs grab and release events to the Console.
    /// Attach to any XRGrabInteractable to verify the XR interaction system is working.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class HelloVRLogger : MonoBehaviour
    {
        XRGrabInteractable _grab;

        void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _grab.selectEntered.AddListener(OnGrabbed);
            _grab.selectExited.AddListener(OnReleased);
        }

        void OnDestroy()
        {
            if (_grab == null) return;
            _grab.selectEntered.RemoveListener(OnGrabbed);
            _grab.selectExited.RemoveListener(OnReleased);
        }

        void OnGrabbed(SelectEnterEventArgs args)
        {
            Debug.Log($"[HelloVR] ✔ Grabbed '{gameObject.name}' " +
                      $"by '{args.interactorObject.transform.gameObject.name}'");
        }

        void OnReleased(SelectExitEventArgs args)
        {
            Debug.Log($"[HelloVR] ✖ Released '{gameObject.name}' " +
                      $"by '{args.interactorObject.transform.gameObject.name}'");
        }
    }
}

