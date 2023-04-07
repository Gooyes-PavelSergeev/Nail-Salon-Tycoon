using NailSalonTycoon.GameLevel.Clients;
using System;
using NailSalonTycoon.Economy.UpgradeSystem.Staffs;
using NailSalonTycoon.Economy.UpgradeSystem;
using NailSalonTycoon.Economy;
using UnityEngine;
using Gooyes.Tools;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class Staff : IUpgradable
    {
        private int _staffId;
        public int StaffId { get => _staffId; }

        private Room _room;
        public RoomId RoomId { get { return _room.RoomId; } }

        private StaffPlace _place;
        public StaffPlace Place { get => _place; }

        public bool Active { get; private set; }

        public IStaffService Service { get => _service; }
        private IStaffService _service;

        public StaffType Type { get; private set; }
        public bool IsBusy { get; private set; }
        public string Name { get; private set; }

        UpgradableType IUpgradable.Type { get => (UpgradableType)(int)_room.RoomId; }
        public Observable<int> CurrentLevel { get; set; }
        public Observable<float> IncomeAmount { get; set; }
        public int Id { get => _staffId; }
        public UpgradeData? LastUpgradeData { get; set; }

        public event Action<UpgradeData> UpgradeEvent;

        private Staff(Room room, StaffType type, string name)
        {
            CurrentLevel = new Observable<int>(1);
            _room = room;
            Name = name;
            _staffId = room.StaffCount;
            LastUpgradeData = null;
            StaffUpgrader.FillUpgradeData(this);

            _service = SetServiceType(type);
            _place = new StaffPlace(this, room);
            Active = _place.Active;
        }

        public static Staff Instantiate(Room room, StaffType type, string name)
        {
            Staff staff = new Staff(room, type, name);
            return staff;
        }

        /// <summary>
        /// Use it to load staff
        /// </summary>
        /// <returns></returns>
        public static Staff Instantiate(Room room, StaffType type, string name, int id, int level)
        {
            Staff staff = new Staff(room, type, name, id, level);
            return staff;
        }

        private Staff(Room room, StaffType type, string name, int id, int level)
        {
            CurrentLevel = new Observable<int>(level);
            _room = room;
            Name = name;
            _staffId = id;
            LastUpgradeData = null;
            StaffUpgrader.FillUpgradeData(this);

            _service = SetServiceType(type);
            _place = new StaffPlace(this, room);
            Active = _place.Active;
        }

        public void Activate(bool active = true)
        {
            //Active = active;
        }

        public void SetClient(Client client, bool set = true)
        {
            if (!Active) Debug.LogWarning($"Staff {_staffId} in room {_room.Name} is inactive");
            if (!set)
                _service.FinishServing(client);
            IsBusy = set;
        }

        private IStaffService SetServiceType(StaffType type)
        {
            this.Type = type;
            IStaffService service = null;

            switch (type)
            {
                case StaffType.Reception:
                    service = new StaffReception(this, _room);
                    break;
                case StaffType.Rest:
                    service = new StaffRest(this, _room);
                    break;
                case StaffType.Regular:
                    service = new StaffRegular(this, _room);
                    break;
            }

            if (service == null)
                throw new Exception("Wrong staff service type");

            service.ServiceFinishEvent += OnServeFinish;
            service.MoneyEarnedEvent += OnMoneyEarned;
            return service;
        }

        public void ServeClient(Client client, Action<Staff> callback)
        {
            if (!Active) Debug.LogWarning($"Staff {_staffId} in room {_room.Name} is inactive");
            IsBusy = true;
            _service.Serve(client, callback);
        }

        private void OnServeFinish(Staff staff)
        {
            IsBusy = false;
        }

        public bool Upgrade()
        {
            if (StaffUpgrader.TryUpgrade(this, out UpgradeData upgradeData))
            {
                UpgradeEvent?.Invoke(upgradeData);
                return true;
            }
            return false;
        }

        public UpgradeData GetNextUpgradeData()
        {
            return StaffUpgrader.GetNextUpgradeData(this);
        }

        private void OnMoneyEarned(float moneyAmount)
        {
            Wallet.AddMoney(moneyAmount, null, out _);
        }
    }
}
