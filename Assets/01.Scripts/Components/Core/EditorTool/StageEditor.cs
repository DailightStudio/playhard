#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bubble))]
public class StageEditor : Editor
{
    private BubbleColor previousColor;
    float radius = 0.4f; // ĳ��Ʈ�� ���� �ݰ�

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

        if (e.type == EventType.MouseDown && e.button == 0) // ���� ���콺 ��ư Ŭ��
        {
            // SceneView������ Ŭ���� ���� ��ǥ�� ��ȯ
            Vector2 mousePosition = e.mousePosition;
            Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
            mousePosition.y = sceneCamera.pixelHeight - mousePosition.y; // Y�� ����
            Vector3 worldMousePosition = sceneCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));

            // CircleCast�� ����Ͽ� Ŭ���� ��ġ ��ó�� ��� Collider�� ������
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

            // ���� ����� ��ü�� ó��
            if (closestBubble != null)
            {
                if (closestBubble.bubbleColor != previousColor)
                {
                    // Undo ��� �� ���� ����
                    Undo.RecordObject(closestBubble.gameObject, "Change Bubble Color");
                    closestBubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                    previousColor = closestBubble.bubbleColor;

                    // �� ��� ������ ���� ������Ʈ
                    EditorUtility.SetDirty(closestBubble.gameObject);
                    //SceneView.RepaintAll();
                }
            }
        }
    }
}
#endif