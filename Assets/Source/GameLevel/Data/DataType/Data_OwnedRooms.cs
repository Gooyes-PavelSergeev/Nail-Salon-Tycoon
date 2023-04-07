using Firebase.Analytics;
using NailSalonTycoon.Economy.UpgradeSystem.Rooms;
using NailSalonTycoon.GameLevel.Rooms;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    internal class Data_OwnedRooms : DataToSave
    {
        public List<RoomId> ownedRooms;
        public Dictionary<RoomId, List<StaffSaveConfig>> staffMap;
        public Dictionary<RoomId, List<UpgradeId>> interiorMap;

        public override T Load<T>()
        {
            Data_OwnedRooms data = new Data_OwnedRooms();/////////////////////////////Load rooms
            data.ownedRooms = new List<RoomId>();                                   //
            for (int i = 0; i < Enum.GetNames(typeof(RoomId)).Length; i++)          //
            {                                                                       //
                string roomStatus = PlayerPrefs.GetString($"{(RoomId)i}", "false"); //
                if (Convert.ToBoolean(roomStatus))                                  //
                {                                                                   //
                    RoomId room = (RoomId)i;                                        //
                    data.ownedRooms.Add(room);                                      //
                }                                                                   //
            }/////////////////////////////////////////////////////////////////////////End load rooms

            data.staffMap = new Dictionary<RoomId, List<StaffSaveConfig>>();/////////////////////////////Staff load
            foreach (RoomId room in data.ownedRooms)                                                   //
            {                                                                                          //
                List<StaffSaveConfig> staffs = new List<StaffSaveConfig>();                            //
                for (int i = 0; i < 5; i++)                                                            //
                {                                                                                      //
                    string staffSave = PlayerPrefs.GetString($"{room} {i}", "false");                  //
                    if (staffSave.Length > 5)                                                          //
                    {                                                                                  //
                        string[] staffData = staffSave.Split(" ");                                     //
                        if (staffData.Length != 2)                                                     //
                            throw new Exception($"Wrong save configuration for staff {i} in {room}");  //
                        StaffSaveConfig staff = new StaffSaveConfig();                                 //
                        staff.id = i;                                                                  //
                        staff.level = Convert.ToInt32(staffData[1]);                                   //
                        staffs.Add(staff);                                                             //
                    }                                                                                  //
                }                                                                                      //
                if (staffs.Count > 0)                                                                  //
                {                                                                                      //
                    data.staffMap[room] = staffs;////////////////////////////////////////////////////////Staff load
                }
            }

            data.interiorMap = new Dictionary<RoomId, List<UpgradeId>>();////////////////////////////////Interior load
            foreach (RoomId room in data.ownedRooms)                                                   //
            {                                                                                          //
                List<UpgradeId> upgrades = new List<UpgradeId>();                                      //
                for (int i = 0; i < Enum.GetNames(typeof(UpgradeId)).Length; i++)                      //
                {                                                                                      //
                    string interiorString = PlayerPrefs.GetString($"{room} {(UpgradeId)i}", "false");  //
                    if (Convert.ToBoolean(interiorString))                                             //
                    {                                                                                  //
                        upgrades.Add((UpgradeId)i);                                                    //
                    }                                                                                  //
                }                                                                                      //
                if (upgrades.Count > 0)                                                                //
                {                                                                                      //
                    data.interiorMap[room] = upgrades;                                                 //
                }                                                                                      //
            }////////////////////////////////////////////////////////////////////////////////////////////Interior load
            return data as T;
        }

        public override void Save(object sender)
        {
            Room room = sender as Room;
            if (room == null || room.View == null)
            {
                throw new NullReferenceException($"You tried to save wrong object of type {sender.GetType().Name}");
            }
            string roomName = room.Name;
            #if UNITY_ANDROID && !UNITY_EDITOR
            string savedRoom = PlayerPrefs.GetString($"{roomName}", "false");
            if (savedRoom == "false")
            {
                FirebaseAnalytics.LogEvent("Tycoon", "Room_ID", (int)room.RoomId + 1);
            }
            #endif
            PlayerPrefs.SetString($"{roomName}", "true"); //Save Room

            #if UNITY_ANDROID && !UNITY_EDITOR
            int staffCountLoaded = PlayerPrefs.GetInt($"{roomName} count", -1);
            int staffCount = room.StaffCount;
            if (staffCountLoaded != staffCount)
            {
                FirebaseAnalytics.LogEvent("MainScene", $"Room{(int)room.RoomId + 1}_Workers_Count", staffCount);
            }
            #endif
            PlayerPrefs.SetInt($"{roomName} count", room.StaffCount);

            List<Staff> staffs = room.Staffs;
            foreach (Staff staff in staffs) //Save staff id's and levels
            {
                string toSave = $"true {staff.CurrentLevel}";
                string saved = PlayerPrefs.GetString($"{roomName} {staff.Id}", "false");
                PlayerPrefs.SetString($"{roomName} {staff.Id}", toSave);
#if UNITY_ANDROID && !UNITY_EDITOR
                if (saved == "false")
                {
                    FirebaseAnalytics.LogEvent("MainScene", $"Worker_ID_Room{(int)room.RoomId + 1}", staff.Id + 1);
                    FirebaseAnalytics.LogEvent("MainScene", $"Worker_Level_Room{(int)room.RoomId + 1}", staff.CurrentLevel.Value);
                }
                else
                {
                    string[] staffData = saved.Split(" ");
                    int levelSaved = Convert.ToInt32(staffData[1]);
                    if (levelSaved != staff.CurrentLevel.Value)
                    {
                        FirebaseAnalytics.LogEvent("MainScene", $"Worker_Level_Room{(int)room.RoomId + 1}", staff.CurrentLevel.Value);
                    }
                }
                #endif
            }

            List<UpgradeId> interiors = room.OwnedUpgrades.Value;
            foreach (UpgradeId upgrade in interiors) //Save interiors
            {
                PlayerPrefs.SetString($"{roomName} {upgrade}", "true");
            }
        }

        #region Clear
        public override void Clear()
        {
            for (int i = 0; i < Enum.GetNames(typeof(RoomId)).Length; i++)
            {
                RoomId roomId = (RoomId)i;
                PlayerPrefs.SetString($"{roomId}", "false");
                ClearStaffs(roomId);
                ClearInteriors(roomId);
            }
        }

        private void ClearStaffs(RoomId room)
        {
            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetString($"{room} {i}", "false");
            }
        }

        private void ClearInteriors(RoomId room)
        {
            for (int i = 0; i < Enum.GetNames(typeof(UpgradeId)).Length; i++)
            {
                PlayerPrefs.SetString($"{room} {(UpgradeId)i}", "false");
            }
        }
        #endregion

        internal struct StaffSaveConfig
        {
            public int id;
            public int level;
        }
    }
}
