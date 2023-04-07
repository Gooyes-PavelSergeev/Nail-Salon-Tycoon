using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gooyes.Tools
{
    public static class Coroutines
    {
        private static CoroutineObject _coroutineObject;
        private static bool _inited;

        private static void Init()
        {
            _coroutineObject = new GameObject().AddComponent<CoroutineObject>();
            _coroutineObject.name = "Coroutine Object";
            _inited = true;
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            if (!_inited) Init();
            if (coroutine == null) return null;
            return _coroutineObject.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (_coroutineObject == null || coroutine == null) return;
            _coroutineObject.StopCoroutine(coroutine);
        }

        public static void StopCoroutine(IEnumerator coroutine)
        {
            if (_coroutineObject == null || coroutine == null) return;
            _coroutineObject.StopCoroutine(coroutine);
        }

        private class CoroutineObject : MonoBehaviour
        {

        }
    }
}
