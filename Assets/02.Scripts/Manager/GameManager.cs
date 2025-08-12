using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    private int relicCount = 0;             // 현재 모은 부적 개수
    private int requiredRelics = 5;         // 클리어 조건 부적 개수
    
    // 추가 초기화 필요 시 여기에
    public override void Awake()
    {
        base.Awake();

    }
    

    // 부적 개수 증가 (InventoryManager에서 호출)
    public void IncreaseRelicCount()
    {
        relicCount++;
        SoundManager.Instance.PlaySFX("GetRelic");
        Debug.Log($"부적 개수: {relicCount}");
    }

    public void InitMainScene()
    {
        Player.Instance.StartPosition = GameObject.Find("StartPosition").GetComponent<Transform>();

        Player.Instance.condition.Life = 3;
        Player.Instance.controller.DisableMovementFor(0.3f);
        Player.Instance.transform.position = Player.Instance.StartPosition.position;
        Player.Instance.transform.rotation = Quaternion.Euler(0,0,0);
        // 카메라 리셋-> DeathEffect로 바뀐 피벗과 상태 복구
        var ctrl = Player.Instance.controller;
        ctrl.currentState = PlayerState.Walking;
        ctrl.isDead = false;
        Player.Instance.condition.isDead = false;
        // 피벗 회전 원위치
        if (ctrl.deadMotionPivot != null)
            ctrl.deadMotionPivot.rotation = Quaternion.Euler(0,0,0);
        // 카메라 컨테이너 각도 원위치
        if (ctrl.cameraContainer != null)
            ctrl.cameraContainer.rotation = Quaternion.Euler(0,0,0);
        // 상태를 걷기 모드로
        GameObject interactionPanel = GameObject.Find("InteractionPanel");
        if (interactionPanel != null)
        {
            Player.Instance.interaction.interactionUI = interactionPanel;

            // InteractionText는 하위 오브젝트로 존재하므로 GetComponentInChildren 사용
            TextMeshProUGUI interactionText = interactionPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (interactionText != null)
            {
                Player.Instance.interaction.interactionText = interactionText;
            }
            else
            {
                Debug.LogWarning("InteractionText (TextMeshProUGUI)가 InteractionPanel 하위에서 발견되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("InteractionPanel을 찾을 수 없습니다. 경로를 다시 확인하세요.");
        }
    }


    // 부적 개수 반환
    public int GetRelicCount()
    {
        return relicCount;
    }

    // 부적 5개 이상 모았는지 체크
    public bool HasCollectedAllRelics()
    {
        return relicCount >= requiredRelics;
    }

    // 플레이어가 칼로 자살 시 호출 (클리어 조건 판단)
    public void TryClearGame()
    {
        if (HasCollectedAllRelics())
        {
            Debug.Log("게임 클리어! 부적 5개 모으고 칼로 자살 성공");
            // 여기에 클리어 처리 (씬 전환, UI 표시 등)
        }
        else
        {
            Debug.Log("아직 부적이 부족합니다.");
        }
    }
    public void ClearGame()
    {
        SceneManager.LoadScene(Const.EndingScene);
    }

    IEnumerator GameOverCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(Const.GameOverScene);
    }

    public void GameOver(float time)
    {
        StartCoroutine(GameOverCoroutine(time));
        // 게임 오버 처리
    }
}

