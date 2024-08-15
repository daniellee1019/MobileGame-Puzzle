using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    void Interact(GameObject player); // �÷��̾���� ��ȣ�ۿ��� ó��
}

public class PlayerInteractionManager : MonoBehaviour
{
    public float interactionRadius = 2f; // ��ȣ�ۿ� ���� �ݰ�
    public Button rideButton; // UI ��ư
    private IInteractable currentInteractable;

    void Start()
    {
        if (rideButton != null)
        {
            rideButton.gameObject.SetActive(false); // ó���� ��ư ��Ȱ��ȭ
            rideButton.onClick.AddListener(OnRideButtonClick);
        }
    }

    void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                rideButton.gameObject.SetActive(true); // �ͷ� �ݰ� ���� ������ ��ư Ȱ��ȭ
                return;
            }
        }

        // �ݰ� ���� ��ȣ�ۿ� ������ ������Ʈ�� ������ ��ư ��Ȱ��ȭ
        rideButton.gameObject.SetActive(false);
    }

    private void OnRideButtonClick()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }
}
