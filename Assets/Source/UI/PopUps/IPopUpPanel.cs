using System;
using UnityEngine;

namespace NailSalonTycoon.UI.PopUps
{
    public interface IPopUpPanel
    {
        public event Action<bool> PopUpShowEvent;
        public static event Action<bool, IPopUpPanel> ShowEventStatic;
        public bool IsOpened { get; set; }
        public void ClosePanel();
    }
}
