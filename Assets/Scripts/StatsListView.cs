using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsListView : MonoBehaviour
{
	[SerializeField] private StatSliderView _rowPrefab;

	public void SetList(List<Stat> stats)
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (var stat in stats)
		{
			var row = Instantiate(_rowPrefab, transform);
			row.SetSource(stat);
		}
	}
}
