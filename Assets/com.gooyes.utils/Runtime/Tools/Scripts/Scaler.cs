using UnityEngine;

namespace Gooyes.Tools
{
    public static class Scaler
    {
        private static bool _inited;
        private static float _fieldVertExtent;
        private static float _fieldHorExtent;

        private static void Init()
        {
            _fieldVertExtent = Camera.main.orthographicSize;
            _fieldHorExtent = _fieldVertExtent * Screen.width / Screen.height;
            _inited = true;
        }

        public static void ScalePlaneAsBackground(Transform targetTransform)
        {
            if (!_inited) Init();
            Vector3 scale = new Vector3(_fieldHorExtent * 2 / 10 + 0.05f, 1, _fieldVertExtent * 2 / 10 + 0.05f);
            targetTransform.localScale = scale;
        }
    }
}
