using System.Collections.Generic;
using UnityEngine;
using static Nibbs.Events;

namespace Nibbs
{
    internal class EffectsHandler : MonoBehaviour
    {
        internal static EventIn_PerformNibbsEffect EventIn_PerformNibbsEffect = new EventIn_PerformNibbsEffect();

        [SerializeField] private GameObject prefabParticleEffect = null;
        [SerializeField] private int particleBuffer = 0;

        private List<NibbsEffect> nibbsEffects = new List<NibbsEffect>();
        private List<NibbsEffect> effectsActive = new List<NibbsEffect>();
        private Transform myTransform = null;


        internal void Init()
        {
            this.myTransform = this.GetComponent<Transform>();
            EventIn_PerformNibbsEffect.AddListenerSingle(PerformNibbsEffect);
            CreateNibbsEffects();
        }

        private void CreateNibbsEffects()
        {
            this.effectsActive.Clear();
            this.nibbsEffects.Clear();
            for (int i = 0; i < particleBuffer; i++)
            {
                GameObject goEffect = Instantiate(this.prefabParticleEffect);
                goEffect.name = "effect_" + i;
                NibbsEffect effect = goEffect.GetComponent<NibbsEffect>();
                effect.Init(OnNibbEffectFinished, this.myTransform);
                this.nibbsEffects.Add(effect);
            }
        }

        private int nibbsEffectsCount = 0;
        private void PerformNibbsEffect(Vector3 worldPosition)
        {
            this.nibbsEffectsCount = this.nibbsEffects.Count;
            if(this.nibbsEffectsCount > 0)
            {
                NibbsEffect effect = this.nibbsEffects[this.nibbsEffectsCount - 1];
                this.effectsActive.Add(effect);
                this.nibbsEffects.Remove(effect);
                effect.PerformNibbsEffect(worldPosition);
            }
        }

        private void OnNibbEffectFinished(NibbsEffect effect)
        {
            this.effectsActive.Remove(effect);
            this.nibbsEffects.Add(effect);
        }
    }
}
