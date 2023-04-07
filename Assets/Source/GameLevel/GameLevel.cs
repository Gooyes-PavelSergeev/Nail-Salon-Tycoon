using System.Collections.Generic;
using UnityEngine;
using Gooyes.Tools;
using NailSalonTycoon.GameLevel.Rooms.Concrete;
using NailSalonTycoon.GameLevel.Rooms;
using System;
using NailSalonTycoon.GameLevel.Clients;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.Economy;
using System.Collections;
using NailSalonTycoon.Data;

namespace NailSalonTycoon.GameLevel
{
    [CreateAssetMenu(fileName = "GameLevel")]
    public class GameLevel : ScriptableObject
    {
        [SerializeField] private int _levelNum;
        public int LevelNum { get => _levelNum; }

        [SerializeField] private LevelUnlockSequence _unlockSequence;

        public List<RoomConfig> roomConfigs;

        [SerializeField] private RoomContainer _roomContainer;

        [SerializeField] private ClientSpawner _clientSpawner;

        private Dictionary<RoomId, Room> _roomMap;

        public Observable<List<Staff>> Staffs { get; private set; }
        public List<Room> ActiveRooms { get; private set; }
        public event Action<Room> RoomAddedEvent;

        private Room _lastActiveRoom;
        private Room _lastAvailableRoom { get => GetNextRoomInSequence(_lastActiveRoom); }
        [SerializeField] private bool _devMode;
        public bool DevMode { get => _devMode; set => _devMode = value; }

        public void Start() 
        {
            Staffs = new Observable<List<Staff>>();
            ActiveRooms = new List<Room>();
            Staffs.Value = new List<Staff>();
            CreateRooms();
            ActivateStartRooms();
            PlaceRooms();
            _clientSpawner.Initialize(this);
            //Wallet.Initialize(_devMode, 500);
            //MoodSystem.Initialize(this, 100);
            DataSaver.Initialize(this);
            LoadData();
        }

        private void CreateRooms()
        {
            _roomMap = new Dictionary<RoomId, Room>();

            foreach (RoomConfig roomConfig in roomConfigs)
            {
                Room room = null;

                switch (roomConfig.id)
                {
                    case RoomId.Reception:
                        room = new RoomReception(roomConfig, _roomContainer);
                        break;
                    case RoomId.Manicure:
                        room = new RoomManicure(roomConfig, _roomContainer);
                        break;
                    case RoomId.Pedicure:
                        room = new RoomPedicure(roomConfig, _roomContainer);
                        break;
                    case RoomId.Rest:
                        room = new RoomRest(roomConfig, _roomContainer);
                        break;
                    case RoomId.Visage:
                        room = new RoomVisage(roomConfig, _roomContainer);
                        break;
                }

                room.StaffAddedEvent += AddStaff;
                _roomMap.Add(roomConfig.id, room);
            }
        }

        private void ActivateStartRooms()
        {
            ActivateRoom(GetRoom(RoomId.Reception));
            ActivateRoom(GetRoom(RoomId.Manicure));
        }

        private void ActivateRoom(Room room)
        {
            room.Activate();
            ActiveRooms.Add(room);
            RoomAddedEvent?.Invoke(room);
            _lastActiveRoom = room;
            Room nextRoom = GetNextRoomInSequence(room);
            if (nextRoom != null) nextRoom.SetAvailable();
        }

        private void PlaceRooms()
        {
            foreach (var kvp in _roomMap)
            {
                kvp.Value.View.SetTransform();
            }
        }

        private Room GetNextRoomInSequence(Room room)
        {
            RoomId id = room.RoomId;
            RoomId? nextId = null;

            if (_unlockSequence.Sequence.Contains(id))
            {
                int indexInSequence = _unlockSequence.Sequence.IndexOf(id);

                if (indexInSequence < _unlockSequence.Sequence.Count - 1)
                    nextId = _unlockSequence.Sequence[indexInSequence + 1];
            }

            if (nextId != null)
                return GetRoom(nextId.Value);

            return null;
        }

        private Room GetRoom(RoomId id)
        {
            if (_roomMap.TryGetValue(id, out Room room))
            {
                return room;
            }

            throw new Exception($"You don't have room {room.Name} in your map");
        }

        public void BuyNewRoom()
        {
            if (ValidateNewRoom(out _))
            {
                if (Wallet.SpendMoney(_lastAvailableRoom.PurchasePrice, null, out _))
                {
                    ActivateRoom(_lastAvailableRoom);
                }
            }
        }

        public bool ValidateNewRoom(out int statementFailure)
        {
            bool statementOne = _lastActiveRoom != null && _lastAvailableRoom != null;
            statementFailure = 1;
            if (!statementOne) return false;
            bool statementTwo = _lastActiveRoom.StaffCount > 0 && _roomMap[RoomId.Reception].StaffCount > 0;
            statementFailure = 2;
            if (!statementTwo && !_devMode) return false;
            bool statementThree = false;
            foreach (Staff staff in _lastActiveRoom.Staffs)
            {
                if (staff.CurrentLevel.Value >= 25)
                {
                    statementThree = true;
                    break;
                }
            }
            statementFailure = 3;
            if (!statementThree && !_devMode) return false;
            return true;
        }

        private void AddStaff(Staff staff)
        {
            if (staff != null)
            {
                List<Staff> staffList = Staffs.Value;
                staffList.Add(staff);
                Staffs.Value = staffList;
            }
        }

        private void LoadData()
        {
            Data_OwnedRooms data = DataSaver.Load<Data_OwnedRooms>();
            List<RoomId> loadedRooms = data.ownedRooms;
            loadedRooms = SortWithUnlockOrder(loadedRooms);
            foreach (RoomId roomId in loadedRooms)
            {
                Room room = GetRoom(roomId);
                if (roomId != RoomId.Reception && roomId != RoomId.Manicure) ActivateRoom(room);
                if (data.staffMap.TryGetValue(roomId, out var staffs))
                {
                    room.LoadStaff(staffs);
                }
                if (data.interiorMap.TryGetValue(roomId, out var interiors))
                {
                    room.LoadUpgrades(interiors);
                }
            }

            Wallet.Initialize(_devMode, DataSaver.Load<Data_SoftCurrency>().moneyAmount);
            DataSaver.Save<Data_SoftCurrency>(null);

            MoodSystem.Initialize(this, DataSaver.Load<Data_Mood>().moodValue);

            float? minutesPassed = DataSaver.Load<Data_SessionTime>().minutesFromLastSession;
            DataSaver.Save<Data_SessionTime>(null);
            if (minutesPassed != null)
            {
                float offlineIncome = GetMaxIncomePerMinute() * minutesPassed.Value / 5;
                Wallet.AddMoney(offlineIncome, null, out _);
                DataSaver.Save<Data_SoftCurrency>(null);
            }
            Data_DailyReward rewardDataLoad = DataSaver.Load<Data_DailyReward>();
            RewardSaveData rewardData = new RewardSaveData(rewardDataLoad.days, rewardDataLoad.daysFromSave);
            DailyRewardSystem.Initialize(rewardData);
        }

        public float GetMaxIncomePerMinute()
        {
            float income = 0;
            foreach (Staff staff in Staffs.Value)
            {
                float staffIncomePerMinute = 0;

                if (staff.Type != StaffType.Reception)
                {
                    float staffIncome = staff.IncomeAmount.Value;
                    staffIncomePerMinute = staffIncome * (60 / staff.Service.ServingTime);
                }

                income += staffIncomePerMinute;
            }
            income = MoodSystem.AffectMoneyByMood(ref income);
            return income;
        }

        private List<RoomId> SortWithUnlockOrder(List<RoomId> rooms)
        {
            List<RoomId> result = new List<RoomId>();
            for (int i = 0; i < rooms.Count; i++)
            {
                RoomId room = _unlockSequence.Sequence[i];
                result.Add(room);
            }
            return result;
        }
    }
}
