using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private Player _playerStats;
    [SerializeField] private Image _gunContainer;
    [SerializeField] private Image _activeItemContainer;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private TextMeshProUGUI _hpCount;

    private string _playerTag = "Player";
    private bool _isCooldownActive = false; 

    private void Start()
    {
        _playerStats = GameObject.FindWithTag(_playerTag).GetComponent<Player>();
        UpdateHealthUI(_playerStats.CurrentHealthPoints, _playerStats.MaxHealthPoints);
        _playerStats.OnHealthChanged += UpdateHealthUI;
        _playerStats.OnWeaponChanged += UpdateGunSprite;
        _playerStats.OnActiveItemChange += UpdateItemSprite;
    }

    private void Update()
    {
        ItemCooldownCheck();
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        _hpBar.value = (float)currentHealth / maxHealth;
        _hpCount.text = $"{currentHealth}/{maxHealth}";
    }

    private void UpdateGunSprite(Weapon oldWeapon, Weapon newWeapon)
    {
        if (newWeapon == null) return;
        var sprite = newWeapon.GetComponentInChildren<SpriteRenderer>().sprite;
        _gunContainer.sprite = sprite;
    }

    private void UpdateItemSprite(IActivatable item)
    {
        if (item == null) return;

        GameObject itemGameObject = (item as MonoBehaviour).gameObject;
        if (itemGameObject == null) return;

        var spriteRenderer = itemGameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return;

        var sprite = spriteRenderer.sprite;
        _activeItemContainer.sprite = sprite;
        
    }

    public void ItemCooldownCheck()
    {
        if (_isCooldownActive)
        {
            var color = _activeItemContainer.color;
            color.a = 0.5f; 
            _activeItemContainer.color = color;
        }
        else
        {
            var color = _activeItemContainer.color;
            color.a = 1f; 
            _activeItemContainer.color = color;
        }
    }

    private void OnDestroy()
    {
        _playerStats.OnHealthChanged -= UpdateHealthUI;
        _playerStats.OnWeaponChanged -= UpdateGunSprite;
        _playerStats.OnActiveItemChange -= UpdateItemSprite;
    }

    private void OnDisable()
    {
        _playerStats.OnHealthChanged -= UpdateHealthUI;
        _playerStats.OnWeaponChanged -= UpdateGunSprite;
        _playerStats.OnActiveItemChange -= UpdateItemSprite;
    }

    public void SetCooldownActive(bool isActive)
    {
        _isCooldownActive = isActive;
        UpdateItemSprite(_playerStats.CurrentActiveItem); 
    }
}
