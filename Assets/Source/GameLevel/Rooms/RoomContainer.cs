using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms
{
    [CreateAssetMenu(menuName = "Room Container")]
    public class RoomContainer : ScriptableObject
    {
        [SerializeField] private List<RoomView> _rooms;

        public RoomView GetRoomView(RoomId id)
        {
            foreach (RoomView room in _rooms)
            {
                if (room.RoomId == id)
                {
                    return room;
                }
            }
            Debug.LogWarning("You didn't add room " + id.ToString() + " to container");
            return null;
        }
    }
}
