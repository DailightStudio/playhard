using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprRenderer;
    public BubbleColor bubbleColor { get; set; } = BubbleColor.White;

    public Vector2 currentXY;
    public Vector2 nextStep { get; set; }

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ChangeColor(BubbleColor _color)
    {
        bubbleColor = _color;
        Sprite _colorHexCode = BubbleManager.Instance.bubbleColorCodeDic[bubbleColor];
        sprRenderer.sprite = _colorHexCode;
    }

    public void ChangeRigidBody(RigidbodyType2D _type)
    {
        rb.bodyType = _type;
    }
}
