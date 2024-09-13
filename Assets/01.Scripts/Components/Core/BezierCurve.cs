using UnityEditor;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform[] controlPoints;

    private void Awake()
    {
        controlPoints = new Transform[4]
        {
            transform.GetChild(0),
            transform.GetChild(1),
            transform.GetChild(2),
            transform.GetChild(3)
        };
    }

    public float speed = 0.5f;
    public float collisionThreshold = 0.1f;

    private bool isBubbleMoving = false;
    private float t = 0f;
    private Bubble currentBubble;

    public void AssignBubble(Bubble bubble)
    {
        if (!isBubbleMoving)
        {
            currentBubble = bubble;
            isBubbleMoving = true;
            t = 0f;
        }
    }

    private void Update()
    {
        if (isBubbleMoving && currentBubble != null)
        {
            MoveBubbleAlongCurve();
        }
    }

    private void MoveBubbleAlongCurve()
    {
        if (t <= 1)
        {
            t += Time.deltaTime * speed;
            Vector3 newPosition = CalculateBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position);
            currentBubble.transform.position = newPosition;
        }
        else
        {
            isBubbleMoving = false;
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bubble bubble = other.GetComponent<Bubble>();
        if (bubble != null && !isBubbleMoving)
        {
            AssignBubble(bubble);
        }
    }
//#if UNITY_EDITOR
//    public void OnDrawGizmos()
//    {

//        Handles.DrawBezier(controlPoints[0].position, controlPoints[3].position, controlPoints[1].position, controlPoints[2].position, Color.red, null, 2f);

//    }
//#endif
}