using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Reflection;

public class DebugToolWindow : EditorWindow
{
    // [Step 2] 성능 측정 관련 변수
    private float fps;
    private int frameCount = 0;
    private float fpsUpdateTime = 0f;
    private float lastUpdateTime = 0f;
    private List<float> fpsHistory = new List<float>();
    private const int maxHistory = 100; // FPS 데이터 포인트 최대 개수

    // [Step 3] 로그 필터링 관련 변수
    private Vector2 logScrollPosition;
    private List<LogEntry> logEntries = new List<LogEntry>();
    private bool showLogs = true;
    private bool showWarnings = true;
    private bool showErrors = true;
    private string logSearchString = "";

    // 로그 데이터 저장 클래스
    public class LogEntry
    {
        public string message;
        public string stackTrace;
        public LogType type;
        public LogEntry(string message, string stackTrace, LogType type)
        {
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    [MenuItem("Window/Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<DebugToolWindow>("Debug Tool");
    }

    private void OnEnable()
    {
        lastUpdateTime = (float)EditorApplication.timeSinceStartup;
        EditorApplication.update += UpdateWindow;
        Application.logMessageReceived += HandleLog; // 로그 수신 이벤트 등록
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateWindow;
        Application.logMessageReceived -= HandleLog; // 이벤트 해제
    }

    private void UpdateWindow()
    {
        // FPS 계산 로직
        float currentTime = (float)EditorApplication.timeSinceStartup;
        float delta = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        frameCount++;
        fpsUpdateTime += delta;
        if (fpsUpdateTime >= 1.0f)
        {
            fps = frameCount / fpsUpdateTime;
            frameCount = 0;
            fpsUpdateTime = 0f;
            fpsHistory.Add(fps);
            if (fpsHistory.Count > maxHistory)
                fpsHistory.RemoveAt(0);
            Repaint();
        }
    }

    // 로그 수신 콜백 함수
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        logEntries.Add(new LogEntry(logString, stackTrace, type));
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("실시간 성능 모니터링", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // FPS 및 메모리 정보 표시
        GUILayout.Label("FPS: " + fps.ToString("F2"));
        long gcMemory = System.GC.GetTotalMemory(false);
        long allocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
        GUILayout.Label("GC 메모리 사용량: " + FormatBytes(gcMemory));
        GUILayout.Label("Profiler 메모리 사용량: " + FormatBytes(allocatedMemory));

        // 드로우 콜 수 표시 (내부 UnityStats API 사용)
        GUILayout.Label("Draw Call: " + GetDrawCallCount());

        GUILayout.Space(20);
        GUILayout.Label("FPS 그래프");
        Rect graphRect = GUILayoutUtility.GetRect(300, 100);
        DrawFPSGraph(graphRect);

        GUILayout.Space(20);
        DrawLogFilteringSection();
    }

    // FPS 그래프 그리기 함수
    private void DrawFPSGraph(Rect rect)
    {
        if (fpsHistory == null || fpsHistory.Count < 2)
            return;

        // 그래프의 최대 FPS 결정 (최소 60 FPS 기준)
        float maxFPSValue = 0f;
        foreach (float value in fpsHistory)
            if (value > maxFPSValue)
                maxFPSValue = value;
        if (maxFPSValue < 60) maxFPSValue = 60;

        // 그래프 배경 그리기
        EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f, 1f));

        // FPS 데이터를 선으로 연결하여 그래프 그리기
        Handles.BeginGUI();
        Handles.color = Color.green;
        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i < fpsHistory.Count; i++)
        {
            float x = rect.x + (i / (float)(maxHistory - 1)) * rect.width;
            float y = rect.y + rect.height - ((fpsHistory[i] / maxFPSValue) * rect.height);
            Vector3 currentPoint = new Vector3(x, y, 0);
            if (i > 0)
                Handles.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
        Handles.EndGUI();
    }

    // 로그 필터링 UI 섹션 그리기
    private void DrawLogFilteringSection()
    {
        GUILayout.Label("로그 필터링 시스템", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // 로그 레벨 필터 버튼
        GUILayout.BeginHorizontal();
        showLogs = GUILayout.Toggle(showLogs, "Log");
        showWarnings = GUILayout.Toggle(showWarnings, "Warning");
        showErrors = GUILayout.Toggle(showErrors, "Error");
        GUILayout.EndHorizontal();

        // 키워드 검색 입력창
        logSearchString = EditorGUILayout.TextField("검색", logSearchString);

        // 스크롤뷰를 통한 로그 리스트 표시
        logScrollPosition = GUILayout.BeginScrollView(logScrollPosition, GUILayout.Height(200));
        foreach (var log in logEntries)
        {
            // 로그 타입 필터링
            if ((log.type == LogType.Log && !showLogs) ||
                (log.type == LogType.Warning && !showWarnings) ||
                ((log.type == LogType.Error || log.type == LogType.Exception) && !showErrors))
                continue;

            // 검색어 필터링 (대소문자 구분 없이)
            if (!string.IsNullOrEmpty(logSearchString) &&
                log.message.IndexOf(logSearchString, System.StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            Color prevColor = GUI.contentColor;
            // 로그 타입에 따른 색상 지정
            if (log.type == LogType.Warning)
                GUI.contentColor = Color.yellow;
            else if (log.type == LogType.Error || log.type == LogType.Exception)
                GUI.contentColor = Color.red;
            else
                GUI.contentColor = Color.white;

            GUILayout.Label($"[{log.type}] {log.message}");
            GUI.contentColor = prevColor;
        }
        GUILayout.EndScrollView();
    }

    // 바이트 단위를 사람이 읽기 좋은 형식으로 변환하는 함수
    private string FormatBytes(long bytes)
    {
        if (bytes < 1024)
            return bytes + " B";
        else if (bytes < 1024 * 1024)
            return (bytes / 1024f).ToString("F2") + " KB";
        else if (bytes < 1024 * 1024 * 1024)
            return (bytes / (1024f * 1024f)).ToString("F2") + " MB";
        else
            return (bytes / (1024f * 1024f * 1024f)).ToString("F2") + " GB";
    }

    // 드로우 콜 수를 가져오는 함수 (내부 UnityStats API 사용 - 리플렉션)
    private int GetDrawCallCount()
    {
        System.Type unityStatsType = System.Type.GetType("UnityEditor.UnityStats,UnityEditor.dll");
        if (unityStatsType != null)
        {
            var property = unityStatsType.GetProperty("drawCalls", BindingFlags.Static | BindingFlags.Public);
            if (property != null)
            {
                return (int)property.GetValue(null, null);
            }
        }
        return 0;
    }
}
