using NailSalonTycoon.GameLevel.Rooms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    public abstract class DataToSave
    {
        public abstract void Save(object sender);
        public abstract T Load<T>() where T : DataToSave, new();
        public abstract void Clear();
    }
}
