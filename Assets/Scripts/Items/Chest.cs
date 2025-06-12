using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour, IInteractable
{
    [field: SerializeField] public bool IsInteractable { get; set; } = true;
    public Material ObjectMaterial { get; set; }
    private bool _hasInteracted  = false;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private Item[] _gameItems;

    private void Awake()
    {
        ObjectMaterial = GetComponent<SpriteRenderer>().material;
        SetOutlineEffect(false);
    }

    public void Interact()
    {
        if(!IsInteractable) return;
        if (_hasInteracted) return;
        IsInteractable = false;
        _hasInteracted = true;
        SoundFXManager.Instance.PlaySoundFXClip(_openSound, transform, 1f);
        Instantiate(GetRandomItem(), transform);
    }

    public void SetOutlineEffect(bool isEnabled)
    {
        if (ObjectMaterial != null)
        {
            ObjectMaterial.SetFloat("_Effect_Strength", isEnabled ? 1f : 0f);
        }
    }

    private Item GetRandomItem()
    {
        return _gameItems[Random.Range(0, _gameItems.Length)];
    }
}
