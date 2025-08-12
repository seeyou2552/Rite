/*
작성일자 : 2025.06.16
작성자 : 오수호 

const 값을 저장하는 클래스 

 */

public class Const
{
    /* 씬 이름 const 값 */
    public static readonly string IntroScene = "IntroScene";
    public static readonly string TitleScene = "TitleScene";
    public static readonly string MainGameScene = "Main";
    public static readonly string GameOverScene = "GameOverScene";
    public static readonly string EndingScene = "EndingScene";
    
    
    /* IntroScene 출력 대사 const 값*/
    public static readonly string[] introDialogues = new string[]
    {
        "나는 오컬트적인 요소에 관심이 많은 편이였다.",

        "꼭 그런 걸 진심으로 믿는 건 아니었다.",

        "단지, 지루한 일상 속에서 잠시나마 판타지 같은 분위기를 느낄 수 있다는 게 좋았다.",

        "나와 같은 취향을 가진 친구들과 어울려 주술을 흉내 내보는 것도, 하나의 재미였다.",

        "이번에도… 그냥 그랬을 뿐이다.",

        "그러니까… 이건 내 잘못이 아니야."

    };
    public static readonly string[] introEndDialogues = new string[]
    {
        "나는 열심히 주문을 외웠다.",

        "단지 그뿐이였을텐데도 나의 정신이 점점 아득해져감을 느꼈다.",

        "이내 내가 주문을 외우고 있는 지, 서 있기는 한건 지 조차도 알 수 없을 때 쯤",

        "나는 확실하게 정신을 잃었다.",

    };    
    
    public static readonly string[] endingDialogues = new string[]
    {
        "모든 부적을 모으고 스스로의 목을 찌른 그 순간",

        "나는 이 공간으로 떨어질 때의 아득함을 다시금 느꼈다.",

        "나는 직감적으로 원래의 세상으로 돌아갈 수 있다는 확신을 느꼈다.",

    };    
    
    public static readonly string[] endingEndDialougues = new string[]
    {
        "원래 세상으로 돌아왔다는 안도감은 잠시 뿐",

        "눈을 뜨자마자 보이는 것은 함께 다른 세계에 떨어졌던 친구들의 시신뿐이였다.",

        "내 잘못이 아니라고 되뇌이고 되뇌어도",
        
        "죄책감은 영원히 나의 정신을 갉아먹을 것이다.",

    };
    
    
    
    
    public static readonly string[] introMainDialogues = new string[]
    {
        "좋아. 칼 5개랑 부적 5개 모두 있네. 이제 각자 부적에다가 피 한방울 씩 뿌려.",

        "칼 하나씩 챙기고, 의식 중에 눈뜨지마.",

        "이제 족자 앞으로 와.",

        "족자에 부적도 다 붙였고..",
        
        "그럼 이제 시작한다."

    };

    public static readonly string introIndicatiorStr = "아무 키나 눌러 진행하세요..";
    public static readonly string introSkipIndicatiorIsPressed = "Skip";
    public static readonly string introSkipIndicatiorIsCancled = "ESC";
    
    /* IntroScene 알파 스피드 조절 값 */
    public static readonly int alphaChangeSpeed = 1;
    public static readonly float alphaChangeDelay = 1f;
    
    /* SoundClip의 string 값 */
    public static readonly string BloodSquish = "BloodSquish";
    
    public static readonly string GameOverPanel = "GameOverPanel";
    
    /* 튜토리얼 */
    public static readonly string TutorialText = "이동 : WASD\n\n앉기 : Ctrl\n\n상호작용 : E\n\n달리기 : Shift\n\n아이템 사용 : 좌클릭\n\n아이템 선택  : 휠 돌리기\n\n아이템 버리기 : G\n\n플래시 라이트 : F\n\n튜토리얼 접기 : K\n";
    public static readonly string TutorialFoldText = "튜토리얼 열기 : K";

}
