using UnityEngine;

public class HealthPointsPowerup : Item, IInteractable
{
    [SerializeField]
    private float _percentIncrease = 1.2f;

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
        int count = Mathf.RoundToInt(player.MaxHealthPoints * _percentIncrease);
        if (count == player.MaxHealthPoints) count = 1;

        player.MaxHealthPoints = count;
        player.CurrentHealthPoints = player.MaxHealthPoints;
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
