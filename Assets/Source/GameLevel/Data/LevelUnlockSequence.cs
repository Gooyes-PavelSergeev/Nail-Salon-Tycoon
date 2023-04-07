using System;
using System.Collections.Generic;
using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms;

namespace NailSalonTycoon.GameLevel
{
    [Serializable]
    public class LevelUnlockSequence
    {
        [SerializeField] private List<RoomId> _sequence;

        public List<RoomId> Sequence { get => _sequence; }
    }
}
