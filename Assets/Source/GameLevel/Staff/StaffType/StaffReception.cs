using NailSalonTycoon.GameLevel.Clients;
using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class StaffReception : IStaffService
    {
        public event Action<Staff> ServiceFinishEvent;

        public event Action<Staff> ServiceStartEvent;

        public event Action<float> MoneyEarnedEvent;

        private Staff _staff;

        private Room _room;

        private Client _currentClient;

        public float ServingTime { get => 0; }

        public StaffReception(Staff staff, Room room)
        {
            _room = room;
            _staff = staff;
        }

        public void Serve(Client client, Action<Staff> callback)
        {
            if (client != null)
            {
                _currentClient = client;
                ServiceStartEvent.Invoke(_staff);
                ServiceFinishEvent?.Invoke(_staff);
                callback?.Invoke(_staff);
                _currentClient = null;
            }
        }

        public void FinishServing(Client client)
        {
            if (_currentClient == client)
            {
                _currentClient = null;
                ServiceFinishEvent?.Invoke(_staff);
            }
        }
    }
}
