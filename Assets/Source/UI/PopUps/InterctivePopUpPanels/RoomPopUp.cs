using NailSalonTycoon.GameLevel.Interactivity;
using NailSalonTycoon.GameLevel.Rooms;
using TMPro;
using UnityEngine;

namespace NailSalonTycoon.UI.PopUps
{
    internal abstract class RoomPopUp : InteractivePopUp
    {
        protected Room _lastInteractedRoom;
        protected void OnRoomValidate(Room room)
        {
            _lastInteractedRoom = room;
            IsOpened = true;
            gameObject.SetActive(true);
            InvokePopUpShowEvent(true);
            FillData();
        }
    }
}
