using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    [SerializeField] private float _healPercent = 0.2f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private AudioClip _heal;
    private GameObject _player;
    private string _playerTag = "Player";
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag(_playerTag);
        _rigidbody.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            _rigidbody.velocity = direction * movementSpeed;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_playerTag))
        {
            var player = other.GetComponent<Player>();
            int healAmount = Mathf.RoundToInt(player.MaxHealthPoints * _healPercent);
            if (healAmount <= 0) healAmount = 1;
            player.Heal(healAmount);
            SoundFXManager.Instance.PlaySoundFXClip(_heal, transform, 1f);
            Destroy(gameObject);
        }
    }
}
