using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class UIHelper
{
    public static bool IsPointerOverUIButton()
    {
        if (Input.touchCount > 0) // 터치 입력이 존재하는 경우
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 가져오기
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = touch.position // 모바일 터치 위치 사용
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("UIButton")) // UI 버튼 태그 확인
                {
                    return true; // UI 버튼 위에서 터치됨
                }
            }
        }
        return false; // 버튼 위에서 터치되지 않음
    }
}
