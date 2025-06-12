using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public DoorDirection _doorDirection;
    private bool _isAvailable = true;
    private bool _isOpen = true;
    [SerializeField] private Collider2D _doorCollider;
    private Animator _doorOpenAnimator;
    [SerializeField] private string _doorOpenInAnimationName;
    [SerializeField] private string _doorOpenOutAnimationName;
    [SerializeField] private GameObject _doorBlocker;
    private bool _isLocked = false;
    
    private void Start()
    {
        _doorOpenAnimator = GetComponentInChildren<Animator>();
        IsOpen = _isOpen;
        IsAvailable = _isAvailable;
    }

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            if (_isOpen)
            {
                _isLocked = false;
                Open();
            }
            else
            {
                _isLocked = true;
                Close();
            }
        }
    }
    public bool IsAvailable
    {
        get => _isAvailable;
        set
        {
            _isAvailable = value;
            if (_isAvailable)
            {
                _doorBlocker.SetActive(true);
            }
            else
            {
                _doorBlocker.SetActive(false);
            }
        }
    }

    public DoorDirection GetOppositeDirection()
    {
        switch (_doorDirection)
        {
            case DoorDirection.Top:
                return DoorDirection.Bottom;
            case DoorDirection.Bottom:
                return DoorDirection.Top;
            case DoorDirection.Left:
                return DoorDirection.Right;
            case DoorDirection.Right:
                return DoorDirection.Left;
        }

        return DoorDirection.Top;
    }

    private string _playerTag = "Player";
    private bool _hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasPlayed || other.CompareTag(_playerTag) == false || _isLocked) return;
        Debug.Log("Doors opened!");
        _hasPlayed = true;
        Vector2 contactPoint = other.bounds.center;
        Vector2 center = _doorCollider.bounds.center;

        Vector2 direction = contactPoint - center;

        DoorDirection enteredDirection = DetermineEnteredDirection(direction);

        if (enteredDirection == _doorDirection)
        {
            _doorOpenAnimator.Play(_doorOpenInAnimationName);
        }
        else if (enteredDirection == GetOppositeDirection())
        {
            _doorOpenAnimator.Play(_doorOpenOutAnimationName);
        }

    }

    private DoorDirection DetermineEnteredDirection(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        if (angle >= -45f && angle <= 45f)
        {
            return DoorDirection.Bottom;
        }
        else if (angle > 45f && angle < 135f)
        {
            return DoorDirection.Right;
        }
        else if (angle >= 135f || angle <= -135f)
        {
            return DoorDirection.Top;
        }
        else if (angle < -45f && angle > -135f)
        {
            return DoorDirection.Left;
        }
        return DoorDirection.Left;
    }

    public void Open()
    {
        _doorCollider.isTrigger = true;
    }

    public void Close()
    {
        _doorCollider.isTrigger = false;
    }
}

public enum DoorDirection
{
    Top,
    Bottom,
    Left,
    Right
}
