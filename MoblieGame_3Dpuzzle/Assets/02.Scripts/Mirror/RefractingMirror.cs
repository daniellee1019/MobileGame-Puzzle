using UnityEngine;

public class RefractingMirror : Mirror
{
    // 거울(또는 유리)의 굴절률 (예: 유리 1.5)
    public float refractiveIndex = 1.5f;

    public override void ReflectLight(Ray ray, out Ray refractedRay)
    {
        Debug.Log("ReflectLight 호출됨");

        // 공기와 재질의 굴절률
        float n_air = 1.0f;
        float n_material = refractiveIndex;
        float n1, n2;

        // 거울의 "앞면" 방향으로 transform.forward를 사용 (원하는 대로 변경 가능)
        Vector3 frontNormal = transform.forward;

        // 레이저가 어느 면에서 들어오는지 판별:
        // ray.direction이 frontNormal 방향과 같은 쪽이면(내적 > 0), 뒷면에서 들어옴.
        if (Vector3.Dot(ray.direction, frontNormal) > 0)
        {
            frontNormal = -frontNormal;
            n1 = n_air;       // 외부(공기)에서 들어오는 빛
            n2 = n_material;  // 내부(재질)로 들어가는 빛
        }
        else
        {
            n1 = n_air;       // 외부(공기)에서 들어오는 빛
            n2 = n_material;  // 내부(재질)로 들어가는 빛
        }

        // 결정된 법선(항상 입사광선과 반대쪽을 향하도록)
        Vector3 normal = frontNormal;

        // 평면(거울)과의 교차점 계산
        float denominator = Vector3.Dot(ray.direction, normal);
        if (Mathf.Abs(denominator) < 0.0001f)
        {
            Debug.Log("레이저가 평면과 평행합니다.");
            refractedRay = new Ray(ray.origin, ray.direction);
            return;
        }

        float t = Vector3.Dot(transform.position - ray.origin, normal) / denominator;
        if (t < 0)
        {
            Debug.Log("레이저가 거울 반대쪽에서 옵니다. t: " + t);
            refractedRay = new Ray(ray.origin, ray.direction);
            return;
        }

        Vector3 hitPoint = ray.origin + ray.direction * t;
        Debug.Log("충돌점: " + hitPoint);

        // 입사 방향(단위벡터)
        Vector3 incident = ray.direction.normalized;

        // 스넬의 법칙 계산
        float ratio = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sinT2 = ratio * ratio * (1.0f - cosI * cosI);

        // 전반사 조건(physically sinT2 > 1)이 발생하면,
        // 굴절 대신 전반사가 발생해야 하지만, 여기서는 굴절을 강제로 적용하기 위해 sinT2 값을 1로 클램핑합니다.
        if (sinT2 > 1.0f)
        {
            Debug.Log("전반사 조건 발생 (sinT2: " + sinT2 + "), 클램핑하여 굴절을 강제합니다.");
            sinT2 = 1.0f;
        }

        float cosT = Mathf.Sqrt(1.0f - sinT2);

        // 굴절된 방향 계산 (벡터 형태)
        Vector3 refractedDirection = ratio * incident + (ratio * cosI - cosT) * normal;
        Debug.Log("굴절된 방향: " + refractedDirection);

        refractedRay = new Ray(hitPoint, refractedDirection.normalized);
        Debug.DrawRay(transform.position, normal.normalized * 5.0f, Color.blue, 2.0f);
        Debug.DrawRay(hitPoint, refractedDirection.normalized, Color.cyan, 2.0f);
    }
}
