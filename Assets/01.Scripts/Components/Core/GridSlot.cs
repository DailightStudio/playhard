
using UnityEngine;

public enum MovementFlag
{
    Down,
    Right,
    Left
}

public class GridSlot : MonoBehaviour
{
    public SpriteRenderer sprRenderer { get; set; }
    public Vector2 gridXY;
    public MovementFlag movementFlag { get; set; } = MovementFlag.Down;

    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
    }
    public void ChangeMovement(MovementFlag _movement)
    {
        movementFlag = _movement;
        Sprite _spr = HexagonGridManager.Instance.movementSprDic[movementFlag];
        sprRenderer.sprite = _spr;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Bubble _bubble))
        {
            switch (movementFlag)
            {
                case MovementFlag.Right:
                    _bubble.nextStep = gridXY + Vector2.right;
                    break;

                case MovementFlag.Left:
                    _bubble.nextStep = gridXY + Vector2.left;
                    break;

                case MovementFlag.Down:
                    _bubble.nextStep = gridXY + Vector2.down;
                    break;
            }
        }
    }

}
