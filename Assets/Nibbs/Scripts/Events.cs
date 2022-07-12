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
        internal class EventIn_SetNibbPosY : EventSystem<float> { }
        internal class EventIn_SetNibbIndex : EventSystem<int> { }
        internal class EventOut_NibbFinishedFalling : EventSystem<int> { }

        // LevelsHandler.cs | NibbsColumn.cs
        internal class EventIn_DestroyNibb : EventSystem<int, int> { }
        internal class EventIn_StartLevel : EventSystem { }
        internal class EventIn_LetColumnsFall : EventSystem <List<int>> { }

        // LevelsHandler.cs
        internal class EventIn_SetupLevel : EventSystem<int> { }

        // NibbsColumn.cs
        internal class EventIn_SetNibbsTypes : EventSystem { }
        internal class EventOut_ColumnStateUpdated : EventSystem { }
        internal class EventIn_SetColumnIndex : EventSystem<int> { }

        // WinEvaluator.cs
        internal class EventIn_EvaluateClickGroup : EventSystem<List<KeyValuePair<int, int>>> { }

        // ColumnShifter.cs
        internal class EventIn_RotateColumns : EventSystem<List<ColumnShiftInstance>, float> { }
        internal class EventOut_ColumnShiftingDone : EventSystem { }
    }
}