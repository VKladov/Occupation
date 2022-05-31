using UnityEngine;

public class Constants
{
	public class LayerMasksData
	{
		public LayerMask PersonLayer;
		public LayerMask GroundLayer;
		public LayerMask BuildingLayer;
		public LayerMask ObstaclesLayer;

		public LayerMasksData(LayerMask personLayer, LayerMask groundLayer, LayerMask buildingLayer, LayerMask obstaclesLayer)
		{
			PersonLayer = personLayer;
			GroundLayer = groundLayer;
			BuildingLayer = buildingLayer;
			ObstaclesLayer = obstaclesLayer;
		}
	}

	public class NavMeshAreaMasksData
	{
		public readonly int Footpath;
		public readonly int Road;
		public readonly int Everywhere;

		public NavMeshAreaMasksData(int footpath, int road, int everywhere)
		{
			Footpath = footpath;
			Road = road;
			Everywhere = everywhere;
		}
	}

	public readonly LayerMasksData LayerMasks;
	public readonly NavMeshAreaMasksData NavMeshAreaMasks;
	
	public Constants(LayerMasksData layerMasks, NavMeshAreaMasksData navMeshAreaMasks)
	{
		LayerMasks = layerMasks;
		NavMeshAreaMasks = navMeshAreaMasks;
	}
}