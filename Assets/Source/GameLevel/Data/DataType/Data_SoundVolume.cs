using System;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    public class Data_SoundVolume : DataToSave
    {
        public VolumeData data;
        public override T Load<T>()
        {
            Data_SoundVolume data = new Data_SoundVolume();
            data.data = new VolumeData();
            data.data.uiVolume = PlayerPrefs.GetFloat("VolumeUI", 1f);
            data.data.backgroundVolume = PlayerPrefs.GetFloat("VolumeBG", 1f);
            return data as T;
        }

        public override void Save(object sender)
        {
            VolumeData data = sender as VolumeData;
            if (data.uiVolume.HasValue) PlayerPrefs.SetFloat("VolumeUI", data.uiVolume.Value);
            if (data.backgroundVolume.HasValue) PlayerPrefs.SetFloat("VolumeBG", data.backgroundVolume.Value);
        }

        public override void Clear()
        {
            PlayerPrefs.SetFloat("VolumeUI", 1f);
            PlayerPrefs.SetFloat("VolumeBG", 1f);
        }

        public class VolumeData
        {
            public float? uiVolume;
            public float? backgroundVolume;
        }
    }
}
