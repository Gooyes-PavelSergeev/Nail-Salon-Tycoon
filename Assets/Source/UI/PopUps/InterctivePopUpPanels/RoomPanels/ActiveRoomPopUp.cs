using NailSalonTycoon.GameLevel.Interactivity;
using NailSalonTycoon.GameLevel.Rooms;
using System;
using UnityEngine;
using UnityEngine.UI;
using Gooyes.Tools;
using TMPro;
using System.Collections.Generic;
using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using NailSalonTycoon.Economy.UpgradeSystem.Staffs;
using NailSalonTycoon.Economy;
using NailSalonTycoon.Economy.UpgradeSystem.Rooms;

namespace NailSalonTycoon.UI.PopUps
{
    internal class ActiveRoomPopUp : RoomPopUp
    {
        [SerializeField] private Tab _staffTab;
        [SerializeField] private Tab _interiorTab;
        [SerializeField] private Sprite _inactiveButtonSprite;
        [SerializeField] private Sprite _activeButtonSprite;
        [SerializeField] private List<HireAndPurchaseElement> _staffs;
        [SerializeField] private List<InteriorUIElement> _interiors;
        [SerializeField] private TextMeshProUGUI _roomName;
        private bool _inited = false;

        public override void Initialize(GameLevel.GameLevel gameLevel)
        {
            _staffTab.tabButton.onClick.AddListener(ChangeTab);
            _staffTab.Init(_inactiveButtonSprite, _activeButtonSprite);
            _interiorTab.tabButton.onClick.AddListener(ChangeTab);
            _interiorTab.Init(_inactiveButtonSprite, _activeButtonSprite);
            for (int i = 0; i < _staffs.Count; i++)
            {
                _staffs[i].id = i;
                _staffs[i].DataChangedEvent += FillData;
            }
            for (int i = 0; i < _interiors.Count; i++)
            {
                _interiors[i].Init(i, this);
            }
            _inited = true;
        }

        private void OnEnable()
        {
            if (!_inited) return;
            Wallet.MoneyChangedEvent += (money) => FillData();
        }

        private void OnDisable()
        {
            if (!_inited) return;
            Wallet.MoneyChangedEvent -= (money) => FillData();
        }

        private void ChangeTab()
        {
            bool staffsIsActive = _staffTab.active.Value;
            _staffTab.active.Value = !staffsIsActive;
            _interiorTab.active.Value = staffsIsActive;
        }

        public override void PopUp(InteractData interactData)
        {
            if (interactData.interactive != null && interactData.interactive.TryGetObject<Room>(out object obj))
            {
                Room room = obj as Room;
                if (room != null && room.Active)
                {
                    OnRoomValidate(room);
                    ShowStaffsTab($"{room.Name} room");
                }
            }
        }

        protected override void FillData()
        {
            List<Staff> staffs = _lastInteractedRoom.Staffs;
            int roomStaffCount = staffs.Count;
            int roomStaffMaxCount = _lastInteractedRoom.MaxStaffCount;
            for (int i = 0; i < _staffs.Count; i++)
            {
                if (i < roomStaffCount)
                {
                    Staff staff = staffs[i];
                    _staffs[i].SetActive(true);
                    _staffs[i].FillData(staff);
                }
                else
                {
                    if (i == roomStaffCount && roomStaffMaxCount > roomStaffCount)
                    {
                        _staffs[i].SetActive(true);
                        _staffs[i].FillData(_lastInteractedRoom);
                    }
                    else
                    {
                        _staffs[i].SetActive(false);
                    }
                }
            }
            for (int i = 0; i < _interiors.Count; i++)
            {
                _interiors[i].FillData(_lastInteractedRoom);
            }
        }

        private void ShowStaffsTab(string roomName)
        {
            _roomName.text = roomName;
            _interiorTab.active.Value = false;
            _staffTab.active.Value = true;
        }

        [Serializable]
        private struct Tab
        {
            public GameObject tabObject;
            public Button tabButton;
            [NonSerialized] public Observable<bool> active;
            private Sprite _inactiveButtonSprite;
            private Sprite _activeButtonSprite;


            public void Init(Sprite inactiveButton, Sprite activeButton)
            {
                _inactiveButtonSprite = inactiveButton;
                _activeButtonSprite = activeButton;
                active = new Observable<bool>();
                active.OnChanged += OnActiveStateChange;
            }

            private void OnActiveStateChange(bool active)
            {
                tabObject.SetActive(active);
                tabButton.enabled = !active;
                tabButton.image.sprite = tabButton.enabled ? _inactiveButtonSprite : _activeButtonSprite;
            }
        }


        [Serializable]
        private class HireAndPurchaseElement
        {
            [NonSerialized] public int id;
            public TextMeshProUGUI mainText;
            public TextMeshProUGUI priceText;
            public Button button;

            public event Action DataChangedEvent;

            public void FillData(Staff staff)
            {
                float price = staff.GetNextUpgradeData().price;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => UpgradeStaff(staff));
                int currentLevel = staff.CurrentLevel.Value;
                mainText.text = "Level " + currentLevel.ToString();
                priceText.text = currentLevel < StaffUpgrader._maxLevel ? TextDisplayer.GetInfo(price, 2).AmountWithPrefix : "MAX";
                TextDisplayer.HandleMoneyTextColor(priceText, price);
                SetAvailable(price);
            }

            public void FillData(Room room)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => AddStaff(room));
                mainText.text = "Hire";
                float price = room.NewStaffPurchasePrice;
                priceText.text = TextDisplayer.GetInfo(price, 2).AmountWithPrefix;
                TextDisplayer.HandleMoneyTextColor(priceText, price);
                SetAvailable(price);
            }

            private void UpgradeStaff(Staff staff)
            {
                staff.Upgrade();
                DataChangedEvent?.Invoke();
            }

            private void AddStaff(Room room)
            {
                room.AddStaff();
                DataChangedEvent?.Invoke();
            }

            public void SetActive(bool active)
            {
                button.gameObject.SetActive(active);
                button.enabled = active;
                mainText.text = "";
                priceText.text = "";
            }

            private void SetAvailable(float price)
            {
                button.enabled = Wallet.CheckEnoughMoney(price);
            }
        }


        [Serializable]
        private class InteriorUIElement
        {
            public TextMeshProUGUI nameText;
            public TextMeshProUGUI priceText;
            public Button button;
            private Image _image;
            [NonSerialized] public int id;
            private UpgradeId _upgradeId;
            private bool _isOpen;
            private Room _lastInteractedRoom;

            public void Init(int id, ActiveRoomPopUp popUp)
            {
                this.id = id;
                _upgradeId = (UpgradeId)id;
                _image = button.gameObject.GetComponent<Image>();
                if (_image == null)
                    throw new Exception($"Place image and {button.gameObject.name} on the same gameObject");
                nameText.text = GetName(_upgradeId);
                popUp.PopUpShowEvent += OnPopUpShow;
                Wallet.MoneyChangedEvent += OnMoneyChange;
                ShowElement();
            }

            private string GetName(UpgradeId upgradeId)
            {
                if (upgradeId == UpgradeId.CoffeeTable)
                    return "Table";
                return upgradeId.ToString();
            }

            private void OnPopUpShow(bool showState)
            {
                _isOpen = showState;
            }

            private void OnMoneyChange(float newValue)
            {
                if (_isOpen && _lastInteractedRoom != null)
                {
                    FillData(_lastInteractedRoom);
                }
            }

            public void FillData(Room room)
            {
                _lastInteractedRoom = room;
                List<UpgradeId> owned = room.OwnedUpgrades.Value;
                foreach (UpgradeId upgradeId in owned)
                {
                    if (upgradeId == _upgradeId)
                    {
                        HideElement();
                        return;
                    }
                }
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => BuyUpgrade(room));
                var dict = UpgradeConfigurator.GetUpgradeMap((Economy.UpgradeSystem.UpgradableType)room.RoomId);
                if (dict.TryGetValue(_upgradeId, out float price))
                {
                    ShowElement();
                    priceText.text = TextDisplayer.GetInfo(price, 2).AmountWithPrefix;
                    TextDisplayer.HandleMoneyTextColor(priceText, price);
                    button.enabled = Wallet.CheckEnoughMoney(price);
                }
                else
                {
                    HideElement();
                }
            }

            private void BuyUpgrade(Room room)
            {
                if (room.Upgrade(_upgradeId))
                {
                    FillData(room);
                }
            }

            private void ShowElement()
            {
                button.enabled = true;
                _image.color = new Color32(255, 255, 255, 255);
                priceText.color = new Color32(255, 255, 255, 255);
                nameText.color = new Color32(255, 255, 255, 255);
                priceText.text = "000.00K";
            }

            private void HideElement()
            {
                button.enabled = false;
                _image.color = new Color32(135, 135, 135, 255);
                priceText.color = new Color32(135, 135, 135, 255);
                nameText.color = new Color32(135, 135, 135, 255);
                priceText.text = "Out";
            }
        }
    }
}
