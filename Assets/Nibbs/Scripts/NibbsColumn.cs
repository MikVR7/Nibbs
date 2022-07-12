using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsColumn : SerializedMonoBehaviour
    {
        internal EventIn_DestroyNibb EventIn_DestroyNibb = new EventIn_DestroyNibb();
        internal EventIn_StartLevel EventIn_StartLevel = new EventIn_StartLevel();
        internal EventIn_SetNibbsTypes EventIn_SetNibbsTypes = new EventIn_SetNibbsTypes();
        internal EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal EventIn_SetColumnIndex EventIn_SetColumnIndex = new EventIn_SetColumnIndex();
        internal EventOut_ColumnStateUpdated EventOut_ColumnStateUpdated = new EventOut_ColumnStateUpdated();

        [SerializeField] internal bool VarOut_ColumnHasActiveNibbs { get; private set; } = true;
        [SerializeField] internal bool VarOut_HasFallingNibbs { get; private set; } = false;

        [SerializeField] private GameObject prefabNibb = null;
        [SerializeField] private Transform tNibbsHolder = null;

        private int columnIndex = 0;
        [SerializeField] private List<Nibb> nibbs = new List<Nibb>();
        [SerializeField] private List<Nibb> nibbsDestroyed = new List<Nibb>();
        internal Transform VarOut_MyTransform { get; private set; } = null;

        internal void Init(int columnIndex, Transform parent)
        {
            this.columnIndex = columnIndex;
            this.gameObject.name = "column_" + columnIndex;
            this.VarOut_MyTransform = this.GetComponent<Transform>();
            this.VarOut_MyTransform.parent = parent;
            this.VarOut_MyTransform.localPosition = Vector3.zero;
            float angle = columnIndex * Mathf.PI * 2f / LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnCount;
            float levelRadius = LevelsHandler.VarOut_Level.VarOut_GetLevel().LevelRadius;
            this.tNibbsHolder.localPosition = new Vector3(Mathf.Cos(angle) * levelRadius, 0f, Mathf.Sin(angle) * levelRadius);

            EventIn_DestroyNibb.AddListener(DestroyNibb);
            EventIn_StartLevel.AddListener(StartLevel);
            EventIn_SetNibbsTypes.AddListener(SetNibbsTypes);
            EventIn_LetColumnsFall.AddListener(LetColumnFall);
            EventIn_SetColumnIndex.AddListener(SetColumnIndex);

            this.CreateNibbs();
        }

        private void SetColumnIndex(int index)
        {
            this.columnIndex = index;
            this.nibbs.ForEach(i => i.EventIn_SetColumnIndex.Invoke(index));
        }

        private void CreateNibbs()
        {
            this.nibbs.Clear();
            this.nibbsDestroyed.Clear();
            for (int i = 0; i < LevelsHandler.VarOut_Level.VarOut_GetLevel().DefaultColumnHeight; i++)
            {
                GameObject goNibb = Instantiate(this.prefabNibb);
                Nibb nibb = goNibb.GetComponent<Nibb>();
                nibb.Init(this.tNibbsHolder, this.columnIndex, i, LevelsHandler.VarOut_Level.VarOut_GetLevel().NibbDefaultScaling, this.VarOut_MyTransform.position);
                this.nibbs.Add(nibb);
            }
        }

        private void StartLevel()
        {
            float nibbsDefaultScaling = LevelsHandler.VarOut_Level.VarOut_GetLevel().NibbDefaultScaling;
            float heightStartOffset = LevelsHandler.VarOut_Level.VarOut_GetLevel().HeightStartOffset;
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                float startHeight = (nibbsDefaultScaling * i) + heightStartOffset;
                nibbs[i].EventIn_SetNibbPosY.Invoke(startHeight);
                nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Idle);
                nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Falling);
            }
        }

        private void DestroyNibb(int columnNr, int indexInColumn)
        {
            this.nibbs[indexInColumn].EventIn_SetNibbState.Invoke(Nibb.State.Destroyed);
            this.CheckIfColumnHasActiveNibbs();
        }

        private void SetNibbsTypes()
        {
            List<List<int>> staticElements = LevelsHandler.VarOut_Level.VarOut_GetLevel().StaticLevelElements;
            List<NibbColor> nibbColors = LevelsHandler.VarOut_Level.VarOut_GetLevel().NibbsColors;
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                //Debug.Log("STATIC ELEMENTS: " + staticElements.Count + " " + staticElements[staticElements.Count - 1].Count);
                if ((staticElements.Count > this.columnIndex) &&
                    (staticElements[this.columnIndex].Count > i) &&
                    staticElements[this.columnIndex][i] >= 0)
                {
                    this.nibbs[i].EventIn_SetColor.Invoke((NibbColor)staticElements[this.columnIndex][i]);
                }
                else
                {
                    NibbColor c = nibbColors[Random.Range(0, nibbColors.Count)];
                    this.nibbs[i].EventIn_SetColor.Invoke(c);
                }


                // TODO: set different types of nibbs HERE!!!
            }
        }

        private void LetColumnFall(List<int> columns)
        {
            bool letFall = false;
            int indexCount = 0;
            for(int i = 0; i < this.nibbs.Count; i++)
            {
                if (this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Destroyed))
                {
                    this.nibbsDestroyed.Add(this.nibbs[i]);
                    letFall = true;
                }
                else if(letFall && this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Idle))
                {
                    this.nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Falling);
                    this.nibbs[i].EventIn_SetNibbIndex.Invoke(indexCount++);
                    this.nibbs[i].EventOut_NibbFinishedFalling.AddListener(OnNibbFinishedFalling);
                    VarOut_HasFallingNibbs = true;
                }
                else
                {
                    indexCount++;
                }
            }
            this.nibbsDestroyed.ForEach(i => this.nibbs.Remove(i));
            if(!VarOut_HasFallingNibbs)
            {
                OnNibbFinishedFalling(-1);
            }
        }

        private void OnNibbFinishedFalling(int index)
        {
            VarOut_HasFallingNibbs = false;
            for (int i = 0; i < this.nibbs.Count; i++) {
                if (this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Falling))
                {
                    VarOut_HasFallingNibbs = true;
                    break;
                }
            }
            EventOut_ColumnStateUpdated.Invoke();
        }

        private void CheckIfColumnHasActiveNibbs()
        {
            VarOut_ColumnHasActiveNibbs = false;
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                if (this.nibbs[i].VarOut_CurrentState != Nibb.State.Destroyed)
                {
                    VarOut_ColumnHasActiveNibbs = true;
                }
            }
            EventOut_ColumnStateUpdated.Invoke();
        }

        internal List<Nibb.State> VarOut_GetNibbsStatesGrid()
        {
            List<Nibb.State> states = new List<Nibb.State>();
            this.nibbs.ForEach(i => states.Add(i.VarOut_CurrentState));
            return states;
        }

        internal bool VarOut_HasNibbAnySameColoredNeighbor()
        {
            for(int i = 0; i < this.nibbs.Count; i++)
            {
                if(this.nibbs[i].NibbHasSameColoredNeighbor())
                {
                    return true;
                }
            }
            return false;
        }
    }
}