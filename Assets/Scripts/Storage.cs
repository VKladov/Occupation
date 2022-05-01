using System;
using System.Collections.Generic;
using System.Linq;

public class Storage
{
	public event Action Changed;
		
	public Dictionary<Items, int> Items = new Dictionary<Items, int>
	{
		{global::Items.Food, 0}
	};

	public int GetCount(Items item)
	{
		return Items.Keys.Contains(item) ? Items[item] : 0;
	}

	public void Add(Items item, int count)
	{
		Items[item] += count;
		Changed?.Invoke();
	}

	public bool TryRemove(Items item, int count)
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
}