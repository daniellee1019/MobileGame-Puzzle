using UnityEngine;
using System.Collections.Generic;

public class ObsSpawnerManager : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeY = 5;
    public float cellSize = 1f; // 각 셀 크기
    public GameObject[] singleTileObstacles; // 1칸 장애물 프리팹
    public GameObject[] doubleTileObstacles; // 2칸 장애물 프리팹
    public Transform spawnArea; // 장애물이 배치될 부모 오브젝트 (스폰 위치)

    private int[,] grid;

    void Start()
    {
        grid = new int[gridSizeX, gridSizeY];
        GenerateObstacles();
    }

    /// <summary>
    /// 장애물을 스폰아리아 위치에 생성하며, 기존 장애물은 제거
    /// </summary>
    public void GenerateObstacles()
    {
        ClearObstacles();
        List<Vector2Int> availablePositions = GetAvailablePositions();
        PlaceDoubleTileObstacles(availablePositions);
        PlaceSingleTileObstacles(availablePositions);
    }

    /// <summary>
    /// DFS를 사용하여 배치 가능한 연결된 공간을 탐색
    /// </summary>
    private List<Vector2Int> GetAvailablePositions()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        bool[,] visited = new bool[gridSizeX, gridSizeY]; // 방문 여부 체크

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y] == 0 && !visited[x, y])
                {
                    List<Vector2Int> region = new List<Vector2Int>();
                    DFS(x, y, visited, region); // DFS 탐색 수행
                    availablePositions.AddRange(region);
                }
            }
        }
        return availablePositions;
    }

    /// <summary>
    /// DFS 알고리즘을 사용하여 연결된 빈 공간을 찾음
    /// </summary>
    private void DFS(int x, int y, bool[,] visited, List<Vector2Int> region)
    {
        if (x < 0 || y < 0 || x >= gridSizeX || y >= gridSizeY) return; // 경계 체크
        if (visited[x, y] || grid[x, y] == 1) return; // 방문했거나 장애물이 이미 있으면 종료

        visited[x, y] = true; // 현재 위치 방문 처리
        region.Add(new Vector2Int(x, y)); // 현재 위치를 추가

        // 상, 하, 좌, 우 방향으로 재귀 호출
        DFS(x + 1, y, visited, region);
        DFS(x - 1, y, visited, region);
        DFS(x, y + 1, visited, region);
        DFS(x, y - 1, visited, region);
    }

    /// <summary>
    /// 1칸짜리 장애물을 랜덤 위치에 배치
    /// </summary>
    private void PlaceSingleTileObstacles(List<Vector2Int> availablePositions)
    {
        int obstacleCount = Random.Range(3, 6);
        for (int i = 0; i < obstacleCount; i++)
        {
            if (availablePositions.Count == 0) break;
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2Int pos = availablePositions[randomIndex];

            GameObject obstaclePrefab = singleTileObstacles[Random.Range(0, singleTileObstacles.Length)];

            // 장애물 위치를 중앙 정렬하여 배치
            Vector3 spawnPosition = spawnArea.position + new Vector3(pos.x * cellSize + cellSize / 2, 0, pos.y * cellSize + cellSize / 2);
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, spawnArea);

            grid[pos.x, pos.y] = 1;
            availablePositions.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 2칸짜리 장애물을 랜덤 위치에 배치 (가로/세로 방향 고려)
    /// </summary>
    private void PlaceDoubleTileObstacles(List<Vector2Int> availablePositions)
    {
        int obstacleCount = Random.Range(2, 4); // 배치할 장애물 개수 랜덤 결정
        for (int i = 0; i < obstacleCount; i++)
        {
            if (availablePositions.Count < 2) break; // 최소 2칸 필요
            int randomIndex = Random.Range(0, availablePositions.Count); // 랜덤한 위치 선택
            Vector2Int pos = availablePositions[randomIndex];

            // 장애물을 가로 또는 세로로 배치할지 랜덤 결정
            bool isHorizontal = Random.value > 0.5f;
            Vector2Int secondPos = isHorizontal ? new Vector2Int(pos.x + 1, pos.y) : new Vector2Int(pos.x, pos.y + 1);

            if (IsPositionAvailable(secondPos)) // 두 번째 칸이 비어있는 경우 배치 진행
            {
                // 랜덤한 2칸짜리 장애물 프리팹 선택
                GameObject obstaclePrefab = doubleTileObstacles[Random.Range(0, doubleTileObstacles.Length)];

                // 두 칸의 중앙 위치를 기준으로 배치
                float centerX = (pos.x + secondPos.x) / 2f * cellSize;
                float centerY = (pos.y + secondPos.y) / 2f * cellSize;
                Vector3 spawnPosition = spawnArea.position + new Vector3(centerX + cellSize / 2, 0, centerY + cellSize / 2);

                // 장애물 생성
                GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, spawnArea);

                // 방향에 따라 정확한 회전 적용
                if (isHorizontal)
                {
                    // 가로 방향: 기본 회전 유지 (0도)
                    obstacle.transform.rotation = Quaternion.identity;
                }
                else
                {
                    // 세로 방향: 90도 회전
                    obstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

                // 두 번째 칸을 장애물 위치로 인식
                grid[pos.x, pos.y] = 1;
                grid[secondPos.x, secondPos.y] = 1;

                availablePositions.Remove(pos); // 사용한 위치 제거
                availablePositions.Remove(secondPos);
            }
        }
    }

    private bool IsPositionAvailable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= gridSizeX || pos.y >= gridSizeY) return false;
        return grid[pos.x, pos.y] == 0;
    }

    /// <summary>
    /// 기존 장애물을 삭제하고 그리드를 초기화
    /// </summary>
    private void ClearObstacles()
    {
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }
        grid = new int[gridSizeX, gridSizeY];
    }

    // Scene 창에서 그리드를 그리는 기능 추가
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.black;

        for (int x = 0; x <= gridSizeX; x++)
        {
            Vector3 start = spawnArea.position + new Vector3(x * cellSize, 0.01f, 0);
            Vector3 end = spawnArea.position + new Vector3(x * cellSize, 0.01f, gridSizeY * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridSizeY; y++)
        {
            Vector3 start = spawnArea.position + new Vector3(0, 0.01f, y * cellSize);
            Vector3 end = spawnArea.position + new Vector3(gridSizeX * cellSize, 0.01f, y * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // 장애물과 일치하는 위치로 수정
                Vector3 cellPosition = spawnArea.position + new Vector3(x * cellSize + cellSize / 2, 0.02f, y * cellSize + cellSize / 2);

                if (grid[x, y] == 1)
                {
                    Gizmos.color = new Color(1f, 0, 0, 0.5f);
                    Gizmos.DrawCube(cellPosition, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
                }
                else
                {
                    Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                    Gizmos.DrawCube(cellPosition, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));
                }
            }
        }
    }

}
