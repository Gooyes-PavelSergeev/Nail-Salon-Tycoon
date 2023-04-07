using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Interactivity
{
    public interface IInteractive
    {
        event Action<InteractData> InteractionEvent;
        void Interact(InteractData data);
        void SetInteractiveObject();
        bool TryGetObject<T>(out object obj);
        string Name { get; }
        bool Active { get; }
    }
}
