using NailSalonTycoon.Economy.UpgradeSystem.Rooms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms
{
    internal class InteriorContainerView : MonoBehaviour
    {
        [HideInInspector] public InteriorView[] interiors;

        public bool mustHaveInteriors = true;

        private void Start()
        {
            interiors = GetComponentsInChildren<InteriorView>(true);
            if ((interiors == null || interiors.Length == 0) && mustHaveInteriors) throw new Exception($"Container {gameObject.name} is empty");
            UpgradeId? id = null;
            foreach (InteriorView view in interiors)
            {
                if (id.HasValue && id == view.id) throw new Exception($"You can't have more than one {id}");
                id = view.id;
                view.Activate(false);
            }
        }

        public void AddInteriors(List<UpgradeId> upgrades)
        {
            foreach (UpgradeId upgrade in upgrades)
            {
                AddInterior(upgrade);
            }
        }

        public void AddInterior(UpgradeId id)
        {
            foreach (InteriorView view in interiors)
            {
                if (view.id == id)
                {
                    view.Activate(true);
                    break;
                }
            }
        }
    }
}
