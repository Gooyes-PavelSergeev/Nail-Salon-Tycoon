using System;
using Gooyes.BehaviourMachine;
using UnityEngine;
using NailSalonTycoon.GameLevel;

namespace NailSalonTycoon.GameLevel.Rooms.BM
{
    public class RoomBehaviourMachine : BehaviourMachine<RoomBehaviour>
    {
        private Room _room;

        public RoomBehaviourMachine(Room room)
        {
            _room = room;
            FillBehaviours();
        }

        protected override void FillBehaviours()
        {
            _behavioursMap[typeof(RoomBehaviourInactive)] = new RoomBehaviourInactive();
            _behavioursMap[typeof(RoomBehaviourAvailable)] = new RoomBehaviourAvailable();
            _behavioursMap[typeof(RoomBehaviourActive)] = new RoomBehaviourActive();
            foreach (var behaviour in _behavioursMap)
            {
                behaviour.Value.Init(Switcher, _room);
            }
            SetDefaultBehaviour();
        }

        public void Update()
        {
            if (_currentBehaviour != null)
                _currentBehaviour.Update();
        }

        protected override RoomBehaviour SetDefaultBehaviour()
        {
            return Switcher.SwitchBehaviour<RoomBehaviourInactive>();
        }
    }
}
