
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum BubbleColor
{
    Red,
    Blue,
    Green,
    Yellow,
    White
}

public class BubbleManager : Singleton<BubbleManager>
{
    public Sprite bubbleRed;
    public Sprite bubbleBlue;
    public Sprite bubbleGreen;
    public Sprite bubbleYellow;
    public Sprite bubbleWhite;

    public Dictionary<BubbleColor, Sprite> bubbleColorCodeDic => new Dictionary<BubbleColor, Sprite>()
    {
        { BubbleColor.Red, bubbleRed},
        { BubbleColor.Blue, bubbleBlue},
        { BubbleColor.Green, bubbleGreen},
        { BubbleColor.Yellow, bubbleYellow},
        { BubbleColor.White, bubbleWhite},
    };
}