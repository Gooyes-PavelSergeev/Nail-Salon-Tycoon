using NailSalonTycoon.Economy;
using System;
using UnityEngine;

namespace NailSalonTycoon.UI.PopUps
{
    internal class DailyRewardPopUp : UIButtonPopUp
    {
        private RewardDayInfo[] dailies { get => DailyRewardSystem.Dailies; }
        [SerializeField] private GameObject _buttonsContainer;
        private DailyRewardButton[] _buttons;
        public Sprite softPrice;
        public Sprite hardPrice;
        public override void Initialize()
        {
            _buttons = _buttonsContainer.transform.GetComponentsInChildren<DailyRewardButton>(true);
            if (dailies.Length != _buttons.Length) throw new Exception("Your buttons amount doesn't match dailies amount");
            for (int i = 0; i < _buttons.Length; i++)
            {
                RewardDay day = _buttons[i].day;
                _buttons[i].Init(softPrice, hardPrice, DailyRewardSystem.GetRewardInfo(day));
                _buttons[i].button.onClick.AddListener(() => CollectReward(day));
            }
            FillData();
            ClosePanel();
        }

        private void FillData()
        {
            for (int i = 0; i < dailies.Length; i++)
            {
                _buttons[i].isCollected = dailies[i].isCollected;
                _buttons[i].isAvailable.Value = dailies[i].isAvailable;
            }
        }

        private void CollectReward(RewardDay day)
        {
            DailyRewardSystem.CollectReward(day);
            FillData();
        }

        protected override void ShowPanel()
        {
            FillData();
            IsOpened = true;
            gameObject.SetActive(true);
            InvokePopUpShowEvent(true);
        }
    }
}
