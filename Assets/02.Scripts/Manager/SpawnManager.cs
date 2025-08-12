using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : SingleTon<SpawnManager>
{
    // 스폰 지점으로 사용할 Transform 배열
    [SerializeField] private Transform[] spawnPoints;

    public override void Awake()
    {
        base.Awake();

        // spawnPoints가 없으면 씬에서 자동으로 찾아서 할당 시도
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = FindObjectsOfType<Transform>();
        }
    }

    /// <summary>
    /// 랜덤 스폰 포인트 반환 (없으면 Vector3.zero 반환)
    /// </summary>
    public Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return Vector3.zero;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }
}
