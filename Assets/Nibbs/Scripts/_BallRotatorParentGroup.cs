//using CodeEvents;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Nibbs
//{
//    internal class EventIn_SaveOriginalParent : EventSystem <int, int, Transform> { }
//    internal class EventIn_RotateByDegrees : EventSystem<float, float> { }
//    internal class BallRotatorParentGroup : MonoBehaviour
//    {
//        internal class NibbOriginalParent
//        {
//            internal int CircleNr { get; set; } = 0;
//            internal int IndexInCircle { get; set; } = 0;
//            internal Transform OriginalParent { get; set; } = null;
//        }

//        internal EventIn_SaveOriginalParent EventIn_SaveOriginalParent = new EventIn_SaveOriginalParent();
//        internal EventIn_RotateByDegrees EventIn_RotateByDegrees = new EventIn_RotateByDegrees();

//        internal Transform VarOut_MyTransform { get; private set; } = null;
//        internal List<int> VarOut_ColumnGroup { get; private set; } = new List<int>();

//        internal List<NibbOriginalParent> VarOut_NibbsOriginalParents { get; private set; } = new List<NibbOriginalParent>();
//        internal int VarOut_Index { get; private set; } = 0;
//        private float startRotationTime = 0f;
//        private AnimationCurve animCurveRotation = null;
//        private float rotationDuration = 0f;
//        private float startRotationY = 0f;
//        private float degreesToRotate = 0f;

//        internal void Init(int index, List<int> columnGroup, Transform parent, AnimationCurve animCurveRotation)
//        {
//            this.VarOut_Index = index;
//            this.VarOut_ColumnGroup = columnGroup;
//            this.animCurveRotation = animCurveRotation;

//            this.gameObject.name = "rot_group_" + index;
//            this.VarOut_MyTransform = this.GetComponent<Transform>();
//            this.VarOut_MyTransform.parent = parent;
//            this.VarOut_MyTransform.localPosition = Vector3.zero;

//            EventIn_SaveOriginalParent.AddListener(SaveOriginalParent);
//            EventIn_RotateByDegrees.AddListener(RotateByDegrees);
//        }

//        private void SaveOriginalParent(int circleNr, int indexInCircle, Transform originalParent)
//        {
//            NibbOriginalParent nibbOriginalParent = new NibbOriginalParent()
//            {
//                CircleNr = circleNr,
//                IndexInCircle = indexInCircle,
//                OriginalParent = originalParent
//            };
//            VarOut_NibbsOriginalParents.Add(nibbOriginalParent);
//        }

//        private void RotateByDegrees(float degrees, float rotationDuration)
//        {
//            this.startRotationTime = Time.time;
//            this.rotationDuration = rotationDuration;
//            this.degreesToRotate = degrees;
//            this.startRotationY = this.VarOut_MyTransform.localEulerAngles.y;
//            NibbsGameHandler.EventOut_OnUpdate.AddListener(OnUpdate);
//        }

//        private void OnUpdate()
//        {
//            float curveAmount = this.animCurveRotation.Evaluate((Time.time - this.startRotationTime) / rotationDuration);
//            Debug.Log("CURVE AMOUNT: " + this.gameObject.name + " " + curveAmount + " " + this.degreesToRotate + " " + (this.startRotationY + (curveAmount * this.degreesToRotate)));
//            this.VarOut_MyTransform.localEulerAngles = new Vector3(
//                this.VarOut_MyTransform.localEulerAngles.x,
//                this.startRotationY + (curveAmount * this.degreesToRotate),
//                this.VarOut_MyTransform.localEulerAngles.z);

//            if(curveAmount >= 1f)
//            {
//                this.VarOut_MyTransform.localEulerAngles = new Vector3(
//                this.VarOut_MyTransform.localEulerAngles.x,
//                this.startRotationY + this.degreesToRotate,
//                this.VarOut_MyTransform.localEulerAngles.z);
//                NibbsGameHandler.EventOut_OnUpdate.RemoveListener(OnUpdate);
//                BallRotator.EventIn_FinishedRotatingGroup.Invoke(this.VarOut_Index);
//            }
//        }
//    }
//}