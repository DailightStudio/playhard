
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
        // 기존 위치의 버블 데이터를 null로 설정
        if (BubbleManager.Instance.bubbleDatasDic.ContainsKey(bubble.currentXY))
        {
            Debug.Log($"Removing bubble from current position: {bubble.currentXY}");
            BubbleManager.Instance.bubbleDatasDic[bubble.currentXY].bubble = null;
        }

        // 새로운 위치에 버블 데이터를 설정
        Debug.Log($"Setting bubble at new position: {gridXY}");
        BubbleManager.Instance.bubbleDatasDic[gridXY].bubble = bubble;

        // 버블의 현재 위치를 업데이트
        bubble.currentXY = gridXY; // 현재 위치 갱신
        Debug.Log($"Bubble currentXY updated to: {bubble.currentXY}");

    }
}
