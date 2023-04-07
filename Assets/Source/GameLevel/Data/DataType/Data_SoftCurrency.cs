using Firebase.Analytics;
using NailSalonTycoon.Economy;
using System;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    internal class Data_SoftCurrency : DataToSave
    {
        private const string keyName = "SoftMoney";
        public float moneyAmount;
        public override T Load<T>()
        {
            Data_SoftCurrency data = new Data_SoftCurrency();
            data.moneyAmount = PlayerPrefs.GetFloat(keyName, 0);
            return data as T;
        }

        public override void Save(object sender)
        {
            float value = Wallet.MoneyAmount;
            #if UNITY_ANDROID && !UNITY_EDITOR
            float loaded = PlayerPrefs.GetFloat(keyName, -1);
            if (loaded != value)
            {
                FirebaseAnalytics.LogEvent("Tycoon", "Current_Soft_Value", value);
            }
            #endif
            PlayerPrefs.SetFloat(keyName, value);
        }

        public override void Clear()
        {
            PlayerPrefs.SetFloat(keyName, 0);
        }
    }
}
