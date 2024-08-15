using UnityEngine;

public class ReflectingMirror : Mirror
{
    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        // �ſ� ǥ���� ���� ���
        Vector3 mirrorNormal = this.transform.forward; // �ſ��� "��" ������ ������ ��

        // ���� �ݻ� ���� ���
        Vector3 reflectedDirection = Vector3.Reflect(ray.direction, mirrorNormal);

        // �ݻ�� ���� ��θ� ����
        reflectedRay = new Ray(ray.origin, reflectedDirection);

        // (Optional) �ſ��� ��ġ�� �ݻ� ���� ���������� ����
        reflectedRay.origin = ray.GetPoint(Vector3.Distance(ray.origin, this.transform.position));

        // Debug������ Ray�� �ð������� Ȯ��
        Debug.DrawRay(reflectedRay.origin, reflectedDirection * 10f, Color.green, 2f);
    }
}
