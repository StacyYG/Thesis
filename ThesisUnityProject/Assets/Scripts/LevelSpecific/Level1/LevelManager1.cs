using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LevelManager1 : MonoBehaviour
{
    public LevelCfg1 cfg1;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj, _shadeObj, _gateObj, _flagObj, _highlightObj;
    public GameCfg gameCfg;

    private bool
        _hasShowCtrlSqr,
        _hasRemind,
        _hasShowCancelButton,
        _hasCancelInstruction,
        _isCancelInstructions,
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
    
    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        _controlSqrObj = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = new ControllerSquare(_controlSqrObj.transform);
        _targetSqrObj = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqrObj.GetComponent<TargetSquare>();
        _targetRB = _targetSqrObj.GetComponent<Rigidbody2D>();
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = new CancelButton(_cancelButtonObj);
        Services.CameraController = new CameraController(Services.MainCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _gateObj = GameObject.FindGameObjectWithTag("GateConstant");
        Services.Gate = _gateObj.GetComponent<Gate>();
        _gateObj.SetActive(false);
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _flagObj.SetActive(false);
        Services.VelocityBar = new VelocityBar(GameObject.FindGameObjectWithTag("SpeedBar").transform,
            GameObject.FindGameObjectWithTag("DirectionPointer").transform, _targetRB,
            GameObject.FindGameObjectWithTag("SpeedWarning"));
        _highlightObj = GameObject.FindGameObjectWithTag("Highlight");
        _highlightObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _targetRB.velocity = cfg1.v0;
        ShowCtrlSqr();
        Instantiate(cfg1.chaseItem);
    }
    

    private void FixedUpdate()
    {
        Services.Input.Update();
        Services.TargetSquare.OnFixedUpdate();
        Services.CameraController.Update();

    }
    
    // Update is called once per frame
    void Update()
    {
        Services.TargetSquare.OnUpdate();
        Services.VelocityBar.Update();
        CheckTarget();
        if (_controlButtonGrowing)
        {
            _controlButtonGrowTimer += Time.deltaTime;
            if (Services.ControllerSquare.boundCircle.GrownUp(_controlButtonGrowTimer)) 
                _controlButtonGrowing = false;
        }

        if (_isCancelInstructions)
        {
            _cancelInstructionTimer += Time.deltaTime;
            if (_cancelInstructionTimer > cfg1.waitTime && !_hasCancelInstruction)
            {
                _tmp.text = cfg1.cancelInstruction;
                _cancelButtonObj.transform.localPosition = new Vector3(-6f, -2.5f, 10f);
                _cancelButtonGrowing = true;
                Services.EventManager.Register<FirstCancel>(OnFirstCancel);
                _hasCancelInstruction = true;
            }

            if (_cancelButtonGrowing)
            {
                _cancelButtonGrowTimer += Time.deltaTime;
                if (Services.CancelButton.boundCircle.GrownUp(_cancelButtonGrowTimer))
                    _cancelButtonGrowing = false;
            }
        }
    }
    
    private void LateUpdate()
    {
        Services.ControllerSquare.LateUpdate();
        Services.TargetSquare.OnLateUpdate();
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
    

    private void ShowCtrlSqr()
    {
        _controlSqrObj.transform.localPosition = new Vector3(6f, -2.5f, 10f);
        _controlButtonGrowing = true;
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
            Services.CameraController.IsFollowing = true;
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


