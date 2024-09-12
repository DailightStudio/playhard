#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bubble))]
public class StageEditor : Editor
{
    private BubbleColor previousColor;
    float radius = 0.4f; // 캐스트할 원의 반경

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (CustomEditorWindow._isColorPaletteEnabled == false) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) // 왼쪽 마우스 버튼 클릭
        {
            // SceneView에서의 클릭을 월드 좌표로 변환
            Vector2 mousePosition = e.mousePosition;
            Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
            mousePosition.y = sceneCamera.pixelHeight - mousePosition.y; // Y축 반전
            Vector3 worldMousePosition = sceneCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));

            // CircleCast를 사용하여 클릭한 위치 근처의 모든 Collider를 가져옴
            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldMousePosition, radius);
            Bubble closestBubble = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out Bubble hitBubble))
                {
                    float distance = Vector2.Distance(worldMousePosition, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBubble = hitBubble;
                    }
                }
            }

            // 가장 가까운 객체를 처리
            if (closestBubble != null)
            {
                if (closestBubble.bubbleColor != previousColor)
                {
                    // Undo 기록 및 색상 변경
                    Undo.RecordObject(closestBubble.gameObject, "Change Bubble Color");
                    closestBubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                    previousColor = closestBubble.bubbleColor;

                    // 씬 뷰와 에디터 상태 업데이트
                    EditorUtility.SetDirty(closestBubble.gameObject);
                    //SceneView.RepaintAll();
                }
            }
        }
    }
}
#endif