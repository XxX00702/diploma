using UnityEngine;

public interface IInteractable
{
    public bool IsInteractable { get; set; }
    public Material ObjectMaterial { get; set; }
    public void Interact();
    void SetOutlineEffect(bool isEnabled);
}