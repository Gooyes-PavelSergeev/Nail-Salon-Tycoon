using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gooyes.BehaviourMachine
{
    public abstract class BehaviourMachine<T> where T : IStateBehaviour
    {
        protected Dictionary<Type, T> _behavioursMap = new();
        protected T _previousBehaviour;
        protected T _currentBehaviour;
        public event Action<T> SwitchBehaviourEvent;
        private BehaviourSwitcher<T> _switcher;
        public BehaviourSwitcher<T> Switcher
        { 
            get
            {
                if (_switcher == null)
                    _switcher = new BehaviourSwitcher<T>(this);
                return _switcher;
            }
        }

        protected abstract void FillBehaviours();

        public T GetBehaviour<TP>() where TP : T
        {
            var type = typeof(TP);
            return _behavioursMap[type];
        }

        public T AddBehaviour<TP>() where TP : T, new()
        {
            var type = typeof(TP);
            if (_behavioursMap.ContainsKey(type))
            {
                throw new Exception("This behavior already exists");
            }
            _behavioursMap.Add(type, new TP());
            return _behavioursMap[type];
        }

        internal T SwitchBehaviour(Type type)
        {
            if (_behavioursMap.TryGetValue(type, out T behaviour))
            {
                if (behaviour == null) throw new NullReferenceException("Behaviour is not setup");
                if (_currentBehaviour != null) _currentBehaviour.Exit();
                _previousBehaviour = _currentBehaviour;
                _currentBehaviour = behaviour;
                _currentBehaviour.Enter();
                SwitchBehaviourEvent?.Invoke(_currentBehaviour);
            }
            else
                throw new Exception($"Behaviour {type.Name} cannot be setup");
            return _currentBehaviour;
        }

        internal T SwitchToPrevious()
        {
            if (_previousBehaviour != null)
                return SwitchBehaviour(_previousBehaviour.GetType());
            return _currentBehaviour;
        }

        protected abstract T SetDefaultBehaviour();
    }
}
