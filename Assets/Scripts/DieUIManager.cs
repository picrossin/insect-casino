using UnityEngine;
using UnityEngine.UI;

public class DieUIManager : MonoBehaviour
{
    [SerializeField] private Image die1Loading;
    [SerializeField] private Image die2Loading;
    [SerializeField] private Image die3Loading;
    [SerializeField] private GameObject sfx;
    
    private bool _die1Spawned;
    private bool _die2Spawned;
    private bool _die3Spawned;

    private void FixedUpdate()
    {
        if (GameManager.Instance.DieReady)
        {
            die1Loading.fillAmount = 0f;
            die2Loading.fillAmount = 0f;
            die3Loading.fillAmount = 0f;
            
            if (!_die1Spawned)
            {
                _die1Spawned = true;
                transform.Find("Die 1").gameObject.SetActive(true);
                Instantiate(sfx);
                GameManager.Instance.DieReady = false;
            }
            else if (!_die2Spawned)
            {
                _die2Spawned = true;
                transform.Find("Die 2").gameObject.SetActive(true);
                Instantiate(sfx);
                GameManager.Instance.DieReady = false;
            }
            else if (!_die3Spawned)
            {
                _die3Spawned = true;
                transform.Find("Die 3").gameObject.SetActive(true);
                Instantiate(sfx);
                GameManager.Instance.DieReady = false;
            }
        }
        else if (GameManager.Instance.State == GameManager.GameState.Normal && !GameManager.Instance.Busy)
        {
            if (!_die1Spawned)
            {
                die1Loading.fillAmount = GameManager.Instance.ProductionPercentage;
            }
            else if (!_die2Spawned)
            {
                die2Loading.fillAmount = GameManager.Instance.ProductionPercentage;
            }
            else if (!_die3Spawned)
            {
                die3Loading.fillAmount = GameManager.Instance.ProductionPercentage;
            }
        }
        else if (GameManager.Instance.Busy)
        {
            die1Loading.fillAmount = 0f;
            die2Loading.fillAmount = 0f;
            die3Loading.fillAmount = 0f;
        }
    }

    public void OpenDieMenu(int die)
    {
        if (GameManager.Instance.State != GameManager.GameState.Normal || GameManager.Instance.Busy) return;

        DieRoller roller = transform.Find($"Die {die}/Die Model {die}").GetComponent<DieRoller>();
        
        if (roller.Glyph)
        {
            GameManager.Instance.MakeUnit(roller);
        }
        else
        {
            GameManager.Instance.UpgradeUnit(roller);
        }

        // transform.Find("Die 1").GetComponent<DieUI>().SetMenu(false);
        // transform.Find("Die 2").GetComponent<DieUI>().SetMenu(false);
        // transform.Find("Die 3").GetComponent<DieUI>().SetMenu(false);
        //
        // transform.Find($"Die {die}").GetComponent<DieUI>().SetMenu(true);
    }

    public void SetSpawned(int die, bool spawned)
    {
        switch (die)
        {
            case 1:
                _die1Spawned = spawned;
                break;
            case 2:
                _die2Spawned = spawned;
                break;
            case 3:
                _die3Spawned = spawned;
                break;
        }
    }

    public bool AllSpawned()
    {
        return _die1Spawned && _die2Spawned && _die3Spawned;
    }
}
