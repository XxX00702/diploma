using UnityEngine;

public class LatencyMovement : MonoBehaviour, IMovable
{
private Transform _playerTransform;
private Rigidbody2D _rigidbody;

[SerializeField] private float _rotationStrength = 360f; 
[SerializeField] private float _maxMovementSpeed = 5f;
[SerializeField] private float _accelerationTime = 1f;
[SerializeField] private float _decelerationTime = 1f;  
[SerializeField] private float _repulsionForce = 2f;    // Adjust as needed

private float _currentSpeed = 0f;
private float _accelerationRate;
private float _decelerationRate;
private bool _isAccelerating = true;
private Vector2 _previousPosition;

private void Awake()
{
    _rigidbody = GetComponent<Rigidbody2D>();
    _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform; // Handle case where player might not exist
    _rigidbody.gravityScale = 0f;
    _rigidbody.isKinematic = false;

    _accelerationRate = _maxMovementSpeed / _accelerationTime;
    _decelerationRate = _maxMovementSpeed / _decelerationTime;
    _previousPosition = transform.position;
}

private void FixedUpdate()
{
    Move(_maxMovementSpeed);
    _previousPosition = transform.position;

    // Check for overlap with other enemies
    AvoidOverlap();
}

public void Move(float maxMovementSpeed)
{
    if (Time.timeScale == 0f || _playerTransform == null) return;

    Vector2 direction = (_playerTransform.position - transform.position).normalized;

    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, _rotationStrength * Time.deltaTime);
    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    if (_isAccelerating)
    {
        _currentSpeed += _accelerationRate * Time.deltaTime;
        if (_currentSpeed >= _maxMovementSpeed)
        {
            _currentSpeed = _maxMovementSpeed;
            _isAccelerating = false;
        }
    }
    else
    {
        _currentSpeed -= _decelerationRate * Time.deltaTime;
        if (_currentSpeed <= 0f)
        {
            _currentSpeed = 0f;
            if (HasPassedPlayer())
            {
                _isAccelerating = true;
            }
        }
    }
    _rigidbody.velocity = transform.up * _currentSpeed;
}

private bool HasPassedPlayer()
{
    Vector2 toPlayer = _playerTransform.position - transform.position;
    Vector2 toPreviousPosition = _previousPosition - (Vector2)transform.position;

    return Vector2.Dot(toPlayer, toPreviousPosition) < 0;
}

private void AvoidOverlap()
{
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f); 
    foreach (Collider2D collider in colliders)
    {
        if (collider.gameObject != gameObject && collider.GetComponent<LatencyMovement>() != null)
        {
            Vector2 repulsionDirection = (transform.position - collider.transform.position).normalized;
            _rigidbody.AddForce(repulsionDirection * _repulsionForce, ForceMode2D.Force);
        }
    }
}
}