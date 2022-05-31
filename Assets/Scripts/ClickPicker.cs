using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using Zenject;

public class ClickPicker : ITickable
{
	public class ClickResult
	{
		public Person Person;
		public Vector3 GroundPoint;
		public Building Building;
	}
		
	public event Action<ClickResult> ClickedLeft;
	public event Action<ClickResult> ClickedRight;
	public event Action<Vector2> LeftDragStarted;
	public event Action<Vector2> LeftDragChanged;
	public event Action<Vector2> LeftDragEnded;
	public event Action<Vector2> RightDragStarted;
	public event Action<Vector2> RightDragChanged;
	public event Action<Vector2> RightDragEnded;

	[Inject] 
	private Constants _constants;

	[Inject]
	private Camera _camera;
	
	private Vector2 _clickStartPosition;
	private bool _inLeftDrag;
	private bool _inRightDrag;

	public ClickResult CheckClick()
	{
		var ray = _camera.ScreenPointToRay(Input.mousePosition);
		Person person = null;
		if (Physics.Raycast(ray, out var hitInfo, 1000f, _constants.LayerMasks.PersonLayer))
		{
			hitInfo.collider.TryGetComponent(out person);
		}

		Building building = null;
		if (Physics.Raycast(ray, out var buildingHit, 1000f, _constants.LayerMasks.BuildingLayer))
		{
			Debug.Log("buildingHit");
			buildingHit.collider.TryGetComponent(out building);
		}

		var groundPoint = Vector3.zero;
		if (Physics.Raycast(ray, out var groundHit, 1000f, _constants.LayerMasks.GroundLayer))
		{
			groundPoint = groundHit.point;
		}
				
		return new ClickResult
		{
			Building = building,
			Person = person,
			GroundPoint = groundPoint
		};
	}

	public void Tick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_clickStartPosition = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0))
		{
			if (_inLeftDrag)
			{
				LeftDragChanged?.Invoke(Input.mousePosition);
			}
			else if (Vector2.Distance(Input.mousePosition, _clickStartPosition) > 20)
			{
				_inLeftDrag = true;
				LeftDragStarted?.Invoke(_clickStartPosition);
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (_inLeftDrag)
			{
				LeftDragEnded?.Invoke(Input.mousePosition);
			}
			else
			{
				ClickedLeft?.Invoke(CheckClick());
			}
			_inLeftDrag = false;
		}
		
		
		if (Input.GetMouseButtonDown(1))
		{
			_clickStartPosition = Input.mousePosition;
		}
		else if (Input.GetMouseButton(1))
		{
			if (_inRightDrag)
			{
				RightDragChanged?.Invoke(Input.mousePosition);
			}
			else if (Vector2.Distance(Input.mousePosition, _clickStartPosition) > 20)
			{
				_inRightDrag = true;
				RightDragStarted?.Invoke(_clickStartPosition);
			}
		}
		else if (Input.GetMouseButtonUp(1))
		{
			if (_inRightDrag)
			{
				RightDragEnded?.Invoke(Input.mousePosition);
			}
			else
			{
				ClickedRight?.Invoke(CheckClick());
			}
			_inRightDrag = false;
		}
	}
}