using UnityEngine;

namespace Nibbs
{
    internal class BallRotatorColumn : MonoBehaviour
    {
        private int index = 0;
        internal void Init(int index)
        {
            this.index = index;
        }
    }
}