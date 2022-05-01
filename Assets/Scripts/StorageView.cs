using System;
using DefaultNamespace;
using UnityEngine;

public class StorageView : MonoBehaviour
{
	[SerializeField] private StorageItemView _rowPrefab;

	private Storage _storage;

	public void SetStorage(Storage storage)
	{
		_storage = storage;
		_storage.Changed += Refresh;
		Refresh();
	}

	public void OnDestroy()
	{
		_storage.Changed -= Refresh;
	}

	private void Refresh()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (var storageItem in _storage.Items)
		{
			if (storageItem.Value == 0)
			{
				continue;
			}
			
			var row = Instantiate(_rowPrefab, transform);
			row.ShowData(storageItem.Key.ToString(), storageItem.Value);
		}
	}
}