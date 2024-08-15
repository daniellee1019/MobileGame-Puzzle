using UnityEngine;

public class AmplifyingMirror : Mirror
{
    public float amplificationFactor = 2.0f;
    public LineRenderer lineRenderer; // LineRenderer 참조

    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        base.ReflectLight(ray, out reflectedRay);

        // 증폭된 빛의 방향 계산 (반사된 위치를 amplificationFactor 만큼 이동)
        reflectedRay.origin += reflectedRay.direction * amplificationFactor;

        // 빛의 크기 증폭 (LineRenderer의 두께 조절)
        if (lineRenderer != null)
        {
            float newWidth = lineRenderer.startWidth * amplificationFactor;
            lineRenderer.startWidth = newWidth;
            lineRenderer.endWidth = newWidth;
        }
    }
}
