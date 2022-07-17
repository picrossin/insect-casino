using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private bool debugForceUnitType;
    [SerializeField] private UnitType debugUnitType;
    
    [SerializeField] private GameObject[] units; 
    [SerializeField] private float produceTime = 10f;
    [SerializeField] private DieUIManager dieUIManager;
    [SerializeField] private GameObject[] hands;
    
    [SerializeField] private Grid grid;
    public Grid TileGrid => grid;

    [SerializeField] private GameGrid gameGrid;
    public GameGrid GameGrid => gameGrid;

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

    private Unit _unitToUpgrade;
    public Unit UnitToUpgrade
    {
        get => _unitToUpgrade;
        set => _unitToUpgrade = value;
    }
    
    private float _productionPercentage;
    public float ProductionPercentage => _productionPercentage;

    private bool _busy;
    public bool Busy => _busy;
    
    private HashSet<Hand> _handsInPlay = new HashSet<Hand>();
    public HashSet<Hand> HandsInPlay => _handsInPlay;
    
    private HashSet<float> _availableHandAngles = new HashSet<float> {0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f};
    public HashSet<float> AvailableHandAngles => _availableHandAngles;
    
    private float _timeWaited = 0f;
    public float TimeWaited
    {
        get => _timeWaited;
        set => _timeWaited = value;
    }

    private float _handWaitTime = 5f;
    private List<Chips> _chipTargets = new List<Chips>();
    private HashSet<Chips> _chipPiles = new HashSet<Chips>();

    private void Start()
    {
        Instance = this;
        
        _state = GameState.Normal;

        StartCoroutine(ProduceDice());
        StartCoroutine(SpawnHands());
    }

    public void AddChipPile(Chips chipPile)
    {
        _chipPiles.Add(chipPile);
        _chipTargets.Add(chipPile);
    }

    public void RemoveChipPile(Chips chipPile)
    {
        _chipPiles.Remove(chipPile);

        if (_chipPiles.Count == 0)
        {
            // TODO: Lose game
            Debug.Log("YOU LOSE!!!!");
        }
    }

    public void ReturnChipPile(Chips chipPile)
    {
        _chipTargets.Add(chipPile);
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
        
        dieToRoll.Throw(true);
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        Unit newUnit = Instantiate(units[debugForceUnitType ? (int) debugUnitType - 1 : dieToRoll.GetDieSide() - 1], 
            TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector3(0.5f, 1.5f, 0.0f), 
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
        _state = GameState.Upgrading;
        foreach (Unit unit in gameGrid.GetAllUnits())
        {
            unit.ShowStrength();
        }
        
        yield return new WaitUntil(() => UnitToUpgrade != null);
        
        dieToRoll.Throw();
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        int extraStrength = dieToRoll.GetDieSide();
        _unitToUpgrade.AddStrength(extraStrength);
        
        dieToRoll.Spin();
        dieToRoll.SetSpawned(false);
        dieToRoll.transform.parent.gameObject.SetActive(false);
        
        _unitToUpgrade = null;
        _state = GameState.Normal;
    }

    private IEnumerator ProduceDice()
    {
        _productionPercentage = 0f;
        
        while (true)
        {
            if (!_dieReady && !dieUIManager.AllSpawned())
            {
                _timeWaited = 0f;
                while (_timeWaited < produceTime)
                {
                    yield return new WaitUntil(() => !_busy);
                    yield return new WaitForEndOfFrame();
                    _timeWaited += Time.deltaTime;
                    _productionPercentage = _timeWaited / produceTime;
                }

                _dieReady = true;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator SpawnHands()
    {
        while (true)
        {
            yield return new WaitForSeconds(_handWaitTime);

            Chips goal = GetChipPile();
            if (goal != null)
            {
                Instantiate(hands[Random.Range(0, hands.Length)], goal.transform.position, Quaternion.identity).GetComponent<Hand>().ChipGoal = goal;
            }
        }
    }
    
    private Chips GetChipPile()
    {
        Chips target = null;

        if (_chipTargets.Count > 0)
        {
            int targetIdx = Random.Range(0, _chipTargets.Count);
            target = _chipTargets[targetIdx];
            _chipTargets.RemoveAt(targetIdx);
        }
        
        return target;
    }
}
