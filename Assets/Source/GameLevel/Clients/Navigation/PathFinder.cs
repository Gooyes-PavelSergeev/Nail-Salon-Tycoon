using NailSalonTycoon.GameLevel.Rooms;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using Dreamteck.Splines;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Clients.Navigation
{
    public static class PathFinder
    {
        private static List<RoomView> rooms = new List<RoomView>();
        private static Dictionary<RoomId, List<PathNodeObject>> _pathNodeMap = new Dictionary<RoomId, List<PathNodeObject>>();

        public static void AddRoom(RoomView room)
        {
            rooms.Add(room);
            _pathNodeMap[room.RoomId] = room.pathNodes;
        }

        public static List<PathNodeObject> GetRoomPath(StaffPlaceView target, PathNodeObject currentNode)
        {
            PathNodeObject targetNode = null;
            List<PathNodeObject> targetRoomNodes = _pathNodeMap[target.RoomId];
            foreach (PathNodeObject node in targetRoomNodes)
            {
                if (node.isEndPoint)
                {
                    StaffPlaceView staff = node.endPointStaff;
                    if (staff == target)
                    {
                        targetNode = node;
                        break;
                    }
                }
            }
            return GetRoomPath(targetNode, currentNode);
        }

        public static List<PathNodeObject> GetRoomPath(PathNodeObject targetNode, PathNodeObject currentNode)
        {
            if (targetNode == null || currentNode == null)
                throw new System.Exception($"Node {targetNode.name} or node {currentNode.name} is null");
            var path = GetPathFromEnd(targetNode, currentNode);
            if (path == null || path.Count == 0)
                throw new System.Exception($"There is no path to node {targetNode.name} in {targetNode.ownerId} from {currentNode.name} in {currentNode.ownerId}");
            return path;
        }

        public static SplineComputer GetSpline(PathNodeObject endNode, PathNodeObject startNode)
        {
            List<NextNodeConfig> nextNodes = startNode.nodesTo;
            foreach (NextNodeConfig nodeTo in nextNodes)
            {
                if (nodeTo.hasNodeObjectInSameRoom && nodeTo.nodeObject != null && nodeTo.nodeObject == endNode)
                {
                    return nodeTo.spline;
                }
                if (!nodeTo.hasNodeObjectInSameRoom && nodeTo.targetRoom == endNode.ownerId)
                {
                    return nodeTo.spline;
                }
            }
            Debug.LogWarning($"Spline between nodes in {startNode.name} and {endNode.name} was not found");
            return null;
        }

        private static List<PathNodeObject> GetPathFromEnd(PathNodeObject endPoint, PathNodeObject startPoint)
        {
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();
            PathNode startNode = new PathNode()
            {
                nodeObject = startPoint,
                position = startPoint.transform.position,
                cameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(endPoint, startPoint)
            };
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node => node.EstimatedFullPathLength).First();
                if (currentNode.nodeObject == endPoint) return GetPathForNode(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in GetNeighbours(currentNode))
                {
                    if (closedSet.Count(node => node.position == neighbourNode.position) > 0) continue;

                    var openNode = openSet.FirstOrDefault(node => node.position == neighbourNode.position);

                    if (openNode == null) openSet.Add(neighbourNode);
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.cameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        private static float GetHeuristicPathLength(PathNodeObject endPoint, PathNodeObject startPoint)
        {
            return Vector3.Distance(endPoint.transform.position, startPoint.transform.position);
        }

        private static List<PathNode> GetNeighbours(PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();
            List<NextNodeConfig> nextNodes =  node.nodeObject.nodesTo;
            foreach (NextNodeConfig nextNode in nextNodes)
            {
                if (nextNode.nodeObject != null)
                {
                    PathNode neighbour = new PathNode
                    {
                        nodeObject = nextNode.nodeObject,
                        position = nextNode.nodeObject.transform.position,
                        cameFrom = node,
                        PathLengthFromStart = node.PathLengthFromStart + GetSpline(nextNode.nodeObject, node.nodeObject).CalculateLength(),
                        HeuristicEstimatePathLength = GetHeuristicPathLength(node.nodeObject, nextNode.nodeObject)
                    };
                    neighbours.Add(neighbour);
                }
                else
                {
                    RoomId? roomId = null;
                    if (!nextNode.hasNodeObjectInSameRoom) roomId = nextNode.targetRoom;
                    if (!roomId.HasValue) continue;
                    List<PathNodeObject> pathNodeObjects = _pathNodeMap[roomId.Value];
                    foreach (PathNodeObject pathNodeObject in pathNodeObjects)
                    {
                        if (pathNodeObject.cameFromId == node.nodeObject.ownerId)
                        {
                            PathNode neighbour = new PathNode
                            {
                                nodeObject = pathNodeObject,
                                position = pathNodeObject.transform.position,
                                cameFrom = node,
                                PathLengthFromStart = node.PathLengthFromStart + 1,
                                HeuristicEstimatePathLength = GetHeuristicPathLength(node.nodeObject, pathNodeObject)
                            };
                            neighbours.Add(neighbour);
                            break;
                        }
                    }
                }
            }
            return neighbours;
        }

        private static List<PathNodeObject> GetPathForNode(PathNode pathNode)
        {
            var result = new List<PathNodeObject>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.nodeObject);
                currentNode = currentNode.cameFrom;
            }
            result.Reverse();
            return result;
        }
    }
}
