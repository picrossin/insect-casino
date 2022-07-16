using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Normal,
        Placing,
        Upgrading,
        Rolling
    }

    public static GameManager Instance;

    [SerializeField] private float produceTime = 10f;

    private GameState _state;
    public GameState State
    {
        get => _state;
        set => _state = value;
    }

    private bool _dieReady;
    public bool DieReady
    {
        get => _dieReady;
        set => _dieReady = value;
    }

    private void Start()
    {
        Instance = this;
        _state = GameState.Normal;
        StartCoroutine(ProduceDice());
    }

    private IEnumerator ProduceDice()
    {
        while (true)
        {
            if (!_dieReady)
            {
                float timeWaited = 0f;
                while (timeWaited < produceTime)
                {
                    yield return new WaitForEndOfFrame();
                    timeWaited += Time.deltaTime;
                }

                _dieReady = true;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
