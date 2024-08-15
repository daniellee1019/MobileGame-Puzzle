using UnityEngine;

public class AmplifyingMirror : Mirror
{
    public float amplificationFactor = 2.0f;

    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        base.ReflectLight(ray, out reflectedRay);
        // ������ ���� ���� ���
        reflectedRay.origin += reflectedRay.direction * amplificationFactor;
    }
}
