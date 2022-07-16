using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Normal,
        Placing,
        Upgrading
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

    public void MakeUnit(DieRoller dieToRoll)
    {
        StartCoroutine(MakeUnitAnim(dieToRoll));
    }
    public void UpgradeUnit(DieRoller dieToRoll)
    {
        StartCoroutine(UpgradeUnitAnim(dieToRoll));
    }
    
    private IEnumerator MakeUnitAnim(DieRoller dieToRoll)
    {
        // Immediately roll glyph die
        dieToRoll.Throw();
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        print(dieToRoll.GetDieSide());
        // Switch to place mode
        _state = GameState.Placing;
        // Wait until player places unit
        // Return to normal
        _state = GameState.Normal;
        yield break;
    }

    private IEnumerator UpgradeUnitAnim(DieRoller dieToRoll)
    {
        // Switch to upgrade mode
        _state = GameState.Upgrading;
        // Wait until player selects unit
        // Roll standard die
        dieToRoll.Throw();
        // Play upgrade animation
        // Return to normal
        _state = GameState.Normal;
        yield break;
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
