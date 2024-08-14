using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Terresquall
{

    [System.Serializable]
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour
    {

        public Image controlStick;

        [Header("Debug")]
        public bool consolePrintAxis = false;

        [Header("Settings")]
        public bool onlyOnMobile = true;
        public Color dragColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        public float sensitivity = 2f;
        [Range(0, 2)] public float radius = 0.7f;
        [Range(0, 1)] public float deadzone = 0.3f;

        public bool edgeSnap;
        [Range(0, 20)] public int directions = 0;
        public float angleOffset = 0;
        public bool snapsToTouch = false;
        public Rect boundaries;

        internal Vector2 desiredPosition, axis, origin, lastAxis;
        internal Color originalColor;
        int currentPointerId = -2;

        internal static List<VirtualJoystick> instances = new List<VirtualJoystick>();

        public const string VERSION = "1.0.3";
        public const string DATE = "2 June 2024";

        Vector2Int lastScreen;
        Canvas canvas;

        public static int CountActiveInstances()
        {
            int count = 0;
            foreach (VirtualJoystick j in instances)
            {
                if (j.isActiveAndEnabled)
                    count++;
            }
            return count;
        }

        public static float GetAxis(string axe, int index = 0)
        {
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return 0;
            }

            switch (axe.ToLower())
            {
                case "horizontal":
                case "h":
                case "x":
                    return instances[index].axis.x;
                case "vertical":
                case "v":
                case "y":
                    return instances[index].axis.y;
            }
            return 0;
        }

        public Vector2 GetAxisDelta() { return GetAxis() - lastAxis; }
        public static Vector2 GetAxisDelta(int index = 0)
        {
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return Vector2.zero;
            }

            return instances[index].GetAxisDelta();
        }

        public Vector2 GetAxis() { return axis; }
        public static Vector2 GetAxis(int index = 0)
        {
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return Vector2.zero;
            }

            return instances[index].axis;
        }

        public Vector2 GetAxisRaw()
        {
            return new Vector2(
                Mathf.Abs(axis.x) < deadzone || Mathf.Approximately(axis.x, 0) ? 0 : Mathf.Sign(axis.x),
                Mathf.Abs(axis.y) < deadzone || Mathf.Approximately(axis.y, 0) ? 0 : Mathf.Sign(axis.y)
            );
        }

        public float GetAxisRaw(string axe)
        {
            float f = GetAxis(axe);
            if (Mathf.Abs(f) < deadzone || Mathf.Approximately(f, 0))
                return 0;
            return Mathf.Sign(GetAxis(axe));
        }

        public static float GetAxisRaw(string axe, int index = 0)
        {
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return 0;
            }

            return instances[index].GetAxisRaw(axe);
        }

        public static Vector2 GetAxisRaw(int index = 0)
        {
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return Vector2.zero;
            }

            return instances[index].GetAxisRaw();
        }

        public float GetRadius()
        {
            RectTransform t = transform as RectTransform;
            if (t)
                return radius * t.rect.width * 0.5f;
            return radius;
        }

        public void OnPointerDown(PointerEventData data)
        {
            currentPointerId = data.pointerId;
            SetPosition(data.position);
            controlStick.color = dragColor;
        }

        public void OnPointerUp(PointerEventData data)
        {
            desiredPosition = transform.position;
            controlStick.color = originalColor;
            currentPointerId = -2;
        }

        protected void SetPosition(Vector2 position)
        {
            Vector2 diff = position - (Vector2)transform.position;
            float radius = GetRadius();
            bool snapToEdge = edgeSnap && (diff / radius).magnitude > deadzone;

            if (directions <= 0)
            {
                if (snapToEdge)
                {
                    desiredPosition = (Vector2)transform.position + diff.normalized * radius;
                }
                else
                {
                    desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(diff, radius);
                }
            }
            else
            {
                Vector2 snapDirection = SnapDirection(diff.normalized, directions, ((360f / directions) + angleOffset) * Mathf.Deg2Rad);
                if (snapToEdge)
                {
                    desiredPosition = (Vector2)transform.position + snapDirection * radius;
                }
                else
                {
                    desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(snapDirection * diff.magnitude, radius);
                }
            }
        }

        private Vector2 SnapDirection(Vector2 vector, int directions, float symmetryAngle)
        {
            Vector2 symmetryLine = new Vector2(Mathf.Cos(symmetryAngle), Mathf.Sin(symmetryAngle));
            float angle = Vector2.SignedAngle(symmetryLine, vector);
            angle /= 180f / directions;
            angle = (angle >= 0f) ? Mathf.Floor(angle) : Mathf.Ceil(angle);

            if ((int)Mathf.Abs(angle) % 2 == 1)
            {
                angle += (angle >= 0f) ? 1 : -1;
            }

            angle *= 180f / directions;
            angle *= Mathf.Deg2Rad;

            Vector2 result = new Vector2(Mathf.Cos(angle + symmetryAngle), Mathf.Sin(angle + symmetryAngle));
            result *= vector.magnitude;
            return result;
        }

        void Reset()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Image img = transform.GetChild(i).GetComponent<Image>();
                if (img)
                {
                    controlStick = img;
                    break;
                }
            }
        }

        public Rect GetBounds()
        {
            if (!snapsToTouch) return new Rect(0, 0, 0, 0);
            return new Rect(boundaries.x, boundaries.y, Screen.width * boundaries.width, Screen.height * boundaries.height);
        }

        void OnEnable()
        {
            if (!Application.isMobilePlatform && onlyOnMobile)
            {
                gameObject.SetActive(false);
                return;
            }

            canvas = GetComponentInParent<Canvas>();
            if (!canvas)
            {
                Debug.LogError(
                    string.Format("Your Virtual Joystick ({0}) is not attached to a Canvas, so it won't work. It has been disabled.", name),
                    gameObject
                );
                enabled = false;
            }

            try
            {
                Vector2 v = Input.mousePosition;
            }
            catch (System.InvalidOperationException)
            {
                enabled = false;
                Debug.LogError("The Virtual Joystick will not work because the old Input system is not available. Please enable it by going to Project Settings > Player > Other Settings > Active Input Handling and setting it to Both.", this);
            }

            origin = desiredPosition = transform.position;
            StartCoroutine(Activate());
            originalColor = controlStick.color;

            lastScreen = new Vector2Int(Screen.width, Screen.height);
            instances.Insert(0, this);
        }

        IEnumerator Activate()
        {
            yield return new WaitForEndOfFrame();
            origin = desiredPosition = transform.position;
        }

        void OnDisable()
        {
            instances.Remove(this);
        }

        void Update()
        {
            PositionUpdate();
            if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
            {
                lastScreen = new Vector2Int(Screen.width, Screen.height);
                OnEnable();
            }

            if (currentPointerId > -2)
            {
                if (currentPointerId > -1)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch t = Input.GetTouch(i);
                        if (t.fingerId == currentPointerId)
                        {
                            SetPosition(t.position);
                            break;
                        }
                    }
                }
                else
                {
                    SetPosition(Input.mousePosition);
                }
            }

            lastAxis = axis;

            controlStick.transform.position = Vector2.MoveTowards(controlStick.transform.position, desiredPosition, sensitivity);

            axis = (controlStick.transform.position - transform.position) / GetRadius();
            if (axis.magnitude < deadzone)
                axis = Vector2.zero;

            if (axis.sqrMagnitude > 0)
            {
                if (consolePrintAxis)
                    Debug.Log(string.Format("Virtual Joystick ({0}): {1}", name, axis));
            }
        }

        void CheckForInteraction(Vector2 position, int pointerId = -1)
        {
            PointerEventData data = new PointerEventData(null);
            data.position = position;
            data.pointerId = pointerId;

            List<RaycastResult> results = new List<RaycastResult>();
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(data, results);

            foreach (RaycastResult result in results)
            {
                if (IsGameObjectOrChild(result.gameObject, gameObject))
                {
                    OnPointerDown(data);
                    break;
                }
            }
        }

        bool IsGameObjectOrChild(GameObject hitObject, GameObject target)
        {
            if (hitObject == target) return true;

            foreach (Transform child in target.transform)
                if (IsGameObjectOrChild(hitObject, child.gameObject)) return true;

            return false;
        }

        void PositionUpdate()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    switch (t.phase)
                    {
                        case TouchPhase.Began:
                            CheckForInteraction(t.position, t.fingerId);

                            if (currentPointerId < -1)
                            {
                                if (GetBounds().Contains(t.position))
                                {
                                    Uproot(t.position, t.fingerId);
                                    return;
                                }
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (currentPointerId == t.fingerId)
                                OnPointerUp(new PointerEventData(null));
                            break;
                    }
                }

            }
            else if (Input.GetMouseButtonDown(0))
            {
                CheckForInteraction(Input.mousePosition, -1);

                if (currentPointerId < -1)
                {
                    if (GetBounds().Contains(Input.mousePosition))
                    {
                        Uproot(Input.mousePosition);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && currentPointerId == -1)
            {
                OnPointerUp(new PointerEventData(null));
            }
        }

        public void Uproot(Vector2 newPos, int newPointerId = -1)
        {
            if (Vector2.Distance(transform.position, newPos) < radius)
                return;

            transform.position = newPos;
            desiredPosition = transform.position;

            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = newPos;
            data.pointerId = newPointerId;
            OnPointerDown(data);
        }
    }
}
