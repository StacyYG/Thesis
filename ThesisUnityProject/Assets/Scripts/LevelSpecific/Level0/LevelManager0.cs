using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager0 : MonoBehaviour
{
    public LevelCfg0 levelCfg0;
    private TextMeshPro _tmp;
    private List<string> _instructions;
    private List<float> _startTimes;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj, _shade, _goal;

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
        _isGoal,
        _hasShowGoal;
    private BoxCollider2D _ctrlSqrCollider;
    private bool _isInitialInstruction = true;
    private float _timeSinceFirstForce,
        _firstForceMoment,
        _secondForceMoment,
        _lastInstructionStartMoment,
        _firstCancelMoment;
    private int _lastIndex;
    private CircleCollider2D _cancelButtonCollider;

    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        Services.MyCamera = Camera.main;
        _controlSqrObj = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = _controlSqrObj.GetComponent<ControllerSquare>();
        _targetSqrObj = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqrObj.GetComponent<TargetSquare>();
        _targetRB = _targetSqrObj.GetComponent<Rigidbody2D>();
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = _cancelButtonObj.GetComponent<CancelButton>();
        Services.CameraController = new CameraController(Services.MyCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _ctrlSqrCollider = _controlSqrObj.GetComponent<BoxCollider2D>();
        _ctrlSqrCollider.enabled = false;
        _cancelButtonCollider = _cancelButtonObj.GetComponent<CircleCollider2D>();
        _cancelButtonCollider.enabled = false;
        _shade = GameObject.FindGameObjectWithTag("Shade");
        _shade.SetActive(false);
        _goal = GameObject.FindGameObjectWithTag("Goal");
        _goal.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        ParseTexts(levelCfg0.initialInstructions);
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }
    


    
    // Update is called once per frame
    void Update()
    {
        CheckTarget();
        if (_isInitialInstruction)
        {
            var duration = Time.timeSinceLevelLoad;
            var i = InstructionIndex(duration, _lastIndex);
            if (i > _lastIndex)
            {
                _tmp.text = _instructions[i];
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
                _ctrlSqrCollider.enabled = true;
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
            Destroy(_shade);
            _tmp.text = "";
            ParseTexts(levelCfg0.lastInstructions);
            _isLastInstructions = true;
            _hasHideShade = true;
        }

        else if (_isLastInstructions)
        {
            var duration = Time.timeSinceLevelLoad - _lastInstructionStartMoment;
            var i = InstructionIndex(duration, _lastIndex);
            if (i > _lastIndex)
            {
                _tmp.text = _instructions[i];
                _lastIndex = i;
            }

            if (duration > levelCfg0.showCancelButtonTime && !_hasShowCancelButton)
            {
                ShowCancelButton();
                _hasShowCancelButton = true;
            }

            else if (duration > levelCfg0.allowCancelTime && !_hasAllowCancel)
            {
                _cancelButtonCollider.enabled = true;
                Services.EventManager.Register<FirstCancel>(OnFirstCancel);
            }
        }

        else if (_isGoal)
        {
            var duration = Time.timeSinceLevelLoad - _firstCancelMoment;
            if (duration > levelCfg0.showGoalTime && !_hasShowGoal)
            {
                ShowGoal();
                _hasShowGoal = true;
                _tmp.text = levelCfg0.goalExplanation;
            }
        }

    }

    private void ParseTexts(List<string> toParse)
    {
        _instructions = new List<string>();
        _startTimes = new List<float>();
        _lastIndex = -1;
        for (int i = 0; i < toParse.Count; i++)
        {
            var line = toParse[i].Split('^');
            float t1;
            if (float.TryParse(line[0], out t1)) 
                _startTimes.Add(t1);
            _instructions.Add(line[1]);
        }
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
        _shade.SetActive(true);
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
        _isGoal = true;
        Services.EventManager.Unregister<FirstCancel>(OnFirstCancel);
    }
    
    private int InstructionIndex(float time, int i)
    {
        if (i >= _startTimes.Count - 1)
        {
            return _startTimes.Count - 1;
        }
        if (time < _startTimes[i + 1])
        {
            return i;
        }
        else
        {
            return InstructionIndex(time, i + 1);
        }
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
        _goal.transform.position = transform.position + new Vector3(8f, 0f, 0f);
        _goal.SetActive(true);
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
            Services.CameraController._isFollowing = true;
            _checked = true;
        }
    }
}


