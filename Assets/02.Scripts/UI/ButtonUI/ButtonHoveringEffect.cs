using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
/*
 작성일자 : 2025.06.17
 작성자 : 오수호
 
 버튼 위에 마우스를 올려놓으면 할당되어있는 Text의 Size가 커지고, 색이 흰색으로 바뀌면서 
 어떤 버튼위에 마우스 포인터가 올라가 있는  지 명확히 보여줍니다.
 
 또한, 씬 이동을 위한 버튼 클릭 이벤트도 모아놓았습니다.
 추가적인 클래스 추가를 해도 괜찮지만, 이 게임에서 들어가는 거의 모든 버튼에 
 이 스크립트가 추가되므로 해당 스크립트가 적용된 오브젝트를 즉시 할당하여 작업하는 것이 빠르다고 생각이되어
 편의상 이 클래스에 버튼이벤트도 모두 추가되었다는 점을 알려드립니다.
 
 
 */
public class ButtonHoveringEffect : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    
    public TextMeshProUGUI buttonText; // 색깔과 스케일이 커질 버튼 텍스트 ( 인스펙터 창에서 할당 필요 )
    public float hoverScale = 1.2f; // 커질 크기 

    private Vector3 originalScale; // 원래 크기
    private Color originalColor; // hover되지 않았을 때의 색 : gray
    private Color hoverColor; // hover되었을 때의 색  : white
    
    private bool isHovering = false; // hover되었는 지 확인할 수 있는 bool값
    
    void Start()
    {
        originalScale = buttonText.rectTransform.localScale;
        hoverColor = Color.white;
        originalColor = Color.gray;
    }

    public void ChangeButtonTextScale() // 텍스트의 크기를 바꾸는 메서드
    {
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
        buttonText.rectTransform.localScale = targetScale;
    }

    public void ChangeButtonTextColor() // 텍스트의 색을 바꾸는 메서드 
    {
        Color targetColor = isHovering ? hoverColor : originalColor;
        buttonText.color = targetColor;
    }

    public void OnPointerEnter(PointerEventData eventData) // 버튼 위에 마우스 포인터가 올라갔을 때 호출되는 메서드 
    {
        isHovering = true;
        ChangeButtonTextScale();
        ChangeButtonTextColor();
    }

    public void OnPointerExit(PointerEventData eventData) // 버튼 위에 있던 마우스 포인터가 빠져나가면 호출되는 메서드
    {
        isHovering = false;
        ChangeButtonTextScale();
        ChangeButtonTextColor();
    }
    
    /*
     *  여기서부터 각종 버튼 클릭 이벤트
     * 
     */

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene(Const.IntroScene);
    }    
    public void OnExitButtonClick()
    {
     #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; //play모드를 false로.
     #elif UNITY_WEBPLAYER
        Application.OpenURL("http://google.com"); //구글웹으로 전환
     #else
        Application.Quit(); //어플리케이션 종료
     #endif
    }
    
}

