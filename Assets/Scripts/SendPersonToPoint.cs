using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class SendPersonToPoint : IDisposable
{
	private struct PesonNavigationDistance
	{
		public Person Person;
		public float PathLength;
	}
	
	private readonly ClickPicker _clickPicker;
	private readonly PersonSelection _selection;
	private const float Distance = 3f;
	private List<Vector3> _targets = new List<Vector3>();

	[Inject]
	public SendPersonToPoint(ClickPicker clickPicker, PersonSelection selection)
	{
		_clickPicker = clickPicker;
		_selection = selection;
		_clickPicker.ClickedLeft += ClickPickerOnClickedLeft;
	}

	public void Dispose()
	{
		_clickPicker.ClickedLeft -= ClickPickerOnClickedLeft;
	}

	private void ClickPickerOnClickedLeft(ClickPicker.ClickResult click)
	{
		if (click.Person != null)
		{
			return;
		}

		if (!_selection.Selected.Any())
		{
			return;
		}

		var positions = GetPositionsForGroup(_selection.Selected.Count(), click.GroundPoint);
		var personWithoutPosition = _selection.Selected.ToArray().ToList();
		foreach (var position in positions)
		{
			var closest = personWithoutPosition.First();
			var minDistance = float.MaxValue;

			foreach (var person in personWithoutPosition)
			{
				var distance = Vector3.Distance(position, person.transform.position);
				if (distance < minDistance)
				{
					minDistance = distance;
					closest = person;
				}
			}

			Debug.DrawLine(position, position + Vector3.up * 2, Color.green, 2f);
			personWithoutPosition.Remove(closest);
			closest.SetMainObjective(position);
			// closest.Movement.MoveTo(position);
		}
	}

	private List<Vector3> GetPositionsForGroup(int number, Vector3 center)
	{
		var result = new List<Vector3>() { center };
		var lastPoints = new List<Vector3>() { center };
		while (result.Count() < number)
		{
			var newPoints = new List<Vector3>();
			foreach (var lastPoint in lastPoints)
			{
				var possiblePoints = GetPositionNeighbors(lastPoint)
					.Where(item => !result.Contains(item) && NavMesh.SamplePosition(item, out var hit, 1f, NavMesh.AllAreas));

				foreach (var possiblePoint in possiblePoints)
				{
					if (result.Count + newPoints.Count() < number)
					{
						newPoints.Add(possiblePoint);
					}
				}
			}

			result.AddRange(newPoints);
			lastPoints = newPoints;
		}

		return result;
	}

	private List<Vector3> GetPositionNeighbors(Vector3 position)
	{
		return new List<Vector3>
		{
			new Vector3(position.x + Distance, position.y, position.z),
			new Vector3(position.x, position.y, position.z + Distance),
			new Vector3(position.x - Distance, position.y, position.z),
			new Vector3(position.x, position.y, position.z - Distance),
			new Vector3(position.x + Distance, position.y, position.z + Distance),
			new Vector3(position.x + Distance, position.y, position.z - Distance),
			new Vector3(position.x - Distance, position.y, position.z - Distance),
			new Vector3(position.x - Distance, position.y, position.z + Distance),
		};
	}
	
	public static float GetPathLength(NavMeshPath path)
	{
		var lng = 0.0f;
		if ((path.status == NavMeshPathStatus.PathInvalid) || (path.corners.Length <= 1))
		{
			return lng;
		}

		for (var i = 1; i < path.corners.Length; ++i )
		{
			lng += Vector3.Distance( path.corners[i-1], path.corners[i] );
		}

		return lng;
	}
}