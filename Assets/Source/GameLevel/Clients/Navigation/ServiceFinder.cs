using NailSalonTycoon.GameLevel.Clients.Navigation;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Clients
{
    public class ServiceFinder
    {
        private GameLevel _gameLevel;

        public PathNodeObject StartNode { get; private set; }
        public PathNodeObject EndNode { get; private set; }

        private Staff _receptionStaff;

        private List<Staff> _regularStaffs = new List<Staff>();

        private List<Staff> _restPlaces = new List<Staff>();

        public event Action<Staff> RegularStaffFreeEvent;

        public ServiceFinder(GameLevel gameLevel)
        {
            _gameLevel = gameLevel;
            _gameLevel.Staffs.OnChanged += AddStaff;

            StartNode = GameObject.FindGameObjectWithTag("StartNode").GetComponent<PathNodeObject>();
            EndNode = GameObject.FindGameObjectWithTag("EndNode").GetComponent<PathNodeObject>();
        }

        private void AddStaff(List<Staff> staffs)
        {
            if (staffs == null || staffs.Count == 0)
                return;

            Staff staff = staffs[staffs.Count - 1];
            switch (staff.Type)
            {
                case StaffType.Reception:
                    _receptionStaff = staff;
                    break;
                case StaffType.Rest:
                    _restPlaces.Add(staff);
                    break;
                case StaffType.Regular:
                    staff.Service.ServiceFinishEvent += OnRegularStaffFree;
                    _regularStaffs.Add(staff);
                    break;
            }
        }

        private void OnRegularStaffFree(Staff staff)
        {
            RegularStaffFreeEvent?.Invoke(staff);
        }

        public Staff GetReceptionStaff()
        {
            if (_receptionStaff != null)
                return _receptionStaff;

            throw new Exception("Reception staff is not found");
        }

        public bool GetValidTarget(out Staff staff)
        {
            List<Staff> regularFreeStaffs = GetFreeStaffs(StaffType.Regular);
            if (regularFreeStaffs.Count != 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, regularFreeStaffs.Count);
                staff = regularFreeStaffs[randomIndex];
                return true;
            }

            List<Staff> restFreeStaffs = GetFreeStaffs(StaffType.Rest);
            if (restFreeStaffs.Count != 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, restFreeStaffs.Count);
                staff = restFreeStaffs[randomIndex];
                return false;
            }
            staff = null;
            return false;
        }

        private List<Staff> GetFreeStaffs(StaffType staffType)
        {
            if (staffType == StaffType.Reception)
                Debug.LogWarning("Reception staff is always free");
            List<Staff> staffs = new List<Staff>();
            switch (staffType)
            {
                case StaffType.Rest:
                    staffs = _restPlaces;
                    break;
                case StaffType.Regular:
                    staffs = _regularStaffs;
                    break;
            }
            if (staffs.Count == 0)
                throw new Exception("No rest or regular staff exists");

            List<Staff> freeStaffs = new List<Staff>();
            foreach(Staff staff in staffs)
            {
                if (!staff.IsBusy)
                    freeStaffs.Add(staff);
            }
            return freeStaffs;
        }

        public bool HasFreeStaffOrRestSpace()
        {
            foreach (Staff staff in _gameLevel.Staffs.Value)
            {
                if (!staff.IsBusy && staff.Type != StaffType.Reception && _receptionStaff != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
