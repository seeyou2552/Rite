using UnityEngine;

// 너가 만든 싱글톤 클래스를 상속함으로써 자동으로 인스턴스화됨
public class Flashlight : SingleTon<Flashlight>
{
    public Light flashlightLight;
    private bool isOn = false;
    
    

    // 손전등 토글 (켜기/끄기)
    public void Toggle()
    {
        isOn = !isOn;
        flashlightLight.enabled = isOn;
    }

    // 손전등 켜기
    public void TurnOn()
    {
        isOn = true;
        flashlightLight.enabled = true;
    }

    // 손전등 끄기
    public void TurnOff()
    {
        isOn = false;
        flashlightLight.enabled = false;
    }

    // 너가 만든 싱글톤 구조에선 Awake override 필요함!
    public override void Awake()
    {
        base.Awake(); // 반드시 호출
        // 추가 초기화가 필요하다면 여기서 진행
    }
}
