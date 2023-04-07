using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using System;
using NailSalonTycoon.Economy;
using NailSalonTycoon.Data;

namespace NailSalonTycoon.GameLevel.Rooms.Concrete
{
    public class RoomManicure : Room
    {
        public override float PurchasePrice { get { return 0; } }

        public override int MaxStaffCount { get => 5; }
        public override int MaxInteriorCount { get => 5; }

        public override event Action<Staff> StaffAddedEvent;
        public RoomManicure(RoomConfig config, RoomContainer container) : base(config, container)
        {

        }

        public override void ActivateAllStaff(bool active = true)
        {
            throw new NotImplementedException();
        }

        public override void AddStaff()
        {
            if (StaffCount >= MaxStaffCount) return;
            float price = NewStaffPurchasePrice;
            if (Wallet.CheckEnoughMoney(price))
            {
                Staff staff = Staff.Instantiate(this, StaffType.Regular, "Manicurist");
                if (staff.Active)
                {
                    _staffs.Add(staff);
                    Wallet.SpendMoney(price, null, out _);
                    StaffAddedEvent?.Invoke(staff);
                }
            }
        }

        internal override void AddLoadedStaff(Data_OwnedRooms.StaffSaveConfig config)
        {
            int id = config.id;
            int level = config.level;
            Staff staff = Staff.Instantiate(this, StaffType.Regular, "Manicurist", id, level);
            if (!staff.Active)
                throw new Exception($"Loaded staff with id {id} and level {level} in {_roomId} doesn't have place");
            _staffs.Add(staff);
            StaffAddedEvent?.Invoke(staff);
        }
    }
}
