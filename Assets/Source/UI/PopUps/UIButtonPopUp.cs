using System;
using UnityEngine;
using UnityEngine.UI;

namespace NailSalonTycoon.UI.PopUps
{
    public abstract class UIButtonPopUp : MonoBehaviour, IPopUpPanel
    {
        [SerializeField] protected Button _targetButton;

        [SerializeField] protected Button _closeButton;

        public event Action<bool> PopUpShowEvent;
        public static event Action<bool, IPopUpPanel> ShowEventStatic;

        public bool IsOpened { get; set; }

        public void SetButtonsOnClick()
        {
            if (_targetButton == null || _closeButton == null)
                throw new Exception($"Set up UI button script for {gameObject.name}");
            _targetButton.onClick.AddListener(ShowPanel);
            _closeButton.onClick.AddListener(ClosePanel);
        }

        public void ClosePanel()
        {
            IsOpened = false;
            gameObject.SetActive(false);
            InvokePopUpShowEvent(false);
        }

        public abstract void Initialize();

        protected abstract void ShowPanel();

        protected void InvokePopUpShowEvent(bool showState)
        {
            PopUpShowEvent?.Invoke(showState);
            ShowEventStatic?.Invoke(showState, this);
        }
    }
}
