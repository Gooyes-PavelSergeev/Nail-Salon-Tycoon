using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using UnityEngine;
using System;
using NailSalonTycoon.GameLevel.Rooms;

namespace NailSalonTycoon.GameLevel.Clients
{
    public class Client
    {
        private ClientView _view;

        private Staff _targetStaff;
        private Staff _receptionStaff;

        private ServiceFinder _serviceFinder;

        public bool IsServing { get => _isServing; }
        private bool _isServing;

        public RoomId LastServiceRoom { get => _servedInRoom; }
        private RoomId _servedInRoom;


        public static Client Instantiate(ServiceFinder finder, ClientConfig config)
        {
            Client client = new Client(finder, config);
            return client;
        }

        private Client(ServiceFinder finder, ClientConfig config)
        {
            _serviceFinder = finder;
            _serviceFinder.RegularStaffFreeEvent += OnRegularStaffFree;

            ClientView view = GameObject.Instantiate(config.view);
            _view = view;
            _view.Init(_serviceFinder.StartNode, _serviceFinder.EndNode, this);

            _receptionStaff = finder.GetReceptionStaff();

            SetStaff();

            MoveToStaff(_receptionStaff);
        }

        private void SetStaff()
        {
            _serviceFinder.GetValidTarget(out _targetStaff);
            if (_targetStaff == null)
            {
                throw new System.Exception("There is no free staff");
            }
            _targetStaff.SetClient(this);
        }

        private void OnRegularStaffFree(Staff staff)
        {
            if (_targetStaff.Type == StaffType.Rest)
            {
                if (!staff.IsBusy)
                {
                    _targetStaff.SetClient(this, false);
                    staff.SetClient(this);
                    _targetStaff = staff;
                    MoveToStaff(staff);
                }
            }
        }

        private void MoveToStaff(Staff staff, Action<bool, Staff> callback = null)
        {
            if (callback == null) _view.StartMovement(staff, OnMovementFinish);
            else _view.StartMovement(staff, callback);
        }

        private void OnMovementFinish(bool success, Staff targetStaff)
        {
            if (!success)
                Debug.LogWarning("Client had error in movement process");
            _view.PlaySitAnimation(targetStaff.RoomId);
            _isServing = true;
            _servedInRoom = targetStaff.RoomId;
            targetStaff.ServeClient(this, OnServingFinish);
        }

        private void OnServingFinish(Staff servingStaff)
        {
            StaffType staffType = servingStaff.Type;
            _servedInRoom = servingStaff.RoomId;
            switch (staffType)
            {
                case StaffType.Regular:
                    _serviceFinder.RegularStaffFreeEvent -= OnRegularStaffFree;
                    _view.StartMovement(_serviceFinder.EndNode, () => Remove());
                    break;
                case StaffType.Reception:
                    MoveToStaff(_targetStaff);
                    break;
                case StaffType.Rest:
                    break;
            }
            _isServing = false;
        }

        private void Remove()
        {
            _view.Remove();
        }
    }
}
