using NailSalonTycoon.GameLevel.Clients;
using System;
using UnityEngine;
using Gooyes.Tools;
using System.Collections;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class StaffRegular : IStaffService
    {
        public event Action<Staff> ServiceFinishEvent;

        public event Action<Staff> ServiceStartEvent;

        public event Action<float> MoneyEarnedEvent;

        private Staff _staff;

        private Room _room;

        public float ServingTime { get => 13; }

        private Client _currentClient;

        public StaffRegular(Staff staff, Room room)
        {
            _staff = staff;
            _room = room;
        }

        public void Serve(Client client, Action<Staff> callback)
        {
            if (client != null)
            {
                _currentClient = client;
                Coroutines.StartCoroutine(ServeForSeconds(ServingTime, callback));
            }
        }

        private IEnumerator ServeForSeconds(float time, Action<Staff> callback)
        {
            ServiceStartEvent?.Invoke(_staff);
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            float income = _staff.IncomeAmount.Value;
            MoodSystem.AffectMoneyByMood(ref income);
            _currentClient = null;
            ServiceFinishEvent?.Invoke(_staff);
            MoneyEarnedEvent?.Invoke(income);
            callback?.Invoke(_staff);
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
