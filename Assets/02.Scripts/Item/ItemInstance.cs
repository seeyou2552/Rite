using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance : MonoBehaviour, IUsableItem
{
    public ItemData data;
    public GameObject sprayLightPrefab;
    public FlashScreenManager flashScreenManager; // 인스펙터에서 연결;  // 인스펙터에 할당할 화면 플래시용


    [Header("Direct References")]
    public Player player; // 인스펙터에서 연결하거나 Start에서 할당

    public void Use()
    {
        bool usedSuccessfully = true;  // 기본값 true

        switch (data.type)
        {
            case ItemType.Knife:
                UseKnife();
                break;
            case ItemType.Doll:
                UseDoll();
                break;
            case ItemType.Camera:
                UseCamera();
                SoundManager.Instance.PlaySFX("cameraflash");
                break;
            case ItemType.Spray:
                UseSpray();
                SoundManager.Instance.PlaySFX("spray");
                break;
            case ItemType.Key:
                usedSuccessfully = UseKey();  // ✅ 성공 여부 저장
                break;
        }

        // 성공했을 때만 인벤토리에서 제거
        if (data.isConsumable && usedSuccessfully)
            InventoryManager.Instance.RemoveItem(this.gameObject);
    }


    void UseKnife()
    {
        if (Player.Instance.condition.Life <= 0)
            return;

        // 애니메이션 없이 기능만 테스트 - 나중에 애니메이션 추가시 주석 해제
        // Player.Instance.animator.SetTrigger("Stab");

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
            // dropPosition이 없으면 플레이어 위치 사용
            dropPosition = Player.Instance.transform.position;
        }

        Instantiate(data.prefab, dropPosition, rotation);

        // PlayerCondition의 UseKnifeForSuicide 메서드 호출
        Player.Instance.condition.UseKnifeForSuicide();
    }

    public void UseDoll()
    {
        GameObject dollObj = Instantiate(data.prefab, Player.Instance.throwOrigin.position, Player.Instance.throwOrigin.rotation);

        // DollBehavior 컴포넌트 가져와 울음 시작
        DollBehavior dollBehavior = dollObj.GetComponent<DollBehavior>();
        if (dollBehavior != null)
        {
            dollBehavior.StartCry();
        }

        Rigidbody rb = dollObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            Vector3 throwDir = Player.Instance.throwOrigin.forward + Player.Instance.throwOrigin.up * 0.5f;
            throwDir.Normalize();
            rb.velocity = Vector3.zero;
            rb.AddForce(throwDir * 10f, ForceMode.Impulse);
        }
    }

    void UseCamera()
    {
        foreach (var hit in Physics.OverlapSphere(player.transform.position, 10f))
        {
            if (hit.CompareTag("Monster"))
                hit.transform.root.GetComponent<MonsterManager>()?.Stun(2f);
        }
        // 플래시 효과 실행
        if (flashScreenManager != null)
        {
            flashScreenManager.Flash();
        }
    }

    private void UseSpray()
    {
        Vector3 spawnPos = Player.Instance.transform.position + Player.Instance.transform.forward * 2f;
        Quaternion rot = Quaternion.identity;

        if (sprayLightPrefab != null)
        {
            GameObject light = Instantiate(sprayLightPrefab, spawnPos, rot);
            // Destroy 제거 - 스프레이 이펙트는 영구적으로 남게 의도함
        }
    }

    bool UseKey()
    {
        float range = 5f;
        Camera playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("플레이어 카메라를 찾을 수 없습니다!");
            return false;
        }

        DoorInteract door = null;
        if (Player.Instance.interaction.curInteractGameObject != null)
        { 
            door = Player.Instance.interaction.curInteractGameObject.GetComponent<DoorInteract>();
        }
            if (door != null && door.isLocked)
            {
                door.UnLock();
                SoundManager.Instance.PlaySFX("UnLockDoor");
                Debug.Log("열쇠로 문을 열었습니다!");
                return true;  // 성공적으로 열쇠 사용
            }
        

        Debug.Log("문을 바라보고 사용해주세요.");
        return false;  // 실패
    }



    void Start()
    {
        // player 참조가 없으면 자동 할당
        if (player == null)
            player = FindObjectOfType<Player>();
        if (flashScreenManager == null)
            flashScreenManager = FindObjectOfType<FlashScreenManager>();

        
    }
}
