using UnityEngine;

public class DieUIManager : MonoBehaviour
{
    private bool die1Spawned;
    private bool die2Spawned;
    private bool die3Spawned;

    private void FixedUpdate()
    {
        if (GameManager.Instance.DieReady)
        {
            if (!die1Spawned)
            {
                die1Spawned = true;
                transform.Find("Die 1").gameObject.SetActive(true);
                GameManager.Instance.DieReady = false;
            }
            else if (!die2Spawned)
            {
                die2Spawned = true;
                transform.Find("Die 2").gameObject.SetActive(true);
                GameManager.Instance.DieReady = false;
            }
            else if (!die3Spawned)
            {
                die3Spawned = true;
                transform.Find("Die 3").gameObject.SetActive(true);
                GameManager.Instance.DieReady = false;
            }
        }
    }

    public void OpenDieMenu(int die)
    {
        transform.Find("Die 1").GetComponent<DieUI>().SetMenu(false);
        transform.Find("Die 2").GetComponent<DieUI>().SetMenu(false);
        transform.Find("Die 3").GetComponent<DieUI>().SetMenu(false);
        
        transform.Find($"Die {die}").GetComponent<DieUI>().SetMenu(true);
    }
}
