using UnityEngine;

public class ReflectingMirror : Mirror
{
    public override void ReflectLight(Ray ray, out Ray reflectedRay)
    {
        // 거울 표면의 법선 계산
        Vector3 mirrorNormal = this.transform.forward; // 거울의 "앞" 방향이 법선이 됨

        // 빛의 반사 방향 계산
        Vector3 reflectedDirection = Vector3.Reflect(ray.direction, mirrorNormal);

        // 반사된 빛의 경로를 설정
        reflectedRay = new Ray(ray.origin, reflectedDirection);

        // (Optional) 거울의 위치를 반사 빛의 시작점으로 설정
        reflectedRay.origin = ray.GetPoint(Vector3.Distance(ray.origin, this.transform.position));

        // Debug용으로 Ray를 시각적으로 확인
        Debug.DrawRay(reflectedRay.origin, reflectedDirection * 10f, Color.green, 2f);
    }
}
