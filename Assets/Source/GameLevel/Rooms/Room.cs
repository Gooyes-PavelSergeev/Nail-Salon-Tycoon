using System;
using System.Collections.Generic;
using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms.BM;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.Economy.UpgradeSystem.Rooms;
using Gooyes.Tools;
using NailSalonTycoon.Economy.UpgradeSystem;
using NailSalonTycoon.Economy.UpgradeSystem.Staffs;
using NailSalonTycoon.Data;

namespace NailSalonTycoon.GameLevel.Rooms
{
    public abstract class Room : Economy.UpgradeSystem.Rooms.StaffPlaceView
    {
        public abstract event Action<Staff> StaffAddedEvent;

        protected RoomState _state;
        public bool Active { get => _state == RoomState.Active; }


        protected RoomId _roomId;
        public RoomId RoomId { get => _roomId; }


        protected List<Staff> _staffs = new List<Staff>();
        public List<Staff> Staffs { get => _staffs; }
        public int StaffCount { get { return _staffs.Count; } }
        public List<StaffPlace> StaffPlaces { get
            {
                List<StaffPlace> places = new List<StaffPlace>();

                foreach (var staff in _staffs)
                {
                    if (staff.Active)
                        places.Add(staff.Place);
                }

                return places;
            }
        }

        protected RoomBehaviourMachine _bm;

        protected RoomView _view;
        public RoomView View { get => _view; }

        public Quaternion Rotation { get => _rotation; }
        private Quaternion _rotation;
        public Vector3 Position { get => _position; }
        protected Vector3 _position;
        public Vector3 Scale { get => _scale; }
        protected Vector3 _scale;

        public string Name { get => _roomId.ToString(); }

        public abstract float PurchasePrice { get; }
        public float NewStaffPurchasePrice
        {
            get
            {
                Dictionary<int, float> staffPurchaseMap =
                    UpgradeConfigurator.GetStaffPurchasePriceMap((Economy.UpgradeSystem.UpgradableType)_roomId);
                if (staffPurchaseMap.TryGetValue(StaffCount, out float value))
                {
                    return value;
                }
                return 0;
            }
        }
        public abstract int MaxStaffCount { get; }
        public abstract int MaxInteriorCount { get; }
        public Observable<List<UpgradeId>> OwnedUpgrades { get; set; }
        public UpgradableType Type { get => (UpgradableType)(int)_roomId; }

        public Room(RoomConfig config, RoomContainer container)
        {
            _state = RoomState.Inactive;
            _roomId = config.id;
            _position = config.position;
            _rotation = config.rotation;
            _scale = config.scale;
            OwnedUpgrades = new Observable<List<UpgradeId>>();
            OwnedUpgrades.Value = new List<UpgradeId>();
            _view = container.GetRoomView(config.id).Instantiate(this);
            _bm = new RoomBehaviourMachine(this);
            //_view.ActiveStateClickEvent += (interactData) => AddStaff();
        }

        public void Activate(bool active = true)
        {
            _state = active ? RoomState.Active : RoomState.Inactive;
            if (active) _bm.Switcher.SwitchBehaviour<RoomBehaviourActive>();
            else _bm.Switcher.SwitchBehaviour<RoomBehaviourInactive>();
        }

        public void SetAvailable()
        {
            _state = RoomState.Available;
            _bm.Switcher.SwitchBehaviour<RoomBehaviourAvailable>();
        }

        public abstract void AddStaff();
        internal abstract void AddLoadedStaff(Data_OwnedRooms.StaffSaveConfig config);
        public abstract void ActivateAllStaff(bool active = true);

        public bool Upgrade()
        {
            bool success = RoomUpgrader.TryUpgrade(this);
            return success;
        }
        public bool Upgrade(UpgradeId upgradeId)
        {
            bool success = RoomUpgrader.TryUpgrade(this, upgradeId);
            return success;
        }

        internal void LoadStaff(List<Data_OwnedRooms.StaffSaveConfig> staffs)
        {
            foreach (Data_OwnedRooms.StaffSaveConfig config in staffs)
            {
                AddLoadedStaff(config);
            }
        }

        internal void LoadUpgrades(List<UpgradeId> upgrades)
        {
            if (upgrades == null || upgrades.Count == 0) OwnedUpgrades.Value = new List<UpgradeId>();
            else OwnedUpgrades.Value = upgrades;
        }
    }
}
