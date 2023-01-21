using UnityEngine;

public class DumbDieSide : MonoBehaviour, IDieSide
{
    [SerializeField] private short _dieSide;
    [SerializeField] private bool _facingCamera = false;

    private BoxCollider _boxCollider;

    public short Side { get { return _dieSide; } }

    public void Activate()
    {
        print($"Side {Side} was activated!");
    }

    public bool FacingCamera()
    {
        return _facingCamera;
    }

    private void Awake()
    {
        if(_boxCollider is null)
            _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _facingCamera = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _facingCamera = false;
    }
}
