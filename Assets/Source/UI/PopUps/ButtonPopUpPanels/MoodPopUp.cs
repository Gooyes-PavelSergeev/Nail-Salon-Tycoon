using NailSalonTycoon.GameLevel;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace NailSalonTycoon.UI.PopUps
{
    internal class MoodPopUp : UIButtonPopUp
    {
        [SerializeField] private TextMeshProUGUI _moodText;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private MoodButtonConfig[] _moodButtons;

        private Image _buttonImage;

        public override void Initialize()
        {
            IsOpened = false;
            _buttonImage = _targetButton.GetComponent<Image>();
            MoodSystem.CurrentMood.OnChanged += OnMoodChange;
            _closeButton.onClick.RemoveAllListeners();
            _targetButton.onClick.RemoveAllListeners();
            _targetButton.onClick.AddListener(ShowPanel);
            _mainPanel.SetActive(true);
            ShowPanel(false);
            OnMoodChange(MoodSystem.CurrentMood.Value);
            FindObjectOfType<NewCameraControl>().TouchReceivedEvent += OnTouchReceive;
        }

        private void OnTouchReceive(int touchCount)
        {
            if (!IsOpened) return;
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended && !CheckPointerOverGO())
            {
                ShowPanel(false);
            }
        }

        private bool CheckPointerOverGO()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach (RaycastResult raycastResult in raycastResults)
            {
                GameObject raycastGO = raycastResult.gameObject;
                if (raycastGO == _buttonImage.gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void ShowPanel()
        {
            ShowPanel(!IsOpened);
        }

        private void ShowPanel(bool showState)
        {
            IsOpened = showState;
            float mood = 100;
            if (MoodSystem.CurrentMood != null)
                mood = MoodSystem.CurrentMood.Value;
            _moodText.text = $"Current mood is {mood}. Income is x{mood / 100}";
            gameObject.SetActive(showState);
            InvokePopUpShowEvent(showState);
        }

        private void OnMoodChange(float mood)
        {
            MoodButtonState state = MoodButtonState.Normal;
            if (mood < 50) state = MoodButtonState.Low;
            else if (mood >= 100) state = MoodButtonState.High;
            foreach (var button in _moodButtons)
                if (button.state == state)
                {
                    _buttonImage.sprite = button.sprite;
                    break;
                }
        }

        [Serializable]
        private struct MoodButtonConfig
        {
            public MoodButtonState state;
            public Sprite sprite;
        }

        [Serializable]
        private enum MoodButtonState
        {
            Low,
            Normal,
            High
        }
    }
}
