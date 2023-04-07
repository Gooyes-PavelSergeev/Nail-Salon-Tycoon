using NailSalonTycoon.GameLevel.Rooms.StaffSystem;
using Gooyes.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NailSalonTycoon.GameLevel.Clients
{
    [CreateAssetMenu(fileName = "ClientSpawner")]
    public class ClientSpawner : ScriptableObject
    {
        private bool _active;

        [SerializeField] private List<ClientConfig> _clientConfigs;

        private Staff _receptionStaff;

        private GameLevel _gameLevel;

        private ServiceFinder _serviceFinder;

        private float _spawnRate;

        public void Initialize(GameLevel gameLevel)
        {
            _active = true;
            _gameLevel = gameLevel;
            _serviceFinder = new ServiceFinder(gameLevel);
            gameLevel.Staffs.OnChanged += SetReceptionStaff;
        }

        private void SetReceptionStaff(List<Staff> staffs)
        {
            Staff staff = staffs[staffs.Count - 1];
            if (staff.Type == StaffType.Reception)
            {
                _receptionStaff = staff;
                _spawnRate = staff.IncomeAmount.Value;
                staff.IncomeAmount.OnChanged += ChangeSpawnRate;
                Coroutines.StartCoroutine(RunSpawningProcess());
                _gameLevel.Staffs.OnChanged -= SetReceptionStaff;
            }
        }

        private void ChangeSpawnRate(float newValue)
        {
            _spawnRate = newValue;
        }

        private IEnumerator RunSpawningProcess()
        {
            float timer = 0;

            while (_active)
            {
                timer += Time.deltaTime;
                if (timer >= 60 / _spawnRate)
                {
                    SpawnClient();
                    timer = 0;
                }
                yield return null;
            }
        }

        public Client SpawnClient(bool fromToggle = false)
        {
            if (fromToggle)
            {
                Debug.Log("Don't forget to make it private");
            }
            if (!_active)
            {
                Debug.LogWarning("Client spawner is inactive");
                return null;
            }

            if (_serviceFinder.HasFreeStaffOrRestSpace())
            {
                Client client = Client.Instantiate(_serviceFinder, GetRandomConfig());
                return client;
            }

            return null;
        }

        private ClientConfig GetRandomConfig()
        {
            int index = UnityEngine.Random.Range(0, _clientConfigs.Count);
            return _clientConfigs[index];
        }
    }
}
