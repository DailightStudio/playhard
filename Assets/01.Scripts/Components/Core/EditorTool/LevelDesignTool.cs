#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomEditorWindow : EditorWindow
{
    HexagonGridManager grid => HexagonGridManager.Instance;
    private int _selectedOptionIndex;
    private string[] _options;
    public static bool _isColorPaletteEnabled;

    int contensSpace = 10;
    int lastSpace = 10;
    bool isMade;

    [MenuItem("Window/Level Design Editor")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditorWindow>("Level Design Editor");
    }
    private void OnEnable()
    {
        _options = Enum.GetNames(typeof(BubbleColor));
        _selectedOptionIndex = (int)BubbleColor.White;
        isMade = HexagonGridManager.Instance.bezierParent.childCount > 0;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("스테이지 레벨 디자인", EditorStyles.boldLabel);
        HexagonGridManager.Instance.row = EditorGUILayout.IntField("행 사이즈", HexagonGridManager.Instance.row);
        HexagonGridManager.Instance.column = EditorGUILayout.IntField("열 사이즈", HexagonGridManager.Instance.column);
        HexagonGridManager.Instance.startColumn = EditorGUILayout.IntField("시작 열", HexagonGridManager.Instance.startColumn);


        if (GUILayout.Button("스테이지 만들기"))
        {
            isMade = true;
            grid.CreateGrid();
        }

        if (GUILayout.Button("스테이지 모두 지우기"))
        {
            isMade = false;
            grid.RemoveStageAll();
        }
        GUILayout.EndVertical();

        if (isMade)
        {
            GUILayout.Space(contensSpace);
            GUILayout.BeginVertical("box");
            GUILayout.Label("구슬 레벨 디자인", EditorStyles.boldLabel);
            // 컬러 팔레트 활성화 토글
            _isColorPaletteEnabled = EditorGUILayout.Toggle("구슬 컬러 팔레트 활성화", _isColorPaletteEnabled);
            if (_isColorPaletteEnabled)
            {
                // 컬러 팔레트 선택
                _selectedOptionIndex = EditorGUILayout.Popup("구슬 컬러 선택", _selectedOptionIndex, _options);
                grid.selectColorEditor = (BubbleColor)_selectedOptionIndex;
                Tools.current = Tool.Custom;
            }
            else
            {
                Tools.current = Tool.Move;
            }

            // 오브젝트의 트랜스폼 설정 공간 추가
            GUILayout.Label("구슬 애니메이션 베지어 구간 설정", EditorStyles.boldLabel);

            if (GUILayout.Button("베지어 애니메이션 구간 생성"))
            {
                GameObject _obj = Instantiate(HexagonGridManager.Instance.bezierCurvePrafab, Vector2.zero, Quaternion.identity);
                _obj.transform.SetParent(HexagonGridManager.Instance.bezierParent);
                BubbleBezierCurveManager.Instance.bezier = _obj;
            }
            if (GUILayout.Button("이전 베지어 지우기"))
            {
                BubbleBezierCurveManager.Instance.RemovePreviousBeziers();
            }
            if (GUILayout.Button("모든 베지어 지우기"))
            {
                BubbleBezierCurveManager.Instance.RemoveBeziersAll();
            }

            //Repaint();

            GUILayout.EndVertical();

            GUILayout.Space(lastSpace);

            GUILayout.BeginVertical("box");
            GUILayout.Label("스테이지 저장", EditorStyles.boldLabel);
            GUILayout.Label($"{HexagonGridManager.Instance.saveFilePath}/");
            HexagonGridManager.Instance.stageFilePath = EditorGUILayout.TextField("프리팹 명", HexagonGridManager.Instance.stageFilePath);
            if (GUILayout.Button("스테이지를 프리팹으로 저장"))
            {
                // 저장할 프리팹의 경로를 설정
                string prefabName = $"{grid.stageFilePath}.prefab";
                string savePath = Path.Combine(grid.saveFilePath, prefabName).Replace("\\", "/"); // 슬래시 통일

                if (File.Exists(savePath))
                {
                    // 팝업 대화상자 표시
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Save Stage",
                        grid.stageFilePath + " 스테이지를 이 정보로 덮어 씌우시겠습니까?",
                        "Yes",
                        "No"
                    );

                    // 사용자가 'Yes'를 클릭했을 때만 SaveStage 메서드 호출
                    if (confirmed)
                    {
                        _isColorPaletteEnabled = false;
                        grid.SaveStage();
                    }
                }
                else
                {
                    _isColorPaletteEnabled = false;
                    grid.SaveStage();
                }
            }
            GUILayout.EndVertical();
        }
    }
}
#endif