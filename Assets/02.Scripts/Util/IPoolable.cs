using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Initialize(System.Action<SoundSource> returnCallback);
    void OnSpawn();
}
