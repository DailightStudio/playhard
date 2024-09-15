#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bubble))]

public class BubbleEditor : Editor
{
    float radius = 0.4f; // ĳ��Ʈ�� ���� �ݰ�

    string bubbleLayer = "Bubble";
    UnityEngine.LayerMask colLayerMask => UnityEngine.LayerMask.GetMask(bubbleLayer);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (CustomEditorWindow.isColorPaletteEnabled == false &&
            CustomEditorWindow.isBubbleRemoveEnabled == false) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) // ���� ���콺 ��ư Ŭ��
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 worldMousePosition = worldRay.origin;

            // Ŭ���� ��ġ ��ó�� ��� Collider�� ������
            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldMousePosition, radius, colLayerMask);
            if (colliders.Length == 0) return;

            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(worldMousePosition, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = collider.transform;
                }
            }

            // ���� ����� ��ü�� ó��
            if (closest != null)
            {
                Bubble _closetBubble = closest.GetComponent<Bubble>();
                if (CustomEditorWindow.isColorPaletteEnabled)
                {
                    _closetBubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                    EditorUtility.SetDirty(_closetBubble.gameObject);
                }
                else if (CustomEditorWindow.isBubbleRemoveEnabled)
                {
                    closest.gameObject.SetActive(false);
                    EditorUtility.SetDirty(closest.gameObject);
                    BubbleManager.Instance.bubbleDatasDic.Remove(_closetBubble.currentXY);
                    DestroyImmediate(closest.gameObject);
                }
                else Debug.LogError("����");
            }
            else Debug.LogError("����");
        }
    }
}
#endif