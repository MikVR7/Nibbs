using System;
using System.Collections;
using UnityEngine;

namespace Nibbs
{
    internal class NibbsEffect : MonoBehaviour
    {
        [SerializeField] private GameObject particleEffect = null;
        internal bool VarOut_IsActive { get; private set; } = false;
        private Action<NibbsEffect> eventEffectReady = null;
        private Transform myTransform = null;

        internal void Init(Action<NibbsEffect> eventEffectReady, Transform parent)
        {
            this.eventEffectReady = eventEffectReady;
            this.myTransform = this.GetComponent<Transform>();
            this.myTransform.parent = parent;
            this.myTransform.position = Vector3.zero;
            this.SetActiveEffect(false);
        }

        private void SetActiveEffect(bool active)
        {
            this.gameObject.SetActive(active);
            particleEffect.SetActive(active);
            VarOut_IsActive = active;
        }

        internal void PerformNibbsEffect(Vector3 worldPosition)
        {
            this.myTransform.position = worldPosition;
            SetActiveEffect(true);
            StartCoroutine(TurnOffNibbsEffect());
        }

        private IEnumerator TurnOffNibbsEffect()
        {
            yield return new WaitForSecondsRealtime(3f);
            SetActiveEffect(false);
            this.eventEffectReady.Invoke(this);
        }
    }
}
