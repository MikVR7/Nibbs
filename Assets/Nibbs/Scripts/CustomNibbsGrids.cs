using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal enum NibbsGrid
    {
    }
    internal class CustomNibbsGrids
    {
        
        internal void Init()
        {
        }

        internal static List<List<int>> VarOut_GetCustomNibbsGrid(int gridType, int columnsCount, int nibbsCount)
        {
            Debug.Log("COLUMNS: " + columnsCount + " " + nibbsCount);
            List<List<int>> result = new List<List<int>>();
            if(gridType == 0)
            {
               result = GetQuadraticGrid(columnsCount, nibbsCount, 2);
            }
            return result;
        }

        private static List<List<int>> GetQuadraticGrid(int columnsCount, int nibbsCount, int quadraticGridCount)
        {
            List<List<int>> result = new List<List<int>>();
            List<NibbColor> colors = LevelsHandler.VarOut_Level.VarOut_GetLevel().NibbsColors;
            int currentColor = 0;
            for (int i = 0; i < columnsCount; i += quadraticGridCount)
            {
                List<int> column = new List<int>();
                for (int j = 0; j < nibbsCount; j += quadraticGridCount)
                {
                    for (int k = 0; k < quadraticGridCount; k++)
                    {
                        column.Add((int)colors[currentColor]);
                    }
                    currentColor = (currentColor + 1) % colors.Count;
                }
                //currentColor = (currentColor + 1) % colors.Count;
                for (int k = 0; k < quadraticGridCount; k++)
                {
                    result.Add(column);
                }
            }
            return result;
        }

        //private void CreateNibbsGrid(NibbsGrid grid)
        //{
        //    switch (grid)
        //    {
        //        case NibbsGrid.SetCurrentSingleColor: SetCurrentSingleColor(); break;
        //        case NibbsGrid.SetCurrentOneColumnDifferent: SetCurrentOneColumnDifferent(); break;
        //        case NibbsGrid.SetCurrentTestClosingGaps: SetCurrentTestClosingGaps(); break;
        //        case NibbsGrid.SetCurrentTestClosingGaps2: SetCurrentTestClosingGaps2(); break;
        //    }
        //}

        //private void SetCurrentSingleColor()
        //{
        //    // TODO: 
        //    //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
        //    //for (int i = 0; i < nibbsGrid.Count; i++)
        //    //{
        //    //    for (int j = 0; j < nibbsGrid[i].Count; j++)
        //    //    {
        //    //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, 1, true);
        //    //    }
        //    //}
        //}

        //private void SetCurrentOneColumnDifferent()
        //{
        //    // TODO: 
        //    //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
        //    //for (int i = 0; i < nibbsGrid.Count; i++)
        //    //{
        //    //    for (int j = 0; j < nibbsGrid[i].Count; j++)
        //    //    {
        //    //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, (j == 1) ? 1 : 2, true);
        //    //    }
        //    //}
        //}

        //private void SetCurrentTestClosingGaps()
        //{
        //    // TODO: 
        //    //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
        //    //for (int i = 0; i < nibbsGrid.Count; i++)
        //    //{
        //    //    for (int j = 0; j < nibbsGrid[i].Count; j++)
        //    //    {
        //    //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, ((j == 1) || (j == 4) || (j == 7)) ? 1 : 2, true);
        //    //    }
        //    //}
        //}

        //private void SetCurrentTestClosingGaps2()
        //{
        //    // TODO: 
        //    //List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
        //    //for (int i = 0; i < nibbsGrid.Count; i++)
        //    //{
        //    //    for (int j = 0; j < nibbsGrid[i].Count; j++)
        //    //    {
        //    //        NibbsGameHandler.EventIn_SetNibbState.Invoke(i, j, (
        //    //            (j == 0) ||
        //    //            (j == 1) ||
        //    //            (j == 2) ||
        //    //            (j == 3) ||
        //    //            (j == 4) ||

        //    //            (j == 7) ||
        //    //            (j == 8) ||
        //    //            (j == 9) ||

        //    //            (j == 14) ||
        //    //            (j == 15) ||
        //    //            (j == 16) ||
        //    //            (j == 17) ||
        //    //            (j == 18) ||
        //    //            (j == 19)
        //    //        ) ? 1 : 2, true);
        //    //    }
        //    //}
        //}
    }
}