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
        internal EventIn_RotateLevel EventIn_RotateLevel = new EventIn_RotateLevel();
        internal EventOut_ColumnShiftingDone EventOut_ColumnShiftingDone = new EventOut_ColumnShiftingDone();

        [SerializeField] private AnimationCurve animCurveRotation = null;
        [SerializeField] private float rotationDuration = 1f;

        private List<ColumnShiftInstance> shiftInstances = new List<ColumnShiftInstance>();
        internal bool VarOut_IsPerformingRotationAnimation { get; private set; } = false;
        private float startRotationTime = 0f;
        //[SerializeField] private Transform tLevel = null;
        private Transform tColumnsHolder = null;

        // level rotation
        private bool rotateLevel = false;
        private float levelRotateY = 0f;
        private float levelRotationYStart = 0f;


#if UNITY_EDITOR
        [SerializeField] private int activeColumns = 0;
        [Button("Rotate Level")]
        private void OnBtnRotateLevel()
        {
            this.RotateLevel(true, activeColumns);
        }
#endif

        internal void Init(Transform tColumnsHolder)
        {
            this.tColumnsHolder = tColumnsHolder;
            EventIn_RotateColumns.AddListenerSingle(RotateColumns);
            EventIn_RotateLevel.AddListenerSingle(RotateLevel);
        }

        private void RotateLevel(bool immediately, int activeColumns/*, Transform tLevel*/)
        {
            float degreesByColumn = LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnsDegree / LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnCount;
            float rotationAmount = -(((activeColumns-1) / 2f) * degreesByColumn)/*-90f*/;
            if (immediately)
            {
                tColumnsHolder.localEulerAngles = new Vector3(tColumnsHolder.localEulerAngles.x, rotationAmount, tColumnsHolder.localEulerAngles.z);
                this.rotateLevel = false;
            }
            else
            {
                this.levelRotateY = rotationAmount+360f;
                this.levelRotationYStart = tColumnsHolder.localEulerAngles.y;
                this.rotateLevel = true;
            }
        }

        private void RotateColumns(List<ColumnShiftInstance> shiftInstances)
        {
            StartCoroutine(RotateColumnsDelayed(shiftInstances));
        }

        private IEnumerator RotateColumnsDelayed(List<ColumnShiftInstance> shiftInstances)
        {
            yield return new WaitForEndOfFrame();

            this.shiftInstances = shiftInstances;
            float degreesByColumn = LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnsDegree / LevelsHandler.VarOut_Level.VarOut_GetLevel().ColumnCount;
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
            int activeColumnCount = 0;
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
                    activeColumnCount++;
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
            this.RotateLevel(false, activeColumnCount);
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
            for (int i = 0; i < this.shiftInstances.Count; i++)
            {
                if (shiftInstances[i] != null)
                {
                    this.shiftInstances[i].TColumn.localEulerAngles = new Vector3(
                        this.shiftInstances[i].TColumn.localEulerAngles.x,
                        this.shiftInstances[i].OriginalRotation - (this.shiftInstances[i].DegreesToRotate * rotationPercent),
                        this.shiftInstances[i].TColumn.localEulerAngles.z
                    );
                }
                if (i == 0)
                {
                    //Debug.Log("SHIFT INSTANCES: " + this.shiftInstances[i].TColumn.name + " " + i + " " + this.shiftInstances[i].TColumn.localEulerAngles.y + " " + rotationPercent);
                }
            }

            if (this.rotateLevel) {
                float angleY = this.levelRotationYStart + ((this.levelRotateY - levelRotationYStart) * rotationPercent);
                this.tColumnsHolder.localEulerAngles = new Vector3(tColumnsHolder.localEulerAngles.x, angleY, tColumnsHolder.localEulerAngles.z);
                Debug.Log("ROTATE ANGLE Y: " + angleY + " " + levelRotationYStart + " " + this.levelRotateY + " (" + (levelRotationYStart - this.levelRotateY) + ")");
            }

            if (animCurvePosition >= 1f)
            {
                for (int i = 0; i < this.shiftInstances.Count; i++)
                {
                    if (shiftInstances[i] != null)
                    {
                        this.shiftInstances[i].TColumn.localEulerAngles = new Vector3(
                        this.shiftInstances[i].TColumn.localEulerAngles.x,
                        this.shiftInstances[i].OriginalRotation - this.shiftInstances[i].DegreesToRotate,
                        this.shiftInstances[i].TColumn.localEulerAngles.z
                        );
                    }
                }
                if (this.rotateLevel)
                {
                    this.tColumnsHolder.localEulerAngles = new Vector3(tColumnsHolder.localEulerAngles.x, this.levelRotateY, tColumnsHolder.localEulerAngles.z);
                    this.rotateLevel = false;
                }
                ReportRotationFinished();
            }
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