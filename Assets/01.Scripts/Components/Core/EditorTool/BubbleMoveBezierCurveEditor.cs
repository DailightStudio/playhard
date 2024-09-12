#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BubbleBezierCurveManager))]
public class BubbleMoveBezierCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        
        BubbleBezierCurveManager bezierCurve = (BubbleBezierCurveManager)target;

        EditorGUI.BeginChangeCheck();
        if (bezierCurve.bezier == null) return;

        Vector3 point0 = Handles.PositionHandle(bezierCurve.point0.position, Quaternion.identity);
        Vector3 point1 = Handles.PositionHandle(bezierCurve.point1.position, Quaternion.identity);
        Vector3 point2 = Handles.PositionHandle(bezierCurve.point2.position, Quaternion.identity);
        Vector3 point3 = Handles.PositionHandle(bezierCurve.point3.position, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(bezierCurve, "Move Bezier Points");
            bezierCurve.point0.position = point0;
            bezierCurve.point1.position = point1;
            bezierCurve.point2.position = point2;
            bezierCurve.point3.position = point3;
            SceneView.RepaintAll();
        }

        Handles.DrawBezier(bezierCurve.point0.position, bezierCurve.point3.position,
                            bezierCurve.point1.position, bezierCurve.point2.position,
                            Color.red, null, 2f);
        // 핸들에 클릭 가능한 점 그리기
        DrawClickablePoint(bezierCurve.point0.position, "Point0");
        DrawClickablePoint(bezierCurve.point1.position, "Point1");
        DrawClickablePoint(bezierCurve.point2.position, "Point2");
        DrawClickablePoint(bezierCurve.point3.position, "Point3");

        void DrawClickablePoint(Vector3 position, string label)
        {
            // 핸들을 그리면서 클릭할 수 있는 점을 그립니다.
            Handles.color = Color.blue;
            Handles.DrawSolidDisc(position, Vector3.up, 0.1f);

            // 점에 대한 레이블 표시
            Handles.Label(position, label);
        }
    }
}
#endif