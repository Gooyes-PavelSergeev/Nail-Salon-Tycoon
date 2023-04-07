using NailSalonTycoon.Economy.UpgradeSystem.Rooms;
using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms
{
    internal class InteriorView : MonoBehaviour
    {
        public UpgradeId id;

        public bool active;

        public void Activate(bool state)
        {
            active = state;
            gameObject.SetActive(state);
        }
    }
}
