using Dreamteck.Splines;
using NailSalonTycoon.GameLevel.Rooms;
using System;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    [Serializable]
    public struct NextNodeConfig
    {
        public bool hasNodeObjectInSameRoom;
        public PathNodeObject nodeObject;
        public RoomId targetRoom;
        public SplineComputer spline;
    }
}
