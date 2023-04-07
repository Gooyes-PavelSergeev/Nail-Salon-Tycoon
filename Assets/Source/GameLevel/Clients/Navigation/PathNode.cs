using UnityEngine;
using NailSalonTycoon.GameLevel.Rooms;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    internal class PathNode
    {
        public PathNodeObject nodeObject;
        public PathNode cameFrom;
        public Vector3 position;
        public float PathLengthFromStart { get; set; }
        public float HeuristicEstimatePathLength { get; set; }
        public float EstimatedFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }
}
