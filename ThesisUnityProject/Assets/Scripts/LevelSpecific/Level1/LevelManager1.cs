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
    private Instructions0 _instructions0;
    private GameObject _controlSqrObj, _cancelButtonObj, _gateObj, _flagObj, _highlightObj;

    private bool
        _hasShowCtrlSqr,
        _hasRemind,
        _hasShowCancelButton,
        _hasCancelInstruction,
        _isCancelInstructions = true,
        _hasAllowCancel,
        _isChasePhase,
        _isGoalPhase,
        _hasShowGoal,
        _hasStartDetect,
        _cancelButtonGrowing,
        _controlButtonGrowing;
    private bool
        _isInitialInstruction = true;

    private float _firstForceTimer,
        _secondForceTimer,
        _cancelInstructionTimer,
        _goalTimer,
        _chaseTimer,
        _cancelButtonGrowTimer,
        _controlButtonGrowTimer;

    private int _lastIndex = -1;
    private int _failTimes;
    
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        _cancelButtonObj.SetActive(false);
        // _gateObj = GameObject.FindGameObjectWithTag("GateConstant");
        // Services.Gate = _gateObj.GetComponent<Gate>();
        // _gateObj.SetActive(false);
        // _flagObj = GameObject.FindGameObjectWithTag("Goal");
        // _flagObj.SetActive(false);
        _highlightObj = GameObject.FindGameObjectWithTag("Highlight");
        _highlightObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        targetRb.velocity = cfg1.v0;
        _controlButtonGrowing = true;
        //Instantiate(cfg1.chaseItem);
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        CheckTarget();

        if (_isCancelInstructions)
        {
            _cancelInstructionTimer += Time.deltaTime;
            if (_cancelInstructionTimer > cfg1.waitTime && !_hasCancelInstruction)
            {
                _tmp.text = cfg1.cancelInstruction;
                _cancelButtonObj.SetActive(true);
                _cancelButtonGrowing = true;
                Services.EventManager.Register<FirstCancel>(OnFirstCancel);
                _hasCancelInstruction = true;
            }
        }
    }
    
    private void LateUpdate()
    {
        foreach (var force in Services.Forces)
            force.Draw();
        
        Services.ControllerSquare.LateUpdate();
    }

    private void OnFirstCancel(AGPEvent e)
    {
        _isCancelInstructions = false;
        _tmp.text = cfg1.whenFirstCancel;
        StartCoroutine(ClearText(cfg1.duration));
        Services.EventManager.Unregister<FirstCancel>(OnFirstCancel);
    }

    private IEnumerator ClearText(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _tmp.text = "";
        
    }
    // private void OnShowGate(AGPEvent e)
    // {
    //     ShowGoal();
    //     _tmp.text = cfg0.goalExplanation;
    //     _targetRB.velocity = Vector2.zero;
    //     Services.EventManager.Register<Success>(OnSuccess);
    //     Services.EventManager.Register<LoseLife>(OnLoseLife);
    //     Services.EventManager.Unregister<ShowGate>(OnShowGate);
    // }
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
    

    // private void ShowGoal()
    // {
    //     _flagObj.transform.position = _gateObj.transform.position =
    //         _targetSqrObj.transform.position + cfg0.chaseItemPositions.Last();
    //     _gateObj.SetActive(true);
    //     _flagObj.SetActive(true);
    // }
    
    private bool _checked;
    private void CheckTarget()
    {
        if (_checked)
        {
            return;
        }
        
        if (Services.TargetSquare.transform.position.x > 0)
        {
            Services.CameraController.isFollowing = true;
            _checked = true;
        }
    }

    // private void OnSuccess(AGPEvent e)
    // {
    //     _tmp.text = cfg0.whenSuccess;
    //     _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    //     _flagObj.transform.parent = _targetSqrObj.transform;
    //     var p = Instantiate(gameCfg.successParticles, _targetSqrObj.transform.position, Quaternion.identity);
    //     StartCoroutine(WaitAndDestroy(p));
    //     Services.EventManager.Unregister<Success>(OnSuccess);
    //     Services.EventManager.Unregister<LoseLife>(OnLoseLife);
    // }

    private IEnumerator WaitAndDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy);
    }

    // private void OnLoseLife(AGPEvent e)
    // {
    //     var totalHintNum = cfg0.errorHints.Count;
    //     if (_failTimes >= totalHintNum)
    //     {
    //         _tmp.text = cfg0.errorHints[totalHintNum - 1];
    //         return;
    //     }
    //     _tmp.text = cfg0.errorHints[_failTimes];
    //     if (_failTimes == 1)
    //         _highlightObj.SetActive(true);
    //     
    //     else
    //         _highlightObj.SetActive(false);
    //     _failTimes++;
    // }
}


