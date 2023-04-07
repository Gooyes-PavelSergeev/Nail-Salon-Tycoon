using NailSalonTycoon.Data;
using NailSalonTycoon.GameLevel.Interactivity;
using NailSalonTycoon.GameLevel.Rooms;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Gooyes.Tools;

namespace NailSalonTycoon
{
    public class LevelLoader : MonoBehaviour, IInteractive
    {
        public static LevelLoader Instance;

        [SerializeField] private List<GameLevel.GameLevel> gameLevels;

        public Observable<GameLevel.GameLevel> currentLevel;

        public string Name { get => "Level Loader"; }
        public bool Active { get => true; }

        public event Action<InteractData> InteractionEvent;

        private InteractableObject _interactableObject;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.2f);
            currentLevel = new Observable<GameLevel.GameLevel>(LoadLevel(1));
            _interactableObject = GetComponent<InteractableObject>();
            SetInteractiveObject();
        }

        private void Update()
        {
            if (currentLevel != null && currentLevel.Value.DevMode && Input.GetKeyDown(KeyCode.LeftAlt))
            {
                DataSaver.ClearAll();
            }
        }

        public GameLevel.GameLevel LoadLevel(int levelNum)
        {
            foreach (GameLevel.GameLevel level in gameLevels)
            {
                if (levelNum == level.LevelNum)
                {
                    level.Start();
                    return level;
                }
            }

            return null;
        }

        private void OnApplicationQuit()
        {
            foreach (Room room in currentLevel.Value.ActiveRooms)
            {
                DataSaver.Save<Data_OwnedRooms>(room);
            }
            DataSaver.Save<Data_Mood>(null);
            DataSaver.Save<Data_SessionTime>(null);
            DataSaver.Save<Data_SoftCurrency>(null);
            Debug.Log("SAVE");
        }

        public void Interact(InteractData data)
        {
            DataSaver.ClearAll();
        }

        public void SetInteractiveObject()
        {
            _interactableObject.SetObject(this);
        }

        public bool TryGetObject<T>(out object obj)
        {
            obj = null;
            return false;
        }
    }
}
