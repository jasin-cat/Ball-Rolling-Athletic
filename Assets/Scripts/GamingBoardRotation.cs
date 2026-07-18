using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GamingBoardRotation : MonoBehaviour
{
    [SerializeField] private InputAction _inputArrow;
    [SerializeField] private float _speed = 10f;
    private Vector2 _tiltValue;
    public Vector2 TiltValue => _tiltValue;
    private Vector2 _oldTiltValue;
    private float x;
    private float z;

    void Awake()
    {
        _inputArrow.performed += OnTilt;
        _inputArrow.canceled += ReverseTilt;
        _inputArrow.Enable();
    }

    void OnTilt(InputAction.CallbackContext context)
    {
        _tiltValue = context.ReadValue<Vector2>();
    }

    void ReverseTilt(InputAction.CallbackContext context)
    {
        _tiltValue = Vector2.zero;
    }

    void Update()
    {
        x -= _tiltValue.y * Time.deltaTime * _speed;
        z += _tiltValue.x * Time.deltaTime * _speed;

        this.transform.rotation = 
            Quaternion.AngleAxis(x, Vector3.right)
            * Quaternion.AngleAxis(z, Vector3.forward);

        x = Mathf.Clamp(x, -20, 20);
        z = Mathf.Clamp(z, -20, 20);
    }


    void OnDisable()
    {
        _inputArrow.performed -= OnTilt;
        _inputArrow.Disable();
    }
}