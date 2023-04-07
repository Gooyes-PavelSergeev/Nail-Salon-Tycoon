using System;
using Firebase.Analytics;
using Gooyes.Tools;
using UnityEngine;

namespace NailSalonTycoon.Economy
{
    public static class Wallet
    {
        private static Observable<float> _moneyAmount = new Observable<float>(0);

        public static float MoneyAmount { get => _moneyAmount.Value; }

        public static event Action<float> MoneyChangedEvent;
        
        private const int maxMoneyAmount = 1000000000;

        private static bool _devMode;

        public static void Initialize(bool devMode, float moneyAmount)
        {
            _devMode = devMode;
            _moneyAmount.OnChanged += OnMoneyChanged;
            _moneyAmount.Value = moneyAmount;
        }

        private static void OnMoneyChanged(float newValue)
        {
            MoneyChangedEvent?.Invoke(newValue);
        }

        public static bool CheckEnoughMoney(float price)
        {
            if (price > _moneyAmount.Value)
            {
                return _devMode;
            }
            return true;
        }

        public static bool SpendMoney(float price, IEconomyAgent sender, out float newMoneyAmount)
        {
            if (price > _moneyAmount.Value)
            {
                newMoneyAmount = _moneyAmount.Value;
                return _devMode;
            }

            _moneyAmount.Value -= price;
            newMoneyAmount = _moneyAmount.Value;
            return true;
        }

        public static bool AddMoney(float amount, IEconomyAgent sender, out float newMoneyAmount)
        {
            if (amount + _moneyAmount.Value > maxMoneyAmount)
            {
                newMoneyAmount = _moneyAmount.Value;
                return false;
            }

            _moneyAmount.Value += amount;
            newMoneyAmount = _moneyAmount.Value;
            return true;
        }
    }
}
