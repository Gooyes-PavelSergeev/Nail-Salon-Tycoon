using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms
{
    [Serializable]
    public struct RoomConfig
    {
        public RoomId id;

        public Vector3 position;

        public Quaternion rotation;

        public Vector3 scale;
    }
}
