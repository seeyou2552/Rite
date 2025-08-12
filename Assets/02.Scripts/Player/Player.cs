using System;
using Cinemachine;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : SingleTon<Player> //  상속받기
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Interaction interaction;
    public CameraEffects effects;
    public Action addItem;
    public Transform dropPosition;
    public Transform throwOrigin; //  던지기 위치 추가
    public Animator animator;     //  애니메이터 참조
    public CinemachineBrain cinemachineBrain;
    public Transform StartPosition;

    [SerializeField] private float interactDistance = 2f;  // 상호작용 최대 거리

    public override void Awake()
    {
        base.Awake(); // 싱글톤 초기화
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        effects = GetComponent<CameraEffects>();
        animator = GetComponent<Animator>();          //  Animator 설정
        interaction = GetComponent<Interaction>();
        cinemachineBrain.enabled = false;
        throwOrigin ??= transform;                    //  없을 때 예외 방지
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Teleport(Vector3 position) //  텔레포트 함수 직접 구현
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.position = position;
            cc.enabled = true;
        }
        else
        {
            transform.position = position;
        }
    }
 
}
