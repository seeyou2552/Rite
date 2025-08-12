using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDefinition//������ ���� ����
{
    [Header("������ �̸�")]
    public string itemName;
    [Header("������ ������")]
    public GameObject itemPrefab;
    [Header("�ּ�/�ִ� ��ġ ����")]
    public int minCount;
    public int maxCount;
}
public class ItemRandomSpawn : MonoBehaviour
{
    [Header("Item Point �±�")]
    public string itemPointTag = "ItemPoint";

    [Header("Item Definitions �����ϱ�")]
    public List<ItemDefinition> itemDefs = new List<ItemDefinition>();

    private void Start()
    {
        var points = GameObject.FindGameObjectsWithTag(itemPointTag);//points ������ �±�����
        int totalPoints = points.Length;
        var itemPoints = new List<Transform>();
        foreach (var p in points)
            itemPoints.Add(p.transform);

        // ������ ��ġ �� ���� ����
        var spawnCounts = SpawnItems(itemPoints);

        // ��� �α� ���
        int totalSpawned = 0;
        Debug.Log($"[ItemRandomSpawn] ItemPoint �� ���� : {totalPoints}");
        foreach (var key in spawnCounts)
        {
            Debug.Log($"������ '{key.Key}'�� ��������: {key.Value}");
            totalSpawned += key.Value;
        }
        Debug.Log($"[ItemRandomSpawn] ��� ������ ���� ����: {totalSpawned}");
    }

    private Dictionary<string, int> SpawnItems(List<Transform> itemPoints)
    {
        // �����ۺ� ��ġ ī��Ʈ �ʱ�ȭ
        var counts = new Dictionary<string, int>();
        foreach (var item in itemDefs)
            counts[item.itemName] = 0;

        // ���ǵ� ������ ��� ��ȸ
        foreach (var item in itemDefs)
        {
            int countToSpawn = Random.Range(item.minCount, item.maxCount + 1);
            for (int i = 0; i < countToSpawn; i++)
            {
                if (itemPoints.Count == 0)
                    return counts; // ����Ʈ ���� �� ����

                int idx = Random.Range(0, itemPoints.Count);
                Transform spawnPoint = itemPoints[idx];
                //Ʃ�ʹ��� �����ֽ� �ڵ�
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
