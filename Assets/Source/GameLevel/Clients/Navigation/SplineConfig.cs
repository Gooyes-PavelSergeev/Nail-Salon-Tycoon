using System;
using Dreamteck.Splines;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    [Serializable]
    public struct SplineConfig
    {
        public SplineComputer spline;
        public StaffPlaceView destination;
    }
}
