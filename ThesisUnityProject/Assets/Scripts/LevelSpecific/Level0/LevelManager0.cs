using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager0 : MonoBehaviour
{
    public LevelCfg0 levelCfg0;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj, _shadeObj, _gateObj, _flagObj;
    public GameCfg gameCfg;

    private bool _hasShowTargetSqr,
        _hasShowCtrlSqr,
        _hasAllowControl,
        _hasFirstForce,
        _hasRemind,
        _hasShowCancelButton,
        _hasSecondForce,
        _hasHideShade,
        _isLastInstructions,
        _hasAllowCancel,
        _isGoalPhase,
        _hasShowGoal,
        _hasStartDetect;
    private bool _isInitialInstruction = true;
    private float _timeSinceFirstForce,
        _firstForceMoment,
        _secondForceMoment,
        _lastInstructionStartMoment,
        _firstCancelMoment;
    private int _lastIndex = -1;
    private int _failTimes;
    
    public void Awake()
    {
        Init();
        Services.ControllerSquare.Awake();
        Services.CancelButton.Start();
    }

    private void Init()
    {
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        Services.GameCfg = gameCfg;
        _controlSqrObj = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = new ControllerSquare(_controlSqrObj.transform);
        Services.ControllerSquare.Respond = false;
        _targetSqrObj = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqrObj.GetComponent<TargetSquare>();
        _targetRB = _targetSqrObj.GetComponent<Rigidbody2D>();
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = new CancelButton(_cancelButtonObj);
        Services.CancelButton.Respond = false;
        Services.CameraController = new CameraController(Services.MainCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _gateObj = GameObject.FindGameObjectWithTag("GateConstant");
        Services.Gate = _gateObj.GetComponent<Gate>();
        _gateObj.SetActive(false);
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _flagObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        _instructions0 = Instructions0.Load(levelCfg0);
    }
    
    // Start is called before the first frame update
    void Start()
    {

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
        CheckTarget();
        if (_isInitialInstruction)
        {
            var duration = Time.timeSinceLevelLoad;
            var i = InstructionIndex(_instructions0.InitialInstructions, duration, _lastIndex);
            if (i > _lastIndex)
            {
                _tmp.text = _instructions0.InitialInstructions[i].content;
                _lastIndex = i;
            }
            
            if (duration > levelCfg0.showTargetSqrTime && !_hasShowTargetSqr)
            {
                ShowTargetSqr();
                _hasShowTargetSqr = true;
            }

            else if (duration > levelCfg0.showCtrlSqrTime && !_hasShowCtrlSqr)
            {
                ShowCtrlSqr();
                _hasShowCtrlSqr = true;
            }

            else if (duration > levelCfg0.allowControlTime && !_hasAllowControl)
            {
                Services.ControllerSquare.Respond = true;
                Services.EventManager.Register<FirstForce>(OnFirstForce);
                Services.EventManager.Register<SecondForce>(OnSecondForce);
                _hasAllowControl = true;
            }
        }
        
        else if (_hasFirstForce && !_hasSecondForce && Time.timeSinceLevelLoad - _firstForceMoment > levelCfg0.secondForceRemindTime && !_hasRemind)
        {
            _tmp.text = levelCfg0.secondForceReminder;
            _hasRemind = true;
        }

        else if (_hasSecondForce && Time.timeSinceLevelLoad - _secondForceMoment > levelCfg0.secondForceInstructionDuration && !_hasHideShade)
        {
            _lastInstructionStartMoment = Time.timeSinceLevelLoad;
            Destroy(_shadeObj);
            _tmp.text = "";
            _lastIndex = -1;
            _isLastInstructions = true;
            _hasHideShade = true;
        }

        else if (_isLastInstructions)
        {
            var duration = Time.timeSinceLevelLoad - _lastInstructionStartMoment;
            var i = InstructionIndex(_instructions0.LastInstructions, duration, _lastIndex);
            if (i > _lastIndex)
            {
                _tmp.text = _instructions0.LastInstructions[i].content;
                _lastIndex = i;
            }

            if (duration > levelCfg0.showCancelButtonTime && !_hasShowCancelButton)
            {
                ShowCancelButton();
                _hasShowCancelButton = true;
            }

            else if (duration > levelCfg0.allowCancelTime && !_hasAllowCancel)
            {
                Services.CancelButton.Respond = true;
                Services.EventManager.Register<FirstCancel>(OnFirstCancel);
                _hasAllowCancel = true;
            }
        }

        else if (_isGoalPhase)
        {
            var duration = Time.timeSinceLevelLoad - _firstCancelMoment;
            if (duration > levelCfg0.showGoalTime && !_hasShowGoal)
            {
                ShowGoal();
                _hasShowGoal = true;
                _tmp.text = levelCfg0.goalExplanation;
                _targetRB.velocity = Vector2.zero;
            }

            else if (duration > levelCfg0.startDetectTime && !_hasStartDetect)
            {
                Services.Gate.isDetect = true;
                Services.EventManager.Register<Success>(OnSuccess);
                Services.EventManager.Register<LoseLife>(OnLoseLife);
                _hasStartDetect = true;
            }
        }
    }
    
    private void LateUpdate()
    {
        Services.ControllerSquare.LateUpdate();
        Services.TargetSquare.OnLateUpdate();
    }
    private void OnFirstForce(AGPEvent e)
    {
        _isInitialInstruction = false;
        _hasFirstForce = true;
        _firstForceMoment = Time.timeSinceLevelLoad;
        _tmp.text = levelCfg0.whenFirstForce;
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
    }
    
    private void OnSecondForce(AGPEvent e)
    {
        _shadeObj.SetActive(true);
        _hasSecondForce = true;
        _secondForceMoment = Time.timeSinceLevelLoad;
        _tmp.text = levelCfg0.whenSecondForce;
        Services.EventManager.Unregister<SecondForce>(OnSecondForce);
    }

    private void OnFirstCancel(AGPEvent e)
    {
        _isLastInstructions = false;
        _tmp.text = levelCfg0.whenFirstCancel;
        _firstCancelMoment = Time.timeSinceLevelLoad;
        _isGoalPhase = true;
        Services.EventManager.Unregister<FirstCancel>(OnFirstCancel);
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
        _targetRB.velocity = levelCfg0.v0;
    }

    private void ShowCtrlSqr()
    {
        _controlSqrObj.transform.localPosition = new Vector3(6f, -2.5f, 10f);
        Services.ControllerSquare.DrawBoundCircle();
    }

    private void ShowCancelButton()
    {
        _cancelButtonObj.transform.localPosition = new Vector3(-6f, -2.5f, 10f);
        Services.CancelButton.DrawBoundCircle();
    }
    
    private void ShowGoal()
    {
        _gateObj.transform.position = _targetSqrObj.transform.position + new Vector3(6f, 0f, 0f);
        _flagObj.transform.position = _gateObj.transform.position;
        _gateObj.SetActive(true);
        _flagObj.SetActive(true);
    }
    
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

    private void OnSuccess(AGPEvent e)
    {
        _tmp.text = levelCfg0.whenSuccess;
        _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _flagObj.transform.parent = _targetSqrObj.transform;
        var p = Instantiate(levelCfg0.successParticles, _targetSqrObj.transform.position, Quaternion.identity);
        StartCoroutine(WaitAndDestroy(p));
        Services.EventManager.Unregister<Success>(OnSuccess);
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
    }

    private IEnumerator WaitAndDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy);
    }

    private void OnLoseLife(AGPEvent e)
    {
        var totalHintNum = levelCfg0.errorHints.Count;
        if (_failTimes >= totalHintNum)
        {
            _tmp.text = levelCfg0.errorHints[totalHintNum - 1];
            return;
        }
        _tmp.text = levelCfg0.errorHints[_failTimes];
        _failTimes++;
    }
}


