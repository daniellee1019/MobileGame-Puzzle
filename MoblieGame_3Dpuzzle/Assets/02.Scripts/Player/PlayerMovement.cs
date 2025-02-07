using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private JoystickHandler joystick;
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody _rigidbody;
    private Animator _animator; // 애니메이터 추가

    private bool canMove = true; // 움직임 제어

    public float MoveSpeed // 런타임 속도 조정 가능
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value); // 음수 방지
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true; // 회전을 물리적으로 고정

        _animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        if (_animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트가 없습니다. 애니메이션 기능이 비활성화됩니다.");
        }
    }

    private void Start()
    {
        if (joystick == null) // 조이스틱이 할당되지 않았다면
        {
            joystick = ObjectManager.Instance.GetJoystick();
            if (joystick == null)
            {
                Debug.LogError("Joystick is not registered in ObjectManager!");
            }
        }
    }


    private void FixedUpdate()
    {
        if (joystick == null)
        {
            Debug.LogWarning("조이스틱을 할당해주세요!");
            return;
        }

        Move(joystick.InputDirection);
    }

    private void Move(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            _rigidbody.velocity = Vector3.zero;

            // 애니메이션 상태 업데이트
            if (_animator != null)
            {
                _animator.SetBool("isMove", false); // 이동 중 아님
                _animator.SetFloat("Blend", 0f); // Blend 값을 0으로 설정 (Idle 상태)
            }

            return;
        }

        Vector3 movement = new Vector3(direction.x, 0, direction.y) * moveSpeed;
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);

        RotateTowardsDirection(movement);

        // 애니메이션 상태 업데이트
        if (_animator != null)
        {
            _animator.SetBool("isMove", true); // 이동 중
            _animator.SetFloat("Blend", 1f); // Blend 값을 1로 설정 (Move 상태)
        }
    }

    private void RotateTowardsDirection(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // 플레이어 이동을 허용하는 메서드
    public void EnableMovement()
    {
        canMove = true; // 이동 가능하도록 설정
        joystick.GetComponent<JoystickHandler>().enabled = true;
    }

    // 플레이어 이동을 차단하는 메서드
    public void DisableMovement()
    {
        canMove = false; // 이동 차단
        //_rigidbody.velocity = Vector3.zero; // 즉시 정지

        joystick.GetComponent<JoystickHandler>().enabled = false;

        if (_animator != null)
        {
            _animator.SetBool("isMove", false); // 애니메이션 정지
            _animator.SetFloat("Blend", 0f); // Idle 상태로 전환
        }
    }
}