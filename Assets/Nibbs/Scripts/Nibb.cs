
using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class Nibb : MonoBehaviour
    {
        private SphereCollider myCollider = null;
        private Rigidbody myRigidbody = null;
        private Transform myTransform = null;
        private MeshRenderer myRenderer = null;
        private DistanceGrabbable distanceGrabbable = null;
        [SerializeField] private List<Material> materials = new List<Material>();
        [SerializeField] private Material matWhite = null;
        private int currentColor = 0;
        private float scaling = 0f;
        private List<Nibb> neighbors = new List<Nibb>();
        private string currentTag = string.Empty;
        internal int lineNr { get; private set; } = 0;
        internal int indexInLine { get; private set; } = 0;
        private bool isFalling = false;

        internal void Init(float scaling, int lineNr, int indexInLine)
        {
            this.lineNr = lineNr;
            this.indexInLine = indexInLine;
            this.scaling = scaling;
            this.myCollider = this.GetComponent<SphereCollider>();
            this.myRigidbody = this.GetComponent<Rigidbody>();
            this.myTransform = this.GetComponent<Transform>();
            this.myRenderer = this.GetComponent<MeshRenderer>();
            this.distanceGrabbable = this.GetComponent<DistanceGrabbable>();
            this.distanceGrabbable.OnPull.AddListener(OnPull);

            this.LetNibbsFallByPhysics();

            this.currentTag = this.gameObject.tag;

            this.SetRandomColor();
        }

        internal void LetNibbsFallByPhysics()
        {
            this.myRigidbody.isKinematic = false;
            this.myRigidbody.AddRelativeForce(new Vector3(0f, -10f, 0f), ForceMode.Impulse);
            this.isFalling = true;
            StartCoroutine(StopFalling());
        }

        private IEnumerator StopFalling()
        {
            yield return new WaitForSeconds(3f);
            NibbsHandler.EventOut_OnUpdate.AddListener(OnUpdate);
            this.isFalling = false;
        }

        private void SetRandomColor()
        {
            this.currentColor = Random.Range(0, this.materials.Count-1);

            this.myRenderer.material = this.materials[this.currentColor];
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
                }
            }
        }

        private void GetNeighbors()
        {
            this.neighbors.Clear();
            this.ShootRay(this.myTransform.TransformDirection(Vector3.up));
            this.ShootRay(this.myTransform.TransformDirection(Vector3.down));
            this.ShootRay(this.myTransform.TransformDirection(Vector3.left));
            this.ShootRay(this.myTransform.TransformDirection(Vector3.right));
        }

        private void ShootRay(Vector3 direction)
        {
            RaycastHit hit;
            Ray ray = new Ray(this.myTransform.position, direction);
            if (Physics.Raycast(ray, out hit, this.scaling))
            {
                if ((hit.collider != null) && (hit.collider.tag.Equals(this.currentTag))) {
                    Nibb neighbor = hit.transform.GetComponent<Nibb>();
                    if (neighbor.currentColor.Equals(this.currentColor))
                    {
                        Debug.Log("NEIGHBOR: " + neighbor.name);
                        this.neighbors.Add(neighbor);
                    }
                }
            }
        }
    }
}
