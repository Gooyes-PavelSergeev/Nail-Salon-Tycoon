using System;
using UnityEngine.UI;
using TMPro;

namespace NailSalonTycoon.UI.PopUps
{
    public class SettingsPopUp : UIButtonPopUp
    {
        public Scrollbar musicScrollbar;
        private Image _musicForeground;

        public Scrollbar soundScrollbar;
        private Image _soundForeground;
        public override void Initialize()
        {
            _musicForeground = musicScrollbar.GetComponent<Image>();
            musicScrollbar.onValueChanged.AddListener((value) => _musicForeground.fillAmount = 1 - value);

            _soundForeground = soundScrollbar.GetComponent<Image>();
            soundScrollbar.onValueChanged.AddListener((value) => _soundForeground.fillAmount = 1 - value);

            ClosePanel();
        }

        protected override void ShowPanel()
        {
            IsOpened = true;
            gameObject.SetActive(true);
            InvokePopUpShowEvent(true);
        }
    }
}
