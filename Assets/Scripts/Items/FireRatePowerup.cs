using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRatePowerup : Item, IInteractable
{
    [SerializeField]
    private float _percentIncrease = 0.2f;

    [field: SerializeField] public bool IsInteractable { get; set; } = true;
    [SerializeField] private AudioClip _powerup;
    public Material ObjectMaterial { get; set; }

    private void Awake()
    {
        ObjectMaterial = GetComponent<SpriteRenderer>().material;
        SetOutlineEffect(true);
    }

    public void Interact()
    {
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        float count = player.FireCooldownDecrease + _percentIncrease;
        player.FireCooldownDecrease = count;
        SoundFXManager.Instance.PlaySoundFXClip(_powerup, transform, 1f);
        Destroy(gameObject);
    }

    public void SetOutlineEffect(bool isEnabled)
    {
        if (ObjectMaterial != null)
        {
            ObjectMaterial.SetFloat("_Effect_Strength", isEnabled ? 1f : 0f);
        }
    }
}
