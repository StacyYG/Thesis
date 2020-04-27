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
    private GameObject _gateObj, _flagObj, _highlightObj, _shadeObj;
    private int _failTimes;
    private List<PrintAndWait> _printTasks;
    private WaitAndPrint _firstForceReminder;
    private BoxCollider2D _gateCollider;
    

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
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _gateCollider = GameObject.FindGameObjectWithTag("GateConstant").GetComponent<BoxCollider2D>();
        Services.CameraController.lockY = true;
        Services.EventManager.Register<LoseLife>(OnLoseLife);
        Services.EventManager.Register<Success>(OnSuccess);
        Services.EventManager.Register<FirstForce>(OnFirstForce);
        Services.EventManager.Register<FirstCancel>(OnFirstCancel);
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
        _firstForceReminder = new WaitAndPrint(_tmp, cfg1.waitTime, cfg1.firstForceReminder);
        taskManager.Do(_firstForceReminder);
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
        
    }
    
    private void OnSuccess(AGPEvent e)
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
        Services.EventManager.Unregister<Success>(OnSuccess);
        _tmp.text = gameCfg.whenSuccess;
        var waitForNextLevel = new WaitTask(gameCfg.nextLevelTime);
        var transition = new ActionTask(() =>
        {
            _shadeObj.SetActive(true);
            _tmp.text = gameCfg.moreLevels;
            Services.ControllerSquare.Respond = false;
            Services.ControllerSquare.ResetPlayerForce();
            Services.ControllerSquare.LateUpdate();
            cxlButton.SetActive(false);
            ctrlSqr.SetActive(false);
            Services.ControllerSquare.boundCircle.Clear();
            Services.CancelButton.boundCircle.Clear();
        });
        waitForNextLevel.Then(transition);
        _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _flagObj.transform.parent = targetSqr.transform;
        var p = Instantiate(gameCfg.successParticles, targetSqr.transform.position, Quaternion.identity);
        var wait = new WaitTask(1f);
        var clearParticles = new ActionTask(() => Destroy(p));
        wait.Then(clearParticles);
        taskManager.Do(wait);
        taskManager.Do(waitForNextLevel);
    }

    private void OnFirstForce(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
        taskManager.Do(new WaitAndPrint(_tmp, cfg1.showCancelButtonTime, cfg1.cancelInstruction));
        var wait = new WaitTask(cfg1.showCancelButtonTime);
        var showCancelButton = new ActionTask(() =>
        {
            cxlButton.SetActive(true);
            Services.CancelButton.Start();
            taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
        });
        wait.Then(showCancelButton);
        taskManager.Do(wait);
        _gateCollider.isTrigger = true;
    }

    private void OnFirstCancel(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstCancel>(OnFirstCancel);
        var congrats = new PrintAndWait(_tmp, cfg1.duration, cfg1.whenFirstCancel);
        taskManager.Do(congrats);
    }
}
    

