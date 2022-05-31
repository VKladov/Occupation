using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
	public static T Random<T>(this T[] array)
	{
		return array[UnityEngine.Random.Range(0, array.Length)];
	}
	
	public static T Random<T>(this List<T> list)
	{
		return list[UnityEngine.Random.Range(0, list.Count)];
	}
	
	public static T GetNearest<T>(this List<T> list, Vector3 point) where T : MonoBehaviour
	{
		return list.OrderBy(item => Vector3.Distance(point, item.transform.position)).FirstOrDefault();
	}
}