using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [Inject] private ClickPicker _clickPicker;
    
    private CinemachineFreeLook _cinemachine;
    private Vector3 _lastMousePosition;
    private Transform _targetPoint;
    private Vector3 _dragStartPosition;
    private Vector3 _targetPosition;
    private Vector3 _velocity;

    private void Awake()
    {
        _cinemachine = GetComponent<CinemachineFreeLook>();
        _targetPoint = _cinemachine.LookAt;
        _targetPosition = _targetPoint.transform.position;
    }

    private void OnEnable()
    {
        _clickPicker.RightDragStarted += ClickPickerOnRightDragStarted;
        _clickPicker.RightDragChanged += ClickPickerOnRightDragChanged;
        _clickPicker.RightDragEnded += ClickPickerOnRightDragEnded;
    }

    private void OnDisable()
    {
        _clickPicker.RightDragStarted -= ClickPickerOnRightDragStarted;
        _clickPicker.RightDragChanged -= ClickPickerOnRightDragChanged;
        _clickPicker.RightDragEnded -= ClickPickerOnRightDragEnded;
    }

    private void ClickPickerOnRightDragStarted(Vector2 obj)
    {
        _dragStartPosition = _clickPicker.CheckClick().GroundPoint;
    }

    private void ClickPickerOnRightDragChanged(Vector2 obj)
    {
        var delta = _dragStartPosition - _clickPicker.CheckClick().GroundPoint;
        _targetPosition += delta;
    }

    private void ClickPickerOnRightDragEnded(Vector2 obj)
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            var delta = _lastMousePosition - Input.mousePosition;
            _lastMousePosition = Input.mousePosition;
            _cinemachine.m_XAxis.m_InputAxisValue = delta.x / Screen.width;
            _cinemachine.m_YAxis.m_InputAxisValue = delta.y / Screen.height;
        }
        else
        {
            _cinemachine.m_XAxis.m_InputAxisValue = 0;
            _cinemachine.m_YAxis.m_InputAxisValue = 0;
        }

        _targetPoint.transform.position = _targetPosition;
    }
}
