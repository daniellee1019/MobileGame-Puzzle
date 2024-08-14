using UnityEngine;

namespace Terresquall
{
    public class PlayerMovement : MonoBehaviour
    {

        public float maxSpeed = 5f; // �÷��̾��� �ִ� �ӵ�
        private Rigidbody rb;
        private VirtualJoystick joystick;

        void Start()
        {
            // Rigidbody�� �����ɴϴ�. 3D ���ӿ����� Rigidbody�� ����ؾ� �մϴ�.
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }

            // ���� �ִ� VirtualJoystick�� ã�� �����մϴ�.
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
            // ���̽�ƽ �Է��� �޾� �̵� ����� �ӵ��� ����մϴ�.
            Vector2 input = joystick.GetAxis();
            float speedFactor = input.magnitude; // ���̽�ƽ �̵� �Ÿ� ������ ���� �ӵ� ����
            Vector3 moveDirection = new Vector3(input.x, 0, input.y) * speedFactor * maxSpeed;

            // Rigidbody�� ����Ͽ� �÷��̾ �̵���ŵ�ϴ�.
            rb.velocity = moveDirection;
        }
    }
}
