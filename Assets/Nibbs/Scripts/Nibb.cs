
using Autohand;
using CodeEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class EventIn_LetNibbFall : EventSystem { }
    internal class EventIn_SetColor : EventSystem<int> { }
    internal class Nibb : MonoBehaviour
    {
        internal EventIn_LetNibbFall EventIn_LetNibbFall = new EventIn_LetNibbFall();
        internal EventIn_SetColor EventIn_SetColor = new EventIn_SetColor();

        [SerializeField] private List<Material> materials = new List<Material>();
        [SerializeField] private Material matWhite = null;
        private SphereCollider myCollider = null;
        private Rigidbody myRigidbody = null;
        internal Transform VarOut_MyTransform { get; private set; } = null;
        private MeshRenderer myRenderer = null;
        private DistanceGrabbable distanceGrabbable = null;
        
        private float scaling = 0f;
        private List<Nibb> neighbors = new List<Nibb>();
        private string currentTag = string.Empty;
        private bool isFalling = false;

        internal int lineNr { get; private set; } = 0;
        internal int indexInLine { get; private set; } = 0;
        internal int VarOut_CurrentColor { get; private set; } = 0;

        internal void Init(float scaling, int lineNr, int indexInLine)
        {
            EventIn_LetNibbFall.AddListener(LetNibbFall);
            EventIn_SetColor.AddListener(SetColor);

            this.lineNr = lineNr;
            this.indexInLine = indexInLine;
            this.scaling = scaling;
            this.myCollider = this.GetComponent<SphereCollider>();
            this.myRigidbody = this.GetComponent<Rigidbody>();
            this.VarOut_MyTransform = this.GetComponent<Transform>();
            this.myRenderer = this.GetComponent<MeshRenderer>();
            this.distanceGrabbable = this.GetComponent<DistanceGrabbable>();
            this.distanceGrabbable.OnPull.AddListener(OnPull);

            this.LetNibbFall();

            this.currentTag = this.gameObject.tag;

            this.SetRandomColor();
        }

        private void LetNibbFall()
        {
            this.myRigidbody.isKinematic = false;
            this.myRigidbody.AddRelativeForce(new Vector3(0f, -10f, 0f), ForceMode.Impulse);
            this.isFalling = true;
            StartCoroutine(StopFalling());
        }

        private IEnumerator StopFalling()
        {
            yield return new WaitForSeconds(3f);
            //yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            
            NibbsHandler.EventOut_OnUpdate.AddListener(OnUpdate);
            this.isFalling = false;
        }

        private void SetRandomColor()
        {
            SetColor(Random.Range(0, this.materials.Count-1));
        }

        private void SetColor(int color) {
            this.VarOut_CurrentColor = color;
            this.myRenderer.material = this.materials[this.VarOut_CurrentColor];
        }

        internal void OnPull(Hand arg0, Grabbable arg1)
        {
            this.myRenderer.material = this.matWhite;
            List<KeyValuePair<int, int>> nibbsToDestroy = new List<KeyValuePair<int, int>>();
            nibbsToDestroy = DestroySelfAndNeighbors(nibbsToDestroy);
            WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsToDestroy);
        }

        internal List<KeyValuePair<int, int>> DestroySelfAndNeighbors(List<KeyValuePair<int, int>> nibbsToDestroy)
        {
            if(nibbsToDestroy.Contains(new KeyValuePair<int, int>(this.lineNr, this.indexInLine))) { return nibbsToDestroy; }

            GetNeighbors();
            nibbsToDestroy.Add(new KeyValuePair<int, int>(this.lineNr, this.indexInLine));
            this.neighbors.ForEach(i => nibbsToDestroy = i.DestroySelfAndNeighbors(nibbsToDestroy));
            return nibbsToDestroy;
        }

        private void OnUpdate()
        {
            if((this.myRigidbody.isKinematic == false) && (isFalling == false))
            {
                if(this.myRigidbody.velocity.magnitude < 0.1f)
                {
                    this.myRigidbody.isKinematic = true;
                    NibbsHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
                    NibbsHandler.EventIn_NibbFinishedFalling.Invoke(new KeyValuePair<int, int>(this.lineNr, this.indexInLine));
                }
            }
        }

        private void GetNeighbors()
        {
            this.neighbors.Clear();
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.up));
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.down));
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.left));
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.right));
        }

        private void ShootRay(Vector3 direction)
        {
            RaycastHit hit;
            Ray ray = new Ray(this.VarOut_MyTransform.position, direction);
            if (Physics.Raycast(ray, out hit, this.scaling))
            {
                if ((hit.collider != null) && (hit.collider.tag.Equals(this.currentTag))) {
                    Nibb neighbor = hit.transform.GetComponent<Nibb>();
                    if (neighbor.VarOut_CurrentColor.Equals(this.VarOut_CurrentColor))
                    {
                        this.neighbors.Add(neighbor);
                    }
                }
            }
        }
    }
}
