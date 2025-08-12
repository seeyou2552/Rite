using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 상호작용 시스템을 관리하는 클래스
/// 화면 중앙에서 레이캐스트를 통해 상호작용 가능한 오브젝트를 감지하고 상호작용을 처리
/// 오브젝트 타입에 따라 적절한 UI 텍스트를 표시
/// </summary>
public class Interaction : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    public float checkRate = 0.05f;           // 상호작용 체크 주기 (초)
    private float lastCheckTime;              // 마지막 체크 시간
    public float maxCheckDistance;            // 상호작용 가능한 최대 거리
    public LayerMask layerMask;              // 상호작용 가능한 레이어 마스크

    [Header("UI 설정")]
    public TextMeshProUGUI interactionText;   // 상호작용 안내 텍스트 UI
    public GameObject interactionUI;          // 상호작용 UI 패널 (선택사항)

    [Header("상호작용 오브젝트")]
    public GameObject curInteractGameObject;  // 현재 상호작용 가능한 게임오브젝트
    private IInteractable curInteractable;   // 현재 상호작용 가능한 인터페이스
    private ItemInstance itemInstance;       // 현재 아이템 인스턴스
    private Camera camera;                   // 메인 카메라 참조

    /// <summary>
    /// 초기화 - 메인 카메라 참조 설정
    /// </summary>
    private void Start()
    {
        camera = Camera.main;
        HideInteractionUI(); // 시작 시 UI 숨김
    }

    /// <summary>
    /// 매 프레임마다 상호작용 가능한 오브젝트를 체크
    /// 일정 주기(checkRate)마다 화면 중앙에서 레이캐스트를 발사하여 오브젝트 감지
    /// </summary>
    private void Update()
    {
        if (Player.Instance.controller.currentState == PlayerState.Dead) return;
        // 설정된 체크 주기가 지났는지 확인
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            // 화면 중앙에서 레이 생성 (크로스헤어 위치)
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit; // 레이에 맞은 오브젝트의 정보를 저장하는 변수

            // 레이캐스트 실행 - 설정된 거리와 레이어 마스크 내에서 오브젝트 감지
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // 새로운 오브젝트가 감지되었는지 확인
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    // 현재 상호작용 오브젝트 업데이트
                    curInteractGameObject = hit.collider.gameObject;

                    // IInteractable 인터페이스를 가진 컴포넌트 찾기
                    // 해당 오브젝트나 부모 오브젝트에서 인터페이스 검색
                    curInteractable = hit.collider.GetComponent<IInteractable>()
                        ?? hit.collider.GetComponentInParent<IInteractable>();

                    // 아이템 인스턴스 컴포넌트 가져오기
                    itemInstance = curInteractGameObject.GetComponent<ItemInstance>();

                    // 상호작용 UI 업데이트
                    UpdateInteractionUI();

                    // 디버그용 레이 시각화 (빨간색, 0.3초 동안 표시)
                    Debug.DrawRay(camera.transform.position, camera.transform.forward * maxCheckDistance, Color.red, 0.3f);
                }
            }
            else
            {
                // 감지된 오브젝트가 없으면 모든 참조 초기화
                ClearInteraction();
            }
        }
    }

    /// <summary>
    /// 감지된 오브젝트 타입에 따라 적절한 상호작용 UI 텍스트를 표시
    /// </summary>
    private void UpdateInteractionUI()
    {
        if (curInteractGameObject == null)
        {
            HideInteractionUI();
            return;
        }

        string interactionPrompt = GetInteractionText();

        if (!string.IsNullOrEmpty(interactionPrompt))
        {
            ShowInteractionUI(interactionPrompt);
        }
        else
        {
            HideInteractionUI();
        }
    }

    /// <summary>
    /// 오브젝트 타입과 상태에 따른 상호작용 텍스트 반환
    /// </summary>
    /// <returns>표시할 상호작용 텍스트</returns>
    private string GetInteractionText()
    {
        // 아이템인 경우
        if (itemInstance != null)
        {
            return $"[E] {itemInstance.data.itemName} 줍기";
        }

        // IInteractable 오브젝트인 경우
        if (curInteractable != null)
        {
            // 실제 컴포넌트 타입에 따라 텍스트 결정

            // 문 상호작용
            var doorInteract = curInteractGameObject.GetComponent<DoorInteract>();
            if (doorInteract != null)
            {
                if (doorInteract.isLocked)
                    return "[E] 문 - 잠김";
                else
                {
                    // AnimProp 컴포넌트를 통해 열림/닫힘 상태 확인
                    var animPropComponent = curInteractGameObject.GetComponent<AnimProp>();
                    if (animPropComponent != null)
                    {
                        return animPropComponent.isOpen ? "[E] 문 닫기" : "[E] 문 열기";
                    }
                    return "[E] 문 상호작용";
                }
            }

            // 촛불 상호작용
            var candleInteract = curInteractGameObject.GetComponent<CandleInteract>();
            if (candleInteract != null)
            {
                // CandleInteract에 IsLit 프로퍼티가 있다면 사용
                // 현재는 private이므로 일반 텍스트, 나중에 public 프로퍼티 추가 시 수정
                return "[E] 촛불 켜기";
            }

            // 캐비넷 상호작용 (숨는 장소)
            var cabinetInteract = curInteractGameObject.GetComponent<CabinetInteractable>();
            if (cabinetInteract != null)
            {
                return cabinetInteract.isHidden ? "[E] 나오기" : "[E] 숨기";
            }

            // 일반 AnimProp (서랍 등)
            var generalAnimProp = curInteractGameObject.GetComponent<AnimProp>();
            if (generalAnimProp != null)
            {
                // 태그와 상태에 따라 적절한 텍스트 표시
                switch (curInteractGameObject.tag)
                {
                    case "Drawer":
                        return generalAnimProp.isOpen ? "[E] 서랍 닫기" : "[E] 서랍 열기";
                    case "Shelf":
                        return generalAnimProp.isOpen ? "[E] 선반 닫기" : "[E] 선반 열기";
                    default:
                        return generalAnimProp.isOpen ? "[E] 닫기" : "[E] 열기";
                }
            }

            // 기본 상호작용 텍스트
            return "[E] 상호작용";
        }

        return "";
    }

    /// <summary>
    /// 상호작용 UI 표시
    /// </summary>
    /// <param name="text">표시할 텍스트</param>
    private void ShowInteractionUI(string text)
    {
        // UI 패널이 있다면 활성화
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
 

        // 텍스트 업데이트
        if (interactionText != null)
        {
            interactionText.text = text;
        }
    
    }

    /// <summary>
    /// 상호작용 UI 숨김
    /// </summary>
    private void HideInteractionUI()
    {
        // UI 패널이 있다면 비활성화
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        // 텍스트 비우기
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    /// <summary>
    /// 상호작용 관련 모든 참조 초기화
    /// </summary>
    private void ClearInteraction()
    {
        itemInstance = null;
        curInteractGameObject = null;
        curInteractable = null;
        HideInteractionUI();
    }

    /// <summary>
    /// 상호작용 입력 처리 (E키 등의 상호작용 키가 눌렸을 때 호출)
    /// Input System의 콜백 함수
    /// </summary>
    /// <param name="context">입력 컨텍스트 정보</param>
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        // 키가 눌린 순간에만 실행
        if (context.phase == InputActionPhase.Started)
        {
            // 아이템이 감지된 경우 - 인벤토리에 추가 시도
            if (itemInstance != null)
            {
                // 인벤토리 매니저를 통해 아이템 추가 시도
                bool added = InventoryManager.Instance.AddItem(itemInstance.gameObject);

                if (added)
                {
                    Debug.Log("Item added: " + itemInstance.data.itemName);
                    // Destroy(itemInstance.gameObject); // 아이템 파괴 (현재 주석 처리됨)

                    // 아이템을 주웠으므로 참조 초기화
                    ClearInteraction();
                }
                else
                {
                    Debug.Log("Inventory Full"); // 인벤토리가 가득 참
                }
            }
            // 일반 상호작용 오브젝트인 경우
            else if (curInteractable != null)
            {
                // 잠긴 문인지 확인
                var doorInteract = curInteractGameObject.GetComponent<DoorInteract>();
                if (doorInteract != null && doorInteract.isLocked)
                {
                    Debug.Log("문이 잠겨있습니다!");
                    // DoorInteract.Interact()를 호출하면 잠긴 문 사운드가 재생됨
                    curInteractable.Interact();
                    return;
                }

                curInteractable.Interact(); // 인터페이스의 Interact 메서드 호출

                // 상호작용 후 UI 업데이트 (상태가 변경되었을 수 있음)
                UpdateInteractionUI();
            }
        }
    }
}