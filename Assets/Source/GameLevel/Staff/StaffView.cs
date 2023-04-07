using System;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Rooms.StaffSystem
{
    internal class StaffView : MonoBehaviour
    {
        [NonSerialized] public Staff staff;

        [NonSerialized] public Animator animator;

        public void Activate(Staff staff)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null) throw new Exception($"No animator on {staff.Name}");
            this.staff = staff;
            this.staff.Service.ServiceStartEvent += OnServiceStart;
            this.staff.Service.ServiceFinishEvent += OnServiceFinish;
            animator.Play($"Idle{staff.RoomId}");
        }

        private void OnServiceStart(Staff staff)
        {
            animator.Play($"Start{staff.RoomId}");
        }

        private void OnServiceFinish(Staff staff)
        {
            if (staff.Type == StaffType.Reception) return;
            animator.Play($"End{staff.RoomId}");
        }
    }
}
