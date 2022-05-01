using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class RectSelectionView : MonoBehaviour
{
	[Inject] private Canvas _canvas;
	private RectTransform _rectTransform;
	private Image _image;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		_image.enabled = true;
	}

	private void OnDisable()
	{
		_image.enabled = false;
	}

	public void UpdateRect(Vector2 startScreenPoint, Vector2 endScreenPoint)
	{
		var startPoint = startScreenPoint * (1f / _canvas.scaleFactor);
		var endPoint = endScreenPoint * (1f / _canvas.scaleFactor);
		var size = endPoint - startPoint;

		var pivod = Vector3.zero;
		if (size.y < 0)
		{
			pivod.y = 1;
			size.y = -size.y;
		}

		if (size.x < 0)
		{
			pivod.x = 1;
			size.x = -size.x;
		}

		_rectTransform.anchoredPosition = startPoint;
		_rectTransform.pivot = pivod;
		_rectTransform.sizeDelta = size;
	}
}