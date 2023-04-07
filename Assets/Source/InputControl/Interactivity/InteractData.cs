using UnityEngine.EventSystems;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Interactivity
{
    public class InteractData
    {
        public PointerEventData pointerEventData;
        public IInteractive interactive;
        public GameObject interactedGameObject;
        public int touchCountOnDown;
        public Touch? touch0OnDown;
        public Touch? touch1OnDown;
    }
}
