using UnityEngine;
using UnityEngine.UI;
using static Nibbs.Events;

namespace Nibbs
{
    internal class GUIHandler : MonoBehaviour
    {
        internal static EventOut_OnBtnStart EventOut_OnBtnStart = new EventOut_OnBtnStart();

        [SerializeField] private Button btnStart = null;

        internal void Init()
        {
            this.btnStart.onClick.AddListener(OnBtnStart);
        }

        private void OnBtnStart()
        {
            this.btnStart.gameObject.SetActive(false);
            EventOut_OnBtnStart.Invoke();
        }
    }
}