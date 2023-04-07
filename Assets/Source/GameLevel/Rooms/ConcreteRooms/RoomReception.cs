using System;
using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.Economy;
using NailSalonTycoon.Data;

namespace NailSalonTycoon.GameLevel.Rooms.Concrete
{
    public class RoomReception : Room
    {
        public override int MaxStaffCount { get => 1; }
        public override int MaxInteriorCount { get => 0; }

        private Staff _cashier;

        private Transform _cashierPlace;

        public override event Action<Staff> StaffAddedEvent;

        public override float PurchasePrice { get { return 0; } }

        public RoomReception(RoomConfig config, RoomContainer container) : base(config, container)
        {

        }

        public override void ActivateAllStaff(bool active = true)
        {
            //_cashier.Activate(active);
        }

        public override void AddStaff()
        {
            if (StaffCount >= MaxStaffCount) return;
            if (_cashier != null) return;
            float price = NewStaffPurchasePrice;
            if (Wallet.CheckEnoughMoney(price))
            {
                Staff cashier = Staff.Instantiate(this, StaffType.Reception, "Cashier");
                if (cashier.Active)
                {
                    _cashier = cashier;
                    _staffs.Add(cashier);
                    Wallet.SpendMoney(price, null, out _);
                    StaffAddedEvent?.Invoke(cashier);
                }
            }
        }

        internal override void AddLoadedStaff(Data_OwnedRooms.StaffSaveConfig config)
        {
            int id = config.id;
            int level = config.level;
            Staff staff = Staff.Instantiate(this, StaffType.Reception, "Cashier", id, level);
            if (!staff.Active)
                throw new Exception($"Loaded staff with id {id} and level {level} in {_roomId} doesn't have place");
            _staffs.Add(staff);
            _cashier = staff;
            StaffAddedEvent?.Invoke(staff);
        }
    }
}
