using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    public GameObject wallPrefab; // 벽 프리팹
    float wallThickness = 0.5f; // 벽 두께

    void Start()
    {
        SetUpWalls();
    }

    void SetUpWalls()
    {
        Camera cam = Camera.main;
        Vector2 screenBottomLeft = cam.ViewportToWorldPoint(Vector2.zero);
        Vector2 screenTopRight = cam.ViewportToWorldPoint(Vector2.one);

        float screenWidth = screenTopRight.x - screenBottomLeft.x;
        float screenHeight = screenTopRight.y - screenBottomLeft.y;

        // 상단 및 하단 벽 위치 및 크기 조정
        //CreateWall(new Vector2(0f, screenTopRight.y + wallThickness / 2), new Vector2(screenWidth, wallThickness)); // 상단 벽
        CreateWall(new Vector2(0f, screenBottomLeft.y - wallThickness / 2), new Vector2(screenWidth, wallThickness)); // 하단 벽

        // 왼쪽 및 오른쪽 벽 위치 및 크기 조정
        CreateWall(new Vector2(screenBottomLeft.x - wallThickness / 2, 0), new Vector2(wallThickness, screenHeight)); // 왼쪽 벽
        CreateWall(new Vector2(screenTopRight.x + wallThickness / 2, 0), new Vector2(wallThickness, screenHeight)); // 오른쪽 벽
    }

    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.SetParent(transform);

        BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = size;
            wall.transform.localScale = new Vector3(size.x / collider.size.x, size.y / collider.size.y, 1);
        }
    }
}