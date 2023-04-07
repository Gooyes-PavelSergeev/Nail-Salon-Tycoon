using NailSalonTycoon.GameLevel.Clients.Navigation;
using NailSalonTycoon.GameLevel.Rooms;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Clients
{
    public class ClientView : MonoBehaviour
    {
        [SerializeField] private float _speed = 3f;
        public float Speed { get => _speed; set => _speed = value; }

        [Range(0.95f, 1f)] public float _splineDistanceToFinish = 0.99f;

        private PathNodeObject _currentNode;
        private PathNodeObject _nextNode;
        private PathNodeObject _endNode;
        private SplineFollower _follower;
        private Animator _animator;

        private bool _lastMovementFinished;

        private Vector3 _rotationBeforeSitAnim;

        private Client _client;

        private void Awake()
        {
            _follower = GetComponent<SplineFollower>();
            _animator = GetComponentInChildren<Animator>();
            _lastMovementFinished = true;
        }

        public void Init(PathNodeObject startNode, PathNodeObject endNode, Client client)
        {
            _currentNode = startNode;
            _nextNode = _currentNode;
            _endNode = endNode;
            _client = client;
        }

        public void StartMovement(Staff targetStaff, Action<bool, Staff> callback)
        {
            StopAllCoroutines();
            if (!_lastMovementFinished)
            {
                StartCoroutine(FinishMovement(targetStaff, callback));
            }
            else
            {
                StaffPlaceView staffView = targetStaff.Place.View;
                List<PathNodeObject> roomPath = PathFinder.GetRoomPath(staffView, _currentNode);
                StartCoroutine(MoveTo(targetStaff, callback, roomPath));
            }
        }

        public void StartMovement(PathNodeObject targetNode, Action callback)
        {
            List<PathNodeObject> roomPath = PathFinder.GetRoomPath(targetNode, _currentNode);
            StopAllCoroutines();
            StartCoroutine(MoveTo(targetNode, callback, roomPath));
        }

        private IEnumerator FinishMovement(Staff targetStaff, Action<bool, Staff> callback)
        {
            while (_follower.modifiedResult.percent < _splineDistanceToFinish)
            {
                _follower.Move(_speed * Time.deltaTime);
                yield return null;
            }
            _currentNode = _nextNode;
            StaffPlaceView staffView = targetStaff.Place.View;
            List<PathNodeObject> roomPath = PathFinder.GetRoomPath(staffView, _currentNode);
            StartCoroutine(MoveTo(targetStaff, callback, roomPath));
        }

        private IEnumerator MoveTo(Staff targetStaff, Action<bool, Staff> callback, List<PathNodeObject> roomPath)
        {
            _lastMovementFinished = false;

            bool waitForAnimation = _client.IsServing;
            float animDur = PlayGetUpAnimation(_client.LastServiceRoom);
            if (waitForAnimation)
            {
                yield return new WaitForSeconds(animDur);
                Quaternion rotation = transform.rotation;
                Vector3 rotInDegrees = rotation.eulerAngles;
                rotInDegrees.y += 180;
                transform.rotation = Quaternion.Euler(rotInDegrees);
                yield return new WaitForEndOfFrame();
            }

            int pathCount = roomPath.Count;
            for (int i = 0; i < pathCount - 1; i++)
            {
                _nextNode = roomPath[i + 1];
                SplineComputer spline = PathFinder.GetSpline(roomPath[i + 1], _currentNode);
                yield return new WaitForEndOfFrame();
                _follower.spline = spline;
                //_follower.Restart();
                _follower.result.percent = 0;
                while (_follower.result.percent < _splineDistanceToFinish)
                {
                    _follower.Move(_speed * Time.deltaTime);
                    yield return null;
                }
                _currentNode = roomPath[i + 1];
            }
            _lastMovementFinished = true;
            callback?.Invoke(true, targetStaff);
        }

        private IEnumerator MoveTo(PathNodeObject targetNode, Action callback, List<PathNodeObject> roomPath)
        {
            _lastMovementFinished = false;

            bool waitForAnimation = _client.IsServing;
            float animDur = PlayGetUpAnimation(_client.LastServiceRoom);
            Quaternion bufferRot = transform.rotation;
            if (waitForAnimation)
            {
                yield return new WaitForSeconds(animDur);
                _animator.enabled = false;
            }

            int pathCount = roomPath.Count;
            for (int i = 0; i < pathCount - 1; i++)
            {
                _nextNode = roomPath[i + 1];
                SplineComputer spline = PathFinder.GetSpline(roomPath[i + 1], _currentNode);
                yield return new WaitForEndOfFrame();
                _follower.spline = spline;
                _follower.result.percent = 0;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                //transform.rotation = Quaternion.Euler(_rotationBeforeSitAnim + new Vector3(0, 180, 0));
                yield return null;
                _animator.enabled = true;
                while (_follower.result.percent < 0.99)
                {
                    _follower.Move(_speed * Time.deltaTime);
                    yield return null;
                }
                _currentNode = roomPath[i + 1];
            }
            _lastMovementFinished = true;
            callback?.Invoke();
        }

        public void PlaySitAnimation(RoomId id)
        {
            if (id != RoomId.Reception)
            {
                string animName = $"Sit{id}";
                _animator.Play(animName);
                _rotationBeforeSitAnim = transform.rotation.eulerAngles;
            }
        }

        private float PlayGetUpAnimation(RoomId id)
        {
            if (id != RoomId.Reception)
            {
                string animName = $"GetUp{id}";
                AnimationClip animation = FindAnimation($"GetUp{id}");
                if (animation == null) return 0;
                _animator.Play(animName);
                return animation.length;
            }
            return 0;
        }

        public AnimationClip FindAnimation(string name)
        {
            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Equals(name))
                {
                    return clip;
                }
            }
            Debug.Log($"Anim {name} not found");
            return null;
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}
