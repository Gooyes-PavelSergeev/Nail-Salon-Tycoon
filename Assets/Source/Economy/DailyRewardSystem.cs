using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.Economy
{
    public static class DailyRewardSystem
    {
        private static RewardDayInfo[] _dailies;
        public static RewardDayInfo[] Dailies {  get { return _dailies; } }

        private static Dictionary<RewardDay, Reward> _rewardMap = new Dictionary<RewardDay, Reward>
        {
            { RewardDay.Day1, new Reward{ isSoft = true, value = 10000 } },
            { RewardDay.Day2, new Reward { isSoft = true, value = 25000 } },
            { RewardDay.Day3, new Reward { isSoft = true, value = 40000 } },
            { RewardDay.Day4, new Reward { isSoft = true, value = 60000 } },
            { RewardDay.Day5, new Reward { isSoft = true, value = 100000 } },
            { RewardDay.Day6, new Reward { isSoft = true, value = 250000 } },
            { RewardDay.Day7, new Reward{ isSoft = true, value = 400000 } }
        };

        public static event Action<RewardSaveData> SaveEvent;

        public static void Initialize(RewardSaveData saveData)
        {
            _dailies = saveData.dailies;
            int daysFromSave = (int)(saveData.daysFromSave.HasValue ? saveData.daysFromSave.Value : 0);
            int maxDays = Enum.GetNames(typeof(RewardDay)).Length;
            if (daysFromSave > maxDays) daysFromSave = maxDays;
            for (int i = 0; i < daysFromSave; i++)
            {
                RewardDayInfo rewardDayInfo = _dailies[i];
                rewardDayInfo.isAvailable = !rewardDayInfo.isCollected;
                _dailies[i] = rewardDayInfo;
            }
        }

        public static void CollectReward(RewardDay day)
        {
            for (int i = 0; i < _dailies.Length; i++)
            {
                if (_dailies[i].day == day)
                {
                    RewardDayInfo dayInfo = _dailies[i];
                    dayInfo.isCollected = true;
                    dayInfo.isAvailable = false;
                    _dailies[i] = dayInfo;
                }
            }
            Save();
            Reward reward = _rewardMap[day];
            if (reward.isSoft) Wallet.AddMoney(reward.value, null, out _);
            else HardWallet.AddHard(reward.value);
        }

        public static void Save()
        {
            RewardSaveData data = new RewardSaveData(_dailies, _dailies[_dailies.Length - 1].isCollected);
            SaveEvent?.Invoke(data);
        }

        public static Reward GetRewardInfo(RewardDay day)
        {
            return _rewardMap[day];
        }
    }

    public class RewardSaveData
    {
        public RewardDayInfo[] dailies;
        public bool rewrite;
        public float? daysFromSave;
        public RewardSaveData(RewardDayInfo[] days, bool rewrite = false)
        {
            dailies = days;
            this.rewrite = rewrite;
            this.daysFromSave = null;
        }

        public RewardSaveData(RewardDayInfo[] days, float hoursFromSave, bool rewrite = false)
        {
            dailies = days;
            this.rewrite = rewrite;
            this.daysFromSave = hoursFromSave;
        }
    }
    public struct Reward { public bool isSoft; public float value; }
    public struct RewardDayInfo
    {
        public RewardDay day;
        public bool isCollected;
        public bool isAvailable;

        public RewardDayInfo(RewardDay day, bool isCollected)
        {
            this.day = day;
            this.isCollected = isCollected;
            this.isAvailable = false;
        }
    }

    public enum RewardDay
    {
        Day1,
        Day2,
        Day3,
        Day4,
        Day5,
        Day6,
        Day7
    }
}
