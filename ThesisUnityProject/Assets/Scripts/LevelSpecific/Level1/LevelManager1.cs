using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LevelManager1 : LevelManager
{
    public LevelCfg1 cfg1;
    private TextMeshPro _tmp;
    private GameObject _gateObj, _flagObj, _highlightObj;
    private int _failTimes;
    private List<PrintAndWait> _printTasks;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _highlightObj = GameObject.FindGameObjectWithTag("Highlight");
        _highlightObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        Services.CameraController.lockY = true;
        Services.EventManager.Register<LoseLife>(OnLoseLife);
        _printTasks = new List<PrintAndWait>();
        foreach (var instruction in cfg1.failInstructions)
        {
            _printTasks.Add(new PrintAndWait(_tmp, instruction.duration, instruction.content));
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        Services.ControllerSquare.Start();
        taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void OnLoseLife(AGPEvent e)
    {
        _failTimes++;

        switch (_failTimes)
        {
            case 3:
                taskManager.Do(_printTasks[0]);
                break;
            case 6:
                taskManager.Do(_printTasks[1]);
                _printTasks[0].SetStatus(Task.TaskStatus.Success);
                break;
            case 9:
                taskManager.Do(_printTasks[2]);
                _printTasks[1].SetStatus(Task.TaskStatus.Success);
                break;
        }
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
    }
}
    

