using UnityEngine;

public class AmplifyingMirror : Mirror
{
    public float amplificationFactor = 2.0f;
    public LineRenderer lineRenderer; // LineRenderer ����

    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        base.ReflectLight(ray, out reflectedRay);

        // ������ ���� ���� ��� (�ݻ�� ��ġ�� amplificationFactor ��ŭ �̵�)
        reflectedRay.origin += reflectedRay.direction * amplificationFactor;

        // ���� ũ�� ���� (LineRenderer�� �β� ����)
        if (lineRenderer != null)
        {
            float newWidth = lineRenderer.startWidth * amplificationFactor;
            lineRenderer.startWidth = newWidth;
            lineRenderer.endWidth = newWidth;
        }
    }
}
