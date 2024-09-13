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
            // �÷� �ȷ�Ʈ Ȱ��ȭ ���
            _isColorPaletteEnabled = EditorGUILayout.Toggle("���� �÷� �ȷ�Ʈ Ȱ��ȭ", _isColorPaletteEnabled);
            if (_isColorPaletteEnabled)
            {
                // �÷� �ȷ�Ʈ ����
                _selectedOptionIndex = EditorGUILayout.Popup("���� �÷� ����", _selectedOptionIndex, _options);
                grid.selectColorEditor = (BubbleColor)_selectedOptionIndex;
                Tools.current = Tool.Custom;
            }
            else
            {
                Tools.current = Tool.Move;
            }

            // ������Ʈ�� Ʈ������ ���� ���� �߰�
            GUILayout.Label("���� �ִϸ��̼� ������ ���� ����", EditorStyles.boldLabel);

            if (GUILayout.Button("������ �ִϸ��̼� ���� ����"))
            {
                GameObject _obj = Instantiate(HexagonGridManager.Instance.bezierCurvePrafab, Vector2.zero, Quaternion.identity);
                _obj.transform.SetParent(HexagonGridManager.Instance.bezierParent);
                BubbleBezierCurveManager.Instance.bezier = _obj;
            }
            if (GUILayout.Button("���� ������ �����"))
            {
                BubbleBezierCurveManager.Instance.RemovePreviousBeziers();
            }
            if (GUILayout.Button("��� ������ �����"))
            {
                BubbleBezierCurveManager.Instance.RemoveBeziersAll();
            }

            //Repaint();

            GUILayout.EndVertical();

            GUILayout.Space(lastSpace);

            GUILayout.BeginVertical("box");
            GUILayout.Label("�������� ����", EditorStyles.boldLabel);
            GUILayout.Label($"{HexagonGridManager.Instance.saveFilePath}/");
            HexagonGridManager.Instance.stageFilePath = EditorGUILayout.TextField("������ ��", HexagonGridManager.Instance.stageFilePath);
            if (GUILayout.Button("���������� ���������� ����"))
            {
                // ������ �������� ��θ� ����
                string prefabName = $"{grid.stageFilePath}.prefab";
                string savePath = Path.Combine(grid.saveFilePath, prefabName).Replace("\\", "/"); // ������ ����

                if (File.Exists(savePath))
                {
                    // �˾� ��ȭ���� ǥ��
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Save Stage",
                        grid.stageFilePath + " ���������� �� ������ ���� ����ðڽ��ϱ�?",
                        "Yes",
                        "No"
                    );

                    // ����ڰ� 'Yes'�� Ŭ������ ���� SaveStage �޼��� ȣ��
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