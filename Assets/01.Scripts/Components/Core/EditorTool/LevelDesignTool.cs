#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class CustomEditorWindow : EditorWindow
{
    string saveFilePath = "Assets/Resources/Stage"; // 스테이지를 저장할 파일 경로

    string stageFilePath = "Stage_00";

    HexagonGridManager grid => HexagonGridManager.Instance;
    private int selectedColor;
    private int selectedMovement;
    private string[] _optionsColor;
    private string[] _optionsMovement;

    public static bool isColorPaletteEnabled;
    public static bool isBubbleRemoveEnabled;
    public static bool isBubbleCreateEnabled;
    public static bool isMovementChangeEnabled;

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
        _optionsColor = Enum.GetNames(typeof(BubbleColor));
        selectedColor = (int)BubbleColor.White;

        _optionsMovement = Enum.GetNames(typeof(MovementFlag));
        selectedMovement = (int)MovementFlag.Down;

        isMade = HexagonGridManager.Instance.stage.gridParent.childCount > 0;
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

            GUILayout.BeginVertical("box");
            isBubbleCreateEnabled = EditorGUILayout.Toggle("구슬 생성 모드 활성화", isBubbleCreateEnabled);
            if (isBubbleCreateEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleRemoveEnabled = false;
                isMovementChangeEnabled = false;

                // 컬러 팔레트 선택
                selectedColor = EditorGUILayout.Popup("구슬 종류 선택", selectedColor, _optionsColor);
                grid.selectColorEditor = (BubbleColor)selectedColor;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            isBubbleRemoveEnabled = EditorGUILayout.Toggle("구슬 지우기 모드 활성화", isBubbleRemoveEnabled);
            if (isBubbleRemoveEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleCreateEnabled = false;
                isMovementChangeEnabled = false;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            // 컬러 팔레트 활성화 토글
            isColorPaletteEnabled = EditorGUILayout.Toggle("구슬 컬러 팔레트 활성화", isColorPaletteEnabled);
            if (isColorPaletteEnabled == true)
            {
                isBubbleRemoveEnabled = false;
                isBubbleCreateEnabled = false;
                isMovementChangeEnabled = false;

                // 컬러 팔레트 선택
                selectedColor = EditorGUILayout.Popup("구슬 종류 선택", selectedColor, _optionsColor);
                grid.selectColorEditor = (BubbleColor)selectedColor;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            isMovementChangeEnabled = EditorGUILayout.Toggle("이동 타일 종류 팔레트 활성화", isMovementChangeEnabled);
            if (isMovementChangeEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleRemoveEnabled = false;
                isBubbleCreateEnabled = false;

                // 컬러 팔레트 선택
                selectedColor = EditorGUILayout.Popup("이동 타일 종류 선택", selectedMovement, _optionsMovement);
                grid.selectMovementEditor = (MovementFlag)selectedMovement;
            }

            // 만약 모든 모드가 비활성화되었다면, Tool.Move로 설정
            if (!isBubbleCreateEnabled && !isBubbleRemoveEnabled && !isColorPaletteEnabled && isMovementChangeEnabled)
            {
                Tools.current = Tool.Move;
            }
            else Tools.current = Tool.Custom;

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            GUILayout.Space(lastSpace);

            GUILayout.BeginVertical("box");
            GUILayout.Label("스테이지 저장", EditorStyles.boldLabel);
            GUILayout.Label($"{saveFilePath}/");
            stageFilePath = EditorGUILayout.TextField("프리팹 명", stageFilePath);
            if (GUILayout.Button("스테이지를 프리팹으로 저장"))
            {
                // 저장할 프리팹의 경로를 설정
                string prefabName = $"{stageFilePath}.prefab";
                string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // 슬래시 통일

                if (File.Exists(savePath))
                {
                    // 팝업 대화상자 표시
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Save Stage",
                        stageFilePath + " 스테이지를 이 정보로 덮어 씌우시겠습니까?",
                        "Yes",
                        "No"
                    );

                    // 사용자가 'Yes'를 클릭했을 때만 SaveStage 메서드 호출
                    if (confirmed)
                    {
                        isBubbleRemoveEnabled = false;
                        SaveStage();
                    }
                }
                else
                {
                    isBubbleRemoveEnabled = false;
                    SaveStage();
                }
            }
            GUILayout.EndVertical();
        }
    }

    void SaveStage()
    {
        // 프리팹 저장 경로 설정
        string prefabName = $"{stageFilePath}.prefab";
        string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // 슬래시 통일

        // 프리팹 저장
        PrefabUtility.SaveAsPrefabAsset(HexagonGridManager.Instance.stage.gameObject, savePath);

        // 에셋 데이터베이스를 갱신
        AssetDatabase.SaveAssets();
    }
}
#endif