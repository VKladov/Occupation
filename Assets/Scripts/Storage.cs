using System;
using System.Collections.Generic;
using System.Linq;

public class Storage
{
	public event Action Changed;

	public Dictionary<string, int> Items = new Dictionary<string, int>();

	public int GetCount(InventoryItem item) => GetCount(item.ToString());

	public void Add(InventoryItem item, int count) => Add(item.ToString(), count);

	public bool TryRemove(InventoryItem item, int count) => TryRemove(item.ToString(), count);

	private void Add(string item, int count)
	{
		Items[item] += count;
		Changed?.Invoke();
	}

	private bool TryRemove(string item, int count)
	{
		var has = GetCount(item);
		if (has < count)
		{
			return false;
		}

		Items[item] = has - count;
		Changed?.Invoke();
		return true;
	}

	private int GetCount(string item)
	{
		return Items.Keys.Contains(item) ? Items[item] : 0;
	}
}