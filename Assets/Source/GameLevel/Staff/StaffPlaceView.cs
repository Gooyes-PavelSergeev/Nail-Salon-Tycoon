using System;
using UnityEngine;
using NailSalonTycoon.GameLevel.Interactivity;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    public class StaffPlaceView : MonoBehaviour, IInteractive
    {
        [SerializeField] private int _placeId;
        public int PlaceId { get { return _placeId; } }
        public StaffPlace Place { get; private set; }
        public string Name { get { return _staff.Name; } }

        private Staff _staff;
        public RoomId RoomId { get { return _staff.RoomId; } }

        public event Action<InteractData> InteractionEvent;
        internal StaffView staffView;
        public bool Active { get => _staff.Active; }

        public void Activate(bool active = true, Staff staff = null)
        {
            _staff = staff;
            this.gameObject.SetActive(active);
            if (active)
            {
                staffView = GetComponentInChildren<StaffView>();
                if (staffView == null)
                    throw new Exception($"There is no StaffView on staff {_placeId} in {_staff.RoomId}");
                staffView.Activate(staff);
                SetInteractiveObject();
            }
        }

        public void Interact(InteractData interactData)
        {
            InteractionEvent?.Invoke(interactData);
        }

        public void SetInteractiveObject()
        {
            GetComponent<InteractableObject>().SetObject(this);
        }

        public bool TryGetObject<T>(out object obj)
        {
            if (typeof(T) == typeof(Staff))
            {
                obj = _staff;
                return true;
            }

            obj = null;
            return false;
        }
    }
}
