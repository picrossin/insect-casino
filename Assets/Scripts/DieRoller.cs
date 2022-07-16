using UnityEngine;
using Random = UnityEngine.Random;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1f;
    [SerializeField] private float dieLaunchMultiplier = 100f;
    [SerializeField] private LayerMask floorMask;

    private Rigidbody _rigidbody;
    private bool _spinning;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

    private void Spin()
    {
        _spinning = true;
        transform.rotation = Quaternion.identity;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    public void GetDieSide()
    {
        
    }
}
