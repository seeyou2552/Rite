using System;
using System.Collections;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public enum PlayerState
{
    Standing,
    Hiding,
    Walking,
    Running,
    Sitting,
    Dead
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float sitSpeed = 2f;

    private float lastUseTime = -1f;
    private float useCooldown = 0.25f; // 0.25초 쿨타임

    public PlayerState currentState = PlayerState.Standing;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canMove = true;
    public bool isDead = false;
    private float currentSpeed;

    public Transform cameraContainer;
    public Transform deadMotionPivot;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mounseDelta;
    
    public float _moveSoundDelay = 0.6f;
    private float _lastMoveSoundPlayTime = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // PlayerInput 컴포넌트에서 Interact 액션에 이벤트 연결
        var playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (moveInput.magnitude == 0 && currentState != PlayerState.Dead && currentState != PlayerState.Sitting && currentState != PlayerState.Hiding)
        {
            currentState = PlayerState.Standing;
        }
    }

    void FixedUpdate()
    {
        if (canMove)
            Move();
        _lastMoveSoundPlayTime += Time.deltaTime;
        if (moveInput.magnitude > 0 && currentState != PlayerState.Dead && currentState != PlayerState.Sitting)
        {
            PlayWalkSound();
        }

        if (currentState == PlayerState.Running)
        {
            _moveSoundDelay = 0.3f;
        }
        else
        {
            _moveSoundDelay = 0.6f;
        }
    }
    private void LateUpdate()
    {
        if(currentState != PlayerState.Dead)
        {
              CameraLook();
        }
    }

    private void Move()
    {
        
        if (currentState == PlayerState.Dead) return;
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 move = camRight * inputDirection.x + camForward * inputDirection.z;

        switch (currentState)
        {
            case PlayerState.Sitting:
                currentSpeed = sitSpeed;
                break;
            case PlayerState.Running:
                currentSpeed = runSpeed;
                break;
            default:
                currentSpeed = walkSpeed;
                break;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    public void PlayWalkSound()
    {
        if(currentState == PlayerState.Sitting)return;
        if (_lastMoveSoundPlayTime > _moveSoundDelay)
        {
            StringBuilder sb = new StringBuilder();
         
            Ray ray = new Ray(transform.position, Vector3.down);
            if (isGrounded)
            {    
                sb.Append("FootstepsWood");
            }
 
            sb.Append(Random.Range(1, 5).ToString());
            SoundManager.Instance.PlaySFX(sb.ToString());
            _lastMoveSoundPlayTime = 0f;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(currentState == PlayerState.Sitting) return;
        if (currentState == PlayerState.Standing)
        {
            currentState = PlayerState.Walking;
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && currentState != PlayerState.Sitting && currentState != PlayerState.Hiding )
        {
            if (currentState != PlayerState.Running)
            {
                currentState = PlayerState.Running;
            }
        }
        else if (context.canceled && currentState == PlayerState.Running)
        {
            currentState = PlayerState.Walking;
        }
    }
    public void OnSit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentState != PlayerState.Sitting)
            {
                SitDown();
            }
            else if (currentState == PlayerState.Sitting)
            {
 
                StartStandUp();
            }
        }
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mounseDelta = context.ReadValue<Vector2>();
    }


    // 휠 입력 처리: InventoryManager의 ScrollSelectItem 호출로 변경
    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scroll = context.ReadValue<Vector2>().y;
            if (scroll > 0f)
            {
                InventoryManager.Instance.ScrollSelectItem(-1); // 위로 휠 -> 이전 아이템
            }
            else if (scroll < 0f)
            {
                InventoryManager.Instance.ScrollSelectItem(1); // 아래로 휠 -> 다음 아이템
            }
            Debug.Log(scroll);
        }
       
    }
    public void OnFlash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Flashlight.Instance.Toggle();
        }
    }

    // 좌클릭(Interact) 입력 받으면 아이템 사용 호출
    public void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        if (Time.time - lastUseTime < useCooldown)
            return;

        lastUseTime = Time.time;

        Debug.Log("OnUseItemPerformed called at time: " + Time.time);
        InventoryManager.Instance.UseSelectedItem();
    }

    public void OnDropItemPerformed(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time - lastUseTime > useCooldown)
        {
            lastUseTime = Time.time;
            Debug.Log("Drop item input performed with cooldown");
            InventoryManager.Instance.DropSelectedItem();
        }
    }

    void CameraLook()
    {
        camCurXRot += mounseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        Flashlight.Instance.flashlightLight.transform.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, mounseDelta.x * lookSensitivity, 0);
    }

    public void SitDown()
    {
        currentState = PlayerState.Sitting;
        controller.height = 0.5f;
    }
    public void StartStandUp()
    {
        currentState = PlayerState.Standing;
        StartCoroutine(ChangeHeight(0.5f, 3f, 0.3f));
    }

    IEnumerator ChangeHeight(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float height = Mathf.Lerp(from, to, t);
            controller.height = height;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void DisableMovementFor(float seconds)
    {
        StartCoroutine(DisableMoveCoroutine(seconds));
    }

    IEnumerator DisableMoveCoroutine(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    public void DieEffect()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DeathEffectRoutine());
    }
    IEnumerator DeathEffectRoutine()
    {
        // 플레이어 조작 제한
        currentState = PlayerState.Dead;

        Quaternion deadMotionPivotStartRot = deadMotionPivot.localRotation;
        Quaternion cameraContainerStartRot = cameraContainer.localRotation;
        Quaternion targetRot = Quaternion.Euler(90f, 0f, 0f); // 앞으로 쓰러지는 느낌
        Quaternion targetRotForZero = Quaternion.Euler(0f, 0f, 0f);
        float duration = 1.2f;
        float t = 0f;
        Player.Instance.effects.StartSuicideEffect();
        while (t < duration)
        {
            t += Time.deltaTime;
            float speedCurve = Mathf.Pow(t / duration, 3f); // 가속 느낌
            cameraContainer.localRotation = Quaternion.Slerp(cameraContainerStartRot, targetRotForZero, speedCurve);
            yield return null;
        }
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float speedCurve = Mathf.Pow(t / duration, 3f);
            deadMotionPivot.localRotation = Quaternion.Slerp(deadMotionPivotStartRot, targetRot, speedCurve);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Player.Instance.effects.EndSuicideEffect();
    }
}
