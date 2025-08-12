using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoolManager : SingleTon<SoundPoolManager>
{
    public SoundSource[] prefabs = new SoundSource[5];
    private Dictionary<int, Queue<SoundSource>> pools = new Dictionary<int, Queue<SoundSource>>();

    public override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        prefabs[0] = Resources.Load<SoundSource>("SoundSource");
        prefabs[1] = Resources.Load<SoundSource>("RollOffSoundSource");

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] != null)
            {
                pools[i] = new Queue<SoundSource>();
            }
        }
    }

    public SoundSource GetObject(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefabIndex) || prefabs[prefabIndex] == null)
        {
            Debug.LogError($"프리팹 인덱스 {prefabIndex}에 대한 풀이 존재하지 않습니다.");
            return null;
        }

        SoundSource obj = null;

        // 풀에서 유효한 객체 찾기
        while (pools[prefabIndex].Count > 0)
        {
            obj = pools[prefabIndex].Dequeue();
            if (obj != null && obj.gameObject != null)
            {
                break;
            }
            obj = null;
        }

        // 유효한 객체가 없으면 새로 생성
        if (obj == null)
        {
            obj = Instantiate(prefabs[prefabIndex]);
            if (obj != null)
            {
                var poolable = obj.GetComponent<IPoolable>();
                poolable?.Initialize(o => ReturnObject(prefabIndex, o));
            }
        }

        if (obj != null)
        {
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            obj.GetComponent<IPoolable>()?.OnSpawn();
        }

        return obj;
    }

    public void ReturnObject(int prefabIndex, SoundSource obj)
    {
        if (obj == null || obj.gameObject == null)
        {
            return;
        }

        if (!pools.ContainsKey(prefabIndex))
        {
            Destroy(obj.gameObject);
            return;
        }

        obj.gameObject.SetActive(false);
        pools[prefabIndex].Enqueue(obj);
    }

    // 씬 전환 시 풀 정리
    public void ClearPools()
    {
        foreach (var pool in pools.Values)
        {
            while (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                if (obj != null && obj.gameObject != null)
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        ClearPools();
    }
}