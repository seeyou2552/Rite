using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDefinition//아이템 종류 정의
{
    [Header("아이템 이름")]
    public string itemName;
    [Header("아이템 프리팹")]
    public GameObject itemPrefab;
    [Header("최소/최대 배치 개수")]
    public int minCount;
    public int maxCount;
}
public class ItemRandomSpawn : MonoBehaviour
{
    [Header("Item Point 태그")]
    public string itemPointTag = "ItemPoint";

    [Header("Item Definitions 설정하기")]
    public List<ItemDefinition> itemDefs = new List<ItemDefinition>();

    private void Start()
    {
        var points = GameObject.FindGameObjectsWithTag(itemPointTag);//points 변수에 태그저장
        int totalPoints = points.Length;
        var itemPoints = new List<Transform>();
        foreach (var p in points)
            itemPoints.Add(p.transform);

        // 아이템 배치 및 개수 수집
        var spawnCounts = SpawnItems(itemPoints);

        // 결과 로그 출력
        int totalSpawned = 0;
        Debug.Log($"[ItemRandomSpawn] ItemPoint 총 개수 : {totalPoints}");
        foreach (var key in spawnCounts)
        {
            Debug.Log($"아이템 '{key.Key}'의 스폰개수: {key.Value}");
            totalSpawned += key.Value;
        }
        Debug.Log($"[ItemRandomSpawn] 모든 아이템 스폰 개수: {totalSpawned}");
    }

    private Dictionary<string, int> SpawnItems(List<Transform> itemPoints)
    {
        // 아이템별 배치 카운트 초기화
        var counts = new Dictionary<string, int>();
        foreach (var item in itemDefs)
            counts[item.itemName] = 0;

        // 정의된 아이템 목록 순회
        foreach (var item in itemDefs)
        {
            int countToSpawn = Random.Range(item.minCount, item.maxCount + 1);
            for (int i = 0; i < countToSpawn; i++)
            {
                if (itemPoints.Count == 0)
                    return counts; // 포인트 부족 시 종료

                int idx = Random.Range(0, itemPoints.Count);
                Transform spawnPoint = itemPoints[idx];
                //튜터님이 도와주신 코드
                var obj = Instantiate(item.itemPrefab, spawnPoint.position, spawnPoint.rotation);
                obj.transform.SetParent(spawnPoint, true);
                //Instantiate(item.itemPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                itemPoints.RemoveAt(idx);
                counts[item.itemName]++;
            }
        }
        return counts;
    }
}
