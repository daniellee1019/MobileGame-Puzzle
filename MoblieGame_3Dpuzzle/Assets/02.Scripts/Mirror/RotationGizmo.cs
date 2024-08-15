using System.Collections.Generic;
using UnityEngine;

public class RotationGizmo : MonoBehaviour
{
    public List<Transform> targetObjects = new List<Transform>(); // ȸ����ų ��� ������Ʈ ����Ʈ
    public float rotationSpeed = 100f; // ȸ�� �ӵ�

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
                        // ���� ȸ���� ���� (Y�� ȸ��)
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
