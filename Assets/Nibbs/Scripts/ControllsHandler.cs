using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
            NibbsGameHandler.EventOut_OnUpdate.AddListenerSingle(OnUpdate);
        }

        private void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.J))
            {

            }

            if (Input.GetMouseButtonDown(0))
            {
                Camera cam = CamerasHandler.VarOut_CurrentMainCamera;
                if (cam != null)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.transform.tag == "NibbsCharacter")
                    {
                        Nibb nibb = hitInfo.transform.GetComponent<Nibb>();
                        if(nibb != null)
                        {
                            nibb.OnClickNibb(null);
                        }
                    }
                }
            }
        }
    }
}