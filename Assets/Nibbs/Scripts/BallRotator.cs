using CodeEvents;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class EventIn_RotateForDebugging : EventSystem { }
    internal class EventIn_CloseColumnGaps : EventSystem { }
    internal class BallRotator : MonoBehaviour
    {
        internal static EventIn_CloseColumnGaps EventIn_CloseColumnGaps= new EventIn_CloseColumnGaps();
        internal static EventIn_RotateForDebugging EventIn_RotateForDebugging = new EventIn_RotateForDebugging();

        //[SerializeField] private GameObject prefabRotatorColumn = null;
        //private List<BallRotatorColumn> rotatorColumns = new List<BallRotatorColumn>();
        [SerializeField] private GameObject prefabParentGroup = null;
        private List<BallRotatorParentGroup> ballRotatorParentGroups = new List<BallRotatorParentGroup>();
        private List<List<int>> columnGroups = new List<List<int>>();
        private Transform myTransform = null;

        internal void Init(/*int columns*/)
        {
            EventIn_CloseColumnGaps.AddListener(CloseColumnGaps);
            EventIn_RotateForDebugging.AddListener(RotateForDebugging);
            this.myTransform = this.GetComponent<Transform>();
            //for (int i = 0; i < columns; i++)
            //{
            //    this.CreateRotatorColumn(i);
            //}
        }

        //private void CreateRotatorColumn(int index)
        //{
        //    GameObject goRotatorColumn = Instantiate(this.prefabRotatorColumn);
        //    goRotatorColumn.name = "rot_col_" + index;
        //    Transform tRotatorColumn = goRotatorColumn.GetComponent<Transform>();
        //    tRotatorColumn.parent = tRotatorColumn;
        //    tRotatorColumn.localPosition = Vector3.zero;
        //    BallRotatorColumn rotatorColumn = goRotatorColumn.GetComponent<BallRotatorColumn>();
        //    rotatorColumn.Init(index);
        //    this.rotatorColumns.Add(rotatorColumn);
        //}

        private void CloseColumnGaps()
        {
            this.FindNibbsGroups();
            this.CreateParentGroups();
            this.SetNibbsParentToGroup();
        }

        private void FindNibbsGroups()
        {
            List<List<int>> nibbsGrid = NibbsHandler.VarOut_GetNibbsGrid();
            List<List<int>> columnGroups = new List<List<int>>();

            // find group columns
            bool groupFound = false;
            for (int j = 0; j < nibbsGrid[0].Count; j++)
            {
                if(!groupFound && nibbsGrid[0][j] >= 0)
                {
                    columnGroups.Add(new List<int>() { j });
                    Debug.Log("ADD FIRST: " + j);
                    groupFound = true;
                }
                else if(groupFound && nibbsGrid[0][j] >= 0)
                {
                    columnGroups[columnGroups.Count - 1].Add(j);
                    Debug.Log("ADD ANOTHER: " + columnGroups.Count + " " + j);
                }
                else if (groupFound && nibbsGrid[0][j] < 0)
                {
                    groupFound = false;
                    Debug.Log("NO GROUP...");
                }
            }
            // check if the last element is connected to the first one - this is also one group
            if(groupFound && (nibbsGrid[0][0] >= 0) && (columnGroups.Count > 1))
            {
                // in that case combine last group to first one
                for(int i = columnGroups[columnGroups.Count-1].Count-1; i >= 0; i--)
                {
                    columnGroups[0].Insert(0, columnGroups[columnGroups.Count - 1][i]);
                }
                columnGroups.RemoveAt(columnGroups.Count-1);
            }


            //// Debug output
            //for(int i = 0; i < columnGroups.Count; i++)
            //{
            //    Debug.Log("Group: " + i);
            //    for(int j = 0; j < columnGroups[i].Count; j++)
            //    {
            //        Debug.Log("G " + i + " - col:" + columnGroups[i][j]);
            //    }
            //}
        }

        private void CreateParentGroups()
        {
            this.ballRotatorParentGroups.Clear();
            for (int i = 0; i < columnGroups.Count; i++)
            {
                GameObject goRotatorGroup = Instantiate(this.prefabParentGroup);
                goRotatorGroup.name = "rot_group_" + i;
                Transform tRotatorGroup = goRotatorGroup.GetComponent<Transform>();
                tRotatorGroup.parent = myTransform;
                tRotatorGroup.localPosition = Vector3.zero;
                BallRotatorParentGroup rotatorGroup = goRotatorGroup.GetComponent<BallRotatorParentGroup>();
                rotatorGroup.Init();
                this.ballRotatorParentGroups.Add(rotatorGroup);
            }
        }

        private void SetNibbsParentToGroup()
        {

        }

        private void RotateForDebugging()
        {

        }
    }
}