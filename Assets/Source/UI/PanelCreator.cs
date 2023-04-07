using NailSalonTycoon.GameLevel.Interactivity;
using UnityEngine;
using Gooyes.Tools;
using System.Collections;
using System;

namespace NailSalonTycoon.UI.PopUps
{
    public class PanelCreator : Singleton<PanelCreator>
    {
        private GameLevel.GameLevel _gameLevel;
        private InteractivePopUp[] _interactivePopUps;
        private UIButtonPopUp[] _buttonPopUps;
        public event Action<bool, IPopUpPanel> PanelShowEvent;
        private void Start()
        {
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.3f);
            _gameLevel = LevelLoader.Instance.currentLevel.Value;
            _interactivePopUps = FindObjectsOfType<RoomPopUp>(true);
            foreach (var popUp in _interactivePopUps)
            {
                popUp.Initialize(_gameLevel);
            }

            _buttonPopUps = FindObjectsOfType<UIButtonPopUp>(true);
            foreach (var popUp in _buttonPopUps)
            {
                popUp.SetButtonsOnClick();
                popUp.Initialize();
            }
        }

        private void OnEnable()
        {
            InteractableObject.InteractEvent += CreatePanel;
            UIButtonPopUp.ShowEventStatic += OnPanelShow;
            InteractivePopUp.ShowEventStatic += OnPanelShow;
        }

        private void OnDisable()
        {
            InteractableObject.InteractEvent -= CreatePanel;
            UIButtonPopUp.ShowEventStatic -= OnPanelShow;
            InteractivePopUp.ShowEventStatic -= OnPanelShow;
        }

        private void OnPanelShow(bool showState, IPopUpPanel sender)
        {
            PanelShowEvent?.Invoke(showState, sender);
            if (!showState) return;
            foreach (IPopUpPanel panel in _buttonPopUps)
            {
                if (panel.IsOpened && panel != sender)
                {
                    panel.ClosePanel();
                }
            }

            foreach (IPopUpPanel panel in _interactivePopUps)
            {
                if (panel.IsOpened && panel != sender)
                {
                    panel.ClosePanel();
                }
            }
        }

        public void CreatePanel(InteractData interactData)
        {
            foreach (InteractivePopUp popUpPanel in _interactivePopUps)
            {
                popUpPanel.PopUp(interactData);
            }
        } 
    }
}
