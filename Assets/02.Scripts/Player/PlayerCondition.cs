using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 사용을 위해 추가

public enum DeathType
{
    Suicide,
    TooManySuicide,
    Runner,
    Sniffer,
    Laugh
}

public class PlayerCondition : MonoBehaviour
{
    public PlayerRandomSpawn respawner;
    public float Delay = 3.5f;

    [SerializeField] private float staminaRecovery = 15;
    [SerializeField] private float runStamina = 20f;
    public float maxStamina = 100f;
    public float stamina = 100f;
    public int Life = 3;

    [Header("Life UI Images")]
    [SerializeField] private Image[] lifeImages = new Image[3]; // 생명 이미지 배열 (Inspector에서 할당)

    public bool isChased = false;
    public int chaseCount = 0;
    private bool prevChaseState = false;
    public bool isHunted = false;

    private float rechaseDelay = 0f;
    
    public bool isDead = false;

    private bool uiAssigned = false;

    public LayerMask layerMask;

    public SoundSource breathSound;

    private void Awake()
    {
        stamina = maxStamina;
        InitializeLifeUI(); // 생명 UI 초기화
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 후 생명 이미지 다시 연결
    }



    private void FixedUpdate()
    {
        if (Player.Instance.controller.currentState == PlayerState.Running)
        {
            UseStamina();
        }
        else
        {
            RecoveryStamina();
        }

        if (stamina < 50)
        {
            PlaySoundBreath();
        }
        else if (stamina >= 70)
        {
            StopSoundBreath();
        }
    }

    public void PlaySoundBreath()
    {
        if (breathSound ==null || !breathSound.gameObject.activeInHierarchy)
        {
            breathSound = SoundManager.Instance.PlaySfxReturnSoundSource("breathingFast");
        }
    }

    public void StopSoundBreath()
    {
        if (breathSound != null)
        {
            breathSound.Disable();
            breathSound = null;
        }
    }

    // 생명 UI 초기화 (게임 시작 시 모든 이미지 활성화)
    private void InitializeLifeUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i] != null)
            {
                lifeImages[i].gameObject.SetActive(true);
            }
        }
    }

    // 생명이 깎일 때 UI 업데이트
    private void UpdateLifeUI()
    {
        // Life 값에 따라 이미지 비활성화
        // Life가 3이면 모든 이미지 켜짐, 2면 하나 꺼짐, 1이면 두개 꺼짐, 0이면 모두 꺼짐
        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i] != null)
            {
                lifeImages[i].gameObject.SetActive(i < Life);
            }
        }
    }

    public void ChangeChaseState()
    {
        if (chaseCount > 0)
        {
            isChased = true;
            rechaseDelay = 0f;
        }
        else
        {
            if (rechaseDelay > 5f)
            {
                isChased = false;
                prevChaseState = false;
            }
            
        }
    }

    private void Update()
    {
        rechaseDelay += Time.deltaTime;
        ChangeChaseState();
        SetChaseState();
        

        if (isDead)
        {
            Player.Instance.controller.DieEffect();
        }
        
    }

    public void UseStamina()
    {
        stamina = Mathf.Max(0, stamina);
        stamina -= runStamina * Time.deltaTime;
        if (stamina <= 0) // 스테미나가 0이되면 강제로 걷기로 변경
        {
            Player.Instance.controller.currentState = PlayerState.Walking;
        }
    }

    public void RecoveryStamina()
    {
        stamina = Mathf.Min(maxStamina, stamina);
        stamina += staminaRecovery * Time.deltaTime;
    }

    // 자살 칼 사용하는 함수 (미사용 중)
    public void UseKnifeForSuicide()
    {
        if (GameManager.Instance.HasCollectedAllRelics() && IsInEscapeZone())
        {
            GameManager.Instance.ClearGame();
        }
        else
        {
            Suicide();
            Debug.Log("칼 사용해서 죽음 : Suicide메서드 호출");
        }
    }

    // 플레이어가 탈출지역에 있는지 확인하는 함수
    private bool IsInEscapeZone()
    {
        // 1인칭에서는 플레이어 위치만큼 거리 계산
        Vector3 rayStart = transform.position;
        RaycastHit hit;

        // 거리를 10f 정도로 넉넉히 설정 (플레이어 키 + 여유분)
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f,layerMask))
        {
            Debug.Log("레이캐스트 맞은 오브젝트: " + hit.collider.name);
            Debug.Log("거리: " + hit.distance);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("EscapeZone"))
            {
                Debug.Log("탈출지역에 있음");
                return true;
            }
        }
        else
        {
            Debug.Log("레이캐스트가 바닥에 닿지 않음");
        }

        Debug.Log("탈출지역에 없음");
        return false;
    }

    public void Die(DeathType deathType) // 사망 시 불러오는 쪽에 죽음의 종류를 함수
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"플레이어 사망 : {deathType}");
    }

    [ContextMenu("자살")]
    public void Suicide() // 자살 시 불러오는 쪽에 처리용 함수
    {
        Life--;
        Debug.Log($"자살 시도 후 남은 생명: {Life}");

        // 생명 UI 업데이트
        UpdateLifeUI();

        if (Life == 0)
        {
            Debug.Log("생명 모두 소진 : 게임오버");
            Die(DeathType.TooManySuicide);
            GameManager.Instance.GameOver(Delay);  // 게임오버 처리
        }
        else
        {
            Die(DeathType.Suicide);
            StartCoroutine(RespawnAfterDelay(Delay));
        }
    }

    private IEnumerator RespawnAfterDelay(float time)
    {
        // 대기
        yield return new WaitForSeconds(time);

        // 리스폰 호출
        if (respawner == null)
        {
            respawner = GameObject.Find("PlayerRandomSpawn").GetComponent<PlayerRandomSpawn>();
        }
        
        respawner.RespawnPlayer();
    
        // 카메라 리셋-> DeathEffect로 바뀐 피벗과 상태 복구
        var ctrl = Player.Instance.controller;
        // 피벗 회전 원위치
        if (ctrl.deadMotionPivot != null)
            ctrl.deadMotionPivot.localRotation = Quaternion.identity;
        // 카메라 컨테이너 각도 원위치
        if (ctrl.cameraContainer != null)
            ctrl.cameraContainer.localEulerAngles = Vector3.zero;
        // 상태를 걷기 모드로
        ctrl.currentState = PlayerState.Walking;
        ctrl.isDead = false;

        // 플레이어 사망 플래그 해제
        isDead = false;
    }

    public void SetChaseState()
    {
        if (isChased && !prevChaseState)
        {
            prevChaseState = true;
            Player.Instance.effects.PlayChaseBurstEffect();  // 0.5초 버스트 + 그레인
        }
        else if (!isChased)
        {
            Player.Instance.effects.EndChaseEffect();
        }
    }
    private void AssignLifeImages()
    {
        lifeImages = new Image[3];

        var raw = GameObject.Find("raw")?.transform;
        if (raw == null)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            var child = raw.Find($"LifeImage{i}");
            if (child != null)
            {
                lifeImages[i] = child.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning($"LifeImage{i}를 찾을 수 없습니다.");
            }
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignLifeImages();   // 씬 전환 후 UI 다시 할당
        InitializeLifeUI();   // UI 초기화
        uiAssigned = true;
    }
}