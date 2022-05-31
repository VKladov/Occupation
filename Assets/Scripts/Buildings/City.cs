using System.Collections.Generic;
using System.Linq;
using Buildings;
using UnityEngine;
using Zenject;

public class City
{
	[Inject] private List<House> _houses;
	[Inject] private List<Supermarket> _supermarkets;

	public Supermarket GetNearestSupermarket(Vector3 point) => _supermarkets.GetNearest(point);
}