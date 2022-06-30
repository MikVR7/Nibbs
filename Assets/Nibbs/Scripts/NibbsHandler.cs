using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class EventIn_DestroyNibb : EventSystem<KeyValuePair<int, int>> { }
    internal class EventIn_LetColumnsFall : EventSystem<List<int>> { }
    internal class EventOut_OnUpdate : EventSystem { }
    internal class NibbsHandler : MonoBehaviour
    {
        internal static EventIn_DestroyNibb EventIn_DestroyNibb = new EventIn_DestroyNibb();
        internal static EventIn_LetColumnsFall EventIn_LetColumnsFall = new EventIn_LetColumnsFall();
        internal static EventOut_OnUpdate EventOut_OnUpdate = new EventOut_OnUpdate();

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

        private void Awake()
        {
            EventIn_DestroyNibb.AddListener(DestroyNibb);
            EventIn_LetColumnsFall.AddListener(LetColumnsFall);
            this.ballRotator.Init(nibbsPerLine);
            this.CreateLines(linesCount);
            StartCoroutine(this.InstantiateInCirclesDelayed(prefabNibb, Vector3.zero, nibbsPerLine, radius));
            this.winEvaluator.Init();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                List<KeyValuePair<int, int>> nibbsDestroyed = new List<KeyValuePair<int, int>>();
                nibbsDestroyed = this.nibbs[1][1].DestroySelfAndNeighbors(nibbsDestroyed);
                WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsDestroyed);
            }
            if(EventOut_OnUpdate.HasListeners())
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

        private IEnumerator InstantiateInCirclesDelayed(GameObject obj, Vector3 location, int howMany, float radius)
        {
            for (int i = 0; i < this.nibbsLines.Count; i++)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                this.InstantiateInCircle(obj, location, howMany, radius, i);
            }
        }

        private void InstantiateInCircle(GameObject obj, Vector3 location, int howMany, float radius, int circleNr)
        {
            for (int j = 0; j < howMany; j++)
            {
                float angle = j * Mathf.PI * 2f / howMany;
                Vector3 newPos = (new Vector3(Mathf.Cos(angle) * radius, this.nibbsLines[circleNr].position.y, Mathf.Sin(angle) * radius));
                GameObject goNibb = Instantiate(obj, newPos, Quaternion.Euler(0, 0, 0), nibbsLines[circleNr]);
                goNibb.name = "nibb_" + circleNr + "_" + j;
                Transform tNibb = goNibb.GetComponent<Transform>();
                tNibb.localPosition = new Vector3(tNibb.localPosition.x, 0f, tNibb.localPosition.z);
                tNibb.localScale = new Vector3(this.lineHeight, this.lineHeight, this.lineHeight);
                tNibb.LookAt(nibbsLines[circleNr]);
                Nibb nibb = goNibb.GetComponent<Nibb>();
                nibb.Init(this.lineHeight, circleNr, j);
                this.nibbs[circleNr].Add(nibb);
            }
        }

        private void DestroyNibb(KeyValuePair<int, int> nibbIndices)
        {
            Destroy(this.nibbs[nibbIndices.Key][nibbIndices.Value].gameObject);
            this.nibbs[nibbIndices.Key][nibbIndices.Value] = null;
        }

        private void LetColumnsFall(List<int> columnsToFall)
        {
            for (int i = 0; i < this.nibbs.Count; i++)
            {
                for (int j = 0; j < columnsToFall.Count; j++)
                {
                    if (this.nibbs[i][columnsToFall[j]] != null)
                    {
                        this.nibbs[i][columnsToFall[j]].LetNibbsFallByPhysics();
                    }
                }
            }
        }
    }
}