using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class LevelsHandler : MonoBehaviour
    {
        internal static EventIn_StartLevel EventIn_StartLevel = new EventIn_StartLevel();
        internal static EventIn_DisableNibb EventIn_DisableNibb = new EventIn_DisableNibb();
        internal static EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();

        private static LevelsHandler Instance = null;
        [SerializeField] private GameObject prefabNibbsColumn = null;
        private Level currentLevel = new Level();
        private Transform myTransform = null;
        private List<NibbsColumn> nibbsColumns = new List<NibbsColumn>();
        private WinEvaluator winEvaluator = new WinEvaluator();

        internal void Init()
        {
            Instance = this;
            this.myTransform = this.GetComponent<Transform>();
            EventIn_DisableNibb.AddListener(DisableNibb);
            EventIn_StartLevel.AddListener(StartLevel);
            EventIn_LetColumnsFall.AddListener(LetColumnsFall);
            this.winEvaluator.Init();
        }

        private void StartLevel(int levelNr) {
            LevelData level = this.currentLevel.VarOut_GetLevel(levelNr);
            this.CleanupPreviousLevel();
            this.CreateNibbsColumns(level);
        }

        private void CleanupPreviousLevel()
        {
            for (int i = this.nibbsColumns.Count - 1; i >= 0; i--) {
                Destroy(nibbsColumns[i].gameObject);
            }
            nibbsColumns.Clear();
        }

        private void CreateNibbsColumns(LevelData level)
        {
            this.nibbsColumns.Clear();
            for (int i = 0; i < level.ColumnCount; i++)
            {
                GameObject goColumn = Instantiate(this.prefabNibbsColumn);
                NibbsColumn nibbsColumn = goColumn.GetComponent<NibbsColumn>();
                nibbsColumn.Init(i, this.myTransform, level);// level.DefaultColumnHeight, angle, level.LevelRadius, level.NibbDefaultScaling);
                this.nibbsColumns.Add(nibbsColumn);
                nibbsColumn.EventIn_StartLevelColumn.Invoke(level);
            }
        }

        private void DisableNibb(int columnNr, int indexInColumn)
        {
            nibbsColumns[columnNr].EventIn_DisableNibb.Invoke(columnNr, indexInColumn);
        }

        private void LetColumnsFall()
        {
            this.nibbsColumns.ForEach(i => i.EventIn_LetColumnsFall.Invoke());
        }

        internal static List<List<Nibb.State>> VarOut_GetNibbsStatesGrid()
        {
            List<List<Nibb.State>> states = new List<List<Nibb.State>>();
            Instance.nibbsColumns.ForEach(i => states.Add(i.VarOut_GetNibbsStatesGrid()));
            return states;
        }
    }
}