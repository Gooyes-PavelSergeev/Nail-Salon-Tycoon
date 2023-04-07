using System;
using System.Collections.Generic;
using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.GameLevel.Interactivity;
using NailSalonTycoon.GameLevel.Clients.Navigation;
using Gooyes.Tools;
using System.Collections;
using NailSalonTycoon.Economy.UpgradeSystem.Rooms;

namespace NailSalonTycoon.GameLevel.Rooms
{
    public class RoomView : MonoBehaviour, IInteractive
    {
        [SerializeField] private GameObject _activeStatePresenter;

        [SerializeField] private GameObject _availableStatePresenter;

        [SerializeField] private InteriorContainerView _interiorContainer;

        [SerializeField] private GameObject _objectToActivateWithFirstStaff;
        private bool _firstStaffAdded;

        [SerializeField] private List<StaffSystem.StaffPlaceView> _staffPlaces;
        public List<StaffSystem.StaffPlaceView> StaffPlaces { get { return _staffPlaces; } }

        [SerializeField] private RoomId _roomId;
        public RoomId RoomId { get => _roomId; }

        public string Name { get { return _room.Name; } }

        public bool Active { get => _room.Active; }

        public List<PathNodeObject> pathNodes;

        public Room Room { get => _room; }
        private Room _room;

        public event Action<InteractData> InteractionEvent;
        public event Action<InteractData> ActiveStateClickEvent;
        public event Action<InteractData> AvailableStateClickEvent;

        public RoomView Instantiate(Room room)
        {
            RoomView createdRoom = Instantiate(this);
            createdRoom._room = room;
            Coroutines.StartCoroutine(createdRoom.InternalInit());
            return createdRoom;
        }

        private void Start()
        {
            gameObject.GetComponentsInChildren<PathNodeObject>(pathNodes);
            PathFinder.AddRoom(this);
            if (pathNodes == null || pathNodes.Count == 0) return;
            for (int i = 0; i < pathNodes.Count; i++)
            {
                pathNodes[i].ownerId = _room.RoomId;
            }
        }

        private IEnumerator InternalInit()
        {
            yield return new WaitForSeconds(0.1f);
            _interiorContainer.AddInteriors(_room.OwnedUpgrades.Value);
            _room.OwnedUpgrades.OnChanged += OnInteriorAdded;
            if (_roomId == RoomId.Rest) _room.StaffAddedEvent += OnStaffAdded;
            SetInteractiveObject();
        }

        private void OnStaffAdded(Staff staff)
        {
            if (_firstStaffAdded) return;
            _firstStaffAdded = true;
            _objectToActivateWithFirstStaff.SetActive(true);
        }

        private void OnInteriorAdded(List<UpgradeId> upgrades)
        {
            UpgradeId id = upgrades[^1];
            _interiorContainer.AddInterior(id);
        }

        public void SetActiveNormalState(bool active)
        {
            _activeStatePresenter.SetActive(active);
            _availableStatePresenter.SetActive(false);
        }

        public void SetActiveAvailableState(bool active)
        {
            _activeStatePresenter.SetActive(false);
            _availableStatePresenter.SetActive(active);
        }

        public void SetTransform()
        {
            transform.rotation = _room.Rotation;
            transform.position = _room.Position;
            transform.localScale = _room.Scale;
        }

        public void Interact(InteractData interactData)
        {
            if (interactData.interactedGameObject == _activeStatePresenter)
                ActiveStateClickEvent?.Invoke(interactData);
            if (interactData.interactedGameObject == _availableStatePresenter)
                AvailableStateClickEvent?.Invoke(interactData);
        }

        public void SetInteractiveObject()
        {
            InteractableObject ioActive = null;
            if (!_activeStatePresenter.TryGetComponent<InteractableObject>(out ioActive))
                _activeStatePresenter.AddComponent<InteractableObject>().SetObject(this);
            else
                ioActive.SetObject(this);

            BoxCollider bcActive = null;
            if (!_activeStatePresenter.TryGetComponent<BoxCollider>(out bcActive))
            {
                _activeStatePresenter.AddComponent<BoxCollider>();
                Debug.LogWarning("Box collider in " + _roomId.ToString() + " active presenter is not configured");
            }


            InteractableObject ioAvailable = null;
            if (!_availableStatePresenter.TryGetComponent<InteractableObject>(out ioAvailable))
                _availableStatePresenter.AddComponent<InteractableObject>().SetObject(this);
            else
                ioAvailable.SetObject(this);

            BoxCollider bcAvailable = null;
            if (!_availableStatePresenter.TryGetComponent<BoxCollider>(out bcAvailable))
            {
                _availableStatePresenter.AddComponent<BoxCollider>();
                Debug.LogWarning("Box collider in " + _roomId.ToString() + " available presenter is not configured");
            }
        }

        public bool TryGetObject<T>(out object obj)
        {
            if (typeof(T) == typeof(Room))
            {
                obj = _room;
                return true;
            }

            obj = null;
            return false;
        }
    }
}
