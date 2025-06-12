using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Weapon : Item, IInteractable
{
    private GameObject _playerHand;
    private Player player;

    [Header("Sound")] 
    [SerializeField] private AudioClip _fireSound;

    [SerializeField] private AudioClip _pickupSound;
    
    [Header("Stats")]
    [SerializeField] private float _attackCooldown = 0.5f;

    [Header("Projectiles")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile[] _weaponProjectiles;
    
    private bool _isCooldownEnded = true;
    private bool _isPickedUp = false;

    private int _currentProjectile = 0;

    private Collider2D _collider2D;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            _playerHand = FindHand(player.transform);
        }
        ObjectMaterial = GetComponentInChildren<SpriteRenderer>().material;
        SetOutlineEffect(true);
    }

    private void FixedUpdate()
    {
        if (_isPickedUp)
        {
            transform.position = _playerHand.transform.position;
        }
    }

    private GameObject FindHand(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Hand"))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private int CurrentProjectile
    {
        get => _currentProjectile;
        set
        {
            if (value < 0)
                throw new ApplicationException("Current projectile cannot be less than zero!!!");
            else if (value > _weaponProjectiles.Length - 1)
                _currentProjectile = 0;
            else
                _currentProjectile = value;
        }
    }

    public void Attack()
    {
        if (!_isPickedUp) return;
        if (!_isCooldownEnded) return;
        var bullet = Instantiate(_weaponProjectiles[_currentProjectile], _firePoint.position, _firePoint.rotation);
        bullet._damage = Mathf.RoundToInt(bullet._damage *  player.DamageIncrease);
        SoundFXManager.Instance.PlaySoundFXClip(_fireSound, bullet.transform, 1f);
        CurrentProjectile++;
        StartCoroutine(IsCooldownEnded());
    }

    private IEnumerator IsCooldownEnded()
    {
        _isCooldownEnded = false;
        yield return new WaitForSeconds(_attackCooldown - player.FireCooldownDecrease);
        _isCooldownEnded = true;
    }

    public void AttachWeaponToHand()
    {
        if (_playerHand != null)
        {
            _collider2D.enabled = false;
            _rigidbody2D.isKinematic = true;
            transform.SetParent(_playerHand.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            player.CurrentWeapon = this;
            _isPickedUp = true;
            SetOutlineEffect(false);
            IsInteractable = false;
        }
    }

    public void Drop()
    {
        transform.SetParent(null);
        _collider2D.enabled = true;
        _rigidbody2D.isKinematic = false;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - transform.position).normalized;
        _rigidbody2D.AddForce(direction * 10, ForceMode2D.Impulse);
        float randomTorque = UnityEngine.Random.Range(-10f, 10f);
        _rigidbody2D.AddTorque(randomTorque, ForceMode2D.Impulse);
        _rigidbody2D.drag = 1f;
        _rigidbody2D.angularDrag = 0.5f;

        _isPickedUp = false;
        SetOutlineEffect(true);
        IsInteractable = true;
    }

    public void DropCurrentWeapon()
    {
        if (player.CurrentWeapon != null)
        {
            player.CurrentWeapon.Drop();
            player.CurrentWeapon = null;
        }
    }

    public bool IsInteractable { get; set; } = true;
    public Material ObjectMaterial { get; set; }
    public void Interact()
    {
        if (!IsInteractable) return;

        if (player.CurrentWeapon != null)
        {
            DropCurrentWeapon();
        }
        SoundFXManager.Instance.PlaySoundFXClip(_pickupSound, transform, 1f);
        AttachWeaponToHand();
    }

    public void SetOutlineEffect(bool isEnabled)
    {
        if (ObjectMaterial != null)
        {
            ObjectMaterial.SetFloat("_Effect_Strength", isEnabled ? 1f : 0f);
        }
    }
}
