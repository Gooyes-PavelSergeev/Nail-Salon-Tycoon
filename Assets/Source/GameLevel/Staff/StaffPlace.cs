using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class StaffPlace
    {
        private int _placeId;
        public int PlaceId { get { return _placeId; } }
        private Transform _position;

        private Staff _staff;

        private Room _room;

        private StaffPlaceView _view;
        public StaffPlaceView View { get { return _view; } }
        public Staff Staff { get; private set; }

        public Vector3 Position { get { return _view.transform.position; } }

        public bool Active { get; private set; }

        public StaffPlace (Staff staff, Room room)
        {
            if (staff == null || room == null)
                throw new NullReferenceException("Staff or room is not setup");
            _room = room;
            _staff = staff;
            _placeId = staff.StaffId;
            SetPosition();
            Active = _position != null;
        }

        public void SetPosition()
        {
            StaffPlaceView view = GetView();
            if (view != null)
            {
                _view = view;
                _position = view.transform;
            }
        }

        private StaffPlaceView GetView()
        {
            List<StaffPlaceView> places = _room.View.StaffPlaces;
            foreach (StaffPlaceView place in places)
            {
                if (place.PlaceId == _placeId)
                {
                    place.Activate(true, _staff);
                    return place;
                }
            }

            return null;
        }
    }
}
