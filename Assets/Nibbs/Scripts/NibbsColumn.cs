using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsColumn : SerializedMonoBehaviour
    {
        internal enum ColumnState
        {
            Inited = 0,
            Idle = 1,
            FallingNibbs = 2,
            Rotating = 3,
            Deactivated = 4
        }

        internal EventIn_DestroyNibb EventIn_DestroyNibb = new EventIn_DestroyNibb();
        internal EventIn_StartLevel EventIn_StartLevel = new EventIn_StartLevel();
        internal EventIn_SetNibbsTypes EventIn_SetNibbsTypes = new EventIn_SetNibbsTypes();
        internal EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal EventIn_SetColumnIndex EventIn_SetColumnIndex = new EventIn_SetColumnIndex();
        //internal EventOut_ColumnStateUpdated EventOut_ColumnStateUpdated = new EventOut_ColumnStateUpdated();

        //[SerializeField] internal bool VarOut_ColumnHasActiveNibbs { get; private set; } = true;
        //[SerializeField] internal bool VarOut_HasFallingNibbs { get; private set; } = false;

        [SerializeField] private GameObject prefabNibb = null;
        [SerializeField] private Transform tNibbsHolder = null;

        private int columnIndex = 0;
        [SerializeField] private List<Nibb> nibbs = new List<Nibb>();
        [SerializeField] private List<Nibb> nibbsDestroyed = new List<Nibb>();
        [SerializeField] private TextMeshPro tmpText = null;
        internal Transform VarOut_MyTransform { get; private set; } = null;
        internal ColumnState VarOut_ColumnState { get; private set; } = ColumnState.Inited;

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
            this.VarOut_ColumnState = ColumnState.Inited;

            EventIn_DestroyNibb.AddListenerSingle(DestroyNibb);
            EventIn_StartLevel.AddListenerSingle(StartLevel);
            EventIn_SetNibbsTypes.AddListenerSingle(SetNibbsTypes);
            EventIn_LetColumnsFall.AddListenerSingle(LetColumnFall);
            EventIn_SetColumnIndex.AddListenerSingle(SetColumnIndex);

            this.CreateNibbs();
            this.SetText();
            this.PrepareText();
        }

        private void SetText()
        {
            tmpText.text = "C: " + columnIndex + "<br>" + "A:" + nibbs.Count + " D:" + this.nibbsDestroyed.Count + "<br>State:<br>" + VarOut_ColumnState;
        }
        private void PrepareText()
        {
            RectTransform rtText = tmpText.rectTransform;
            rtText.LookAt(VarOut_MyTransform);
            rtText.localEulerAngles = new Vector3(90f, rtText.localEulerAngles.y, rtText.localEulerAngles.z+180f);
        }

        private void SetColumnIndex(int index)
        {
            this.columnIndex = index;
            this.gameObject.name = "column_" + columnIndex;
            this.nibbs.ForEach(i => i.EventIn_SetColumnIndex.Invoke(index));
            this.SetText();
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
            this.SetText();
        }

        private void StartLevel()
        {
            float nibbsDefaultScaling = LevelsHandler.VarOut_Level.VarOut_GetLevel().NibbDefaultScaling;
            float heightStartOffset = LevelsHandler.VarOut_Level.VarOut_GetLevel().HeightStartOffset;
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                float startHeight = (nibbsDefaultScaling * i * 2f) + heightStartOffset;
                nibbs[i].EventIn_SetNibbPosY.Invoke(startHeight);
                nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Idle);
                nibbs[i].EventOut_NibbFinishedFalling.AddListenerSingle(OnNibbFinishedFalling);
                nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Falling);
            }
            this.VarOut_ColumnState = ColumnState.FallingNibbs;
            this.SetText();
        }

        private void DestroyNibb(int columnNr, int indexInColumn)
        {
            this.nibbs[indexInColumn].EventIn_SetNibbState.Invoke(Nibb.State.Destroyed);
            bool hasActiveNibb = this.HasColumnActiveNibbs();
            if(!hasActiveNibb) { this.DeactivateColumn(); }
            this.SetText();
        }

        private void DeactivateColumn()
        {
            this.VarOut_ColumnState = ColumnState.Deactivated;
            //this.gameObject.SetActive(false);
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

            this.SetText();
        }

        private void LetColumnFall(List<int> columns)
        {
            bool letFall = false;
            int indexCount = 0;
            VarOut_ColumnState = ColumnState.FallingNibbs;
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                if (this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Destroyed))
                {
                    this.nibbsDestroyed.Add(this.nibbs[i]);
                    letFall = true;
                }
                else if(letFall && this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Idle))
                {
                    Debug.Log("LET FALL: " + letFall);
                    this.nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Falling);
                    this.nibbs[i].EventIn_SetNibbIndex.Invoke(indexCount++);
                    this.nibbs[i].EventOut_NibbFinishedFalling.AddListenerSingle(OnNibbFinishedFalling);
                }
                else
                {
                    indexCount++;
                }
            }
            this.nibbsDestroyed.ForEach(i => this.nibbs.Remove(i));
            bool hasColumnFallingNibbs = this.HasColumnFallingNibb();
            if(!hasColumnFallingNibbs)
            {
                StartCoroutine(OnNibbFinishedFallingDelayed(-1));
            }
            

            this.SetText();
        }

        private IEnumerator OnNibbFinishedFallingDelayed(int index)
        {
            yield return new WaitForEndOfFrame();
            //OnNibbFinishedFalling(index);
            VarOut_ColumnState = ColumnState.Idle;
            LevelsHandler.EventIn_ColumnsFinishedFalling.Invoke();
        }

        private void OnNibbFinishedFalling(int index)
        {
            ////VarOut_HasFallingNibbs = false;
            //for (int i = 0; i < this.nibbs.Count; i++) {
            //    if (this.nibbs[i].VarOut_CurrentState.Equals(Nibb.State.Falling))
            //    {
            //        VarOut_HasFallingNibbs = true;
            //        break;
            //    }
            //}
            Debug.Log("NIBB: " + index);
            Debug.Log("Nibb State: " + this.nibbs[index].VarOut_CurrentState);
            this.nibbs[index].EventOut_NibbFinishedFalling.RemoveListener(OnNibbFinishedFalling);
            bool hasColumnFallingNibbs = this.HasColumnFallingNibb();
            if (!hasColumnFallingNibbs)
            {
                VarOut_ColumnState = ColumnState.Idle;
                LevelsHandler.EventIn_ColumnsFinishedFalling.Invoke();
                //EventOut_ColumnStateUpdated.Invoke();
            }
            else
            {
                VarOut_ColumnState = ColumnState.FallingNibbs;
            }

            this.SetText();
        }

        private bool HasColumnActiveNibbs()
        {
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                if (this.nibbs[i].VarOut_CurrentState != Nibb.State.Destroyed)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasColumnFallingNibb()
        {
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                if (this.nibbs[i].VarOut_CurrentState == Nibb.State.Falling)
                {
                    return true;
                }
            }
            return false;
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