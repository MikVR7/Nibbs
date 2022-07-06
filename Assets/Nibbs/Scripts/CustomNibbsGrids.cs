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
        SetCurrentTestClosingGaps2 = 3,
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
                case NibbsGrid.SetCurrentTestClosingGaps2: SetCurrentTestClosingGaps2(); break;
            }
        }

        private void SetCurrentSingleColor()
        {
            // TODO: 
            //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
            //for (int i = 0; i < nibbsGrid.Count; i++)
            //{
            //    for (int j = 0; j < nibbsGrid[i].Count; j++)
            //    {
            //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, 1, true);
            //    }
            //}
        }

        private void SetCurrentOneColumnDifferent()
        {
            // TODO: 
            //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
            //for (int i = 0; i < nibbsGrid.Count; i++)
            //{
            //    for (int j = 0; j < nibbsGrid[i].Count; j++)
            //    {
            //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, (j == 1) ? 1 : 2, true);
            //    }
            //}
        }

        private void SetCurrentTestClosingGaps()
        {
            // TODO: 
            //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
            //for (int i = 0; i < nibbsGrid.Count; i++)
            //{
            //    for (int j = 0; j < nibbsGrid[i].Count; j++)
            //    {
            //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, ((j == 1) || (j == 4) || (j == 7)) ? 1 : 2, true);
            //    }
            //}
        }

        private void SetCurrentTestClosingGaps2()
        {
            // TODO: 
            //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
            //for (int i = 0; i < nibbsGrid.Count; i++)
            //{
            //    for (int j = 0; j < nibbsGrid[i].Count; j++)
            //    {
            //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, (
            //            (j == 0) ||
            //            (j == 1) ||
            //            (j == 2) ||
            //            (j == 3) ||
            //            (j == 4) ||

            //            (j == 7) ||
            //            (j == 8) ||
            //            (j == 9) ||

            //            (j == 14) ||
            //            (j == 15) ||
            //            (j == 16) ||
            //            (j == 17) ||
            //            (j == 18) ||
            //            (j == 19)
            //        ) ? 1 : 2, true);
            //    }
            //}
        }
    }
}