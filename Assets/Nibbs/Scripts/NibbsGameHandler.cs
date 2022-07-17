using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsGameHandler : MonoBehaviour
    {
        internal static EventOut_OnUpdate EventOut_OnUpdate = new EventOut_OnUpdate();

        [SerializeField] private LevelsHandler levelsHandler = null;
        [SerializeField] private ControllsHandler controllsHandler = null;
        [SerializeField] private GUIHandler guiHandler = null;

        private CustomNibbsGrids customNibbsGrids = new CustomNibbsGrids();
        
        private void Awake()
        {
            this.levelsHandler.Init();
            this.controllsHandler.Init();
            this.customNibbsGrids.Init();
            this.guiHandler.Init();
        }

        private void Update()
        {
            if (EventOut_OnUpdate.HasListeners())
            {
                EventOut_OnUpdate.Invoke();
            }
            // Controls without LEFT CTRL and LEFT SHIFT are main controls
            if (!Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    LevelsHandler.EventIn_SetupLevel.Invoke(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    LevelsHandler.EventIn_SetupLevel.Invoke(9990);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    LevelsHandler.EventIn_StartLevel.Invoke();
                }
            }
        }
    }
}