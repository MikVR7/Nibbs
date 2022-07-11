using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class ColumnShiftInstance
    {
        internal bool ColumnHasNibbs { get; set; } = false;
        internal Transform TColumn { get; set; } = null;
        internal float DegreesToRotate { get; set; } = 0f;
    }
    internal class ColumnShifter : SerializedMonoBehaviour
    {
        internal EventIn_RotateColumns EventIn_RotateColumns = new EventIn_RotateColumns();
        internal EventOut_ColumnShiftingDone EventOut_ColumnShiftingDone = new EventOut_ColumnShiftingDone();

        [SerializeField] private AnimationCurve animCurveRotation = null;

        private List<ColumnShiftInstance> shiftInstances = new List<ColumnShiftInstance>();
        private bool performRotationAnimation = false;
        private float startRotationTime = 0f;
        private float rotationDuration = 1f;

        internal void Init()
        {
            EventIn_RotateColumns.AddListener(RotateColumns);
        }

        private void RotateColumns(List<ColumnShiftInstance> shiftInstances, float degreesByColumn)
        {
            this.shiftInstances = shiftInstances;
            this.GetRotationDegrees(shiftInstances, degreesByColumn);
            if (this.performRotationAnimation)
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
                }
                else
                {
                    if (addedRotationStep)
                    {
                        currentRotation = rotationSteps * degreesByColumn;
                        addedRotationStep = false;
                    }
                    shiftInstances[i].DegreesToRotate = currentRotation;
                }
            }
            this.performRotationAnimation = rotationSteps > 0;
        }

        private void Rotate()
        {
            this.startRotationTime = Time.realtimeSinceStartup;
            NibbsGameHandler.EventOut_OnUpdate.AddListenerSingle(OnUpdate);
            
        }

        private void OnUpdate()
        {
            //for (int i = 0; i < shiftInstances.Count; i++)
            //{
            //    shiftInstances[i].TColumn.
            //}
            // TODO: continue rotating columns here!
        }

        private void ReportRotationFinished()
        {
            NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
            EventOut_ColumnShiftingDone.Invoke();
        }
    }
}