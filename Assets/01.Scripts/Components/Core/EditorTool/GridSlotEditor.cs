#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSlot))]
public class GridSlotEditor : Editor
{
    float radius = 0.4f; // ĳ��Ʈ�� ���� �ݰ�
    string gridLayer = "Grid";
    UnityEngine.LayerMask colLayerMask => UnityEngine.LayerMask.GetMask(gridLayer);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (CustomEditorWindow.isBubbleCreateEnabled == false &&
            CustomEditorWindow.isMovementChangeEnabled == false) return;

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
                GridSlot _slot = closest.GetComponent<GridSlot>();
                if (CustomEditorWindow.isBubbleCreateEnabled)
                {
                    if (HexagonGridManager.Instance.hexaGridDatasDic.ContainsKey(_slot.gridXY) == true) return;

                    GameObject _obj = Instantiate(HexagonGridManager.Instance.bubblePrafab, new Vector3(closest.transform.position.x, closest.transform.position.y, -0.1f), Quaternion.identity);
                    _obj.transform.SetParent(HexagonGridManager.Instance.stage.bubblesParent);
                    Bubble _bubble = _obj.GetComponent<Bubble>();
                    _bubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                    _bubble.currentXY = _slot.gridXY;
                    HexagonGridManager.Instance.hexaGridDatasDic[_slot.gridXY].bubble = _bubble;

                    EditorUtility.SetDirty(_obj.gameObject);
                }
                else if (CustomEditorWindow.isMovementChangeEnabled)
                {
                    _slot.ChangeMovement(HexagonGridManager.Instance.selectMovementEditor);
                    EditorUtility.SetDirty(_slot.gameObject);
                }
            }
        }
    }
}
#endif
