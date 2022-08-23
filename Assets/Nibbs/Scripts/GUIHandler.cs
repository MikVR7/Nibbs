using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Nibbs.Events;

namespace Nibbs
{
    internal class GUIHandler : MonoBehaviour
    {
        internal static EventOut_OnBtnStart EventOut_OnBtnStart = new EventOut_OnBtnStart();
        internal static EventIn_SetPointsCount EventIn_SetPointsCount = new EventIn_SetPointsCount();

        [SerializeField] private Button btnStart = null;
        [SerializeField] private TextMeshPro tmpPointsCounter = null;

        internal void Init()
        {
            this.btnStart.onClick.AddListener(OnBtnStart);
            EventIn_SetPointsCount.AddListener(SetPointsCount);
        }

        private void OnBtnStart()
        {
            this.btnStart.gameObject.SetActive(false);
            EventOut_OnBtnStart.Invoke();
        }

        private void SetPointsCount(int count)
        {
            tmpPointsCounter.text = count.ToString();
        }
    }
}