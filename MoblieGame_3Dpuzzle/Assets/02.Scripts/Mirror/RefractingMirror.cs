using UnityEngine;

public class RefractingMirror : Mirror
{
    public float refractionIndex = 1.5f;

    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        Vector3 normal = transform.forward;
        Vector3 incident = ray.direction.normalized;

        float cosi = Mathf.Clamp(Vector3.Dot(-incident, normal), -1f, 1f);
        float etai = 1f;
        float etat = refractionIndex;

        if (cosi < 0)
        {
            cosi = -cosi;
        }
        else
        {
            float temp = etai;
            etai = etat;
            etat = temp;
            normal = -normal;
        }

        float eta = etai / etat;
        float k = 1 - eta * eta * (1 - cosi * cosi);

        if (k < 0)
        {
            reflectedRay = new Ray(transform.position, Vector3.Reflect(incident, normal));
        }
        else
        {
            Vector3 refractedDirection = eta * incident + (eta * cosi - Mathf.Sqrt(k)) * normal;
            reflectedRay = new Ray(transform.position, refractedDirection);
        }
    }
}
