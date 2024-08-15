using UnityEngine;

public class Mirror : MonoBehaviour
{
    public virtual void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        reflectedRay = new Ray(transform.position, Vector3.Reflect(ray.direction, transform.forward));
    }
}
