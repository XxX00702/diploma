using UnityEditor;
using UnityEngine;

public class Invisibility : Item, IInteractable, IActivatable
{
    [field:SerializeField] public float _cooldown { get; set; } = 30f;
    private Player _player;
    public Material ObjectMaterial { get; set; }
    [field:SerializeField] public bool IsInteractable { get; set; }

    private void Awake()
    {
        ObjectMaterial = GetComponent<SpriteRenderer>().material;
        SetOutlineEffect(true);
        _player = FindObjectOfType<Player>();
    }

    public void Activate()
    {
        _player.ActivateInvincibility();
    }

    public void Interact()
    {
        _player.CurrentActiveItem = this;
        Debug.Log("New player current activatable item is " + _player.CurrentActiveItem);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SetOutlineEffect(bool isEnabled)
    {
        if (ObjectMaterial != null)
        {
            ObjectMaterial.SetFloat("_Effect_Strength", isEnabled ? 1f : 0f);
        }
    }
}