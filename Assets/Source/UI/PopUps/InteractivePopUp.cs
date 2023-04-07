using NailSalonTycoon.GameLevel.Interactivity;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NailSalonTycoon.UI.PopUps
{
    internal abstract class InteractivePopUp : MonoBehaviour, IPopUpPanel
    {
        public bool IsOpened { get; set; }

        private void Start()
        {
            _closeButton.onClick.AddListener(ClosePanel);
        }

        [SerializeField] protected Button _closeButton;
        public event Action<bool> PopUpShowEvent;
        public static event Action<bool, IPopUpPanel> ShowEventStatic;
        public abstract void Initialize(GameLevel.GameLevel gameLevel);
        public abstract void PopUp(InteractData interactData);
        public void ClosePanel()
        {
            IsOpened = false;
            gameObject.SetActive(false);
            PopUpShowEvent?.Invoke(false);
            ShowEventStatic?.Invoke(false, this);
        }
        protected abstract void FillData();
        protected void InvokePopUpShowEvent(bool showState)
        {
            PopUpShowEvent?.Invoke(showState);
            ShowEventStatic?.Invoke(showState, this);
        }
    }
}
