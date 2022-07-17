using UnityEngine;
using Random = UnityEngine.Random;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private float dieLaunchMultiplier = 100f;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private int dieNum;
    [SerializeField] private Texture glyphTex;
    [SerializeField] private GameObject rollSFX;

    private bool _glyph;
    public bool Glyph => _glyph;

    private Rigidbody _rigidbody;
    private bool _spinning;
    private Vector3 _originalPosition;
    private Texture _numTex;
    private Renderer _renderer;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _originalPosition = transform.position;
        _numTex = _renderer.material.mainTexture;
        Spin();
    }

    private void FixedUpdate()
    {
        if (_spinning)
        {
            transform.rotation *= Quaternion.AngleAxis(spinSpeed, new Vector3(0f, 0.5f, 0.5f));
        }
    }
    
    public void Throw()
    {
        _spinning = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        GetComponent<Collider>().enabled = true;

        Instantiate(rollSFX, transform);

        Vector3 randDir = Random.insideUnitCircle.normalized;
        randDir = new Vector3(-Mathf.Abs(randDir.x), randDir.y);
        
        _rigidbody.AddForceAtPosition(randDir * Random.Range(dieLaunchMultiplier, dieLaunchMultiplier),
            transform.position + Vector3.back * 0.75f, 
            ForceMode.Impulse);
    }

    public void Spin()
    {
        _spinning = true;
        _glyph = Random.value > 0.45f;

        if (GameManager.Instance.GameGrid.GetAllUnits(true).Count == 0)
        {
            _glyph = true;
        }

        // Edge case
        int dots = 0;
        foreach (GameObject dieRoller in GameObject.FindGameObjectsWithTag("DieRoller"))
        {
            if (!dieRoller.GetComponent<DieRoller>()._glyph)
            {
                dots++;
            }
        }

        if (dots == 2)
        {
            _glyph = true;
        }
        
        transform.position = _originalPosition;
        transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), new Vector3(Random.value, Random.value, Random.value));
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        _renderer.material.mainTexture = _glyph ? glyphTex : _numTex;
    }

    public void SetSpawned(bool spawned)
    {
        transform.parent.parent.GetComponent<DieUIManager>().SetSpawned(dieNum, spawned);
    }

    public int GetDieSide()
    {
        if (Physics.Raycast(transform.position, transform.up, transform.localScale.x * 1.1f, floorMask))
        {
            return 6;
        }
        
        if (Physics.Raycast(transform.position, -transform.up, transform.localScale.x * 1.1f, floorMask))
        {
            return 1;
        }
        
        if (Physics.Raycast(transform.position, -transform.right, transform.localScale.x * 1.1f, floorMask))
        {
            return 5;
        }
        
        if (Physics.Raycast(transform.position, transform.right, transform.localScale.x * 1.1f, floorMask))
        {
            return 2;
        }
        
        if (Physics.Raycast(transform.position, transform.forward, transform.localScale.x * 1.1f, floorMask))
        {
            return 3;
        }
        
        if (Physics.Raycast(transform.position, -transform.forward, transform.localScale.x * 1.1f, floorMask))
        {
            return 4;
        }

        return 1;
    }
}
