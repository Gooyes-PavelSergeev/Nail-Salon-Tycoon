using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gooyes.BehaviourMachine
{
    public interface IStateBehaviour
    {
        public void Enter();
        public void Update();
        public void Exit();
    }
}
