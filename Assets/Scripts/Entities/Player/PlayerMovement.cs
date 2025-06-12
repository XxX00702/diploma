using UnityEngine;

public class PlayerMovement
{
    private PlayerInputs _playerInput { get; set; }
    private Rigidbody2D _playerRigidbody2D { get; set; }
    private Camera _playerCamera { get; set; }
    private Transform _playerTransform;
    private Transform _cameraTransform;
    private GameObject _playerHand;

    public PlayerMovement(Rigidbody2D playerRigidbody2D, Camera playerCamera, GameObject playerHand)
    {
        _playerInput = Player._playerInputsAction;
        _playerRigidbody2D = playerRigidbody2D;
        _playerCamera = playerCamera;
        _playerTransform = _playerRigidbody2D.transform;
        _cameraTransform = _playerCamera.transform;
        _playerHand = playerHand;
    }
    public void ProcessPlayerMovement(float speed)
    {
        if (Player._isPaused) return;

        Vector2 direction = _playerInput.Player.Move.ReadValue<Vector2>();
        _playerRigidbody2D.velocity = direction * speed;
    }
    public void ProcessHandMovement(float smoothness, float threshold)
    {
        if (Player._isPaused) return;

        Vector3 referencePosition = _playerCamera.ScreenToWorldPoint(Input.mousePosition);
        referencePosition = Vector3.Lerp(_playerHand.transform.position, 
            referencePosition, smoothness * Time.deltaTime);
        
        Vector3 direction =  referencePosition - _playerTransform.position;
        if (direction.magnitude > threshold)
        {
            direction = direction.normalized * threshold;
            direction += _playerTransform.position;
        }
        _playerHand.transform.position = direction;
    }
    
    public void ProcessWeaponRotation(Weapon weapon)
    {
        if (Player._isPaused) return;

        if (weapon == null) return;
        var mouseScreenPos = Input.mousePosition;
        var startingScreenPos = _playerCamera.WorldToScreenPoint(_playerTransform.position);
        mouseScreenPos.x -= startingScreenPos.x;
        mouseScreenPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    public void ProcessCameraMovement(float cameraThreshold)
    {
        if (Player._isPaused) return;

        Vector3 mousePosition = _playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - _playerTransform.position;
        if (direction.magnitude > cameraThreshold)
        {
            direction = direction.normalized * cameraThreshold;
        }
        
        _cameraTransform.position = new Vector3(_playerTransform.position.x + direction.x, 
            _playerTransform.position.y + direction.y, _cameraTransform.position.z);
    }
}
