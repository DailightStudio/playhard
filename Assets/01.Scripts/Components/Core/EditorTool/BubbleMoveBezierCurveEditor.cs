
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BubbleBezierCurveManager))]
public class BubbleMoveBezierCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        BubbleBezierCurveManager bezierCurveManager = (BubbleBezierCurveManager)target;

        EditorGUI.BeginChangeCheck();

        int _num = 0;
        foreach (var bezier in bezierCurveManager.beziers)
        {
            var points = bezierCurveManager.GetBezierPoints(_num);
            Vector3 point0 = Handles.PositionHandle(points[0].position, Quaternion.identity);
            Vector3 point1 = Handles.PositionHandle(points[1].position, Quaternion.identity);
            Vector3 point2 = Handles.PositionHandle(points[2].position, Quaternion.identity);
            Vector3 point3 = Handles.PositionHandle(points[3].position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bezierCurveManager, "Move Bezier Points");
                points[0].position = point0;
                points[1].position = point1;
                points[2].position = point2;
                points[3].position = point3;
                SceneView.RepaintAll();
            }

            Handles.DrawBezier(points[0].position, points[3].position, points[1].position, points[2].position, Color.red, null, 2f);
            DrawClickablePoint(points[0].position, "Point0");
            DrawClickablePoint(points[1].position, "Point1");
            DrawClickablePoint(points[2].position, "Point2");
            DrawClickablePoint(points[3].position, "Point3");

            _num++;
        }

        void DrawClickablePoint(Vector3 position, string label)
        {
            Handles.color = Color.blue;
            Handles.DrawSolidDisc(position, Vector3.up, 0.1f);
            Handles.Label(position, label);
        }
    }
}
#endif