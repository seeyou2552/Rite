using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine;

public enum MonsterType
{
    Sniffer,
    Laugher,
    Runner,
    Ghost
}
public class MonsterController : MonoBehaviour
{
    [Header("몬스터 선택")]
    public MonsterType monsterType;
    MonsterManager monster;
    public PlayableDirector timelineDirector;
    public CinemachineVirtualCamera cam;

    private SoundSource idleSound = null;
    private SoundSource runSound = null;


    void Start()
    {
        monster = GetComponent<MonsterManager>();
        monster.findPlayer = FindPlayer;
        cam = transform.Find("MonsterCam").GetComponent<CinemachineVirtualCamera>();
        timelineDirector = transform.Find("MonsterCam").GetComponent<PlayableDirector>();
        if (monsterType == MonsterType.Runner) monster.running = true;
        else if (monsterType == MonsterType.Laugher) monster.standing = true;
    }

    bool FindPlayer() // Distance 및 시야각 메서드를 통한 플레이어 탐색 
    {
        switch (monsterType)
        {
            case MonsterType.Runner:
                return monster.playerDistance < monster.detectDistance && IsPlayerInFieldOfView();
            case MonsterType.Ghost:
                return monster.playerDistance < monster.detectDistance && IsPlayerInFieldOfView();
            case MonsterType.Sniffer:
                return monster.playerDistance < monster.detectDistance;

            case MonsterType.Laugher:
                return ((monster.playerDistance < monster.addedDistance && (Player.Instance.controller.currentState == PlayerState.Walking
                        ||  Player.Instance.controller.currentState == PlayerState.Running))
                        || (monster.playerDistance < monster.detectDistance && IsPlayerInFieldOfView()));
        }
        return false;
    }


    bool IsPlayerInFieldOfView() // 시야각 플레이어 탐색
    {
        Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);


        if (angle < monster.fieldOfView * 0.5)
        {
            Ray ray = new Ray(transform.position, directionToPlayer);
            Debug.DrawRay(transform.position, directionToPlayer, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, directionToPlayer.magnitude, LayerMask.GetMask("Player", "Obstacle")))
            {
                if (hit.collider.gameObject.CompareTag("Untagged"))
                {
                    return false;
                }
                else if (hit.collider.gameObject.CompareTag("Player"))
                {
                    if (Player.Instance.controller.currentState == PlayerState.Sitting)
                    {
                        Debug.Log("숨어도 죽음");
                        DieAnimation();
                    }
                    return true;
                }
            }
        }

        return false;
    }

    public void DieAnimation()
    {
        switch (monsterType)
        {
            case MonsterType.Runner:
                cam.Priority = 2;   
                timelineDirector.Play();
                StartCoroutine(GameOver(1f));
                break;
            case MonsterType.Ghost:
                cam.Priority = 2;
                StartCoroutine(GameOver(1f));
                break;
            case MonsterType.Sniffer:
                cam.Priority = 2;
                timelineDirector.Play();
                StartCoroutine(GameOver(3f));
                break;
            case MonsterType.Laugher:
                cam.Priority = 2;
                timelineDirector.Play();
                StartCoroutine(GameOver(1.8f));
                break;
        }
    }

    public void IdleSound()
    {
        switch (monsterType)
        {
            case MonsterType.Runner:
                if (runSound != null)
                {
                    runSound.Disable();
                    runSound = null;
                }
                break;
            case MonsterType.Ghost:
                break;
            case MonsterType.Sniffer:
                if (idleSound == null || !idleSound.gameObject.activeInHierarchy)
                {
                    idleSound = SoundManager.Instance.PlaySFXReturnSoundSource("sniffing",transform.position,transform);
                }
                break;
            case MonsterType.Laugher:
                if (idleSound == null || !idleSound.gameObject.activeInHierarchy)
                {
                    idleSound = SoundManager.Instance.PlaySFXReturnSoundSource("laughtersound",transform.position,transform);
                }
                break;
        }
    }
    
    public void WalkSound()
    {
        switch (monsterType)
        {
            case MonsterType.Runner:
                if (runSound == null || !runSound.gameObject.activeInHierarchy)
                {
                    runSound = SoundManager.Instance.PlaySFXReturnSoundSource("runnersound",transform.position,transform);
                }
                break;
            case MonsterType.Ghost:
                break;
            case MonsterType.Sniffer:
                break;
            case MonsterType.Laugher:
                break;
        }
    }
    
    public void RunSound()
    {
        switch (monsterType)
        {
            case MonsterType.Runner:
                if (runSound == null || !runSound.gameObject.activeInHierarchy)
                {
                    runSound = SoundManager.Instance.PlaySFXReturnSoundSource("runnersound",transform.position,transform);
                }
                break;
            case MonsterType.Ghost:
                break;
            case MonsterType.Sniffer:
                if (idleSound != null)
                {
                    idleSound.Disable();
                    idleSound = null;
                }
                break;
            case MonsterType.Laugher:
                if (idleSound != null)
                {
                    idleSound.Disable();
                    idleSound = null;
                }
                break;
        }
    }

    IEnumerator GameOver(float time)
    {
        if (!Player.Instance.condition.isHunted)
        {
            Player.Instance.condition.isHunted = true;   
            SoundManager.Instance.StopBGM();
            GameManager.Instance.GameOver(time);
            yield return new WaitForSeconds(time);
            SoundManager.Instance.PlayBGM("die_player", false);
        }
    }
}
