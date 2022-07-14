using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class ColumnShiftInstance
    {
        internal bool ColumnHasNibbs { get; set; } = false;
        internal Transform TColumn { get; set; } = null;
        internal float OriginalRotation { get; set; } = 0f;
        internal float DegreesToRotate { get; set; } = 0f;
    }
    internal class ColumnShifter : SerializedMonoBehaviour
    {
        internal EventIn_RotateColumns EventIn_RotateColumns = new EventIn_RotateColumns();
        internal EventOut_ColumnShiftingDone EventOut_ColumnShiftingDone = new EventOut_ColumnShiftingDone();

        [SerializeField] private AnimationCurve animCurveRotation = null;
        [SerializeField] private float rotationDuration = 1f;

        private List<ColumnShiftInstance> shiftInstances = new List<ColumnShiftInstance>();
        internal bool VarOut_IsPerformingRotationAnimation { get; private set; } = false;
        private float startRotationTime = 0f;

        internal void Init()
        {
            EventIn_RotateColumns.AddListener(RotateColumns);
        }

        private void RotateColumns(List<ColumnShiftInstance> shiftInstances, float degreesByColumn)
        {
            StartCoroutine(RotateColumnsDelayed(shiftInstances, degreesByColumn));
        }

        private IEnumerator RotateColumnsDelayed(List<ColumnShiftInstance> shiftInstances, float degreesByColumn)
        {
            yield return new WaitForEndOfFrame();

            this.shiftInstances = shiftInstances;
            this.GetRotationDegrees(shiftInstances, degreesByColumn);
            Debug.Log("Rotate COlumns now! " + this.VarOut_IsPerformingRotationAnimation);
            if (this.VarOut_IsPerformingRotationAnimation)
            {
                this.Rotate();
            }
            else
            {
                this.ReportRotationFinished();
            }
        }

        private void GetRotationDegrees(List<ColumnShiftInstance> shiftInstances, float degreesByColumn)
        {
            int rotationSteps = 0;
            float currentRotation = 0f;
            bool addedRotationStep = false;
            for (int i = 0; i < shiftInstances.Count; i++)
            {
                if (!shiftInstances[i].ColumnHasNibbs)
                {
                    rotationSteps++;
                    addedRotationStep = true;
                    Debug.Log("ROTATION STEPS: " + rotationSteps);
                }
                else
                {
                    Debug.Log("NOPE: " + i + " " + addedRotationStep);
                    if (addedRotationStep)
                    {
                        currentRotation = rotationSteps * degreesByColumn;
                        addedRotationStep = false;
                    }
                    shiftInstances[i].DegreesToRotate = currentRotation;
                }
                shiftInstances[i].OriginalRotation = shiftInstances[i].TColumn.localEulerAngles.y;
            }
            this.VarOut_IsPerformingRotationAnimation = rotationSteps > 0;
        }

        private void Rotate()
        {
            this.startRotationTime = Time.realtimeSinceStartup;
            NibbsGameHandler.EventOut_OnUpdate.AddListenerSingle(OnUpdate);
            Debug.Log("ROTATE NOW!");
        }

        private void OnUpdate()
        {
            float animCurvePosition = (Time.realtimeSinceStartup - this.startRotationTime) / this.rotationDuration;
            float rotationPercent = this.animCurveRotation.Evaluate(animCurvePosition);
            Debug.Log("Rotation percentos: " + animCurvePosition + " " + rotationPercent + " " + shiftInstances.Count);
            for (int i = 0; i < this.shiftInstances.Count; i++)
            {
                this.shiftInstances[i].TColumn.localEulerAngles = new Vector3(
                    this.shiftInstances[i].TColumn.localEulerAngles.x,
                    this.shiftInstances[i].OriginalRotation + (this.shiftInstances[i].DegreesToRotate * rotationPercent),
                    this.shiftInstances[i].TColumn.localEulerAngles.z
                );
                if (i == 0)
                {
                    //Debug.Log("SHIFT INSTANCES: " + this.shiftInstances[i].TColumn.name + " " + i + " " + this.shiftInstances[i].TColumn.localEulerAngles.y + " " + rotationPercent);
                }
            }

            if(animCurvePosition >= 1f)
            {
                for (int i = 0; i < this.shiftInstances.Count; i++)
                {
                    this.shiftInstances[i].TColumn.localEulerAngles = new Vector3(
                        this.shiftInstances[i].TColumn.localEulerAngles.x,
                        this.shiftInstances[i].OriginalRotation + this.shiftInstances[i].DegreesToRotate,
                        this.shiftInstances[i].TColumn.localEulerAngles.z
                    );
                    //Debug.Log("SHIFT INSTANCES END: " + this.shiftInstances[i].TColumn.name + " " + i + " " + this.shiftInstances[i].TColumn.localEulerAngles.y);
                }

                ReportRotationFinished();
            }

            //for (int i = 0; i < shiftInstances.Count; i++)
            //{
            //    shiftInstances[i].TColumn.
            //}
            // TODO: continue rotating columns here!
        }

        private void ReportRotationFinished()
        {
            Debug.Log("Rotation finished!");
            VarOut_IsPerformingRotationAnimation = false;
            NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
            EventOut_ColumnShiftingDone.Invoke();
        }
    }
}