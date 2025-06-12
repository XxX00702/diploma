using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    private PlayerInputs _playerInput;
    private float _radius;
    private LayerMask _interactableLayer;
    private HashSet<IInteractable> _currentInteractables = new HashSet<IInteractable>();

    public void Initialize(LayerMask interactableLayer, float radius = 2f)
    {
        _interactableLayer = interactableLayer;
        _radius = radius;
        _playerInput = Player._playerInputsAction;
        _playerInput.Player.Interact.performed += OnInteract;
    }

    private void Update()
    {
        if (Player._isPaused) return;

        CheckForInteractable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Interact.performed -= OnInteract;
    }

    public void CheckForInteractable()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _interactableLayer);
        HashSet<IInteractable> newInteractables = new HashSet<IInteractable>();

        foreach (var hitCollider in hitColliders)
        {
            var interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null && interactable.IsInteractable)
            {
                newInteractables.Add(interactable);
                if (!_currentInteractables.Contains(interactable))
                {
                    interactable.SetOutlineEffect(true);
                }
            }
        }

        foreach (var interactable in _currentInteractables)
        {
            if (!newInteractables.Contains(interactable))
            {
                interactable.SetOutlineEffect(false);
            }
        }

        _currentInteractables = newInteractables;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (Player._isPaused) return;

        foreach (var interactable in _currentInteractables)
        {
            if (interactable.IsInteractable)
            {
                Debug.Log("Player interacted with - " + interactable);
                interactable.Interact();
                return;
            }
        }
    }
}
