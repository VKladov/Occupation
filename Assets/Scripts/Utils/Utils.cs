using System;
using System.Collections.Generic;

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
}