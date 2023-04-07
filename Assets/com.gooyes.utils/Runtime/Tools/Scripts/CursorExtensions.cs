using UnityEngine;

namespace Gooyes.Extensions
{
    public static class CursorExtensions
    {
        public static void Show()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void Hide()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static bool ChangeShowState()
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
            return Cursor.visible;
        }
    }
}
