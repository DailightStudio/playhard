
using System.Collections.Generic;
using UnityEngine;

public enum MovementFlag
{
    Down,
    Up,
    Right,
    Left,
    LeftUp,
    RightUp,
    LeftDown,
    RightDown
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
}
