using UnityEngine;

namespace Terresquall
{
    public class PlayerMovement : MonoBehaviour
    {

        public float maxSpeed = 5f; // 플레이어의 최대 속도
        private Rigidbody rb;
        private VirtualJoystick joystick;

        void Start()
        {
            // Rigidbody를 가져옵니다. 3D 게임에서는 Rigidbody를 사용해야 합니다.
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }

            // 씬에 있는 VirtualJoystick을 찾아 연결합니다.
            joystick = FindObjectOfType<VirtualJoystick>();
            if (joystick == null)
            {
                Debug.LogError("No VirtualJoystick found in the scene. Please add one.");
            }
        }

        void Update()
        {
            if (joystick != null)
            {
                MovePlayer();
            }
        }

        private void MovePlayer()
        {
            // 조이스틱 입력을 받아 이동 방향과 속도를 계산합니다.
            Vector2 input = joystick.GetAxis();
            float speedFactor = input.magnitude; // 조이스틱 이동 거리 비율에 따라 속도 조절
            Vector3 moveDirection = new Vector3(input.x, 0, input.y) * speedFactor * maxSpeed;

            // Rigidbody를 사용하여 플레이어를 이동시킵니다.
            rb.velocity = moveDirection;
        }
    }
}
