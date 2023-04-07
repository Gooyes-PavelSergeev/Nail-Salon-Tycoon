using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NailSalonTycoon.GameLevel.Interactivity
{
    public class InteractableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool active = true;

        public string ioName = "Interactable object";

        private IInteractive _interactive;
        public static event Action <InteractData> InteractEvent;

        private InteractData _lastInteractData;
        [SerializeField] private float _swipeMaxDistance;

        public void SetObject(IInteractive interactive)
        {
            _interactive = interactive;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!active || _interactive == null) return;
            InteractData interactData = new InteractData();
            interactData.pointerEventData = eventData;
            interactData.interactive = _interactive;
            interactData.interactedGameObject = eventData.pointerEnter;
            interactData.touchCountOnDown = Input.touchCount;
            switch (interactData.touchCountOnDown)
            {
                case 1:
                    interactData.touch0OnDown = Input.GetTouch(0);
                    interactData.touch1OnDown = null;
                    break;
                case 2:
                    interactData.touch0OnDown = Input.GetTouch(0);
                    interactData.touch1OnDown = Input.GetTouch(1);
                    break;
                default:
                    interactData.touch0OnDown = null;
                    interactData.touch1OnDown = null;
                    break;
            }
            _lastInteractData = interactData;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!active || _interactive == null) return;
            if (_lastInteractData == null) return;
            if (_lastInteractData.interactedGameObject == eventData.pointerEnter)
            {
                //if (Input.touchCount != 1) return;
                if (_lastInteractData.touch0OnDown.HasValue)
                {
                    float distance = Vector2.Distance(_lastInteractData.touch0OnDown.Value.position, Input.GetTouch(0).position);
                    Debug.Log($"Tap distance {distance}");
                    if (distance > _swipeMaxDistance) return;
                }
                else
                    return;

                _interactive.Interact(_lastInteractData);
                InteractEvent?.Invoke(_lastInteractData);
            }
            _lastInteractData = null;
        }

        public void Interact(GameObject go)
        {
            InteractData interactData = new InteractData();
            interactData.pointerEventData = null;
            interactData.interactive = _interactive;
            interactData.interactedGameObject = go;
            interactData.touchCountOnDown = Input.touchCount;
            InteractEvent?.Invoke(interactData);
        }
    }
}
