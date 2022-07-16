using UnityEngine;
using Random = UnityEngine.Random;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private float dieLaunchMultiplier = 100f;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private int dieNum;

    private Rigidbody _rigidbody;
    private bool _spinning;
    private Vector3 _originalPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _originalPosition = transform.position;
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

        Vector3 randDir = Random.insideUnitCircle.normalized;
        randDir = new Vector3(-Mathf.Abs(randDir.x), Mathf.Abs(randDir.y));
        
        _rigidbody.AddForceAtPosition(randDir * Random.Range(dieLaunchMultiplier, dieLaunchMultiplier),
            transform.position + Vector3.back * 0.75f, 
            ForceMode.Impulse);
    }

    public void Spin()
    {
        _spinning = true;
        transform.position = _originalPosition;
        transform.rotation = Quaternion.identity;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
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
