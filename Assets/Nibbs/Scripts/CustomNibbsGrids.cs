using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class EventIn_CreateNibbsGrid : EventSystem<NibbsGrid> { }
    internal enum NibbsGrid
    {
        SetCurrentSingleColor = 0,
        SetCurrentOneColumnDifferent = 1,
        SetCurrentTestClosingGaps = 2,
    }
    internal class CustomNibbsGrids
    {
        internal static EventIn_CreateNibbsGrid EventIn_CreateNibbsGrid = new EventIn_CreateNibbsGrid();

        internal void Init()
        {
            EventIn_CreateNibbsGrid.AddListener(CreateNibbsGrid);
        }

        private void CreateNibbsGrid(NibbsGrid grid)
        {
            switch (grid)
            {
                case NibbsGrid.SetCurrentSingleColor: SetCurrentSingleColor(); break;
                case NibbsGrid.SetCurrentOneColumnDifferent: SetCurrentOneColumnDifferent(); break;
                case NibbsGrid.SetCurrentTestClosingGaps: SetCurrentTestClosingGaps(); break;
            }
        }

        private void SetCurrentSingleColor()
        {
            List<List<int>> nibbsGrid = NibbsHandler.VarOut_GetNibbsGrid();
            for (int i = 0; i < nibbsGrid.Count; i++)
            {
                for (int j = 0; j < nibbsGrid[i].Count; j++)
                {
                    NibbsHandler.EventIn_SetNibbState.Invoke(i, j, 1, true);
                }
            }
        }

        private void SetCurrentOneColumnDifferent()
        {
            List<List<int>> nibbsGrid = NibbsHandler.VarOut_GetNibbsGrid();
            for (int i = 0; i < nibbsGrid.Count; i++)
            {
                for (int j = 0; j < nibbsGrid[i].Count; j++)
                {
                    NibbsHandler.EventIn_SetNibbState.Invoke(i, j, (j == 1) ? 1 : 2, true);
                }
            }
        }

        private void SetCurrentTestClosingGaps()
        {
            List<List<int>> nibbsGrid = NibbsHandler.VarOut_GetNibbsGrid();
            for (int i = 0; i < nibbsGrid.Count; i++)
            {
                for (int j = 0; j < nibbsGrid[i].Count; j++)
                {
                    NibbsHandler.EventIn_SetNibbState.Invoke(i, j, ((j == 1) || (j == 4) || (j == 7)) ? 1 : 2, true);
                }
            }
        }
    }
}