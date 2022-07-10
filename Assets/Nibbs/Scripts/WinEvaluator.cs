using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class WinEvaluator
    {
        internal static EventIn_EvaluateClickGroup EventIn_EvaluateClickGroup = new EventIn_EvaluateClickGroup();
        private int pointsCount = 0;

        internal void Init()
        {
            EventIn_EvaluateClickGroup.AddListener(EvaluateClickGroup);
        }

        private void EvaluateClickGroup(List<KeyValuePair<int, int>> nibbsToDestroy)
        {
            Debug.Log("Nibbs to destroy: " + nibbsToDestroy.Count);
            if (nibbsToDestroy.Count > 1)
            {
                // collect columns to fall
                List<int> columnsToFall = new List<int>();
                nibbsToDestroy.ForEach(i => {
                    if (!columnsToFall.Contains(i.Key))
                    {
                        columnsToFall.Add(i.Key);
                    }
                    LevelsHandler.EventIn_DestroyNibb.Invoke(i.Key, i.Value);
                });
                LevelsHandler.EventIn_LetColumnsFall.Invoke(columnsToFall);
                pointsCount += (nibbsToDestroy.Count * nibbsToDestroy.Count);
            }
        }
    }
}