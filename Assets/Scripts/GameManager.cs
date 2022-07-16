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

    public enum UnitType
    {
        Cards = 1,
        ExplodingAnt = 2,
        ShooterWorm = 3,
        GoopSnail = 4,
        Honeybee = 5,
        Spider = 6
    }

    public static GameManager Instance;

    [SerializeField] private GameObject[] units; 
    [SerializeField] private float produceTime = 10f;
    [SerializeField] private DieUIManager dieUIManager;
    
    [SerializeField] private Grid grid;
    public Grid GameGrid => grid;

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

    private float _productionPercentage;
    public float ProductionPercentage => _productionPercentage;

    private bool _busy;
    public bool Busy => _busy;

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
        _busy = true;
        
        // Immediately roll glyph die
        dieToRoll.Throw();
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        Unit newUnit = Instantiate(units[dieToRoll.GetDieSide() - 1], 
            GameGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector3(0.5f, 1.5f, 0.0f), 
            Quaternion.identity).GetComponent<Unit>();
        newUnit.Placing = true;
        _state = GameState.Placing;
        yield return new WaitForSeconds(1f);
        dieToRoll.Spin();
        dieToRoll.SetSpawned(false);
        dieToRoll.transform.parent.gameObject.SetActive(false);
        yield return new WaitUntil(() => !newUnit.Placing);
        _state = GameState.Normal;
        
        _busy = false;
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
        _productionPercentage = 0f;
        
        while (true)
        {
            if (!_dieReady && !dieUIManager.AllSpawned())
            {
                float timeWaited = 0f;
                while (timeWaited < produceTime)
                {
                    yield return new WaitUntil(() => !_busy);
                    yield return new WaitForEndOfFrame();
                    timeWaited += Time.deltaTime;
                    _productionPercentage = timeWaited / produceTime;
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