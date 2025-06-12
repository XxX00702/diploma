using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemsInteract
{
    private PlayerInputs _playerInput;
    private Weapon _currentInteractWeapon;

    public PlayerItemsInteract(Weapon currentWeapon)
    {
        _playerInput = Player._playerInputsAction;
        UpdateCurrentWeapon(currentWeapon, currentWeapon);
        _playerInput.Player.Attack.performed += UseWeapon;
    }

    private void UseWeapon(InputAction.CallbackContext ctx)
    {
        if (Player._isPaused) return;
        if (ctx.performed && _currentInteractWeapon != null)
        {
            _currentInteractWeapon.Attack();
        }
    }

    public void UpdateCurrentWeapon(Weapon oldWeapon, Weapon newWeapon)
    {
        _currentInteractWeapon = newWeapon;
    }

    public void Unsubscribe()
    {
        if (_playerInput != null)
        {
            _playerInput.Player.Attack.performed -= UseWeapon;
        }
    }
}