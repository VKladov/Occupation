using System;
using UnityEngine;

public class TempDataManager : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			TempData.TeamID.Value = 0;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			TempData.TeamID.Value = 1;
		}
	}
}