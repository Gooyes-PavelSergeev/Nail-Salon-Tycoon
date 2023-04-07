using UnityEngine;
using System.Collections.Generic;
using NailSalonTycoon.GameLevel.Rooms;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    public class PathNodeObject : MonoBehaviour
    {
        public List<NextNodeConfig> nodesTo;
        public RoomId cameFromId;
        [HideInInspector] public RoomId ownerId;
        public bool isEndPoint;
        public StaffPlaceView endPointStaff;
    }
}
