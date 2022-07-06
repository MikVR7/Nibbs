
using Autohand;
using CodeEvents;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class Nibb : SerializedMonoBehaviour
    {
        internal enum State
        {
            Inited = 0, // ready for level but invisible
            Idle = 1, // visible not moving
            Falling = 2, // visible, physics on
            Destroyed = 3, // invisible
        }

        internal EventIn_SetColor EventIn_SetColor = new EventIn_SetColor();
        internal EventIn_SetNibbState EventIn_SetNibbState = new EventIn_SetNibbState();
        internal EventIn_DisableNibb EventIn_DisableNibb = new EventIn_DisableNibb();

        [SerializeField] private Dictionary<NibbColor, Material> materials = new Dictionary<NibbColor, Material>();
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
        private int columnNr = 0;
        private int indexInColumn = 0;
        internal State VarOut_CurrentState { get; private set; } = State.Idle;

        internal NibbColor VarOut_CurrentColor { get; private set; } = NibbColor.Blue;

        internal void Init(Transform parent, int columnNr, int indexInColumn, float scaling, Vector3 centerPoint)
        {
            EventIn_SetColor.AddListener(SetColor);
            EventIn_SetNibbState.AddListener(SetNibbState);
            this.columnNr = columnNr;
            this.indexInColumn = indexInColumn;

            this.gameObject.name = "nibb_" + columnNr + "_" + indexInColumn;
            this.VarOut_MyTransform = this.GetComponent<Transform>();
            this.VarOut_MyTransform.parent = parent;
            this.VarOut_MyTransform.localPosition = Vector3.zero;
            this.VarOut_MyTransform.localScale = new Vector3(scaling,scaling,scaling);
            this.VarOut_MyTransform.LookAt(centerPoint);
            this.myCollider = this.GetComponent<SphereCollider>();
            this.myRigidbody = this.GetComponent<Rigidbody>();
            this.myRenderer = this.GetComponent<MeshRenderer>();
            this.distanceGrabbable = this.GetComponent<DistanceGrabbable>();
            this.distanceGrabbable.OnPull.AddListener(OnPull);
            this.currentTag = this.gameObject.tag;

            this.SetNibbState(State.Inited);
        }

        private void SetNibbState(State state)
        {
            switch(state)
            {
                case State.Inited:
                    this.gameObject.SetActive(false);
                    break;
                case State.Idle: 
                    this.gameObject.SetActive(true);
                    this.myRigidbody.isKinematic = true;
                    break;
                case State.Falling:
                    this.myRigidbody.isKinematic = false;
                    this.myRigidbody.AddRelativeForce(new Vector3(0f, -10f, 0f), ForceMode.Impulse);
                    StartCoroutine(StopFalling());
                    break;
                case State.Destroyed:
                    this.gameObject.SetActive(false);
                    break;
            }
            this.VarOut_CurrentState = state;
        }

        private IEnumerator StopFalling()
        {
            yield return new WaitForSeconds(1f);
            NibbsGameHandler.EventOut_OnUpdate.AddListener(OnUpdate);
        }

        private void SetColor(NibbColor color) {
            this.VarOut_CurrentColor = color;
            this.myRenderer.material = this.materials[this.VarOut_CurrentColor];
        }

        internal void OnPull(Hand arg0, Grabbable arg1)
        {
            this.myRenderer.material = this.matWhite;
            List<KeyValuePair<int, int>> nibbsToDestroy = new List<KeyValuePair<int, int>>();
            nibbsToDestroy = DestroySelfAndNeighbors(nibbsToDestroy);
            // TODO: 
            //WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsToDestroy);
        }

        internal List<KeyValuePair<int, int>> DestroySelfAndNeighbors(List<KeyValuePair<int, int>> nibbsToDestroy)
        {
            if(nibbsToDestroy.Contains(new KeyValuePair<int, int>(this.columnNr, this.indexInColumn))/* ||*/
                /*TODO: NibbsGameHandler.VarOut_NibbsAreFalling()*/) { return nibbsToDestroy; }

            GetNeighbors();
            nibbsToDestroy.Add(new KeyValuePair<int, int>(this.columnNr, this.indexInColumn));
            this.neighbors.ForEach(i => nibbsToDestroy = i.DestroySelfAndNeighbors(nibbsToDestroy));
            return nibbsToDestroy;
        }

        private void OnUpdate()
        {
            if(this.myRigidbody.isKinematic == false)
            {
                if(this.myRigidbody.velocity.magnitude < 0.1f)
                {
                    //this.myRigidbody.isKinematic = true;
                    NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
                    this.SetNibbState(State.Idle);
                    // TODO: 
                    //NibbsGameHandler.EventIn_NibbFinishedFalling.Invoke(new KeyValuePair<int, int>(this.columnNr, this.indexInColumn));
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
