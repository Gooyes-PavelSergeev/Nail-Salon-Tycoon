using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    public class Data_EventCooldown : DataToSave
    {
        private const string KEY_VIP = "AdEventVIP";
        private const string KEY_POPULARITY = "AdEventPopularity";
        private const string KEY_INSPIRATION = "AdEventInspiration";
        private Dictionary<EventType, string> KEY_MAP = new Dictionary<EventType, string>
        {
            { EventType.VIP, KEY_VIP },
            { EventType.Popularity, KEY_POPULARITY },
            { EventType.Inspiration, KEY_INSPIRATION }
        };
        public float vipTimer = 7;
        public float popularityTimer = 7;
        public float inspirationTimer = 7;
        public override T Load<T>()
        {
            Data_EventCooldown data = new Data_EventCooldown();
            for (int i = 0; i < Enum.GetNames(typeof(EventType)).Length; i++)
            {
                string key = KEY_MAP[(EventType)i];
                string timeString = PlayerPrefs.GetString(key, "true");
                if (timeString == "true")
                {
                    SetKeyTimer(data, key, 10);
                    continue;
                }
                if (DateTime.TryParse(timeString, out DateTime lastSaveTime))
                {
                    TimeSpan timeSpan = DateTime.Now - lastSaveTime;
                    float minutes = (float)timeSpan.TotalMinutes;
                    minutes = Mathf.Clamp(minutes, 0, 10);
                    SetKeyTimer(data, key, minutes);
                    continue;
                }
                throw new Exception($"Time {timeString} is wrong");
            }
            return data as T;
        }

        public override void Save(object sender)
        {
            EventType? dataNullable = sender as EventType?;
            if (dataNullable == null) throw new Exception("Send EventType to EventCooldown Save()"); 
            EventType type = dataNullable.Value;
            string key = KEY_MAP[type];
            string time = DateTime.Now.ToString();
            PlayerPrefs.SetString(key, time);
        }

        public override void Clear()
        {
            PlayerPrefs.DeleteKey(KEY_VIP);
            PlayerPrefs.DeleteKey(KEY_POPULARITY);
            PlayerPrefs.DeleteKey(KEY_INSPIRATION);
        }

        private void SetKeyTimer(Data_EventCooldown data, string key, float timer)
        {
            switch (key)
            {
                case KEY_VIP:
                    data.vipTimer = timer;
                    break;
                case KEY_POPULARITY:
                    data.popularityTimer = timer;
                    break;
                case KEY_INSPIRATION:
                    data.inspirationTimer = timer;
                    break;
            }
        }

        public enum EventType
        {
            VIP,
            Popularity,
            Inspiration
        }
    }
}
