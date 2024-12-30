using System.Collections.Generic;
using UnityEngine;

public class VFXResizer : MonoBehaviour
{
    private List<ParticleSystem> particleSystemList = new();

    private void Awake()
    {
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            particleSystemList.Add(p);
        }
    }

    public void ChangeSize(float newSize)
    {
        foreach (ParticleSystem p in particleSystemList)
        {
            var main = p.main;
            main.startSizeMultiplier *= newSize;
            main.startSpeedMultiplier *= newSize;
            var shape = p.shape;
            shape.radius *= newSize;
        }
    }
}
