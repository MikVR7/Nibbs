using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class NibbsColumn : MonoBehaviour
    {
        internal EventIn_DisableNibb EventIn_DisableNibb = new EventIn_DisableNibb();
        internal EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal EventIn_StartLevelColumn EventIn_StartLevelColumn = new EventIn_StartLevelColumn();

        [SerializeField] private GameObject prefabNibb = null;
        [SerializeField] private Transform tNibbsHolder = null;
        private List<Nibb> nibbs = new List<Nibb>();
        private int columnHeight = 0;
        private int columnIndex = 0;
        private Transform myTransform = null;

        internal void Init(int columnIndex, Transform parent, LevelData level)// int columnHeight, float angle, float radius, float nibbsScaling)
        {
            this.columnIndex = columnIndex;
            this.columnHeight = level.DefaultColumnHeight;
            this.gameObject.name = "column_" + columnIndex;
            this.myTransform = this.GetComponent<Transform>();
            this.myTransform.parent = parent;
            this.myTransform.localPosition = Vector3.zero;
            float angle = columnIndex * Mathf.PI * 2f / level.ColumnCount;
            this.tNibbsHolder.localPosition = new Vector3(Mathf.Cos(angle) * level.LevelRadius, 0f, Mathf.Sin(angle) * level.LevelRadius);

            EventIn_DisableNibb.AddListener(DisableNibb);
            EventIn_LetColumnsFall.AddListener(LetColumnsFall);

            this.CreateNibbs(level.NibbDefaultScaling);
        }

        private void CreateNibbs(float nibbsScaling)
        {
            for (int i = 0; i < this.columnHeight; i++)
            {
                GameObject goNibb = Instantiate(this.prefabNibb);
                Nibb nibb = goNibb.GetComponent<Nibb>();
                nibb.Init(this.tNibbsHolder, this.columnIndex, i, nibbsScaling, this.myTransform.position);
                this.nibbs.Add(nibb);
            }
        }

        private void DisableNibb(int columnNr, int indexInColumn)
        {
            this.nibbs[indexInColumn].EventIn_SetNibbState.Invoke(Nibb.State.Destroyed);
        }

        private void LetColumnsFall()
        {
            for (int i = 0; i < this.nibbs.Count; i++) {
                this.nibbs[i].EventIn_SetNibbState.Invoke(Nibb.State.Falling);
            }
        }

        internal List<Nibb.State> VarOut_GetNibbsStatesGrid()
        {
            List<Nibb.State> states = new List<Nibb.State>();
            this.nibbs.ForEach(i => states.Add(i.VarOut_CurrentState));
            return states;
        }
    }
}