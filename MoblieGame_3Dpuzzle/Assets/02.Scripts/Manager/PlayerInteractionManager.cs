using UnityEngine;
using Cinemachine;

public interface IInteractable
{
    void Interact(GameObject player); // 플레이어와의 상호작용을 처리
}

public class PlayerInteractionManager : MonoBehaviour
{
    public float interactionDistance = 2f; // 상호작용 가능 거리
    private IInteractable currentInteractable;
    private Camera activeCamera;
    private bool isTouching = false;

    void Start()
    {
        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (cinemachineBrain != null)
        {
            activeCamera = cinemachineBrain.OutputCamera;
        }
        else
        {
            activeCamera = Camera.main;
        }
    }

    void Update()
    {
        DetectInteractable();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && currentInteractable != null)
            {
                isTouching = !isTouching;
                currentInteractable.Interact(gameObject);
            }
        }
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = activeCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    currentInteractable = interactable;
                }
            }
        }
    }
}
