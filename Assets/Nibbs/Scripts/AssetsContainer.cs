using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Nibbs
{
    internal class AssetsContainer : SerializedMonoBehaviour
    {
        internal static AssetsContainer Instance = null;
        [SerializeField] internal Dictionary<NibbColor, Material> nibbsMaterials = new Dictionary<NibbColor, Material>();
        [SerializeField] internal List<Mesh> nibbsMeshes = new List<Mesh>();

        private void Awake()
        {
            Instance = this;
        }
    }
}
