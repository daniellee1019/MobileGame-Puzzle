using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    void Interact(GameObject player); // 플레이어와의 상호작용을 처리
}

public class PlayerInteractionManager : MonoBehaviour
{
    public float interactionRadius = 2f; // 상호작용 가능 반경
    public Button rideButton; // UI 버튼

    private IInteractable currentInteractable;
    private PlayerMovement player;

    void Start()
    {
        if (rideButton != null)
        {
            rideButton.gameObject.SetActive(false); // 처음에 버튼 비활성화
            rideButton.onClick.AddListener(OnRideButtonClick);
        }

        if (player == null) // 조이스틱이 할당되지 않았다면
        {
            player = ObjectManager.Instance.GetPlayer();
            if (player == null)
            {
                Debug.LogError("Joystick is not registered in ObjectManager!");
            }
        }
    }

    void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, interactionRadius);

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                rideButton.gameObject.SetActive(true); // 터렛 반경 내에 들어오면 버튼 활성화
                return;
            }
        }

        // 반경 내에 상호작용 가능한 오브젝트가 없으면 버튼 비활성화
        rideButton.gameObject.SetActive(false);
    }

    private void OnRideButtonClick()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(player.gameObject);
        }
    }
}
