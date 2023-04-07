using NailSalonTycoon.GameLevel.Rooms;
using System.Collections.Generic;
using UnityEngine;
using Gooyes.Tools;
using NailSalonTycoon.Data;
using NailSalonTycoon.GameLevel.Interactivity;
using System;

namespace NailSalonTycoon.GameLevel
{
    public class LevelCreator : Singleton<LevelCreator>
    {
        [SerializeField] private GameLevel _gameLevel;

        private void Update()
        {
            if (_gameLevel.DevMode && Input.GetKeyDown(KeyCode.F6))
            {
                _gameLevel.roomConfigs = new List<RoomConfig>();
                RoomView[] rooms = FindObjectsOfType<RoomView>();
                if (rooms == null || rooms.Length == 0)
                {
                    throw new System.Exception("There are no rooms to save");
                }
                foreach (RoomView room in rooms)
                {
                    RoomConfig roomConfig = new RoomConfig();
                    roomConfig.position = room.transform.position;
                    roomConfig.id = room.RoomId;
                    roomConfig.rotation = room.transform.rotation;
                    roomConfig.scale = room.transform.localScale;
                    _gameLevel.roomConfigs.Add(roomConfig);
                }
                Debug.Log("Saved succesfully");
            }
        }
    }
}
