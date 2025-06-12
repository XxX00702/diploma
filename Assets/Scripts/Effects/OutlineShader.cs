using UnityEngine;

public class OutlineShader
{
    public void AplyOutline(Material material, float _effectStrength = 1f)
    {
        material.SetFloat("_EffectStrength", _effectStrength);
    }
}
