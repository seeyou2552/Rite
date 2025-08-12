using UnityEngine;

/*
 작성일자 : 2025. 06. 12
 최종 수정 : 2025. 06. 12
 작성자 : 오수호
 
 SingleTon 디자인 패턴 사용을 원하는 클래스를 만들 때, 이 추상클래스를 상속받아
 사용하면 쉽게 SingleTon클래스를 만들 수 있습니다.
 
 사용 시 주의사항)
 Awake문 사용 시 Override받아 사용
 Awake문 Override후 base.Awake();를 반드시 포함
 SingleTon으로 지정되면 하나의 씬에서 하나만 존재가능
  
 */
public abstract class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance // 외부에서 접근 시 이 프로퍼티를 통해 접근합니다.
    {
        get
        {
            if (!_instance)
            {   string singletonName = typeof(T).Name;
                _instance = new GameObject(singletonName).AddComponent<T>();
            }return _instance;
        }

        set => _instance = value;
    }

    public virtual void Awake()
    {
        if (!_instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        
    }
    
}
