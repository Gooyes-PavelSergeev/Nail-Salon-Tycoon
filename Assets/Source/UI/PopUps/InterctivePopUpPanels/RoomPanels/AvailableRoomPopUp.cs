using NailSalonTycoon.Economy;
using NailSalonTycoon.GameLevel.Interactivity;
using NailSalonTycoon.GameLevel.Rooms;
using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NailSalonTycoon.UI.PopUps
{
    internal class AvailableRoomPopUp : RoomPopUp
    {
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private Sprite _ableToBuyTexture;
        [SerializeField] private Sprite _unableToBuyTexture;
        [SerializeField] private TextMeshProUGUI _roomPriceText;
        [SerializeField] private TextMeshProUGUI _purchaseInfo;
        [SerializeField] private Button _purchaseButton;
        private GameLevel.GameLevel _gameLevel;
        private Image purchaseButtonImage;
        private bool _ableToBuy;

        public override void Initialize(GameLevel.GameLevel gameLevel)
        {
            _gameLevel = gameLevel;
            purchaseButtonImage = _purchaseButton.gameObject.GetComponent<Image>();
            _purchaseButton.onClick.AddListener(OnPurchaseClick);
        }

        private void OnPurchaseClick()
        {
            if (!_ableToBuy) return;
            _gameLevel.BuyNewRoom();
            ClosePanel();
        }

        public override void PopUp(InteractData interactData)
        {
            if (interactData.interactive != null && interactData.interactive.TryGetObject<Room>(out object obj))
            {
                Room room = obj as Room;
                if (room != null && !room.Active) OnRoomValidate(room);
            }
        }

        protected override void FillData()
        {
            float price = _lastInteractedRoom.PurchasePrice;
            _roomPriceText.text = TextDisplayer.GetInfo(price, 2).AmountWithPrefix;
            _roomNameText.text = _lastInteractedRoom.Name;
            bool ableToBuy = _gameLevel.ValidateNewRoom(out int statementFailure);
            bool enoughMoney = Wallet.CheckEnoughMoney(price);
            purchaseButtonImage.sprite = (ableToBuy && enoughMoney) ? _ableToBuyTexture : _unableToBuyTexture;
            if (ableToBuy)
            {
                _purchaseInfo.text = "Open new room to serve more clients";
            }
            else
            {
                switch (statementFailure)
                {
                    case 1:
                        _purchaseInfo.text = "Previous room is inactive";
                        break;
                    case 2:
                        _purchaseInfo.text = "You must have cashier in reception and at least one employee in previous room";
                        break;
                    case 3:
                        _purchaseInfo.text = "At least one of your employees in previous room must be 25 lvl or higher";
                        break;
                }
            }
            _ableToBuy = ableToBuy;
            TextDisplayer.HandleMoneyTextColor(_roomPriceText, price);
            _purchaseButton.enabled = ableToBuy && enoughMoney;
        }
    }
}
