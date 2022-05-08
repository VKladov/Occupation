using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class PersonSelection : ITickable, IInitializable, IDisposable
{
	public event Action<List<Person>> Changed;
	
	public List<Person> Selected { get; private set; } = new List<Person>();
	
	[Inject] private SpawnPerson _spawnPerson;
	[Inject] private RectSelectionView _rectSelection;
	[Inject] private ClickPicker _clickPicker;
	[Inject] private Camera _camera;
	
	private Vector2 _rectSelectionStartPoint;
	
	public void Initialize()
	{
		_clickPicker.ClickedLeft += ClickPickerOnClickedLeft;
		_clickPicker.ClickedRight += ClickPickerOnClickedRight;
		_clickPicker.LeftDragStarted += ClickPickerOnLeftDragStarted;
		_clickPicker.LeftDragChanged += ClickPickerOnLeftDragChanged;
		_clickPicker.LeftDragEnded += ClickPickerOnLeftDragEnded;
		_spawnPerson.PersonDied += SpawnPersonOnPersonDied;
	}

	public void Dispose()
	{
		_clickPicker.ClickedLeft -= ClickPickerOnClickedLeft;
		_clickPicker.ClickedRight -= ClickPickerOnClickedRight;
		_clickPicker.LeftDragStarted -= ClickPickerOnLeftDragStarted;
		_clickPicker.LeftDragChanged -= ClickPickerOnLeftDragChanged;
		_clickPicker.LeftDragEnded -= ClickPickerOnLeftDragEnded;
		_spawnPerson.PersonDied -= SpawnPersonOnPersonDied;
	}

	public void Tick()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			foreach (var person in Selected)
			{
				person.SetMovementState(PersonMovementState.Prone);
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			foreach (var person in Selected)
			{
				person.SetMovementState(PersonMovementState.Crouch);
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			foreach (var person in Selected)
			{
				person.SetMovementState(PersonMovementState.Stand);
			}
		}
	}

	private void SpawnPersonOnPersonDied(Person person)
	{
		if (Selected.Contains(person))
		{
			Selected.Remove(person);
			Changed?.Invoke(Selected);
		}
	}

	private void ClickPickerOnClickedRight(ClickPicker.ClickResult obj)
	{
		Selected.Clear();
		Changed?.Invoke(Selected);
	}

	private void ClickPickerOnLeftDragEnded(Vector2 screenPoint)
	{
		_rectSelection.enabled = false;
	}

	private void ClickPickerOnLeftDragChanged(Vector2 screenPoint)
	{
		Selected = GetPeopleInRect(_rectSelectionStartPoint, screenPoint).ToList();
		_rectSelection.enabled = true;
		_rectSelection.UpdateRect(_rectSelectionStartPoint, screenPoint);
		Changed?.Invoke(Selected);
	}

	private void ClickPickerOnLeftDragStarted(Vector2 screenPoint)
	{
		_rectSelectionStartPoint = screenPoint;
		Selected.Clear();
		Changed?.Invoke(Selected);
	}

	private void ClickPickerOnClickedLeft(ClickPicker.ClickResult obj)
	{
		if (obj.Person == null)
		{
			return;
		}

		Selected.Clear();
		Selected.Add(obj.Person);
		Changed?.Invoke(Selected);
	}

	private IEnumerable<Person> GetPeopleInRect(Vector2 rectStart, Vector3 rectEnd)
	{
		var x = Mathf.Min(rectStart.x, rectEnd.x);
		var y = Mathf.Min(rectStart.y, rectEnd.y);
		var width = Mathf.Abs(rectStart.x - rectEnd.x);
		var height = Mathf.Abs(rectStart.y - rectEnd.y);
		var rect = new Rect(x, y, width, height);
		return _spawnPerson.People.Where(person => person.IsAlive && rect.Contains(_camera.WorldToScreenPoint(person.transform.position)));
	}
}