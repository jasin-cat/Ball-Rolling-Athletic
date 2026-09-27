using R3;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationGameBoard : MonoBehaviour
{
    [SerializeField] private InputAction _leftClick;
    [SerializeField] private Transform _gameBoard;
    // 回転の遊び
    [SerializeField] private float _rotPlaying = 15f;
    // 最大回転角度
    [SerializeField] private float _maxRot = 30f;
    // 回転スピード
    [SerializeField] private float _rotSpeed = 5f;
    private MouseInput _mouseInput;
    private bool _onClick;
    private Vector2 _centerPos;

    void Start()
    {
        Click();
    }

    void Click()
    {
        if (_mouseInput is null)
        {
            _mouseInput = new(_leftClick);
        }

        _mouseInput.OnLeftClickObservable
            .Subscribe(x => _onClick = x)
            .AddTo(this);
    }

    void Update()
    {
        if (!_onClick) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _centerPos = Mouse.current.position.ReadValue();
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 delta = mousePos - _centerPos;

        if (delta.magnitude < _rotPlaying) return;

        float x = math.clamp(
            delta.y * _rotSpeed * Time.deltaTime,
            -_maxRot,
            _maxRot
        );

        float z = math.clamp(
            delta.x * _rotSpeed * Time.deltaTime,
            -_maxRot,
            _maxRot
        );

        _gameBoard.rotation = Quaternion.Euler(x, 0, -z);
    }
}