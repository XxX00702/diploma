using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActiveItems : MonoBehaviour
{
    [field: SerializeField] public float Cooldown { get; set; }
    public bool isCooldownActive = false;
    private Player _player;
    private Coroutine cooldownCoroutine;
    private PlayerInterface _playerInterface;
    private IActivatable _previousActiveItem;

    public void Initialize()
    {
        _player = FindObjectOfType<Player>();
        _playerInterface = FindObjectOfType<PlayerInterface>();

        _player.OnActiveItemChange += HandleActiveItemChange;
        Player._playerInputsAction.Player.Activatespecialitem.performed += Activate;
    }

    private void OnDisable()
    {
        _player.OnActiveItemChange -= HandleActiveItemChange;
        Player._playerInputsAction.Player.Activatespecialitem.performed -= Activate;
    }

    private void HandleActiveItemChange(IActivatable activeItem)
    {
        if (_previousActiveItem != null)
        {
            GameObject previousItemGameObject = (_previousActiveItem as MonoBehaviour).gameObject;
            if (previousItemGameObject != null)
            {
                previousItemGameObject.SetActive(true);
                previousItemGameObject.transform.position = _player.transform.position;
                var collider = previousItemGameObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }

                var spriteRenderer = previousItemGameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    spriteRenderer.enabled = true; 
            }
        }

        _previousActiveItem = activeItem; 

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        Cooldown = activeItem._cooldown;

        _playerInterface.SetCooldownActive(isCooldownActive);
    }

    public void Activate(InputAction.CallbackContext ctx)
    {
        Debug.Log("Player tried activate item - " + _player.CurrentActiveItem);

        if(_player.CurrentActiveItem == null) return;
        if (isCooldownActive)
        {
            Debug.Log("Item have cooldown");
            return;
        }
        Debug.Log("Player activated item - " + _player.CurrentActiveItem);
        _player.CurrentActiveItem.Activate();
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        Debug.Log("Activatable item cooldown started");
        isCooldownActive = true;
        _playerInterface.SetCooldownActive(isCooldownActive);
        yield return new WaitForSeconds(Cooldown);
        isCooldownActive = false;
        Debug.Log("Activatable item cooldown ended");
        cooldownCoroutine = null;

        _playerInterface.SetCooldownActive(isCooldownActive);
    }
}
