using NailSalonTycoon.GameLevel;
using System;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    internal class Data_Mood : DataToSave
    {
        public float moodValue;

        private const string saveKey = "Mood";

        public override T Load<T>()
        {
            Data_Mood data = new Data_Mood();
            data.moodValue = PlayerPrefs.GetFloat(saveKey, MoodSystem.defaultMoodValue);
            return data as T;
        }

        public override void Save(object sender)
        {
            float moodValue = MoodSystem.CurrentMood.Value;
            PlayerPrefs.SetFloat(saveKey, moodValue);
        }

        public override void Clear()
        {
            PlayerPrefs.SetFloat(saveKey, MoodSystem.defaultMoodValue);
        }
    }
}
