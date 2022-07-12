using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class LevelsHandler : SerializedMonoBehaviour
    {
        internal static EventIn_SetupLevel EventIn_SetupLevel = new EventIn_SetupLevel();
        internal static EventIn_StartLevel EventIn_StartLevel = new EventIn_StartLevel();
        internal static EventIn_DestroyNibb EventIn_DestroyNibb = new EventIn_DestroyNibb();
        internal static EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        
        private static LevelsHandler Instance = null;
        [SerializeField] private GameObject prefabNibbsColumn = null;
        internal static Level VarOut_Level { get; private set; } = new Level();
        [SerializeField] internal static bool VarOut_NibbsAreFalling { get; private set; } = false;
        private Transform myTransform = null;
        private List<NibbsColumn> nibbsColumns = new List<NibbsColumn>();
        private List<NibbsColumn> nibbsColumnsEmpty = new List<NibbsColumn>();
        private WinEvaluator winEvaluator = new WinEvaluator();
        [SerializeField] private ColumnShifter columnShifter = null;

        internal void Init()
        {
            Instance = this;
            this.myTransform = this.GetComponent<Transform>();
            EventIn_DestroyNibb.AddListener(DestroyNibb);
            EventIn_SetupLevel.AddListener(SetupLevel);
            EventIn_StartLevel.AddListener(StartLevel);
            EventIn_LetColumnsFall.AddListener(LetColumnsFall);
            VarOut_Level.Init();
            this.winEvaluator.Init();
            this.columnShifter.Init();
        }

        private void SetupLevel(int levelNr) {
            VarOut_Level.EventIn_SetupLevel.Invoke(levelNr);
            this.CleanupPreviousLevel();
            this.CreateNibbsColumns();
            this.SetNibbsTypes();
        }

        private void StartLevel()
        {
            this.nibbsColumns.ForEach(i => i.EventIn_StartLevel.Invoke());
        }

        private void CleanupPreviousLevel()
        {
            for (int i = this.nibbsColumns.Count - 1; i >= 0; i--) {
                Destroy(nibbsColumns[i].gameObject);
            }
            for (int i = this.nibbsColumnsEmpty.Count - 1; i >= 0; i--)
            {
                Destroy(nibbsColumnsEmpty[i].gameObject);
            }
            nibbsColumns.Clear();
            nibbsColumnsEmpty.Clear();
        }

        private void CreateNibbsColumns()
        {
            this.nibbsColumns.Clear();
            for (int i = 0; i < VarOut_Level.VarOut_GetLevel().ColumnCount; i++)
            {
                GameObject goColumn = Instantiate(this.prefabNibbsColumn);
                NibbsColumn nibbsColumn = goColumn.GetComponent<NibbsColumn>();
                nibbsColumn.Init(i, this.myTransform);
                this.nibbsColumns.Add(nibbsColumn);
            }
        }

        private void SetNibbsTypes()
        {
            for (int i = 0; i < this.nibbsColumns.Count; i++)
            {
                this.nibbsColumns[i].EventIn_SetNibbsTypes.Invoke();
            }
        }

        private void DestroyNibb(int columnNr, int indexInColumn)
        {
            nibbsColumns[columnNr].EventIn_DestroyNibb.Invoke(columnNr, indexInColumn);
        }

        private void LetColumnsFall(List<int> columnIndices)
        {
            VarOut_NibbsAreFalling = true;
            for(int i = 0; i < columnIndices.Count; i++)
            {
                this.nibbsColumns[columnIndices[i]].EventOut_ColumnStateUpdated.AddListener(ColumnStateUpdated);
                this.nibbsColumns[columnIndices[i]].EventIn_LetColumnsFall.Invoke(columnIndices);
            }
        }

        private void ColumnStateUpdated()
        {
            bool columnsAreReadyForNextState = true;
            for(int i = 0; i < this.nibbsColumns.Count; i++)
            {
                if(this.nibbsColumns[i].VarOut_HasFallingNibbs) {
                    columnsAreReadyForNextState = false; break;
                }
            }
            if (columnsAreReadyForNextState) {
                VarOut_NibbsAreFalling = false;
                this.RotateColumns();
            }
        }

        private void RotateColumns()
        {
            if (!this.columnShifter.VarOut_IsPerformingRotationAnimation)
            {
                List<ColumnShiftInstance> shiftInstances = new List<ColumnShiftInstance>();
                this.nibbsColumns.ForEach(i => shiftInstances.Add(new ColumnShiftInstance()
                {
                    ColumnHasNibbs = i.VarOut_ColumnHasActiveNibbs,
                    TColumn = i.VarOut_MyTransform

                }));
                this.columnShifter.EventOut_ColumnShiftingDone.AddListenerSingle(ColumnShiftingDone);
                this.columnShifter.EventIn_RotateColumns.Invoke(shiftInstances, 360f / LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnCount);
            }
        }

        private void ColumnShiftingDone()
        {
            this.columnShifter.EventOut_ColumnShiftingDone.RemoveListener(ColumnShiftingDone);
            for(int i = this.nibbsColumns.Count-1; i >= 0; i--) {
                if (!this.nibbsColumns[i].VarOut_ColumnHasActiveNibbs)
                {
                    this.nibbsColumnsEmpty.Add(this.nibbsColumns[i]);
                    this.nibbsColumns.RemoveAt(i);
                }
            }
            for(int i = 0; i < this.nibbsColumns.Count; i++)
            {
                this.nibbsColumns[i].EventIn_SetColumnIndex.Invoke(i);
            }

            this.CheckIfThereAreGroupsLeft();
        }


        private void CheckIfThereAreGroupsLeft() {
            bool aNibbHasSameColoredNeighbor = false;
            for (int i = 0; i < this.nibbsColumns.Count; i++)
            {
                if(this.nibbsColumns[i].VarOut_HasNibbAnySameColoredNeighbor()) { aNibbHasSameColoredNeighbor = true; break; }
            }
            if(aNibbHasSameColoredNeighbor)
            {
                // in that case continue level
            }
            else
            {
                // handle level finished here!
            }
        }

        internal static List<List<Nibb.State>> VarOut_GetNibbsStatesGrid()
        {
            List<List<Nibb.State>> states = new List<List<Nibb.State>>();
            Instance.nibbsColumns.ForEach(i => states.Add(i.VarOut_GetNibbsStatesGrid()));
            return states;
        }
    }
}