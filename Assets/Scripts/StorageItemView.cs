using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
	public class StorageItemView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _label;
		[SerializeField] private TMP_Text _amount;

		public void ShowData(string label, int amount)
		{
			_label.text = label;
			_amount.text = amount.ToString();
		}
	}
}