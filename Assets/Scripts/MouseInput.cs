using R3;
using UnityEngine.InputSystem;

public class MouseInput
{
    private Subject<bool> _onLeftClickSub = new();
    public Observable<bool> OnLeftClickObservable => _onLeftClickSub;

    public MouseInput(InputAction leftClick)
    {
        leftClick.performed += OnLeftClick;
        leftClick.canceled += ReleaseLeftClick;
        leftClick.Enable();
    }

    void OnLeftClick(InputAction.CallbackContext context)
    {
        _onLeftClickSub.OnNext(true);
    }

    void ReleaseLeftClick(InputAction.CallbackContext context)
    {
        _onLeftClickSub.OnNext(false);
    }
}