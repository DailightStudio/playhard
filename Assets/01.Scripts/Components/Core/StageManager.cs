using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public List<GameObject> stageList { get; set; } = new List<GameObject>();

    public int stageLevel { get; set; } = 0;
    public Stage currentStage { get; set; }

    new void Awake()
    {
        LoadStages();
        CreateStage();

        InitStageDatas();


        StartCoroutine(ie());
    }

    void CreateStage()
    {
        if (currentStage != null) DestroyImmediate(currentStage);
        currentStage = Instantiate(stageList[stageLevel], Vector2.zero, Quaternion.identity).GetComponent<Stage>();
    }

    public void LoadStages()
    {
        GameObject[] _stages = Resources.LoadAll<GameObject>("Stage");
        foreach (GameObject _prefab in _stages)
        {
            stageList.Add(_prefab);
        }
    }

    IEnumerator ie()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            NextStep();
        }
    }

    public void NextStep()
    {
        foreach (var item in BubbleManager.Instance.bubbleDatasDic)
        {
            Bubble _bubble = BubbleManager.Instance.bubbleDatasDic[item.Key].bubble;

            if (HexagonGridManager.Instance.hexaGridDatasDic.TryGetValue(_bubble.nextStep, out HexaGridData _value))
            {
                Vector2 _nextStep = _value.gridXY;
                _bubble.transform.position = _nextStep;
            }
        }
    }

    public void NextStage()
    {
        if (stageList.Count - 1 > stageLevel)
        {
            Debug.Log("다음 스테이지");
            stageLevel++;

            InitStageDatas();
            CreateStage();
        }
        else Debug.Log("모든 스테이지 클리어");
    }

    void InitStageDatas()
    {
        HexagonGridManager.Instance.SetGrid();
        HexagonGridManager.Instance.SetBubbles();
    }
}
