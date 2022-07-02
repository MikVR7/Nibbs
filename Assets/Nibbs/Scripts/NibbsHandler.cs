using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class EventIn_DisableNibb : EventSystem<KeyValuePair<int, int>> { }
    internal class EventIn_LetColumnsFall : EventSystem<List<int>> { }
    internal class EventIn_SetNibbState : EventSystem<int, int, int, bool> { }
    internal class EventOut_OnUpdate : EventSystem { }
    internal class NibbsHandler : MonoBehaviour
    {
        internal static EventIn_DisableNibb EventIn_DisableNibb = new EventIn_DisableNibb();
        internal static EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal static EventOut_OnUpdate EventOut_OnUpdate = new EventOut_OnUpdate();
        internal static EventIn_SetNibbState EventIn_SetNibbState = new EventIn_SetNibbState();

        private static NibbsHandler Instance = null;
        [SerializeField] private GameObject prefabNibb = null;
        [SerializeField] private GameObject prefabNibbsLine = null;
        [SerializeField] private Transform tLinesHolder = null;
        [SerializeField] private float lineHeight = 0f;
        [SerializeField] private float radius = 0f;
        [SerializeField] private int nibbsPerLine = 0;
        [SerializeField] private int linesCount = 0;
        [SerializeField] private float heightOffset = 0f;
        [SerializeField] private BallRotator ballRotator = null;
        private List<Transform> nibbsLines = new List<Transform>();
        private List<List<Nibb>> nibbs = new List<List<Nibb>>();
        private WinEvaluator winEvaluator = new WinEvaluator();
        private CustomNibbsGrids customNibbsGrids = new CustomNibbsGrids();

        private void Awake()
        {
            Instance = this;
            EventIn_DisableNibb.AddListener(DisableNibb);
            EventIn_LetColumnsFall.AddListener(LetColumnsFall);
            EventIn_SetNibbState.AddListener(SetNibbState);
            this.ballRotator.Init(/*nibbsPerLine*/);
            this.customNibbsGrids.Init();
            this.CreateLines(linesCount);
            StartCoroutine(this.InstantiateInCirclesDelayed(Vector3.zero, nibbsPerLine, radius));
            this.winEvaluator.Init();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                List<KeyValuePair<int, int>> nibbsDestroyed = new List<KeyValuePair<int, int>>();
                nibbsDestroyed = this.nibbs[1][1].DestroySelfAndNeighbors(nibbsDestroyed);
                WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsDestroyed);

                nibbsDestroyed = this.nibbs[1][4].DestroySelfAndNeighbors(nibbsDestroyed);
                WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsDestroyed);

                nibbsDestroyed = this.nibbs[1][7].DestroySelfAndNeighbors(nibbsDestroyed);
                WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsDestroyed);
            }
            if(Input.GetKeyDown(KeyCode.C))
            {
                BallRotator.EventIn_CloseColumnGaps.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                CustomNibbsGrids.EventIn_CreateNibbsGrid.Invoke(NibbsGrid.SetCurrentSingleColor);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CustomNibbsGrids.EventIn_CreateNibbsGrid.Invoke(NibbsGrid.SetCurrentOneColumnDifferent);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CustomNibbsGrids.EventIn_CreateNibbsGrid.Invoke(NibbsGrid.SetCurrentTestClosingGaps);
            }

            if (EventOut_OnUpdate.HasListeners())
            {
                EventOut_OnUpdate.Invoke();
            }
        }

        private void CreateLines(int linesCount)
        {
            this.nibbsLines.Clear();
            for (int i = 0; i < linesCount; i++)
            {
                GameObject goLine = Instantiate(this.prefabNibbsLine);
                goLine.name = "line_" + i;
                Transform tLine = goLine.GetComponent<Transform>();
                tLine.parent = tLinesHolder;
                tLine.localPosition = new Vector3(0f, ((i + (lineHeight)) * lineHeight)+ heightOffset, 0f);
                this.nibbsLines.Add(tLine);
                this.nibbs.Add(new List<Nibb>());
            }
        }

        private IEnumerator InstantiateInCirclesDelayed(Vector3 location, int howMany, float radius)
        {
            for (int i = 0; i < this.nibbsLines.Count; i++)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                this.InstantiateInCircle(location, howMany, radius, i);
            }
        }

        private void InstantiateInCircle(Vector3 location, int howMany, float radius, int circleNr)
        {
            for (int j = 0; j < howMany; j++)
            {
                float angle = j * Mathf.PI * 2f / howMany;
                CreateNibb(angle, circleNr, j);
            }
        }

        private void CreateNibb(float angle, int circleNr, int indexInCircle)
        {
            Vector3 newPos = (new Vector3(Mathf.Cos(angle) * radius, this.nibbsLines[circleNr].position.y, Mathf.Sin(angle) * radius));
            GameObject goNibb = Instantiate(this.prefabNibb, newPos, Quaternion.Euler(0, 0, 0), nibbsLines[circleNr]);
            goNibb.name = "nibb_" + circleNr + "_" + indexInCircle;
            Transform tNibb = goNibb.GetComponent<Transform>();
            tNibb.localPosition = new Vector3(tNibb.localPosition.x, 0f, tNibb.localPosition.z);
            tNibb.localScale = new Vector3(this.lineHeight, this.lineHeight, this.lineHeight);
            tNibb.LookAt(nibbsLines[circleNr]);
            Nibb nibb = goNibb.GetComponent<Nibb>();
            nibb.Init(this.lineHeight, circleNr, indexInCircle);
            this.nibbs[circleNr].Add(nibb);
        }

        private void DisableNibb(KeyValuePair<int, int> nibbIndices)
        {
            this.nibbs[nibbIndices.Key][nibbIndices.Value].gameObject.SetActive(false);
        }

        private void LetColumnsFall(List<int> columnsToFall)
        {
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                for (int j = 0; j < columnsToFall.Count; j++)
                {
                    if (this.nibbs[i][columnsToFall[j]].gameObject.activeSelf)
                    {
                        this.nibbs[i][columnsToFall[j]].EventIn_LetNibbFall.Invoke();
                    }
                }
            }
        }

        internal static List<List<int>> VarOut_GetNibbsGrid()
        {
            List<List<int>> grid = new List<List<int>>();
            for(int i = 0; i < Instance.nibbs.Count; i++)
            {
                List<int> line = new List<int>();
                for (int j = 0; j < Instance.nibbs[i].Count; j++)
                {
                    line.Add(Instance.nibbs[i][j].gameObject.activeSelf ? Instance.nibbs[i][j].VarOut_CurrentColor : -1);
                }
                grid.Add(line);
            }
            return grid;
        }

        private void SetNibbState(int lineNr, int indexInLine, int color, bool enabled)
        {
            this.nibbs[lineNr][indexInLine].EventIn_SetColor.Invoke(color);
            this.nibbs[lineNr][indexInLine].gameObject.SetActive(enabled);
        }
    }
}