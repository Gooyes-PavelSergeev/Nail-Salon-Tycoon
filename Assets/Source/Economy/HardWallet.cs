using Gooyes.Tools;
using System;
using System.Collections.Generic;

namespace NailSalonTycoon.Economy
{
    public static class HardWallet
    {
        private static Observable<float> _hardValue = new Observable<float>(0);
        public static float HardValue { get => _hardValue.Value; }

        public static event Action<float> HardChangedEvent;

        private const int maxHardAmount = 1000000000;

        private static bool _devMode;

        public static void Initialize(bool devMode, float hardValue)
        {
            _devMode = devMode;
            _hardValue.OnChanged += (value) => HardChangedEvent?.Invoke(value);
            _hardValue.Value = hardValue;
        }

        public static bool CheckEnoughHard(float price)
        {
            if (price > _hardValue.Value)
            {
                return _devMode;
            }
            return true;
        }

        public static bool SpendHard(float price)
        {
            if (price > _hardValue.Value)
            {
                return _devMode;
            }

            _hardValue.Value -= price;
            return true;
        }

        public static bool AddHard(float amount)
        {
            if (amount + _hardValue.Value > maxHardAmount)
            {
                return false;
            }

            _hardValue.Value += amount;
            return true;
        }
    }
}
