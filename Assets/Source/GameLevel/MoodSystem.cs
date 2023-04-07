using NailSalonTycoon.Economy.UpgradeSystem.Rooms;
using NailSalonTycoon.GameLevel.Rooms;
using System;
using System.Collections.Generic;
using Gooyes.Tools;
using UnityEngine;

namespace NailSalonTycoon.GameLevel
{
    public static class MoodSystem
    {
        #region InteriorBoostMap

            #region Reception
                private static readonly Dictionary<UpgradeId, float> _receptionInteriorMap = new Dictionary<UpgradeId, float>
                {
                    { UpgradeId.Flowers, 15 },
                    { UpgradeId.TV, 5 },
                    { UpgradeId.Couch, 10 },
                    { UpgradeId.CoffeeTable, 15 },
                    { UpgradeId.Painting, 25 }
                };
            #endregion

            #region Manicure
                private static readonly Dictionary<UpgradeId, float> _manicureInteriorMap = new Dictionary<UpgradeId, float>
                {
                    { UpgradeId.Flowers, 15 },
                    { UpgradeId.TV, 5 },
                    { UpgradeId.Couch, 10 },
                    { UpgradeId.CoffeeTable, 15 },
                    { UpgradeId.Painting, 25 }
                };
            #endregion

            #region Pedicure
                private static readonly Dictionary<UpgradeId, float> _pedicureInteriorMap = new Dictionary<UpgradeId, float>
                {
                    { UpgradeId.Flowers, 5 },
                    { UpgradeId.TV, 10 },
                    { UpgradeId.Couch, 15 },
                    { UpgradeId.CoffeeTable, 20 },
                    { UpgradeId.Painting, 25 }
                };
            #endregion

            #region Rest
                private static readonly Dictionary<UpgradeId, float> _restInteriorMap = new Dictionary<UpgradeId, float>
                {
                    { UpgradeId.Flowers, 5 },
                    { UpgradeId.TV, 10 },
                    { UpgradeId.Couch, 15 },
                    { UpgradeId.CoffeeTable, 20 },
                    { UpgradeId.Painting, 25 }
                };
            #endregion

            #region Visage
                private static readonly Dictionary<UpgradeId, float> _visageInteriorMap = new Dictionary<UpgradeId, float>
                {
                    { UpgradeId.Flowers, 5 },
                    { UpgradeId.TV, 10 },
                    { UpgradeId.Couch, 15 },
                    { UpgradeId.CoffeeTable, 20 },
                    { UpgradeId.Painting, 25 }
                };
            #endregion

        #endregion

        private static GameLevel _gameLevel;
        public static Observable<float> CurrentMood { get; private set; }

        public const float defaultMoodValue = 100;

        public static void Initialize(GameLevel gameLevel, float startMood)
        {
            CurrentMood = new Observable<float>(startMood);
            _gameLevel = gameLevel;
            gameLevel.RoomAddedEvent += OnRoomAdded;
            foreach (Room room in gameLevel.ActiveRooms)
            {
                room.OwnedUpgrades.OnChanged += (upgrades) => OnInteriorAdded(upgrades, room);
            }
        }

        public static float AffectMoneyByMood(ref float value)
        {
            float factor = CurrentMood.Value / 100;
            value *= factor;
            return value;
        }

        private static void OnRoomAdded(Room room)
        {
            RoomId roomId = room.RoomId;
            float effect = 0;
            switch (roomId)
            {
                case RoomId.Pedicure:
                    effect = -25;
                    break;
                case RoomId.Rest:
                    effect = -50;
                    break;
                case RoomId.Visage:
                    effect = -75;
                    break;
            }
            room.OwnedUpgrades.OnChanged += (upgrades) => OnInteriorAdded(upgrades, room);
            float newValue = CurrentMood.Value + effect;
            CurrentMood.Value = newValue < 5 ? 5 : newValue;
        }

        private static void OnInteriorAdded(List<UpgradeId> upgrades, Room room)
        {
            UpgradeId upgrade = upgrades[upgrades.Count - 1];
            Dictionary<UpgradeId, float> interiorMap = GetBoostMap(room.RoomId);
            if (interiorMap.TryGetValue(upgrade, out float value))
            {
                CurrentMood.Value = CurrentMood.Value + value;
            }
            else
            {
                throw new Exception($"There is no interior field {upgrade} for {room.RoomId}");
            }
        }

        private static Dictionary<UpgradeId, float> GetBoostMap(RoomId roomId)
        {
            Dictionary<UpgradeId, float> result = new Dictionary<UpgradeId, float>();
            switch (roomId)
            {
                case RoomId.Reception:
                    result = _receptionInteriorMap;
                    break;
                case RoomId.Manicure:
                    result = _manicureInteriorMap;
                    break;
                case RoomId.Pedicure:
                    result = _pedicureInteriorMap;
                    break;
                case RoomId.Rest:
                    result = _restInteriorMap;
                    break;
                case RoomId.Visage:
                    result = _visageInteriorMap;
                    break;
            }
            if (result.Count < 1)
            {
                throw new Exception($"There is no interior boost map for {roomId}");
            }
            return result;
        }
    }
}
