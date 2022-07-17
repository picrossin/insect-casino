using UnityEngine;
using UnityEngine.UI;

public class StrengthBar : MonoBehaviour
{
	[SerializeField] private Sprite[] frames;

	private Image _image;
	private bool _initialized;

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (_initialized) return;
		_image = GetComponent<Image>();
		_image.sprite = frames[0];
		_initialized = true;
	}

	public void SetStrength(int str)
	{
		Initialize();
		_image.sprite = frames[str];
	}
}
