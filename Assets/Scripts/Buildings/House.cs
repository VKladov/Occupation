using UnityEngine;

namespace Buildings
{
	public class House : Building
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Person person))
			{
				StoreFood(person);
			}
		}

		private void StoreFood(Person person)
		{
			var count = person.Bag.GetCount(Items.Food);
			if (person.Bag.TryRemove(Items.Food, count))
			{
				Storage.Add(Items.Food, count);
			}
		}
	}
}