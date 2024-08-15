using System.Collections.Generic;
using UnityEngine;

public class RotationGizmo : MonoBehaviour
{
    public List<Transform> targetObjects = new List<Transform>(); // 회전시킬 대상 오브젝트 리스트
    public float rotationSpeed = 100f; // 회전 속도

    private Vector2 previousTouchPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.position - previousTouchPosition;
                float rotX = touchDelta.x * rotationSpeed * Mathf.Deg2Rad;

                foreach (Transform targetObject in targetObjects)
                {
                    if (targetObject != null)
                    {
                        // 수평 회전만 적용 (Y축 회전)
                        targetObject.Rotate(Vector3.up, -rotX, Space.World);
                    }
                }

                previousTouchPosition = touch.position;
            }
        }
    }

    public void AddTarget(Transform target)
    {
        if (!targetObjects.Contains(target))
        {
            targetObjects.Add(target);
        }
    }

    public void ClearTargets()
    {
        targetObjects.Clear();
    }
}
