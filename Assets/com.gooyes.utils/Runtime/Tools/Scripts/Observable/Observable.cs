using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gooyes.Tools
{
    public class Observable<T>
    {
        public event Action<T> OnChanged;

        private T _value;
        public T Value
        {
            get { return _value; }
            set { 
                _value = value;
                OnChanged?.Invoke(value);
            }
        }

        public Observable()
        {
            Value = default;
        }

        public Observable(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
