using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsGameHandler : MonoBehaviour
    {
        internal static EventOut_OnUpdate EventOut_OnUpdate = new EventOut_OnUpdate();

        private static NibbsGameHandler Instance = null;

        [SerializeField] private LevelsHandler levelsHandler = null;
        [SerializeField] private ControllsHandler controllsHandler = null;
        [SerializeField] private GUIHandler guiHandler = null;
        [SerializeField] private Transform tCamera = null;
        [SerializeField] private EffectsHandler effectsHandler = null;
        [SerializeField] private CamerasHandler camerasHandler = null;

        private CustomNibbsGrids customNibbsGrids = new CustomNibbsGrids();
        
        private void Awake()
        {
            Instance = this;
            this.levelsHandler.Init();
            this.controllsHandler.Init();
            this.customNibbsGrids.Init();
            this.guiHandler.Init();
            this.effectsHandler.Init();
            this.camerasHandler.Init();
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

        internal static float VarOut_GetMainCamRotationY()
        {
            return Instance.tCamera.eulerAngles.y;
        }
    }
}