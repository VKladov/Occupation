using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PersonStatsVisualization : MonoBehaviour
{
	[SerializeField] private ClickPicker _picker;
	[SerializeField] private TMP_Text _nameLabel;
	[SerializeField] private StatsListView _statsList;
	[SerializeField] private StorageView _storageView;

	private Person _targetPerson;

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	private void PickerOnPersonDeselected()
	{
		_targetPerson = null;
		_nameLabel.enabled = false;
		_statsList.SetList(new List<Stat>());
		_storageView.enabled = false;
	}

	private void PickerOnGroundClicked(Vector3 position)
	{
		if (_targetPerson == null)
		{
			return;
		}

		if (NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas))
		{
			_targetPerson.GetComponent<NavMeshAgent>().destination = hit.position;
		}
	}

	private void PickerOnPersonSelected(Person person)
	{
		_targetPerson = person;
		_nameLabel.enabled = true;
		_nameLabel.text = person.Name;
		_statsList.gameObject.SetActive(true);
		_statsList.SetList(new List<Stat>()
		{
			person.Stats.BellyFull
		});
		_storageView.enabled = true;
		_storageView.SetStorage(person.Bag);
	}

	private void PickerOnBuildingSelected(Building building)
	{
		if (_targetPerson != null)
		{
			_targetPerson.GetComponent<NavMeshAgent>().destination = building.transform.position;
		}
		
		_nameLabel.enabled = true;
		_nameLabel.text = building.gameObject.name;
		_statsList.gameObject.SetActive(false);
		_storageView.enabled = true;
		_storageView.SetStorage(building.Storage);
	}
}