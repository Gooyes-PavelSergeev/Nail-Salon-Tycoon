using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.BM
{
    public class RoomBehaviourActive : RoomBehaviour
    {
        public override void Enter()
        {
            _room.View.SetActiveNormalState(true);
        }

        public override void Exit()
        {
            _room.View.SetActiveNormalState(false);
        }

        public override void Update()
        {

        }
    }
}
