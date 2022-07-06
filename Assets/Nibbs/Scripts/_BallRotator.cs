//using CodeEvents;
//using Sirenix.OdinInspector;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Nibbs
//{
//    internal class EventIn_CloseColumnGaps : EventSystem { }
//    internal class EventIn_FinishedRotatingGroup : EventSystem<int> { }
//    internal class BallRotator : SerializedMonoBehaviour
//    {
//        internal static EventIn_CloseColumnGaps EventIn_CloseColumnGaps= new EventIn_CloseColumnGaps();
//        internal static EventIn_FinishedRotatingGroup EventIn_FinishedRotatingGroup = new EventIn_FinishedRotatingGroup();

//        [SerializeField] private GameObject prefabParentGroup = null;
//        [SerializeField] internal AnimationCurve CurveRotationAnim { get; private set; } = null;
//        [SerializeField] internal float RotationDuration { get; private set; } = 1f;

//        private List<BallRotatorParentGroup> parentGroups = new List<BallRotatorParentGroup>();
//        private List<int> gaps = new List<int>();
//        private Transform myTransform = null;
//        private List<int> rotatingParentGroups = new List<int>();

//        internal void Init()
//        {
//            EventIn_CloseColumnGaps.AddListener(CloseColumnGaps);
//            EventIn_FinishedRotatingGroup.AddListener(FinishedRotatingGroup);
//            this.myTransform = this.GetComponent<Transform>();
//        }

//        private void CloseColumnGaps()
//        {
//            List<List<int>> columnGroups = this.FindNibbsGroups();
//            this.SetNibbsParentToGroup(columnGroups);
//            this.RotateGroups();
//        }

//        private List<List<int>> FindNibbsGroups()
//        {
//            List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
//            List<List<int>> columnGroups = new List<List<int>>();
//            this.gaps.Clear();

//            // find group columns
//            bool groupFound = false;
//            for (int j = 0; j < nibbsGrid[0].Count; j++)
//            {
//                // started a new group
//                if(!groupFound && nibbsGrid[0][j] >= 0)
//                {
//                    columnGroups.Add(new List<int>() { j });
//                    groupFound = true;
//                }
//                // continued an existing group
//                else if(groupFound && nibbsGrid[0][j] >= 0)
//                {
//                    columnGroups[columnGroups.Count - 1].Add(j);
//                }
//                // entered a new gap OR it's the first column and that is already a gap
//                else if ((groupFound && nibbsGrid[0][j] < 0) || (!groupFound && (nibbsGrid[0][j] < 0) && (gaps.Count == 0)))
//                {
//                    gaps.Add(1);
//                    groupFound = false;
//                }
//                // continued an existing gap
//                else if (!groupFound && nibbsGrid[0][j] < 0)
//                {
//                    gaps[gaps.Count - 1]++;
//                }
//            }
//            // check if the last element is connected to the first one - this is also one group
//            if(groupFound && (nibbsGrid[0][0] >= 0) && (columnGroups.Count > 1))
//            {
//                // in that case combine last group to first one
//                for(int i = columnGroups[columnGroups.Count-1].Count-1; i >= 0; i--)
//                {
//                    columnGroups[0].Insert(0, columnGroups[columnGroups.Count - 1][i]);
//                }
//                columnGroups.RemoveAt(columnGroups.Count-1);
//            }
//            return columnGroups;
//        }

//        private void SetNibbsParentToGroup(List<List<int>> columnGroups)
//        {
//            this.parentGroups.Clear();
//            for (int i = 0; i < columnGroups.Count; i++)
//            {
//                BallRotatorParentGroup parentGroup = this.CreateParentGroup(i, columnGroups[i]);
//                for(int j = 0; j < columnGroups[i].Count; j++)
//                {
//                    List<List<int>> nibbsGrid = NibbsGameHandler.VarOut_GetNibbsGrid();
//                    int column = columnGroups[i][j];
//                    for (int k = 0; k < nibbsGrid.Count; k++)
//                    {
//                        Debug.Log("NIBBS GRID: " + k + " " + column + " " + nibbsGrid[k].Count);
//                        if (nibbsGrid[k][column] >= 0)
//                        {
//                            Transform originalParent = NibbsGameHandler.VarOut_GetNibbsParent(k, column);
//                            parentGroup.EventIn_SaveOriginalParent.Invoke(k, column, originalParent);
//                            NibbsGameHandler.EventIn_SetNibbsParent.Invoke(k, column, parentGroup.VarOut_MyTransform);
//                        }
//                    }
//                }
//                this.parentGroups.Add(parentGroup);
//            }
//        }

//        private BallRotatorParentGroup CreateParentGroup(int index, List<int> columnGroup)
//        {
//            GameObject goRotatorGroup = Instantiate(this.prefabParentGroup);
//            BallRotatorParentGroup rotatorGroup = goRotatorGroup.GetComponent<BallRotatorParentGroup>();
//            rotatorGroup.Init(index, columnGroup, myTransform, this.CurveRotationAnim);
//            return rotatorGroup;
//        }

//        private void RotateGroups()
//        {
//            //// no gaps found - don't rotate
//            if((this.gaps.Count == 0) || (this.parentGroups.Count == 0)){ ResetOriginalNibbsParents(); }

//            // add first gap amount
//            int rotationAmount = this.gaps[0];
//            int gapsCount = 0;
//            this.rotatingParentGroups.Clear();
//            bool isRotating = false;
//            // check if first group contains column with index 0
//            // in that case this group doesn't need to be rotated
//            for(int i = (this.parentGroups[0].VarOut_ColumnGroup.Contains(0)) ? 1 : 0; i < this.parentGroups.Count; i++)
//            {
//                this.rotatingParentGroups.Add(this.parentGroups[i].VarOut_Index);
//                this.parentGroups[i].EventIn_RotateByDegrees.Invoke(rotationAmount * NibbsGameHandler.VarOut_GetDegreePerColumn, this.RotationDuration);
//                rotationAmount += this.gaps[++gapsCount];
//                isRotating = true;
//            }

//            // gap found, but nothing to rotate yet!
//            if(!isRotating) { ResetOriginalNibbsParents(); }
//        }

//        private void FinishedRotatingGroup(int groupIndex)
//        {
//            if(this.rotatingParentGroups.Contains(groupIndex))
//            {
//                this.rotatingParentGroups.Remove(groupIndex);
//                if(this.rotatingParentGroups.Count == 0)
//                {
//                    this.ResetOriginalNibbsParents();
//                }
//            }
//        }

//        private void ResetOriginalNibbsParents()
//        {
//            Debug.Log("DONE__________________________________________________");
//            for(int i = 0; i < this.parentGroups.Count; i++)
//            {
//                for(int j = 0; j < this.parentGroups[i].VarOut_NibbsOriginalParents.Count; j++)
//                {
//                    BallRotatorParentGroup.NibbOriginalParent nibbOriginalParent = this.parentGroups[i].VarOut_NibbsOriginalParents[j];
//                    NibbsGameHandler.EventIn_SetNibbsParent.Invoke(
//                        nibbOriginalParent.CircleNr,
//                        nibbOriginalParent.IndexInCircle,
//                        nibbOriginalParent.OriginalParent
//                    );
//                }
//            }
//            this.parentGroups.Clear();
//            this.gaps.Clear();
//        }
//    }
//}