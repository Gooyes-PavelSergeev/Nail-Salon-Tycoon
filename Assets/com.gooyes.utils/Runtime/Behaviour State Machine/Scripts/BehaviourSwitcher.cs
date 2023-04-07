using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gooyes.BehaviourMachine
{
    public class BehaviourSwitcher<T> where T : IStateBehaviour
    {
        private readonly BehaviourMachine<T> _machine;

        public BehaviourSwitcher(BehaviourMachine<T> machine)
        {
            _machine = machine;
        }

        public T SwitchBehaviour(Type type)
        {
            return _machine.SwitchBehaviour(type);
        }

        public T SwitchBehaviour<TP>() where TP : T
        {
            return _machine.SwitchBehaviour(typeof(TP));
        }

        public T SwitchToPrevious()
        {
            return _machine.SwitchToPrevious();
        }
    }
}
