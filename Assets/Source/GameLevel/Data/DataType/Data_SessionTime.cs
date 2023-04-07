using System;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    internal class Data_SessionTime : DataToSave
    {
        public float? minutesFromLastSession;

        private const string keyName = "SessionTime";

        public override T Load<T>()
        {
            Data_SessionTime data = new Data_SessionTime();
            data.minutesFromLastSession = null;
            string lastSessionDateTimeString = PlayerPrefs.GetString(keyName, "false");
            if (lastSessionDateTimeString != "false")
            {
                if (DateTime.TryParse(lastSessionDateTimeString, out DateTime lastSessionDateTime))
                {
                    TimeSpan timeSpan = DateTime.Now - lastSessionDateTime;
                    data.minutesFromLastSession = (float)timeSpan.TotalMinutes;
                }
            }
            Save(null);
            return data as T;
        }

        public override void Save(object sender)
        {
            PlayerPrefs.SetString(keyName, DateTime.Now.ToString());
        }

        public override void Clear()
        {
            PlayerPrefs.SetString(keyName, "false");
        }
    }
}
