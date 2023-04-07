using NailSalonTycoon.GameLevel.Clients;
using System;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public interface IStaffService
    {
        event Action<Staff> ServiceFinishEvent;

        event Action<Staff> ServiceStartEvent;

        event Action<float> MoneyEarnedEvent;

        public float ServingTime { get; }

        void Serve(Client client, Action<Staff> callback);

        void FinishServing(Client client);
    }
}
