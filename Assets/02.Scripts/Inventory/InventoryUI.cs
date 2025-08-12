using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;              // 슬롯 프리팹 (Slot 오브젝트가 Image 컴포넌트)
    public Transform panelTransform;           // 슬롯들이 들어갈 부모(Panel)
    public int slotCount = 4;                  // 슬롯 개수

    [Header("카운터 UI")]
    public TextMeshProUGUI relicCountText;     // 부적 개수 텍스트

    private List<GameObject> slots = new List<GameObject>(); // 슬롯 게임오브젝트 리스트
    private List<Image> slotIcons = new List<Image>();       // 슬롯 아이콘 이미지 리스트
    private List<GameObject> iconObjects = new List<GameObject>(); // 아이콘 GameObject 리스트 추가
    private int selectedIndex = -1;
    private Color defaultColor = Color.white;   // 기본 슬롯 색상
    private Color selectedColor = Color.yellow; // 선택된 슬롯 색상

    void Start()
    {
        RebuildUI();
        UpdateCounters(0, 0); // 초기 카운터
    }

    /// <summary>
    /// 슬롯 UI 전체 재생성
    /// </summary>
    private void RebuildUI()
    {
        // 기존 슬롯 삭제
        foreach (var slot in slots)
        {
            if (slot != null)
                Destroy(slot);
        }
        slots.Clear();
        slotIcons.Clear();
        iconObjects.Clear(); // 아이콘 오브젝트 리스트도 클리어

        // 선택 인덱스 초기화
        selectedIndex = -1;

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, panelTransform);
            slots.Add(slot);

            GameObject iconObject = slot.transform.Find("Icon")?.gameObject;
            if (iconObject != null)
            {
                Image iconImage = iconObject.GetComponent<Image>();
                if (iconImage != null)
                {
                    // 초기 상태: 아이콘 GameObject 비활성화
                    iconObject.SetActive(false);
                    slotIcons.Add(iconImage);
                    iconObjects.Add(iconObject);
                }
                else
                {
                    Debug.LogError($"[InventoryUI] 슬롯[{i}]의 Icon에 Image 컴포넌트가 없습니다.");
                    slotIcons.Add(null);
                    iconObjects.Add(null);
                }
            }
            else
            {
                Debug.LogError($"[InventoryUI] 슬롯[{i}]에 'Icon' 자식이 없습니다.");
                slotIcons.Add(null);
                iconObjects.Add(null);
            }
        }
    }

    /// <summary>
    /// 아이템 목록에 따라 인벤토리 UI 업데이트
    /// </summary>
    public void UpdateInventoryUI(List<ItemData> items)
    {
        // null 체크
        if (items == null)
        {
            Debug.LogWarning("[InventoryUI] items 리스트가 null입니다.");
            return;
        }

        bool needRebuild = false;

        // 슬롯 개수 불일치 확인
        if (slotIcons.Count != slotCount)
        {
            Debug.LogWarning($"[InventoryUI] 슬롯 개수 불일치. 예상: {slotCount}, 실제: {slotIcons.Count}. UI 재생성");
            needRebuild = true;
        }

        // 파괴된 아이콘 확인
        if (!needRebuild)
        {
            for (int i = 0; i < slotIcons.Count; i++)
            {
                if (slotIcons[i] == null || iconObjects[i] == null)
                {
                    Debug.LogWarning($"[InventoryUI] slotIcon[{i}] 또는 iconObject[{i}]가 Destroy되었습니다. UI 슬롯 전체 재생성");
                    needRebuild = true;
                    break;
                }
            }
        }

        if (needRebuild)
        {
            RebuildUI();
        }

        // 아이콘 표시/숨김
        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (slotIcons[i] == null || iconObjects[i] == null) continue; // 추가 안전장치

            if (i < items.Count && items[i] != null)
            {
                // 아이템이 있으면: 스프라이트 설정 + GameObject 활성화
                slotIcons[i].sprite = items[i].icon;
                iconObjects[i].SetActive(true);
            }
            else
            {
                // 아이템이 없으면: GameObject 비활성화
                iconObjects[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 부적/열쇠 개수 텍스트 업데이트
    /// </summary>
    public void UpdateCounters(int relicCount, int keyCount)
    {
        if (relicCountText != null)
            relicCountText.text = relicCount + " / 5";
    }

    /// <summary>
    /// 선택된 슬롯 색상 변경
    /// </summary>
    public void SetSelectedIndex(int index)
    {
        // 유효성 검증 강화
        if (index < -1 || index >= slots.Count)
        {
            Debug.LogWarning($"[InventoryUI] SetSelectedIndex: 잘못된 index 접근: {index} (슬롯 개수: {slots.Count})");
            return;
        }

        // 슬롯이 실제로 존재하는지 확인
        if (index >= 0 && (slots[index] == null || slots[index].GetComponent<Image>() == null))
        {
            Debug.LogWarning($"[InventoryUI] SetSelectedIndex: 슬롯[{index}]이 파괴되었거나 Image 컴포넌트가 없습니다.");
            return;
        }

        // 이전 선택된 슬롯 초기화
        if (selectedIndex >= 0 && selectedIndex < slots.Count && slots[selectedIndex] != null)
        {
            Image prevImage = slots[selectedIndex].GetComponent<Image>();
            if (prevImage != null)
                prevImage.color = defaultColor;
        }

        selectedIndex = index;

        // 새로 선택된 슬롯 색상 변경 (index가 -1이면 선택 해제)
        if (selectedIndex >= 0)
        {
            Image currentImage = slots[selectedIndex].GetComponent<Image>();
            if (currentImage != null)
                currentImage.color = selectedColor;
        }
    }

    /// <summary>
    /// 현재 선택된 인덱스 반환 (유효성 검증 포함)
    /// </summary>
    public int GetValidSelectedIndex()
    {
        if (selectedIndex < 0 || selectedIndex >= slots.Count || slots[selectedIndex] == null)
        {
            return -1; // 유효하지 않은 선택
        }
        return selectedIndex;
    }

    /// <summary>
    /// 슬롯이 유효한지 확인
    /// </summary>
    public bool IsSlotValid(int index)
    {
        return index >= 0 && index < slots.Count && slots[index] != null;
    }
}