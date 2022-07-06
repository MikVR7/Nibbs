using CodeEvents;
using System.Collections.Generic;

namespace Nibbs
{
    internal class Events
    {
        // NibbsGameHandler.cs
        internal class EventOut_OnUpdate : EventSystem { }

        // Nibb.cs
        internal class EventIn_SetColor : EventSystem<NibbColor> { }
        internal class EventIn_SetNibbState : EventSystem<Nibb.State> { }

        // LevelsHandler.cs | NibbsColumn.cs
        internal class EventIn_DisableNibb : EventSystem<int, int> { }
        internal class EventIn_LetColumnsFall : EventSystem { }

        // LevelsHandler.cs
        internal class EventIn_StartLevel : EventSystem<int> { }

        // NibbsColumn.cs
        internal class EventIn_StartLevelColumn : EventSystem<LevelData> { }

        // WinEvaluator.cs
        internal class EventIn_EvaluateClickGroup : EventSystem<List<KeyValuePair<int, int>>> { }
    }
}