using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprRenderer;
    public BubbleColor bubbleColor { get; set; } = BubbleColor.White;

    public Vector2 currentXY;
    public Vector2 nextStep { get; set; }

    public void ChangeColor(BubbleColor _color)
    {
        bubbleColor = _color;
        Sprite _colorHexCode = BubbleManager.Instance.bubbleColorCodeDic[bubbleColor];
        sprRenderer.sprite = _colorHexCode;
    }

}
