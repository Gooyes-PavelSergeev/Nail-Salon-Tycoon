using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.BM
{
    public class RoomBehaviourInactive : RoomBehaviour
    {
        public override void Enter()
        {
            _room.View.SetActiveNormalState(false);
            _room.View.SetActiveAvailableState(false);
        }

        public override void Exit()
        {
            _room.View.SetActiveNormalState(false);
            _room.View.SetActiveAvailableState(false);
        }

        public override void Update()
        {

        }
    }
}
