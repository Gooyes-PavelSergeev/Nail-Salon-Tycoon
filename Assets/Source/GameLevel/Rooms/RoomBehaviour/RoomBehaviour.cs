using System;
using Gooyes.BehaviourMachine;
using UnityEngine;
using NailSalonTycoon.GameLevel;

namespace NailSalonTycoon.GameLevel.Rooms.BM
{
    public abstract class RoomBehaviour : IStateBehaviour
    {
        protected BehaviourSwitcher<RoomBehaviour> _switcher;

        protected Room _room;

        public void Init(BehaviourSwitcher<RoomBehaviour> switcher, Room room)
        {
            _switcher = switcher;
            _room = room;
        }

        public abstract void Enter();

        public abstract void Exit();

        public abstract void Update();
    }
}
