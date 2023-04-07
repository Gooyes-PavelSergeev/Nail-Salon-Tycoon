using NailSalonTycoon.Tutorial;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NailSalonTycoon.UI
{
    internal class TutorialUI : MonoBehaviour
    {
        public TutorialLoader loader;
        public TextMeshProUGUI dialogueText;
        public Image mentorImage;
        public Image dialogueImage;
        public TutorialUIButton wholeScreenButton;
        [SerializeField] private MentorMap[] _mentorMap;
        [SerializeField] private DialogueWindowSize[] _dialogueMap;

        public bool active { get => loader.active; }
        private void Awake()
        {
            gameObject.SetActive(active);
            HideMentor();
        }
        private void OnEnable()
        {
            if (!active) return;
            TutorialObjective.CompleteEvent += OnObjectiveComplete;
            loader.gameTutorial.TutorialStartEvent += OnTutorialStart;
            loader.gameTutorial.TutorialFinishEvent += OnTutorialFinish;
        }

        private void OnDisable()
        {
            if (!active) return;
            TutorialObjective.CompleteEvent -= OnObjectiveComplete;
            loader.gameTutorial.TutorialStartEvent -= OnTutorialStart;
            loader.gameTutorial.TutorialFinishEvent -= OnTutorialFinish;
        }

        private void OnTutorialStart()
        {
            ShowMentor();
        }

        private void OnObjectiveComplete(TutorialObjective objective, bool result)
        {
            if (!result) UpdateMentor(objective);
        }

        private void UpdateMentor(TutorialObjective objective)
        {
            ObjectiveType type = objective.type;

            switch (type)
            {
                case ObjectiveType.Dialogue:
                    Sprite mentorSprite = MentorMap.GetSprite(objective.dialogueType, _mentorMap);
                    mentorImage.sprite = mentorSprite;
                    Sprite dialogueSprite = DialogueWindowSize.GetSprite(objective.dialogueText.Length, _dialogueMap);
                    dialogueText.text = objective.dialogueText;
                    dialogueImage.sprite = dialogueSprite;
                    ShowMentor();
                    break;
                case ObjectiveType.Interactable:
                    foreach (TutorialIOPulser ioPulser in loader.gameTutorial.ioPulsers)
                    {
                        ioPulser.Highlight(false);
                    }
                    TutorialIOPulser pulser = objective.targetObject.GetComponent<TutorialIOPulser>();
                    pulser.Highlight();
                    HideMentor();
                    break;
                case ObjectiveType.UIButton:
                    StartCoroutine(SetUIButton(objective));
                    break;
            }
        }

        private IEnumerator SetUIButton(TutorialObjective objective)
        {
            yield return new WaitForSeconds(0.05f);
            foreach (TutorialUIButton tButton in loader.gameTutorial.uiButtons)
            {
                if (!tButton.active) continue;
                tButton.Highlight(false);
                tButton.button.enabled = false;
            }
            TutorialUIButton button = objective.targetObject.GetComponent<TutorialUIButton>();
            button.button.enabled = true;
            button.Highlight();
            HideMentor();
        }

        private void ShowMentor()
        {
            foreach (TutorialUIButton tButton in loader.gameTutorial.uiButtons)
            {
                if (!tButton.active) continue;
                tButton.Highlight(false);
                tButton.button.enabled = false;
            }
            foreach (TutorialIOPulser ioPulser in loader.gameTutorial.ioPulsers)
            {
                ioPulser.Highlight(false);
            }
            wholeScreenButton.SetActive(true);
            mentorImage.gameObject.SetActive(true);
            dialogueImage.gameObject.SetActive(true);
        }

        private void HideMentor()
        {
            wholeScreenButton.SetActive(false);
            mentorImage.gameObject.SetActive(false);
            dialogueImage.gameObject.SetActive(false);
        }

        private void OnTutorialFinish()
        {
            HideMentor();
            foreach (TutorialUIButton button in loader.gameTutorial.uiButtons)
            {
                if (button.button == null) Debug.Log(button.gameObject);
                button.button.enabled = true;
                button.Highlight(false);
            }
            foreach (TutorialIOPulser ioPulser in loader.gameTutorial.ioPulsers)
            {
                ioPulser.Highlight(false);
            }
        }

        [Serializable]
        private struct MentorMap
        {
            public DialogueType type;
            public Sprite sprite;

            public static Sprite GetSprite(DialogueType type, MentorMap[] map)
            {
                foreach (MentorMap mentor in map)
                {
                    if (mentor.type == type)
                        return mentor.sprite;
                }
                throw new NullReferenceException($"No image for state {type}");
            }
        }

        [Serializable]
        private struct DialogueWindowSize
        {
            public Sprite sprite;
            public int symbolCount;

            public static Sprite GetSprite(int symbolCount, DialogueWindowSize[] map)
            {
                if (symbolCount == 0 || map.Length == 0) throw new Exception("You passed empty string");
                Sprite sprite = map[0].sprite;
                int longest = 0;

                foreach (DialogueWindowSize config in map)
                {
                    int configCount = config.symbolCount;
                    if (symbolCount >= configCount && longest <= configCount)
                    {
                        sprite = config.sprite;
                        longest = configCount;
                    }
                }

                if (sprite != null) return sprite;
                throw new Exception($"No sprite for {symbolCount} symbols found");
            }
        }
    }
}
