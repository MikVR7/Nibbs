using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class BallRotatorParentGroup : MonoBehaviour
    {
        internal class NibbOriginalParent
        {
            internal int CircleNr { get; set; } = 0;
            internal int IndexInCircle { get; set; } = 0;
            internal Transform OriginalParent { get; set; } = null;
        }

        private List<NibbOriginalParent> nibbsOriginalParents = new List<NibbOriginalParent>();

        internal void Init()
        {
            
        }

        private void SaveNibbOriginalParent(int circleNr, int indexInCircle, Transform originalParent)
        {
            NibbOriginalParent nibbOriginalParent = new NibbOriginalParent()
            {
                CircleNr = circleNr,
                IndexInCircle = indexInCircle,
                OriginalParent = originalParent
            };
            nibbsOriginalParents.Add(nibbOriginalParent);
        }
    }
}