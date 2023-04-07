using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NailSalonTycoon.Data
{
    public class Data_Tutorial : DataToSave
    {
        public SavingData savingData;

        public override T Load<T>()
        {
            Data_Tutorial data = new Data_Tutorial();
            data.savingData = new SavingData
            {
                isDone = PlayerPrefs.GetInt("TutorialComplete", 0) == 1,
                currentStep = PlayerPrefs.GetInt("TutorialStep", 0),
                stepObjective = PlayerPrefs.GetInt("TutorialObjective", 0)
            };
            return data as T;
        }

        public override void Save(object sender)
        {
            SavingData data = sender as SavingData;
            PlayerPrefs.SetInt("TutorialComplete", data.isDone ? 1 : 0);
            PlayerPrefs.SetInt("TutorialStep", data.currentStep);
            PlayerPrefs.SetInt("TutorialObjective", data.stepObjective);
        }

        public override void Clear()
        {
            PlayerPrefs.SetInt("TutorialComplete", 0);
            PlayerPrefs.SetInt("TutorialStep", 0);
            PlayerPrefs.SetInt("TutorialObjective", 0);
        }

        public class SavingData
        {
            public bool isDone;
            public int currentStep;
            public int stepObjective;
        }
    }
}
