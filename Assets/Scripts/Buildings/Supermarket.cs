using UnityEngine;

namespace Buildings
{
	public class Supermarket : House
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Person person))
			{
				person.Bag.Add(Items.Food, 5);
			}
		}
	}
}