using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class LevelsHandler : SerializedMonoBehaviour
    {
        internal enum LevelState
        {
            Inited = 0,
            Started = 1,
            Idle = 2,
            FallingNibs = 3,
            RotatingColumns = 4,
            LevelEnd = 5,
        }

        internal static EventIn_SetupLevel EventIn_SetupLevel = new EventIn_SetupLevel();
        internal static EventIn_StartLevel EventIn_StartLevel = new EventIn_StartLevel();
        internal static EventIn_DestroyNibb EventIn_DestroyNibb = new EventIn_DestroyNibb();
        internal static EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal static EventIn_ColumnsFinishedFalling EventIn_ColumnsFinishedFalling = new EventIn_ColumnsFinishedFalling();
        internal static EventIn_ColumnStartedFallingState EventIn_ColumnStartedFallingState = new EventIn_ColumnStartedFallingState();

        private static LevelsHandler Instance = null;
        [SerializeField] private GameObject prefabNibbsColumn = null;
        internal static Level VarOut_Level { get; private set; } = new Level();
        internal static LevelState VarOut_LevelState { get; private set; } = LevelState.Inited;
        //[SerializeField] internal static bool VarOut_NibbsAreFalling { get; private set; } = false;
        //[SerializeField] internal static bool VarOut_WinEvaluationRunning { get; set; } = false;
        private Transform myTransform = null;
        [SerializeField] private Transform tColumnsHolder = null;
        [SerializeField] private List<NibbsColumn> nibbsColumns = new List<NibbsColumn>();
        [SerializeField] private List<NibbsColumn> nibbsColumnsEmpty = new List<NibbsColumn>();
        private WinEvaluator winEvaluator = new WinEvaluator();
        [SerializeField] private ColumnShifter columnShifter = null;
        [SerializeField] private TextMeshPro tmpText = null;

        internal void Init()
        {
            Instance = this;
            VarOut_LevelState = LevelState.Inited;
            this.myTransform = this.GetComponent<Transform>();
            EventIn_DestroyNibb.AddListenerSingle(DestroyNibb);
            EventIn_SetupLevel.AddListenerSingle(SetupLevel);
            EventIn_StartLevel.AddListenerSingle(StartLevel);
            EventIn_LetColumnsFall.AddListenerSingle(LetColumnsFall);
            EventIn_ColumnsFinishedFalling.AddListenerSingle(ColumnsFinishedFalling);
            EventIn_ColumnStartedFallingState.AddListenerSingle(ColumnStartedFallingState);
            GUIHandler.EventOut_OnBtnStart.AddListenerSingle(OnBtnStart);
            VarOut_Level.Init();
            this.winEvaluator.Init();
            this.columnShifter.Init(this.tColumnsHolder);
            this.SetText();
        }

        private void SetText()
        {
            tmpText.text = "Lvl:" + VarOut_Level + "<br>State:<br>" + VarOut_LevelState;
        }

        private void OnBtnStart()
        {
            VarOut_LevelState = LevelState.Started;
            SetupLevel(1);
            StartLevel();
            this.SetText();
        }

        private void SetupLevel(int levelNr) {
            VarOut_Level.EventIn_SetupLevel.Invoke(levelNr);
            //columnShifter.EventIn_RotateLevel.Invoke(true, 0);
            this.CleanupPreviousLevel();
            this.CreateNibbsColumns();
            this.SetNibbsTypes();
            columnShifter.EventIn_RotateLevel.Invoke(true, VarOut_Level.VarOut_GetLevel().ColumnCount);
            this.SetText();
        }

        private void StartLevel()
        {
            VarOut_LevelState = LevelState.FallingNibs;
            winEvaluator.EventIn_ResetPointsCount.Invoke();

            this.nibbsColumns.ForEach(i => i.EventIn_StartLevel.Invoke());
            // TODO: Check if there are falling nibbs and set state
            this.SetText();
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
            this.SetText();
        }

        private void CreateNibbsColumns()
        {
            this.nibbsColumns.Clear();
            for (int i = 0; i < VarOut_Level.VarOut_GetLevel().ColumnCount; i++)
            {
                GameObject goColumn = Instantiate(this.prefabNibbsColumn);
                NibbsColumn nibbsColumn = goColumn.GetComponent<NibbsColumn>();
                nibbsColumn.Init(i, tColumnsHolder);
                this.nibbsColumns.Add(nibbsColumn);
            }
            this.SetText();
        }

        private void SetNibbsTypes()
        {
            for (int i = 0; i < this.nibbsColumns.Count; i++)
            {
                this.nibbsColumns[i].EventIn_SetNibbsTypes.Invoke();
            }
            this.SetText();
        }

        private void DestroyNibb(int columnNr, int indexInColumn)
        {
            nibbsColumns[columnNr].EventIn_DestroyNibb.Invoke(columnNr, indexInColumn);
            this.SetText();
        }

        private void LetColumnsFall(List<int> columnIndices)
        {
            VarOut_LevelState = LevelState.FallingNibs;
            for (int i = 0; i < columnIndices.Count; i++)
            {
                this.nibbsColumns[columnIndices[i]].EventIn_LetColumnsFall.Invoke(columnIndices);
            }
            this.SetText();
        }

        private bool IsAnyColumnInFallingState()
        {
            for (int i = 0; i < this.nibbsColumns.Count; i++)
            {
                if (this.nibbsColumns[i].VarOut_ColumnState == NibbsColumn.ColumnState.FallingNibbs)
                {
                    return true;
                }
            }
            return false;
        }

        private void ColumnStartedFallingState()
        {
            VarOut_LevelState = LevelState.FallingNibs;
            this.SetText();
        }

        private void ColumnsFinishedFalling()
        {
            if(VarOut_LevelState == LevelState.FallingNibs)
            {
                if(!this.IsAnyColumnInFallingState()) {
                    Debug.Log("Columns rotating!");
                    VarOut_LevelState = LevelState.RotatingColumns;
                    RotateColumns();
                }
            }
            this.SetText();
        }

        private void RotateColumns()
        {
            if (!this.columnShifter.VarOut_IsPerformingRotationAnimation)
            {
                List<ColumnShiftInstance> shiftInstances = new List<ColumnShiftInstance>();
                this.nibbsColumns.ForEach(i => shiftInstances.Add(new ColumnShiftInstance()
                {
                    ColumnHasNibbs = (i.VarOut_ColumnState == NibbsColumn.ColumnState.IdleHasNibbs),
                    TColumn = i.VarOut_MyTransform

                }));
                this.columnShifter.EventOut_ColumnShiftingDone.AddListenerSingle(ColumnShiftingDone);
                this.columnShifter.EventIn_RotateColumns.Invoke(shiftInstances);
            }
            this.SetText();
        }

        private void ColumnShiftingDone()
        {
            this.columnShifter.EventOut_ColumnShiftingDone.RemoveListener(ColumnShiftingDone);
            for(int i = this.nibbsColumns.Count-1; i >= 0; i--) {
                if (this.nibbsColumns[i].VarOut_ColumnState == NibbsColumn.ColumnState.IdleNoNibbs)
                {
                    this.nibbsColumns[i].EventIn_DeactivateColumn.Invoke();
                    this.nibbsColumnsEmpty.Add(this.nibbsColumns[i]);
                    this.nibbsColumns.RemoveAt(i);
                }
            }
            for(int i = 0; i < this.nibbsColumns.Count; i++)
            {
                this.nibbsColumns[i].EventIn_SetColumnIndex.Invoke(i);
            }

            this.CheckIfThereAreGroupsLeft();
            this.SetText();
        }


        private void CheckIfThereAreGroupsLeft() {
            bool nibbHasSameColoredNeighbor = false;
            for (int i = 0; i < this.nibbsColumns.Count; i++)
            {
                if(this.nibbsColumns[i].VarOut_HasNibbAnySameColoredNeighbor()) {
                    nibbHasSameColoredNeighbor = true;
                    VarOut_LevelState = LevelState.Idle;
                    break;
                }
            }
            if (!nibbHasSameColoredNeighbor)
            {
                StartCoroutine(StartNextLevelDelayed());
            }
            else
            {
                // continue level
                //VarOut_WinEvaluationRunning = false;
            }
            this.SetText();
        }

        private IEnumerator StartNextLevelDelayed()
        {
            yield return new WaitForSecondsRealtime(2f);
            Debug.Log("STart next level delayed!");
            // handle level finished here!
            LevelsHandler.EventIn_SetupLevel.Invoke(VarOut_Level.VarOut_GetLevel().LevelNr);
            LevelsHandler.EventIn_StartLevel.Invoke();
            //VarOut_WinEvaluationRunning = false;
            this.SetText();
        }

        //internal static List<List<Nibb.State>> VarOut_GetNibbsStatesGrid()
        //{
        //    List<List<Nibb.State>> states = new List<List<Nibb.State>>();
        //    Instance.nibbsColumns.ForEach(i => states.Add(i.VarOut_GetNibbsStatesGrid()));
        //    return states;
        //}
    }
}