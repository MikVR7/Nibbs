using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class BallRotator : MonoBehaviour
    {
        [SerializeField] private GameObject prefabRotatorColumn = null;
        private List<BallRotatorColumn> rotatorColumns = new List<BallRotatorColumn>();

        internal void Init(int columns)
        {
            for(int i = 0; i < columns; i++)
            {
                this.CreateRotatorColumn(i);
            }
        }

        private void CreateRotatorColumn(int index)
        {
            GameObject goRotatorColumn = Instantiate(this.prefabRotatorColumn);
            goRotatorColumn.name = "rot_col_" + index;
            Transform tRotatorColumn = goRotatorColumn.GetComponent<Transform>();
            tRotatorColumn.parent = tRotatorColumn;
            tRotatorColumn.localPosition = Vector3.zero;
            BallRotatorColumn rotatorColumn = goRotatorColumn.GetComponent<BallRotatorColumn>();
            rotatorColumn.Init(index);
            this.rotatorColumns.Add(rotatorColumn);
        }
    }
}