using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    public float _cooldown { get; set; }
    public void Activate();
}
