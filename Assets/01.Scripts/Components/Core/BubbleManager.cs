using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BubbleColor
{
    Empty,
    Red,
    Blue,
    Green,
    Yellow,
    White
}

public class BubbleData
{
    public Bubble bubble;
    public float x;
    public float y;

    public BubbleData(Bubble _bubble, Vector2 _pos)
    {
        this.bubble = _bubble;
        this.x = _pos.x;
        this.y = _pos.y;
    }
}

public class BubbleManager : Singleton<BubbleManager>
{
    public Sprite bubbleEmpty;
    public Sprite bubbleRed;
    public Sprite bubbleBlue;
    public Sprite bubbleGreen;
    public Sprite bubbleYellow;
    public Sprite bubbleWhite;

    public Dictionary<BubbleColor, Sprite> bubbleColorCodeDic => new Dictionary<BubbleColor, Sprite>()
    {
        { BubbleColor.Empty, bubbleEmpty},
        { BubbleColor.Red, bubbleRed},
        { BubbleColor.Blue, bubbleBlue},
        { BubbleColor.Green, bubbleGreen},
        { BubbleColor.Yellow, bubbleYellow},
        { BubbleColor.White, bubbleWhite},
    };

    [HideInInspector] public Dictionary<Vector2, BubbleData> bubbleDatasDic = new Dictionary<Vector2, BubbleData>();

}

public class BubbleBezierCurveManager : Singleton<BubbleBezierCurveManager>
{
    public GameObject bezier;
    public Transform point0 => bezier.transform.GetChild(0);// 베지어 곡선을 그릴 포인트들
    public Transform point1 => bezier.transform.GetChild(1);
    public Transform point2 => bezier.transform.GetChild(2);
    public Transform point3 => bezier.transform.GetChild(3);
    Transform[] points => new Transform[] { point0, point1, point2, point3 };

    public void OnDrawGizmos()
    {
        if (bezier == null) return;
        Handles.DrawBezier(point0.position, point3.position, point1.position, point2.position, Color.red, null, 2f);
        Gizmos.DrawLine(point0.position, point1.position);
        Gizmos.DrawLine(point2.position, point3.position);
        foreach (var item in points)
        {
            Gizmos.color = Color.red; // 색상 설정
            Gizmos.DrawSphere(item.position, 0.01f); // 위치와 반지름으로 구를 그림
        }
    }
    public void RemoveBeziersAll()
    {
        while (HexagonGridManager.Instance.bezierParent.childCount > 0)
        {
            DestroyImmediate(HexagonGridManager.Instance.bezierParent.GetChild(0).gameObject);
        }
    }
    public void RemovePreviousBeziers()
    {
        DestroyImmediate(HexagonGridManager.Instance.bezierParent.GetChild(HexagonGridManager.Instance.bezierParent.childCount - 1).gameObject);
    }
}