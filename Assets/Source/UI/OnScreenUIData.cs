using UnityEngine;
using Gooyes.Tools;
using TMPro;
using NailSalonTycoon.Economy;
using NailSalonTycoon.Economy.UpgradeSystem.Staffs;
using NailSalonTycoon.GameLevel;
using NailSalonTycoon;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using System.Collections.Generic;
using System.Collections;

namespace NailSalonTycoon.UI
{
    internal class OnScreenUIData : Singleton<OnScreenUIData>
    {
        [SerializeField] private TextMeshProUGUI _currentMoneyText;

        [SerializeField] private TextMeshProUGUI _incomePerMinuteText;

        [SerializeField] private TextMeshProUGUI _gemText;

        private GameLevel.GameLevel _gameLevel;

        private void Start()
        {
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.3f);
            Wallet.MoneyChangedEvent += OnMoneyChange;
            HardWallet.HardChangedEvent += OnHardChange;
            _gameLevel = LevelLoader.Instance.currentLevel.Value;
            _gameLevel.Staffs.OnChanged += SubscribeToNewStaff;
            foreach (Staff staff in _gameLevel.Staffs.Value)
            {
                staff.UpgradeEvent += OnStaffUpgrade;
            }
            MoodSystem.CurrentMood.OnChanged += OnMoodChange;
            OnMoneyChange(Wallet.MoneyAmount);
            OnStaffUpgrade(new UpgradeData());
        }

        private void OnHardChange(float value)
        {
            _gemText.text = TextDisplayer.GetInfo(value, 2).AmountWithPrefix;
        }

        public void SubscribeToNewStaff(List<Staff> staffs)
        {
            Staff staff = staffs[staffs.Count - 1];
            staff.UpgradeEvent += OnStaffUpgrade;
            OnStaffUpgrade(staff.LastUpgradeData.Value);
        }

        public void OnMoneyChange(float value)
        {
            _currentMoneyText.text = TextDisplayer.GetInfo(value, 2).AmountWithPrefix;
        }

        private void OnMoodChange(float mood)
        {
            OnStaffUpgrade(new UpgradeData());
        }

        public void OnStaffUpgrade(UpgradeData data)
        {
            float maxIncome = _gameLevel.GetMaxIncomePerMinute();
            _incomePerMinuteText.text = $"{TextDisplayer.GetInfo(maxIncome, 2).AmountWithPrefix}/min";
        }
    }
}
