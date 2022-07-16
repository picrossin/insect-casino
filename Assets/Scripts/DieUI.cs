using UnityEngine;

public class DieUI : MonoBehaviour
{
	public void SetMenu(bool open)
	{
		transform.Find("Button Panel").gameObject.SetActive(open);
	}
}
