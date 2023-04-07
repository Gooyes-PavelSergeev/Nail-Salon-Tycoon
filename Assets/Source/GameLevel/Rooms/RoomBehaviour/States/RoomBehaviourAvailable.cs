using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.BM
{
    public class RoomBehaviourAvailable : RoomBehaviour
    {
        public override void Enter()
        {
            _room.View.SetActiveAvailableState(true);
        }

        public override void Exit()
        {
            _room.View.SetActiveAvailableState(false);
        }

        public override void Update()
        {

        }
    }
}
