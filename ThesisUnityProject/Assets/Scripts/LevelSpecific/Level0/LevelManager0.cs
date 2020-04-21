using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LevelManager0 : LevelManager
{
    public LevelCfg0 cfg0;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private GameObject _shadeObj, _flagObj, _chaseItem;

    private bool _hasShowTargetSqr,
        _hasShowCtrlSqr,
        _hasFirstForce,
        _hasRemind,
        _hasSecondForce,
        _hasHideShade,
        _controlButtonGrowing;
    private bool
        _isInitialInstruction = true;

    private float _firstForceTimer,
        _secondForceTimer,
        _controlButtonGrowTimer;

    private int _lastIndex = -1;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        ctrlSqr.SetActive(false);
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _flagObj.SetActive(false);
        _chaseItem = GameObject.FindGameObjectWithTag("ChaseItem");
        _chaseItem.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        _instructions0 = Instructions0.Load(cfg0);
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < _instructions0.InitialInstructions.Count; i++)
        {
            tasks.Add(new WaitAndPrint(_tmp, _instructions0.InitialInstructions[i].startTime,
                _instructions0.InitialInstructions[i].content));
        }

        for (int i = 0; i < tasks.Count - 1; i++)
        {
            tasks[i].Then(tasks[i + 1]);
        }
        taskManager.Do(tasks[0]);
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        Services.VelocityBar.Update();
        CheckTarget();
        taskManager.Update();
        
        // if (_isInitialInstruction)
        // {
        //     var duration = Time.timeSinceLevelLoad;
        //     var i = InstructionIndex(_instructions0.InitialInstructions, duration, _lastIndex);
        //     if (i > _lastIndex)
        //     {
        //         _tmp.text = _instructions0.InitialInstructions[i].content;
        //         _lastIndex = i;
        //     }
        //     
        //     if (duration > cfg0.showTargetSqrTime && !_hasShowTargetSqr)
        //     {
        //         ShowTargetSqr();
        //         _hasShowTargetSqr = true;
        //     }
        //
        //     else if (duration > cfg0.showCtrlSqrTime && !_hasShowCtrlSqr)
        //     {
        //         ctrlSqr.SetActive(true);
        //         taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        //         Services.EventManager.Register<FirstForce>(OnFirstForce);
        //         Services.EventManager.Register<SecondForce>(OnSecondForce);
        //         _hasShowCtrlSqr = true;
        //     }
        // }
        //
        // else if (_hasFirstForce && !_hasSecondForce && !_hasRemind)
        // {
        //     _firstForceTimer += Time.deltaTime;
        //     if (_firstForceTimer > cfg0.secondForceRemindTime)
        //     {
        //         _tmp.text = cfg0.secondForceReminder;
        //         _hasRemind = true;
        //     }
        // }
        //
        // else if (_hasSecondForce && !_hasHideShade)
        // {
        //     _secondForceTimer += Time.deltaTime;
        //     if (_secondForceTimer > cfg0.secondForceInstructionDuration)
        //     {
        //         Destroy(_shadeObj);
        //         _tmp.text = "";
        //         _lastIndex = -1;
        //         _hasHideShade = true;
        //         _chaseItem.SetActive(true);
        //         Services.EventManager.Register<ShowGate>(OnShowGate);
        //     }
        // }
    }
    
    private void OnFirstForce(AGPEvent e)
    {
        _isInitialInstruction = false;
        _hasFirstForce = true;
        _tmp.text = cfg0.whenFirstForce;
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
    }
    
    private void OnSecondForce(AGPEvent e)
    {
        _shadeObj.SetActive(true);
        _hasSecondForce = true;
        _tmp.text = cfg0.whenSecondForce;
        Services.EventManager.Unregister<SecondForce>(OnSecondForce);
    }
    
    private void OnShowGate(AGPEvent e)
    {
        ShowGoal();
        _tmp.text = cfg0.goalExplanation;
        targetRb.velocity = Vector2.zero;
        Services.EventManager.Register<Success>(OnSuccess);
        Services.EventManager.Unregister<ShowGate>(OnShowGate);
    }
    private int InstructionIndex(List<InstructionItem> instructionItems, float time, int i)
    {
        if (i >= instructionItems.Count - 1)
        {
            return instructionItems.Count - 1;
        }
        if (time < instructionItems[i + 1].startTime)
        {
            return i;
        }
        return InstructionIndex(instructionItems, time, i + 1);
    }

    private void ShowTargetSqr()
    {
        targetRb.velocity = cfg0.v0;
    }

    private void ShowGoal()
    {
        _flagObj.transform.position = _chaseItem.transform.position;
        _flagObj.SetActive(true);
    }
    
    private bool _checked;
    private void CheckTarget()
    {
        if (_checked) return;
        
        if (Services.TargetSquare.transform.position.x > 0)
        {
            Services.CameraController.isFollowing = true;
            _checked = true;
        }
    }

    private void OnSuccess(AGPEvent e)
    {
        _tmp.text = cfg0.whenSuccess;
        _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _flagObj.transform.parent = targetSqr.transform;
        var p = Instantiate(gameCfg.successParticles, targetSqr.transform.position, Quaternion.identity);
        StartCoroutine(WaitAndDestroy(p));
        Services.EventManager.Unregister<Success>(OnSuccess);
    }

    private IEnumerator WaitAndDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy);
    }
    
}

public class WaitAndPrint : Task
{
    private TextMeshPro _tmp;
    private float _waitTime;
    private string _toPrint;
    private float _elapsedTime;
    public WaitAndPrint(TextMeshPro tmp, float waitTime, string toPrint)
    {
        _tmp = tmp;
        _waitTime = waitTime;
        _toPrint = toPrint;
    }

    protected override void Initialize()
    {
        _elapsedTime = 0f;
    }

    internal override void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _waitTime)
        {
            _tmp.text = _toPrint;
            SetStatus(TaskStatus.Success);
        }
    }
}


