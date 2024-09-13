
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    public List<GameObject> beziers = new List<GameObject>(); // ���� ������ ��� ������ ����Ʈ
    public List<Transform[]> bezierCurves = new List<Transform[]>(); // ���� ������ ��� ������ �迭�� ������ ����Ʈ

    public Transform[] GetBezierPoints(int index)
    {
        if (index < bezierCurves.Count)
        {
            return bezierCurves[index];
        }
        return null;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (beziers.Count == 0) return;

        int _num = 0;
        foreach (var bezier in beziers)
        {
            Transform[] points = GetBezierPoints(_num);
    
            // points�� null���� üũ
            if ( points.Length < 4)
            {
                Debug.LogWarning($"������ � {_num}�� ������ �迭�� ��ȿ���� �ʽ��ϴ�.");
                _num++;
                continue; // ��ȿ���� ������ �׸��� �ǳʶ�
            }

            Handles.DrawBezier(points[0].position, points[3].position, points[1].position, points[2].position, Color.red, null, 2f);
            Gizmos.DrawLine(points[0].position, points[1].position);
            Gizmos.DrawLine(points[2].position, points[3].position);

            foreach (var item in points)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(item.position, 0.01f);
            }
            _num++;
#endif
        }
    }

    public void AddBezier()
    {
        var newBezier = Instantiate(HexagonGridManager.Instance.bezierCurvePrafab, HexagonGridManager.Instance.bezierParent);
        beziers.Add(newBezier);
        newBezier.transform.SetParent(HexagonGridManager.Instance.bezierParent);

        Transform[] _childPoint = new Transform[newBezier.transform.childCount];
        for (int i = 0; i < newBezier.transform.childCount; i++)
        {
            _childPoint[i] = newBezier.transform.GetChild(i);
        }
        AddBezierPoint(_childPoint);
    }
    void AddBezierPoint(Transform[] bezierPoints)
    {
        if (bezierPoints.Length == 4)
        {
            bezierCurves.Add(bezierPoints);
        }
        else
        {
            Debug.LogWarning("������ ��� 4���� �������� ������ �մϴ�.");
        }
    }

    public void RemoveBeziersAll()
    {
        foreach (var bezier in beziers)
        {
            DestroyImmediate(bezier);
        }
        beziers.Clear();
    }

    public void RemovePreviousBeziers()
    {
        if (beziers.Count > 0)
        {
            DestroyImmediate(beziers[beziers.Count - 1]);
            beziers.RemoveAt(beziers.Count - 1);
        }
    }
}
