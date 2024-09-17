
using System.Collections.Generic;
using UnityEngine;

public enum MovementFlag
{
    Down,
    Up,
    Right,
    Left,
    LeftUp,
    RightUp
}

public class GridSlot : MonoBehaviour
{
    public SpriteRenderer sprRenderer;
    public Vector2 gridXY;

    [SerializeField] public MovementFlag movementFlag = MovementFlag.Down;

    public void ChangeMovement(MovementFlag _movement)
    {
        movementFlag = _movement;
        Sprite _spr = HexagonGridManager.Instance.movementSprDic[movementFlag];
        sprRenderer.sprite = _spr;
    }

    string bubbleLayer = "Bubble";
    Bubble bubble;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(bubbleLayer))
        {
            bubble = collision.GetComponent<Bubble>();
            bubble.nextStep = StageManager.Instance.directionsDic[movementFlag] + gridXY;
        }
    }

    public void RefleshBubbleData()
    {
        if (bubble == null) return;
        // ���� ��ġ�� ���� �����͸� null�� ����
        if (BubbleManager.Instance.bubbleDatasDic.ContainsKey(bubble.currentXY))
        {
            Debug.Log($"Removing bubble from current position: {bubble.currentXY}");
            BubbleManager.Instance.bubbleDatasDic[bubble.currentXY].bubble = null;
        }

        // ���ο� ��ġ�� ���� �����͸� ����
        Debug.Log($"Setting bubble at new position: {gridXY}");
        BubbleManager.Instance.bubbleDatasDic[gridXY].bubble = bubble;

        // ������ ���� ��ġ�� ������Ʈ
        bubble.currentXY = gridXY; // ���� ��ġ ����
        Debug.Log($"Bubble currentXY updated to: {bubble.currentXY}");

    }
}
