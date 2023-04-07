using System;
using Dreamteck.Splines;
using NailSalonTycoon.GameLevel.Rooms;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    [Serializable]
    public struct ExitSplineConfig
    {
        public SplineComputer spline;
        public RoomId targetRoom;
        [NonSerialized] public RoomId fromRoom;
    }
}
