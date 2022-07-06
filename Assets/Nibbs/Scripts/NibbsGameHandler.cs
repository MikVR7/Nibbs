using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsGameHandler : MonoBehaviour
    {
        internal static EventOut_OnUpdate EventOut_OnUpdate = new EventOut_OnUpdate();

        [SerializeField] private LevelsHandler levelsHandler = null;
        
        private CustomNibbsGrids customNibbsGrids = new CustomNibbsGrids();
        
        private void Awake()
        {
            this.levelsHandler.Init();
            this.customNibbsGrids.Init();
        }

        private void Update()
        {
            if (EventOut_OnUpdate.HasListeners())
            {
                EventOut_OnUpdate.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                LevelsHandler.EventIn_StartLevel.Invoke(1);
            }
        }
    }
}