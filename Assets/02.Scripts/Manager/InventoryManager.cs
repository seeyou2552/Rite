using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingleTon<InventoryManager>
{
    public List<GameObject> items = new List<GameObject>(4);
    public int selectedIndex = 0;
    private InventoryUI inventoryUI;

    // 부적 개수 관리 (열쇠는 제거)
    private int relicCount = 0;

    public GameObject selectedItem => (items.Count > 0 && selectedIndex >= 0 && selectedIndex < items.Count) ? items[selectedIndex] : null;

    public override void Awake()
    {
        base.Awake();

        // InventoryUI 찾기 시도
        FindInventoryUI();
    }

    void Start()
    {
        // Start에서 한 번 더 시도 (UI가 늦게 생성될 수 있음)
        if (inventoryUI == null)
        {
            FindInventoryUI();
        }
    }

    private void FindInventoryUI()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI를 찾을 수 없습니다. UI가 생성될 때까지 대기합니다.");
        }
    }

    public bool AddItem(GameObject itemObj)
    {
        Debug.Log("AddItem called with: " + itemObj.name);

        ItemInstance instance = itemObj.GetComponent<ItemInstance>();
        if (instance != null)
        {
            // 부적인 경우 개수만 증가
            if (instance.data.type == ItemType.Relic)
            {
                relicCount++;
                GameManager.Instance.IncreaseRelicCount();
                Debug.Log("부적 획득! 현재 개수: " + relicCount);
                Destroy(itemObj); // 아이템 오브젝트 삭제
                UpdateUI();
                return true;
            }
        }

        // 일반 아이템 (열쇠 포함)인 경우 인벤토리 슬롯에 추가
        if (items.Count >= 4) return false;

        items.Add(itemObj);
        // 필드에서 사라지게 비활성화
        itemObj.SetActive(false);

        UpdateUI();
        return true;
    }

    public void RemoveItem(GameObject itemObj)
    {
        if (items.Contains(itemObj))
        {
            int index = items.IndexOf(itemObj);
            items.RemoveAt(index);
            Destroy(itemObj);

            // 현재 선택 인덱스 조정 - 안전한 범위로 조정
            ValidateAndAdjustSelectedIndex();
            UpdateUI();
        }
    }

    public void DropSelectedItem()
    {
        // 유효성 검사를 더 엄격하게
        if (!IsSelectedIndexValid())
        {
            Debug.LogWarning("DropSelectedItem(): 유효하지 않은 선택 인덱스 또는 빈 인벤토리");
            return;
        }

        GameObject itemToDrop = items[selectedIndex];
        if (itemToDrop == null)
        {
            Debug.LogWarning("DropSelectedItem(): 선택된 아이템이 null입니다");
            return;
        }

        // ItemInstance 컴포넌트를 가져옴
        ItemInstance instance = itemToDrop.GetComponent<ItemInstance>();
        if (instance == null)
        {
            Debug.LogWarning("DropSelectedItem(): ItemInstance 컴포넌트를 찾을 수 없습니다");
            return;
        }

        Debug.Log($"드롭할 아이템: {instance.data.name}, 인덱스: {selectedIndex}");

        // 드롭 위치: Player의 dropPosition 사용
        Vector3 dropPosition;
        Quaternion rotation = Quaternion.identity;

        if (Player.Instance.dropPosition != null)
        {
            // dropPosition Transform이 설정되어 있으면 그 위치 사용
            dropPosition = Player.Instance.dropPosition.position;
            rotation = Player.Instance.dropPosition.rotation;
        }
        else
        {
            // dropPosition이 없으면 기존 방식 (플레이어 앞)
            dropPosition = Player.Instance.transform.position + Player.Instance.transform.forward * 1.5f;
        }

        // 프리팹으로부터 실제 아이템 생성
        GameObject droppedItem = Instantiate(instance.data.prefab, dropPosition, rotation);
        droppedItem.SetActive(true); // 활성화 확인

        // 인벤토리에서 제거 (선택된 아이템만)
        items.RemoveAt(selectedIndex);
        Destroy(itemToDrop);

        // 선택 인덱스 조정
        ValidateAndAdjustSelectedIndex();
        UpdateUI();
        Debug.Log($"아이템 드롭 완료. 남은 아이템 수: {items.Count}");
    }

    public void UseSelectedItem()
    {
        // 유효성 검사 강화 - 빈 인벤토리일 때는 조용히 리턴
        if (items.Count == 0 || selectedIndex < 0 || selectedIndex >= items.Count)
        {
            // 빈 인벤토리일 때는 경고 없이 조용히 리턴
            if (items.Count == 0)
            {
                return;
            }
            Debug.LogWarning($"UseSelectedItem(): 잘못된 selectedIndex 접근: {selectedIndex} (아이템 수: {items.Count})");
            return;
        }

        GameObject selectedItemObj = items[selectedIndex];
        if (selectedItemObj == null)
        {
            Debug.LogWarning($"UseSelectedItem(): 선택된 아이템[{selectedIndex}]이 null입니다");
            // null 아이템 제거
            items.RemoveAt(selectedIndex);
            ValidateAndAdjustSelectedIndex();
            UpdateUI();
            return;
        }

        var usable = selectedItemObj.GetComponent<IUsableItem>();
        if (usable != null)
        {
            Debug.Log($"아이템 사용: {selectedItemObj.name}");

            // 아이템 사용 전 인덱스 저장
            int currentIndex = selectedIndex;
            usable.Use();
        }
        // try
            // {

                // 사용 후 아이템이 소모되었는지 확인
                // 리스트 크기가 변경되었거나 해당 인덱스의 아이템이 변경되었으면 소모된 것
                // if (currentIndex < items.Count && items[currentIndex] == selectedItemObj)
                // {
                //     // 아이템이 여전히 존재하고 비활성화되었다면 소모된 것
                //     if (selectedItemObj == null || !selectedItemObj.activeInHierarchy)
                //     {
                //         items.RemoveAt(currentIndex);
                //         ValidateAndAdjustSelectedIndex();
                        UpdateUI();
        //             }
        //         }
        //         else
        //         {
        //             // 아이템이 이미 리스트에서 제거되었음 (Use() 메서드에서 처리)
        //             ValidateAndAdjustSelectedIndex();
        //             UpdateUI();
        //         }
        //     }
        //     catch (System.ArgumentOutOfRangeException)
        //     {
        //         Debug.LogWarning("UseSelectedItem(): 아이템 사용 중 인덱스 오류 발생. 인벤토리 상태 재조정");
        //         ValidateAndAdjustSelectedIndex();
        //         UpdateUI();
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("UseSelectedItem(): 선택된 아이템은 IUsableItem이 아님");
        // }
    }

    public void ScrollSelectItem(int direction)
    {
        if (items.Count == 0)
        {
            selectedIndex = 0;
            return;
        }

        selectedIndex += direction;
        if (selectedIndex < 0) selectedIndex = items.Count - 1;
        else if (selectedIndex >= items.Count) selectedIndex = 0;

        // UI 업데이트 시 유효성 검사
        if (inventoryUI != null)
        {
            inventoryUI.SetSelectedIndex(selectedIndex);
        }
        else
        {
            // UI가 없으면 찾아보기
            FindInventoryUI();
            if (inventoryUI != null)
            {
                inventoryUI.SetSelectedIndex(selectedIndex);
            }
        }
    }

    private void UpdateUI()
    {
        // InventoryUI가 없으면 다시 찾아보기
        if (inventoryUI == null)
        {
            FindInventoryUI();
            if (inventoryUI == null)
            {
                // UI가 아직 없으면 조용히 리턴 (로그 스팸 방지)
                return;
            }
        }

        // 일반 아이템들만 슬롯에 표시
        List<ItemData> itemDataList = new List<ItemData>();
        for (int i = items.Count - 1; i >= 0; i--) // 역순으로 순회하여 안전하게 제거
        {
            if (items[i] == null)
            {
                // null 아이템 제거
                items.RemoveAt(i);
                continue;
            }

            ItemInstance instance = items[i].GetComponent<ItemInstance>();
            if (instance != null && instance.data != null)
            {
                itemDataList.Insert(0, instance.data); // 순서 유지를 위해 앞에 삽입
            }
            else
            {
                // 유효하지 않은 아이템 제거
                Debug.LogWarning($"유효하지 않은 아이템 제거: {items[i].name}");
                items.RemoveAt(i);
            }
        }

        // 선택 인덱스 유효성 재검증 후 UI에 반영
        ValidateAndAdjustSelectedIndex();

        // UI 업데이트
        try
        {
            inventoryUI.UpdateInventoryUI(itemDataList);
            inventoryUI.SetSelectedIndex(selectedIndex);
            inventoryUI.UpdateCounters(relicCount, 0);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI 업데이트 중 오류 발생: {e.Message}");
            // UI 재찾기
            inventoryUI = null;
        }
    }

    public int GetItemCount(ItemType type)
    {
        // 부적은 별도 카운터에서 반환
        if (type == ItemType.Relic)
            return relicCount;

        // 일반 아이템들(열쇠 포함)은 인벤토리에서 카운트
        int count = 0;
        foreach (var item in items)
        {
            if (item != null)
            {
                ItemInstance instance = item.GetComponent<ItemInstance>();
                if (instance != null && instance.data != null && instance.data.type == type)
                {
                    count++;
                }
            }
        }
        return count;
    }

    // 부적 개수 getter 메서드
    public int GetRelicCount() => relicCount;

    // 부적 사용 메서드
    public bool UseRelic()
    {
        if (relicCount > 0)
        {
            relicCount--;
            UpdateUI();
            return true;
        }
        return false;
    }

    // 열쇠 보유 확인 메서드
    public bool HasKey()
    {
        return GetItemCount(ItemType.Key) > 0;
    }

    // === 새로 추가된 유틸리티 메서드들 ===

    /// <summary>
    /// 현재 선택된 인덱스가 유효한지 확인
    /// </summary>
    private bool IsSelectedIndexValid()
    {
        return items.Count > 0 && selectedIndex >= 0 && selectedIndex < items.Count;
    }

    /// <summary>
    /// 선택 인덱스를 안전한 범위로 조정
    /// </summary>
    private void ValidateAndAdjustSelectedIndex()
    {
        if (items.Count == 0)
        {
            selectedIndex = 0;
        }
        else if (selectedIndex >= items.Count)
        {
            selectedIndex = items.Count - 1;
        }
        else if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }

        // selectedIndex가 조정되었을 때 로그 출력
        if (selectedIndex != Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, items.Count - 1)))
        {
            Debug.Log($"SelectedIndex 조정됨: {selectedIndex} -> {Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, items.Count - 1))}");
        }
    }

    /// <summary>
    /// 디버깅용: 현재 인벤토리 상태 출력
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugInventoryState()
    {
        Debug.Log($"=== Inventory State ===");
        Debug.Log($"Items Count: {items.Count}");
        Debug.Log($"Selected Index: {selectedIndex}");
        Debug.Log($"Relic Count: {relicCount}");
        Debug.Log($"Is Selected Valid: {IsSelectedIndexValid()}");

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                ItemInstance instance = items[i].GetComponent<ItemInstance>();
                string itemName = instance?.data?.name ?? "Unknown";
                Debug.Log($"  [{i}] {itemName} {(i == selectedIndex ? "(Selected)" : "")}");
            }
            else
            {
                Debug.Log($"  [{i}] NULL ITEM");
            }
        }
    }
}