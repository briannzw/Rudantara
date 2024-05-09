using UnityEngine;

namespace Save
{
    using Data;
    using System;

    public class SaveManager : MonoBehaviour
    {
        public bool IsEncrypted = true;
        public SaveData SaveData;
        [Header("Statistics")]
        [SerializeField] private float saveTime;
        [SerializeField] private float loadTime;
        private IDataPersistence dataPersistence = new DataPersistenceManager();

        public void Save()
        {
            var startTime = DateTime.Now.Ticks;
            if (dataPersistence.WriteData("/data.save", SaveData, IsEncrypted))
            {
                saveTime = (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
                Debug.Log("Data successfuly saved!");
            }
        }

        public void Load()
        {
            var startTime = DateTime.Now.Ticks;
            SaveData = dataPersistence.ReadData<SaveData>("/data.save", IsEncrypted);
            loadTime = (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
        }

        public bool CheckDataExists()
        {
            return dataPersistence.CheckExists("/data.save");
        }
    }
}