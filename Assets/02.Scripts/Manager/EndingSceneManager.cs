using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/* 작성일 : 2025.06.17
   작성자 : 오수호
   
   엔딩 씬의 진행을 담당하는 로직 스크립트 
   
   처음 주인공의 dialogue ( 독백과 비슷 ) 
   =>
   메인 엔딩 씬 ( ShowSubtitle을 통해서 대사 출력 )
   => 
   마지막 주인공의 dialogue ( 독백과 비슷 )
   =>
   메인 게임씬 로드
   
   순으로 진행
   
   모든 대사는 Const클래스에서 string형태로 대사를 가져옴.
   dialogue는 타이핑 효과를 주는 것과, Text의 위치나 크기 값이 다르므로 
   메인 엔딩 씬 내에서의 출력과 dialouge를 출력하는 메서드가 서로 다름
   
   주의!
   Const의 introMainDialogues의 경우 Length가 늘어나거나 줄어들면 오류 발생
   Dialogue의 경우 대사를 추가하거나 제거해도 오류발생하지않음.
   
   엔딩씬의 경우 인트로씬의 매니저를 그대로 복사해서 재사용
   
 */


public class EndingSceneManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // UI 텍스트
    public TextMeshProUGUI indicatorText; // 키 입력 지시문
    public TextMeshProUGUI subtitleText; // 씬 진행 중 나오는 자막 텍스트
    public string[] currentDialogues; // 대사 배열
    public float typingSpeed = 0.05f;
    public PlayableDirector timeline; // 메인 타임라인
    public CinemachineVirtualCamera DarkImageCamera; // 다이얼로그 출력중에 뷰를 가려줄 버추얼카메라
    
    
    /* Dialogue 진행에 사용되는 변수 */
    private int index = 0;
    private bool isTyping = false; // 대사 배열이 타이핑 중일 때
    private bool canSkip = false; // 대사 배열이 타이핑 중인 상태에서 한 번더 입력 시 타이핑 효과 스킵
    
    /* Indicator Text의 알파 값을 변환하는 데 사용 되는 변수 */
    private bool upAlpha = true; // 키 입력 지시문의 알파 값을 조절하는 bool
    private float delayAlpha = 0;
    
    /* 마지막 다이얼로그 진행중인지 확인용 */
    private bool endIntro = false;
    
    /* 메인 인트로씬 진행 중일 때 다른 값은 출력하지않음 */
    private bool isPlaying = false;
    
    /* 메인 인트로씬 내에서의 대사 출력에 사용되는 변수 */
    private int subtitleIndex = 0;
    private Coroutine subtitleCoroutine;
    

    private void Awake() // 초기값 세팅
    {
        currentDialogues = Const.endingDialogues; 
        indicatorText.text = Const.introIndicatiorStr;
    }

    void Start() // 초기값 세팅 후 currentDialogues에 있는 string을 출력
    {
        StartCoroutine(TypeLine());
    }


    void ChageAlpha() // 인디케이터 텍스트 알파값 조절 메서드
    {
        if (indicatorText.alpha >= 0.95f && delayAlpha > Const.alphaChangeDelay)
        {
            upAlpha = false;
            delayAlpha = 0f;
        }
        else if (indicatorText.alpha <= 0.05f && delayAlpha > Const.alphaChangeDelay)
        {
            upAlpha = true;
            delayAlpha = 0f;
        }
        

    }

    void Update()
    {
        if (isPlaying) return; // TimeLine이 play중일때는 return
        
        delayAlpha += Time.deltaTime;
        /* indicator text의 alpha값을 늘렸다 줄이면서 효과를 줌 */
        ChageAlpha();
        if (upAlpha)
        {
            indicatorText.alpha += Time.deltaTime * Const.alphaChangeSpeed;
        }
        else
        {
            indicatorText.alpha -= Time.deltaTime * Const.alphaChangeSpeed;
        }
        /* indicator text의 alpha값을 늘렸다 줄이면서 효과를 줌 */
        if (Input.anyKeyDown&&isTyping) // 대사가 타이핑 중인 경우 스킵
        {
            canSkip = true;
            return;
        }
        if (Input.anyKeyDown && !isTyping) // 대사가 타이핑 중이 아닐 때(출력이 끝났을 때)
        {
            if (index < currentDialogues.Length)
            {
                StartCoroutine(TypeLine()); // 만약 다음 다이얼로그가 남아있으면 출력
            }
            else
            {
                NextDialogueInit();
                if (endIntro)
                {
                    LoadTitleScene();
                }
                timeline.Play(); // 모든 대사 후 타임라인 재생
                isPlaying = true; // 타임라인 재생 중 bool값
                gameObject.SetActive(false); // UI 종료
                DarkImageCamera.gameObject.SetActive(false);
                endIntro = true; // 마지막 다이얼로그임을 알리는 bool값
            }
        }
    }

    public void NextDialogueInit() // 마지막 다이얼로그를 출력하기 전 초기화
    {
        DarkImageCamera.gameObject.SetActive(true);
        currentDialogues = Const.endingEndDialougues; 
        index = 0;
        dialogueText.text = string.Empty;
        indicatorText.alpha = 0f;
    }

    public void LoadTitleScene() // 타이틀씬을 로드하는 메서드
    {
        SceneManager.LoadScene(Const.TitleScene);
    }



    public void StartTypeLineCorutine()
    {
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() // 대사 진행
    {
        isTyping = true;
        dialogueText.text = string.Empty;
        canSkip = false;
        foreach (char letter in currentDialogues[index].ToCharArray())
        {
            if (canSkip)
            {
                dialogueText.text = currentDialogues[index];
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        canSkip = false;
        index++;
    }
  
    public void TurnOffisPlaying()
    {
        isPlaying = false;
    }



    
}