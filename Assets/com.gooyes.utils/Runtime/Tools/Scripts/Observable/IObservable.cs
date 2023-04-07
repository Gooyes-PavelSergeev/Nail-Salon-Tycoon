using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gooyes.Tools
{
    public interface IObservable
    {
        public event Action<object> OnChanged;
    }
}
