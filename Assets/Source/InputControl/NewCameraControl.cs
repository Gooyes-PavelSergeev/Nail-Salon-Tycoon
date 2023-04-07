using Cinemachine;
using Gooyes.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewCameraControl : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID
    public Camera cameraToControl;
    private CinemachineVirtualCamera _virtualCam;
    public BoxCollider confiner;
    private Vector3 _confinerSize;
    public Vector2 maxConfinerSize;

    public Vector2 zoomEdgeValues = new Vector2(2.5f, 10f);
    [Range(0, 1)] public float zoomSpeed = 0.5f;
    [Range(0, 2)] public float scrollSpeedFactor = 0.5f;

    private Plane _plane;

    public Touch touch0;
    private bool _touch0Moved;
    public Touch touch1;
    private int _touchCount;
    public event Action<int> TouchReceivedEvent;

    [SerializeField] private LayerMask _UIMask;

    private void Start()
    {
        if (cameraToControl == null)
            cameraToControl = Camera.main;
        _virtualCam = cameraToControl.GetComponent<CinemachineVirtualCamera>();
        _confinerSize = confiner.size;
        _plane.SetNormalAndPosition(transform.forward, transform.position + transform.forward);
    }

    private void Update()
    {
        _touchCount = Input.touchCount;
        if (_touchCount >= 2)
        {
            touch0 = Input.GetTouch(0);
            touch1 = Input.GetTouch(1);
            _touch0Moved = touch0.phase == TouchPhase.Moved;
            return;
        }
        if (_touchCount >= 1)
        {
            TouchReceivedEvent?.Invoke(_touchCount);
            touch0 = Input.GetTouch(0);
            _touch0Moved = touch0.phase == TouchPhase.Moved;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (CheckPointerOverUI()) return;

        if (_touch0Moved)
        {
            Vector3 deltaTouch = PlanePositionDelta(touch0);
            cameraToControl.transform.Translate(deltaTouch, Space.World);
        }

        if (_touchCount >= 2)
        {
            Vector2 touch0Pos = touch0.position;
            Vector2 touch1Pos = touch1.position;
            Vector2 touch0PosBefore = touch0Pos - touch0.deltaPosition;
            Vector2 touch1PosBefore = touch1Pos - touch1.deltaPosition;

            float zoom = Vector3.Distance(touch0Pos, touch1Pos) / Vector3.Distance(touch0PosBefore, touch1PosBefore);

            if (zoom < 0.5f || zoom > 1.5f)
                return;

            float deltaZoom = (zoom - 1) * zoomSpeed;
            zoom = deltaZoom + 1;
            _virtualCam.m_Lens.OrthographicSize /= zoom;
            _virtualCam.m_Lens.OrthographicSize =
                Mathf.Clamp(_virtualCam.m_Lens.OrthographicSize, zoomEdgeValues.x, zoomEdgeValues.y);
            Vector3 newConfinerSize = _confinerSize;
            newConfinerSize.y *= 10 / _virtualCam.m_Lens.OrthographicSize;
            confiner.size = newConfinerSize;
        }
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        Ray rayBefore = cameraToControl.ScreenPointToRay(touch.position - touch.deltaPosition);
        Ray rayNow = cameraToControl.ScreenPointToRay(touch.position);

        bool onPlaneBefore = _plane.Raycast(rayBefore, out float distanceBefore);
        bool onPlaneNow = _plane.Raycast(rayNow, out float distanceNow);

        if (onPlaneNow && onPlaneBefore)
        {
            Vector3 deltaPosition = rayBefore.GetPoint(distanceBefore) - rayNow.GetPoint(distanceNow);
            return deltaPosition * scrollSpeedFactor;
        }

        return Vector3.zero;
    }

    private bool CheckPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (RaycastResult raycastResult in raycastResults)
        {
            if (1 << raycastResult.gameObject.layer == _UIMask)
            {
                return true;
            }
        }
        return false;
    }
#endif
}
