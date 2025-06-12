using UnityEngine;

public class DamagePowerUp : Item, IInteractable
{

    [SerializeField]
    private float _percentIncrease = 1.6f;

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
        player.DamageIncrease *= _percentIncrease;
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
