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
        Services.ControllerSquare.Awake();
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
        Services.VelocityBar = new VelocityBar(GameObject.FindGameObjectWithTag("SpeedBar").transform,
            GameObject.FindGameObjectWithTag("DirectionPointer").transform, _targetRB);
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
        Services.VelocityBar.Update();
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

            if (_controlButtonGrowing)
            {
                _controlButtonGrowTimer += Time.deltaTime;
                if (Services.ControllerSquare.boundCircle.GrownUp(_controlButtonGrowTimer)) 
                    _controlButtonGrowing = false;
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
        
        else if (_hasFirstForce && !_hasSecondForce && !_hasRemind)
        {
            _firstForceTimer += Time.deltaTime;
            if (_firstForceTimer > levelCfg0.secondForceRemindTime)
            {
                _tmp.text = levelCfg0.secondForceReminder;
                _hasRemind = true;
            }
        }

        else if (_hasSecondForce && !_hasHideShade)
        {
            _secondForceTimer += Time.deltaTime;
            if (_secondForceTimer > levelCfg0.secondForceInstructionDuration)
            {
                Destroy(_shadeObj);
                _tmp.text = "";
                _lastIndex = -1;
                _isCancelInstructions = true;
                _hasHideShade = true;
            }
        }

        else if (_isCancelInstructions)
        {
            _cancelInstructionTimer += Time.deltaTime;
            var i = InstructionIndex(_instructions0.LastInstructions, _cancelInstructionTimer, _lastIndex);
            if (i > _lastIndex)
            {
                _tmp.text = _instructions0.LastInstructions[i].content;
                _lastIndex = i;
            }

            if (_cancelButtonGrowing)
            {
                _cancelButtonGrowTimer += Time.deltaTime;
                if (Services.CancelButton.boundCircle.GrownUp(_cancelButtonGrowTimer))
                    _cancelButtonGrowing = false;
            }
            if (_cancelInstructionTimer > levelCfg0.showCancelButtonTime && !_hasShowCancelButton)
            {
                ShowCancelButton();
                _hasShowCancelButton = true;
            }
            
            else if (_cancelInstructionTimer > levelCfg0.allowCancelTime && !_hasAllowCancel)
            {
                Services.CancelButton.Respond = true;
                Services.EventManager.Register<FirstCancel>(OnFirstCancel);
                _hasAllowCancel = true;
            }
        }
        
        else if (_isChasePhase)
        {
            _chaseTimer += Time.deltaTime;
        }

        else if (_isGoalPhase)
        {
            _goalTimer += Time.deltaTime;
            if (_goalTimer > levelCfg0.showGoalTime && !_hasShowGoal)
            {
                ShowGoal();
                _hasShowGoal = true;
                _tmp.text = levelCfg0.goalExplanation;
                _targetRB.velocity = Vector2.zero;
            }

            else if (_goalTimer > levelCfg0.startDetectTime && !_hasStartDetect)
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
        _tmp.text = levelCfg0.whenFirstForce;
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
    }
    
    private void OnSecondForce(AGPEvent e)
    {
        _shadeObj.SetActive(true);
        _hasSecondForce = true;
        _tmp.text = levelCfg0.whenSecondForce;
        Services.EventManager.Unregister<SecondForce>(OnSecondForce);
    }

    private void OnFirstCancel(AGPEvent e)
    {
        _isCancelInstructions = false;
        _tmp.text = levelCfg0.whenFirstCancel;
        _isChasePhase = true;
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
        _controlButtonGrowing = true;
    }

    private void ShowCancelButton()
    {
        _cancelButtonObj.transform.localPosition = new Vector3(-6f, -2.5f, 10f);
        _cancelButtonGrowing = true;
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


