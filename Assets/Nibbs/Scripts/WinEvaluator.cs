using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class WinEvaluator
    {
        internal EventIn_EvaluateClickGroup EventIn_EvaluateClickGroup = new EventIn_EvaluateClickGroup();
        private int pointsCount = 0;

        internal void Init()
        {
            EventIn_EvaluateClickGroup.AddListener(EvaluateClickGroup);
        }

        internal void EvaluateClickGroup(List<KeyValuePair<int, int>> nibbsToDestroy)
        {
            if (nibbsToDestroy.Count > 1)
            {
                // collect columns to fall
                List<int> columnsToFall = new List<int>();
                nibbsToDestroy.ForEach(i => {
                    if (!columnsToFall.Contains(i.Value))
                    {
                        columnsToFall.Add(i.Value);
                    }
                    //NibbsGameHandler.EventIn_DisableNibb.Invoke(i);
                });
                //NibbsGameHandler.EventIn_LetColumnsFall.Invoke(columnsToFall);
                pointsCount += (nibbsToDestroy.Count * nibbsToDestroy.Count);
            }
        }
    }
}