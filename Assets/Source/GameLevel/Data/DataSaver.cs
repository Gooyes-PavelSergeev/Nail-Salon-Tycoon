using System.Collections;
using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms;
using Gooyes.Tools;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.GameLevel;
using System.Collections.Generic;
using Firebase.Analytics;

namespace NailSalonTycoon.Data
{
    public static class DataSaver
    {
        public static int saveCooldown = 10;
        private static GameLevel.GameLevel _gameLevel;
        private static bool _isActive;
        private static bool isSaving = true;

        public static void Initialize(GameLevel.GameLevel gameLevel)
        {
            _gameLevel = gameLevel;
            _isActive = true;
            Coroutines.StartCoroutine(MyUpdate());
        }

        public static void StopProcess()
        {
            _isActive = false;
        }

        private static IEnumerator MyUpdate()
        {
            yield return new WaitForSeconds(1);

            foreach (Room room in _gameLevel.ActiveRooms) TrackNewRoom(room);
            _gameLevel.RoomAddedEvent += TrackNewRoom;
            Economy.Wallet.MoneyChangedEvent += (money) => Save<Data_SoftCurrency>(null);
            MoodSystem.CurrentMood.OnChanged += (mood) => Save<Data_Mood>(null);
            Economy.DailyRewardSystem.SaveEvent += (data) => Save<Data_DailyReward>(data);
            Save<Data_SessionTime>(null);
            Economy.DailyRewardSystem.Save();

            float timer = 0;

            while (_isActive)
            {
                yield return null;
                timer += Time.deltaTime;
                if (timer > saveCooldown)
                {
                    timer = 0;
                    Save<Data_SessionTime>(null);
                }
            }
        }

        private static void TrackNewRoom(Room room)
        {
            Save<Data_OwnedRooms>(room);
            foreach (Staff staff in room.Staffs) TrackNewStaff(staff, room);
            room.StaffAddedEvent += (staff) => TrackNewStaff(staff, room);
            room.OwnedUpgrades.OnChanged += (list) => Save<Data_OwnedRooms>(room);
        }

        private static void TrackNewStaff(Staff staff, Room room)
        {
            Save<Data_OwnedRooms>(room);
            staff.UpgradeEvent += (data) => Save<Data_OwnedRooms>(room);
        }

        public static void Save<T>(object objectToSave) where T : DataToSave, new()
        {
            if (!isSaving) return;
            T data = new T();
            data.Save(objectToSave);
        }

        public static T Load<T>() where T : DataToSave, new()
        {
            T data = new T();
            return data.Load<T>();
        }

        /// <summary>
        /// Clear all data for given type
        /// </summary>
        public static void Clear<T>() where T : DataToSave, new()
        {
            T data = new T();
            data.Clear();
        }

        /// <summary>
        /// Clear all data for all types
        /// </summary>
        public static void ClearAll(bool dontSaveAfter = true)
        {
            Data_OwnedRooms dataRooms = new Data_OwnedRooms();
            dataRooms.Clear();
            Data_Mood dataMood = new Data_Mood();
            dataMood.Clear();
            Data_SessionTime dataTime = new Data_SessionTime();
            dataTime.Clear();
            Data_SoftCurrency dataSoft = new Data_SoftCurrency();
            dataSoft.Clear();
            Data_Tutorial dataTutorial = new Data_Tutorial();
            dataTutorial.Clear();
            Data_SoundVolume dataVolume = new Data_SoundVolume();
            dataVolume.Clear();
            Data_DailyReward dataDailyReward = new Data_DailyReward();
            dataDailyReward.Clear();
            Data_EventCooldown dataEventCooldown = new Data_EventCooldown();
            dataEventCooldown.Clear();

            if(dontSaveAfter) Debug.LogWarning("CLEAR");
            isSaving = !dontSaveAfter;
        }
    }
}
