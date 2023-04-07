using NailSalonTycoon.GameLevel.Clients;
using System;
using UnityEngine;
using Gooyes.Tools;
using System.Collections;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class StaffRest : IStaffService
    {
        public event Action<Staff> ServiceFinishEvent;

        public event Action<Staff> ServiceStartEvent;

        public event Action<float> MoneyEarnedEvent;

        private Staff _staff;

        private Room _room;

        private Client _currentClient;

        private bool _isServing;

        private Coroutine _servingProcessCoroutine;

        public float ServingTime { get => 13; }

        public StaffRest(Staff staff, Room room)
        {
            _room = room;
            _staff = staff;
        }

        public void Serve(Client client, Action<Staff> callback)
        {
            if (client != null)
            {
                _currentClient = client;
                _isServing = true;
                _servingProcessCoroutine = Coroutines.StartCoroutine(ServeForSeconds());
            }
        }

        public void FinishServing(Client client)
        {
            if (_currentClient == client)
            {
                _currentClient = null;
                _isServing = false;
                ServiceFinishEvent?.Invoke(_staff);
            }
        }

        private IEnumerator ServeForSeconds()
        {
            float timer = 0f;
            while (_isServing)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            float income = _staff.IncomeAmount.Value;
            MoodSystem.AffectMoneyByMood(ref income);
            MoneyEarnedEvent?.Invoke(income);
        }
    }
}
