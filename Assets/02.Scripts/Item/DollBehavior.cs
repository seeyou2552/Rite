using System.Collections;
using UnityEngine;

public class DollBehavior : MonoBehaviour
{
    // 울음 지속 시간 (초)
    public float cryDuration = 10f;
    // 몬스터를 끌어당기는 반경
    public float attractRadius = 15f;
    // Inspector에서 울음소리 클립을 할당할 변수
    public AudioClip cryClip;

    // 바닥에 닿고 일정 시간 후 울음 시작을 위한 딜레이 시간 (초)
    public float delayBeforeCry = 4f;

    // 울음 시작 여부를 중복 방지하기 위한 플래그
    private bool hasStartedCry = false;

    public void StartCry()
    {
        if (!hasStartedCry)
        {
            hasStartedCry = true;
            StartCoroutine(StartCryAfterDelay());
        }
    }
    // 바닥과 충돌 감지
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 바닥 태그("Ground")이고 울음이 아직 시작 안 했으면
        if (!hasStartedCry && collision.gameObject.CompareTag("Obstacle"))
        {
            hasStartedCry = true;
            // 딜레이 후 울음 시작 코루틴 실행
            StartCoroutine(StartCryAfterDelay());
        }
    }

    // 딜레이 후 울음 시작
    IEnumerator StartCryAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeCry);
        StartCoroutine(CryRoutine());
    }

    // 울음 소리 재생 및 몬스터 유인 반복
    IEnumerator CryRoutine()
    {
        // 울음 소리 클립이 할당되어 있으면 한 번 재생
        if (cryClip != null)
        {
            AudioSource.PlayClipAtPoint(cryClip, transform.position);
        }

        float elapsed = 0f;
        // 울음 지속 시간 동안 매초 몬스터 유인 처리
        while (elapsed < cryDuration)
        {
            AttractMonsters();
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        // 울음이 끝나면 오브젝트 파괴
        Destroy(gameObject);
    }

    // 반경 내 몬스터를 유인하는 함수
    void AttractMonsters()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attractRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster"))
            {
                // 몬스터의 AttractTo 메서드를 호출해서 위치 유인
              

                // MonsterController monsterController2 = hit.transform.GetComponent<MonsterController>();
                // Debug.Log(hit.gameObject.name + " : " + monsterController2.gameObject.name);
                MonsterController monsterController3 = hit.transform.root.GetComponent<MonsterController>();
                Debug.Log(hit.gameObject.name + " : " + monsterController3.gameObject.name);
                MonsterType type = hit.transform.root.GetComponent<MonsterController>().monsterType;
                if (type == MonsterType.Laugher)
                {
                    hit.transform.root.GetComponent<MonsterManager>()?.AttractTo(transform.position, cryDuration);
                }
            }
        }
    }
}
