using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class CamerasHandler : MonoBehaviour
    {
        [SerializeField] private GameObject goXRSystem = null;
        [SerializeField] private Camera camVR = null;
        [SerializeField] private Camera camNonVR = null;
        [SerializeField] private Canvas gameCanvas = null;
        internal static Camera VarOut_CurrentMainCamera { get; private set; } = null;

        internal void Init()
        {
            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);

            camNonVR.gameObject.SetActive(inputDevices.Count == 0);
            goXRSystem.SetActive(inputDevices.Count > 0);

            VarOut_CurrentMainCamera = (inputDevices.Count == 0) ? this.camNonVR : this.camVR;
            this.gameCanvas.worldCamera = VarOut_CurrentMainCamera;
            Debug.Log("INPUT DEVICES: " + inputDevices.Count);
        }
    }
}
