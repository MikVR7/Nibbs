
using CodeEvents;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Nibbs.Events;

namespace Nibbs
{
    internal class Nibb : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        [Button("Destroy")]
        private void OnBtnDestroy()
        {
            this.OnClickNibb(null/*null, null*/);
        }
#endif

        internal enum State
        {
            Inited = 0, // ready for level but invisible
            Idle = 1, // visible not moving
            Falling = 2, // visible, physics on
            Destroyed = 3, // invisible
        }

        internal EventIn_SetColor EventIn_SetColor = new EventIn_SetColor();
        internal EventIn_SetNibbState EventIn_SetNibbState = new EventIn_SetNibbState();
        internal EventIn_DestroyNibb EventIn_DisableNibb = new EventIn_DestroyNibb();
        internal EventIn_SetNibbPosY EventIn_SetNibbPosY = new EventIn_SetNibbPosY();
        internal EventIn_SetNibbIndex EventIn_SetNibbIndex = new EventIn_SetNibbIndex();
        internal EventIn_SetColumnIndex EventIn_SetColumnIndex = new EventIn_SetColumnIndex();
        internal EventOut_NibbFinishedFalling EventOut_NibbFinishedFalling = new EventOut_NibbFinishedFalling();

        [SerializeField] private Dictionary<NibbColor, Material> materials = new Dictionary<NibbColor, Material>();
        [SerializeField] private Material matWhite = null;
        [SerializeField] private XRSimpleInteractable xrSimpleInteractable = null;
        [SerializeField] private TextMeshPro tmpText = null;
        private SphereCollider myCollider = null;
        private Rigidbody myRigidbody = null;
        internal Transform VarOut_MyTransform { get; private set; } = null;
        private MeshRenderer myRenderer = null;
        
        private float scaling = 0f;
        private List<Nibb> neighbors = new List<Nibb>();
        [SerializeField] private string currentTag = string.Empty;
        private int columnNr = 0;
        private int indexInColumn = 0;
        private Ray ray = new Ray();
        [SerializeField] internal State VarOut_CurrentState { get; private set; } = State.Idle;

        internal NibbColor VarOut_CurrentColor { get; private set; } = NibbColor.Blue;

        internal void Init(Transform parent, int columnNr, int indexInColumn, float scaling, Vector3 centerPoint)
        {
            EventIn_SetColor.AddListenerSingle(SetColor);
            EventIn_SetNibbState.AddListenerSingle(SetNibbState);
            EventIn_SetNibbPosY.AddListenerSingle(SetNibbPosY);
            EventIn_SetNibbIndex.AddListenerSingle(SetNibbIndex);
            EventIn_SetColumnIndex.AddListenerSingle(SetColumnIndex);
            this.columnNr = columnNr;
            this.indexInColumn = indexInColumn;
            this.scaling = scaling;

            this.gameObject.name = "nibb_" + columnNr + "_" + indexInColumn;
            this.VarOut_MyTransform = this.GetComponent<Transform>();
            this.VarOut_MyTransform.parent = parent;
            this.VarOut_MyTransform.localPosition = Vector3.zero;
            this.VarOut_MyTransform.localScale = new Vector3(scaling,scaling,scaling);
            this.VarOut_MyTransform.LookAt(centerPoint);
            this.myCollider = this.GetComponent<SphereCollider>();
            this.myRigidbody = this.GetComponent<Rigidbody>();
            this.myRenderer = this.GetComponent<MeshRenderer>();
            this.currentTag = this.gameObject.tag;
            this.xrSimpleInteractable.interactionManager = ControllsHandler.VarOut_XRInteractionManager();
            this.xrSimpleInteractable.activated.AddListener(OnClickNibb);
            this.SetNibbState(State.Inited);

            this.SetText();
        }

        private void SetText()
        {
            this.tmpText.text = this.gameObject.name + "<br>" + VarOut_CurrentState;
        }

        private void SetNibbState(State state)
        {
            this.VarOut_CurrentState = state;
            NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
            switch (state)
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
                    this.myRigidbody.AddRelativeForce(new Vector3(0f, -1f, 0f), ForceMode.Impulse);
                    StartCoroutine(StopFalling());
                    break;
                case State.Destroyed:
                    this.gameObject.SetActive(false);
                    break;
            }
            this.SetText();
        }

        private IEnumerator StopFalling()
        {
            yield return new WaitForSeconds(1f);
            NibbsGameHandler.EventOut_OnUpdate.AddListenerSingle(OnUpdate);
        }

        private void SetColor(NibbColor color) {
            this.VarOut_CurrentColor = color;
            this.myRenderer.material = this.materials[this.VarOut_CurrentColor];
        }

        private void SetNibbPosY(float posY)
        {
            this.VarOut_MyTransform.localPosition = new Vector3(
                this.VarOut_MyTransform.localPosition.x,
                posY,
                this.VarOut_MyTransform.localPosition.z);
        }

        private void SetNibbIndex(int index)
        {
            this.indexInColumn = index;
            this.gameObject.name = "nibb_" + this.columnNr + "_" + this.indexInColumn;
            this.SetText();
        }
        private void SetColumnIndex(int index)
        {
            this.columnNr = index;
            this.gameObject.name = "nibb_" + this.columnNr + "_" + this.indexInColumn;
            this.SetText();
        }

        internal void OnClickNibb(ActivateEventArgs arg0/*Hand arg0, Grabbable arg1*/)
        {
            //this.myRenderer.material = this.matWhite;
            List<KeyValuePair<int, int>> nibbsToDestroy = new List<KeyValuePair<int, int>>();
            nibbsToDestroy = DestroySelfAndNeighbors(nibbsToDestroy);
            WinEvaluator.EventIn_EvaluateClickGroup.Invoke(nibbsToDestroy);
        }

        internal List<KeyValuePair<int, int>> DestroySelfAndNeighbors(List<KeyValuePair<int, int>> nibbsToDestroy)
        {
            if(nibbsToDestroy.Contains(new KeyValuePair<int, int>(this.columnNr, this.indexInColumn)) ||
                LevelsHandler.VarOut_LevelState != LevelsHandler.LevelState.Idle/*LevelsHandler.VarOut_NibbsAreFalling || LevelsHandler.VarOut_WinEvaluationRunning*/) { return nibbsToDestroy; }

            GetNeighbors();
            nibbsToDestroy.Add(new KeyValuePair<int, int>(this.columnNr, this.indexInColumn));
            this.neighbors.ForEach(i => nibbsToDestroy = i.DestroySelfAndNeighbors(nibbsToDestroy));
            return nibbsToDestroy;
        }

        private void OnUpdate()
        {
            if(this.myRigidbody.isKinematic == false)
            {
                if(this.myRigidbody.velocity.y < 0.1f)
                {
                    this.neighbors.Clear();
                    this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.down),this.scaling * 0.501f, false);
                    if ((this.indexInColumn == 0) || (this.neighbors.Count > 0) && this.neighbors[0].VarOut_CurrentState.Equals(State.Idle))
                    {
                        StartCoroutine(NibbStoppedFallingDelayed());
                    }
                }
            }
        }

        private IEnumerator NibbStoppedFallingDelayed()
        {
            yield return new WaitForSecondsRealtime(2f);
            NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
            this.SetNibbState(State.Idle);
            this.VarOut_MyTransform.localPosition = new Vector3(
                this.VarOut_MyTransform.localPosition.x,
                (this.scaling / 2f) + (scaling * this.indexInColumn),
                this.VarOut_MyTransform.localPosition.z);
            EventOut_NibbFinishedFalling.Invoke(this.indexInColumn);
        }

        private void GetNeighbors()
        {
            this.neighbors.Clear();
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.up), this.scaling, true);
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.down), this.scaling, true);
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.left), this.scaling, true);
            this.ShootRay(this.VarOut_MyTransform.TransformDirection(Vector3.right), this.scaling, true);
        }

        private void ShootRay(Vector3 direction, float rayLength, bool findEqualColor)
        {
            RaycastHit hit;
            ray.origin = this.VarOut_MyTransform.position;
            ray.direction = direction;
            
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                //Debug.DrawRay(this.VarOut_MyTransform.position, direction, Color.green, 8);
                if ((hit.collider != null) && (hit.collider.tag.Equals(this.currentTag))) {
                    Nibb neighbor = hit.transform.GetComponent<Nibb>();
                    if (findEqualColor && neighbor.VarOut_CurrentColor.Equals(this.VarOut_CurrentColor))
                    {
                        this.neighbors.Add(neighbor);
                    }
                    else if(!findEqualColor)
                    {
                        this.neighbors.Add(neighbor);
                    }
                }
            }
            else
            {
               // Debug.DrawRay(this.VarOut_MyTransform.position, direction, Color.magenta, 8);
            }
        }

        internal bool NibbHasSameColoredNeighbor()
        {
            GetNeighbors();
            return (this.neighbors.Count > 0);
        }
    }
}
