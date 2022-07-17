using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Nibbs
{
    internal class ControllsHandler : MonoBehaviour
    {
        private static ControllsHandler Instance = null;
        internal static XRInteractionManager VarOut_XRInteractionManager() { return Instance.XRInteractionManager; }
        [SerializeField] private XRInteractionManager XRInteractionManager { get; set; } = null;


        internal void Init()
        {
            Instance = this;
            NibbsGameHandler.EventOut_OnUpdate.AddListener(OnUpdate);
        }

        private void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.J))
            {

            }
        }
    }
}