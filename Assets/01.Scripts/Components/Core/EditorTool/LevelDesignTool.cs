#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class CustomEditorWindow : EditorWindow
{
    string saveFilePath = "Assets/Resources/Stage"; // ���������� ������ ���� ���

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
        GUILayout.Label("�������� ���� ������", EditorStyles.boldLabel);
        HexagonGridManager.Instance.row = EditorGUILayout.IntField("�� ������", HexagonGridManager.Instance.row);
        HexagonGridManager.Instance.column = EditorGUILayout.IntField("�� ������", HexagonGridManager.Instance.column);
        HexagonGridManager.Instance.startColumn = EditorGUILayout.IntField("���� ��", HexagonGridManager.Instance.startColumn);


        if (GUILayout.Button("�������� �����"))
        {
            isMade = true;
            grid.CreateGrid();
        }

        if (GUILayout.Button("�������� ��� �����"))
        {
            isMade = false;
            grid.RemoveStageAll();
        }
        GUILayout.EndVertical();

        if (isMade)
        {
            GUILayout.Space(contensSpace);
            GUILayout.BeginVertical("box");
            GUILayout.Label("���� ���� ������", EditorStyles.boldLabel);

            GUILayout.BeginVertical("box");
            isBubbleCreateEnabled = EditorGUILayout.Toggle("���� ���� ��� Ȱ��ȭ", isBubbleCreateEnabled);
            if (isBubbleCreateEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleRemoveEnabled = false;
                isMovementChangeEnabled = false;

                // �÷� �ȷ�Ʈ ����
                selectedColor = EditorGUILayout.Popup("���� ���� ����", selectedColor, _optionsColor);
                grid.selectColorEditor = (BubbleColor)selectedColor;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            isBubbleRemoveEnabled = EditorGUILayout.Toggle("���� ����� ��� Ȱ��ȭ", isBubbleRemoveEnabled);
            if (isBubbleRemoveEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleCreateEnabled = false;
                isMovementChangeEnabled = false;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            // �÷� �ȷ�Ʈ Ȱ��ȭ ���
            isColorPaletteEnabled = EditorGUILayout.Toggle("���� �÷� �ȷ�Ʈ Ȱ��ȭ", isColorPaletteEnabled);
            if (isColorPaletteEnabled == true)
            {
                isBubbleRemoveEnabled = false;
                isBubbleCreateEnabled = false;
                isMovementChangeEnabled = false;

                // �÷� �ȷ�Ʈ ����
                selectedColor = EditorGUILayout.Popup("���� ���� ����", selectedColor, _optionsColor);
                grid.selectColorEditor = (BubbleColor)selectedColor;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            isMovementChangeEnabled = EditorGUILayout.Toggle("�̵� Ÿ�� ���� �ȷ�Ʈ Ȱ��ȭ", isMovementChangeEnabled);
            if (isMovementChangeEnabled == true)
            {
                isColorPaletteEnabled = false;
                isBubbleRemoveEnabled = false;
                isBubbleCreateEnabled = false;

                // �÷� �ȷ�Ʈ ����
                selectedColor = EditorGUILayout.Popup("�̵� Ÿ�� ���� ����", selectedMovement, _optionsMovement);
                grid.selectMovementEditor = (MovementFlag)selectedMovement;
            }

            // ���� ��� ��尡 ��Ȱ��ȭ�Ǿ��ٸ�, Tool.Move�� ����
            if (!isBubbleCreateEnabled && !isBubbleRemoveEnabled && !isColorPaletteEnabled && isMovementChangeEnabled)
            {
                Tools.current = Tool.Move;
            }
            else Tools.current = Tool.Custom;

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            GUILayout.Space(lastSpace);

            GUILayout.BeginVertical("box");
            GUILayout.Label("�������� ����", EditorStyles.boldLabel);
            GUILayout.Label($"{saveFilePath}/");
            stageFilePath = EditorGUILayout.TextField("������ ��", stageFilePath);
            if (GUILayout.Button("���������� ���������� ����"))
            {
                // ������ �������� ��θ� ����
                string prefabName = $"{stageFilePath}.prefab";
                string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // ������ ����

                if (File.Exists(savePath))
                {
                    // �˾� ��ȭ���� ǥ��
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Save Stage",
                        stageFilePath + " ���������� �� ������ ���� ����ðڽ��ϱ�?",
                        "Yes",
                        "No"
                    );

                    // ����ڰ� 'Yes'�� Ŭ������ ���� SaveStage �޼��� ȣ��
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
        // ������ ���� ��� ����
        string prefabName = $"{stageFilePath}.prefab";
        string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // ������ ����

        // ������ ����
        PrefabUtility.SaveAsPrefabAsset(HexagonGridManager.Instance.stage.gameObject, savePath);

        // ���� �����ͺ��̽��� ����
        AssetDatabase.SaveAssets();
    }
}
#endif