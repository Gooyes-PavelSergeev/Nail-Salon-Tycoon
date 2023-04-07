using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NailSalonTycoon.UI
{
    internal class SliderHandler : MonoBehaviour
    {
        private Scrollbar _scrollbar;
        private ScrollRect _scrollRect;

        [SerializeField] private Button _scrollDownArrow;
        [SerializeField] private Button _scrollUpArrow;

        private Image _arrowDownImage;
        private Image _arrowUpImage;

        public float scrollSpeed = 1f;

        [SerializeField] private GameObject[] _content;

        private int _numOfElements = 5;

        private float _selectedHeight;

        public float crossFadeTime = 0.2f;
        public float inactiveAlpha = 0.3f;

        private Coroutine _scrollCoroutine;
        private void Awake()
        {
            SetScrollbar();
            SetScrollRect();
            _arrowDownImage = _scrollDownArrow.GetComponent<Image>();
            _arrowUpImage = _scrollUpArrow.GetComponent<Image>();
            _numOfElements = _content.Length;
        }

        private void Start()
        {
            _scrollDownArrow.onClick.AddListener(() => Scroll(true));
            _scrollUpArrow.onClick.AddListener(() => Scroll(false));
            _numOfElements = _content.Length;
            if (_numOfElements < 4) throw new Exception("Min content count is 4");
            _selectedHeight = _content[1].transform.position.y;
            if (_arrowDownImage == null || _arrowUpImage == null)
                throw new Exception("You don't have image component on slide arrows");
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            HandleArrows(new Vector2(0, _scrollbar.value));
        }

        private void SetScrollbar()
        {
            var scrollbars = GetComponentsInChildren<Scrollbar>();
            if (scrollbars == null || scrollbars.Length > 1)
                throw new Exception("There can't be more than one scrollbar");
            _scrollbar = scrollbars[0];
            _scrollbar.gameObject.SetActive(true);
            _scrollbar.enabled = true;
            _scrollbar.value = 1;
        }

        private void SetScrollRect()
        {
            var scrollRects = GetComponentsInChildren<ScrollRect>();
            if (scrollRects == null || scrollRects.Length > 1) throw new System.Exception("There can't be more than one scrollbar");
            _scrollRect = scrollRects[0];
            _scrollRect.gameObject.SetActive(true);
            _scrollRect.enabled = true;
            _scrollRect.onValueChanged.AddListener(HandleArrows);
        }

        private void HandleArrows(Vector2 position)
        {
            float rateVertical = position.y;
            if (rateVertical <= 0.135)
            {
                _arrowDownImage.CrossFadeAlpha(inactiveAlpha, crossFadeTime, true);
                _arrowUpImage.CrossFadeAlpha(1, crossFadeTime, true);
                _scrollDownArrow.enabled = false;
                _scrollUpArrow.enabled = true;
            }
            else if (rateVertical >= 0.94)
            {
                _arrowDownImage.CrossFadeAlpha(1f, crossFadeTime, true);
                _arrowUpImage.CrossFadeAlpha(inactiveAlpha, crossFadeTime, true);
                _scrollDownArrow.enabled = true;
                _scrollUpArrow.enabled = false;
            }
            else
            {
                _arrowDownImage.CrossFadeAlpha(1f, crossFadeTime, true);
                _arrowUpImage.CrossFadeAlpha(1, crossFadeTime, true);
                _scrollDownArrow.enabled = true;
                _scrollUpArrow.enabled = true;
            }
        }

        private void Scroll(bool down)
        {
            int currentElement = GetCurrentElement();
            int targetElement = currentElement + (down ? 1 : -1);
            targetElement = Mathf.Clamp(targetElement, 1, _numOfElements - 2);
            HandleArrows(new Vector2(0, _scrollbar.value));
            if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = StartCoroutine(Scroll(targetElement, down ? -this.scrollSpeed : this.scrollSpeed));
        }

        private IEnumerator Scroll(int targetElement, float scrollSpeed)
        {
            GameObject targetObject = _content[targetElement];
            while (Mathf.Abs(_selectedHeight - targetObject.transform.position.y) > 5f)
            {
                yield return null;
                _scrollbar.value += scrollSpeed * Time.deltaTime;
            }
        }

        private int GetCurrentElement()
        {
            float bufferDifference = 10000f;
            int selectedElement = 1;
            for (int i = 0; i < _numOfElements; i++)
            {
                float height = _content[i].transform.position.y;
                float difference = Mathf.Abs(height - _selectedHeight);
                if (difference <= bufferDifference)
                {
                    bufferDifference = difference;
                    selectedElement = i;
                }
            }
            return selectedElement;
        }
    }
}
