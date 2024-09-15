#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bubble))]

public class BubbleEditor : Editor
{
    float radius = 0.4f; // 캐스트할 원의 반경

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

        if (e.type == EventType.MouseDown && e.button == 0) // 왼쪽 마우스 버튼 클릭
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 worldMousePosition = worldRay.origin;

            // 클릭한 위치 근처의 모든 Collider를 가져옴
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

            // 가장 가까운 객체를 처리
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
                else Debug.LogError("에러");
            }
            else Debug.LogError("에러");
        }
    }
}
#endif