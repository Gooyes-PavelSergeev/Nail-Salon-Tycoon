using System;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    internal class Data_DailyReward : DataToSave
    {
        public const string dateKey = "FromDailyRewardDate";
        public const string dayKey = "RewardDay";
        public float daysFromSave;
        public Economy.RewardDayInfo[] days;
        private static bool _rewrite = false;
        public override T Load<T>()
        {
            Data_DailyReward data = new Data_DailyReward();
            int length = Enum.GetNames(typeof(Economy.RewardDay)).Length;

            data.days = new Economy.RewardDayInfo[length];
            data.daysFromSave = 0;

            for (int i = 0; i < length; i++)
            {
                int isCollectedInt = PlayerPrefs.GetInt($"{dayKey}{i + 1}", 0);
                bool isCollected = isCollectedInt == 1;
                data.days[i] = new Economy.RewardDayInfo((Economy.RewardDay)i, isCollected);
            }

            string lastSaveDateTimeString = PlayerPrefs.GetString(dateKey, "false");
            if (lastSaveDateTimeString != "false")
            {
                if (DateTime.TryParse(lastSaveDateTimeString, out DateTime lastSaveDateTime))
                {
                    TimeSpan timeSpan = DateTime.Now - lastSaveDateTime;
                    data.daysFromSave = (float)timeSpan.TotalDays;
                }
            }

            return data as T;
        }

        public override void Save(object sender)
        {
            if (sender is not Economy.RewardSaveData data) throw new Exception("Send RewardSaveData to daily reward data");
            if (data.rewrite && !_rewrite) _rewrite = true;
            if (data.rewrite) PlayerPrefs.SetString(dateKey, DateTime.Now.ToString());
            else if (!PlayerPrefs.HasKey(dateKey))
            {
                PlayerPrefs.SetString(dateKey, DateTime.Now.ToString());
            }
            for (int i = 0; i < data.dailies.Length; i++)
            {
                if (_rewrite) PlayerPrefs.DeleteKey($"{dayKey}{(int)data.dailies[i].day + 1}");
                else PlayerPrefs.SetInt($"{dayKey}{(int)data.dailies[i].day + 1}", data.dailies[i].isCollected ? 1 : 0);
            }
        }

        public override void Clear()
        {
            _rewrite = false;
            PlayerPrefs.DeleteKey(dateKey);
            for (int i = 0; i < Enum.GetNames(typeof(Economy.RewardDay)).Length; i++)
                PlayerPrefs.SetInt($"{dayKey}{i + 1}", 0);
        }
    }
}
