using UnityEngine;

namespace Nibbs
{
    internal class ControllsHandler : MonoBehaviour
    {
        internal void Init()
        {
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