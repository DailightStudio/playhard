using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprRenderer;
    [HideInInspector] public BubbleColor bubbleColor = BubbleColor.White;

    public void ChangeColor(BubbleColor _color)
    {
        bubbleColor = _color;
        Sprite _colorHexCode = BubbleManager.Instance.bubbleColorCodeDic[bubbleColor];
        sprRenderer.sprite = _colorHexCode;
    }

}
