#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class StageEditor : Editor
{
    private BubbleColor previousColor;
    private MovementFlag previousMovement;
    float radius = 0.4f; // ĳ��Ʈ�� ���� �ݰ�

    string mask;
    string bubbleLayer = "Bubble";
    string gridLayer = "Grid";
    UnityEngine.LayerMask colLayerMask => UnityEngine.LayerMask.GetMask(mask);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (CustomEditorWindow.isColorPaletteEnabled == false &&
            CustomEditorWindow.isBubbleRemoveEnabled == false &&
            CustomEditorWindow.isBubbleCreateEnabled == false &&
            CustomEditorWindow.isMovementChangeEnabled == false) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) // ���� ���콺 ��ư Ŭ��
        {
            Debug.Log("df");
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 worldMousePosition = worldRay.origin;

            // ���� ���̾� ����
            if (CustomEditorWindow.isColorPaletteEnabled == true ||
                CustomEditorWindow.isBubbleRemoveEnabled == true) mask = bubbleLayer;
            else if (CustomEditorWindow.isBubbleCreateEnabled == true ||
                CustomEditorWindow.isMovementChangeEnabled == true) mask = gridLayer;
            else Debug.LogError("����");

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
                Debug.Log(closest);
                if (CustomEditorWindow.isColorPaletteEnabled || CustomEditorWindow.isBubbleRemoveEnabled)
                {
                    Bubble _closetBubble = closest.GetComponent<Bubble>();
                    if (CustomEditorWindow.isColorPaletteEnabled)
                    {
                        if (_closetBubble.bubbleColor != previousColor)
                        {
                            _closetBubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                            previousColor = _closetBubble.bubbleColor;
                            EditorUtility.SetDirty(_closetBubble.gameObject);
                        }
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
                else if (CustomEditorWindow.isBubbleCreateEnabled || CustomEditorWindow.isMovementChangeEnabled)
                {
                    GridSlot _slot = closest.GetComponent<GridSlot>();
                    if (CustomEditorWindow.isBubbleCreateEnabled)
                    {
                        Vector2 _pos = _slot.gridXY;
                        if (BubbleManager.Instance.bubbleDatasDic.ContainsKey(_pos) == true) return;

                        GameObject _obj = Instantiate(HexagonGridManager.Instance.bubblePrafab, closest.transform.position, Quaternion.identity);
                        _obj.transform.SetParent(HexagonGridManager.Instance.stage.bubblesParent);
                        Bubble _bubble = _obj.GetComponent<Bubble>();
                        _bubble.ChangeColor(HexagonGridManager.Instance.selectColorEditor);
                        BubbleData _bubbleData = new BubbleData(_bubble, _pos);
                        _bubble.currentXY = _pos;
                        BubbleManager.Instance.bubbleDatasDic.Add(_pos, _bubbleData);

                        EditorUtility.SetDirty(_obj.gameObject);
                    }
                    else if (CustomEditorWindow.isMovementChangeEnabled)
                    {
                        Debug.Log("df");
                        if (_slot.movementFlag != previousMovement)
                        {
                            _slot.ChangeMovement(HexagonGridManager.Instance.selectMovementEditor);
                            previousMovement = _slot.movementFlag;
                            EditorUtility.SetDirty(_slot.gameObject);
                        }
                    }
                }
                else Debug.LogError("����");
            }
        }
    }
}
#endif