using UnityEngine;

public class DieUI : MonoBehaviour
{
	public void SetMenu(bool open)
	{
		transform.Find("Button Panel").gameObject.SetActive(open);
		bool glyph = GetComponentInChildren<DieRoller>().Glyph;
		transform.Find("Button Panel/Make Unit Button").gameObject.SetActive(glyph);
		transform.Find("Button Panel/Upgrade Unit Button").gameObject.SetActive(!glyph);
	}
}
