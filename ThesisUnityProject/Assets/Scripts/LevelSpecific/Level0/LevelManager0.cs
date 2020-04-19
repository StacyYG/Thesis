using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LevelManager0 : MonoBehaviour
{
    public LevelCfg0 cfg0;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _shadeObj, _flagObj, _chaseItem;
    public GameCfg gameCfg;

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
        Services.CameraController = new CameraController(Services.MainCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _flagObj.SetActive(false);
        Services.VelocityBar = new VelocityBar(GameObject.FindGameObjectWithTag("SpeedBar").transform,
            GameObject.FindGameObjectWithTag("DirectionPointer").transform, _targetRB,
            GameObject.FindGameObjectWithTag("SpeedWarning"));
        _chaseItem = GameObject.FindGameObjectWithTag("ChaseItem");
        _chaseItem.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        _instructions0 = Instructions0.Load(cfg0);
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
            if (duration > cfg0.showTargetSqrTime && !_hasShowTargetSqr)
            {
                ShowTargetSqr();
                _hasShowTargetSqr = true;
            }

            else if (duration > cfg0.showCtrlSqrTime && !_hasShowCtrlSqr)
            {
                ShowCtrlSqr();
                Services.EventManager.Register<FirstForce>(OnFirstForce);
                Services.EventManager.Register<SecondForce>(OnSecondForce);
                _hasShowCtrlSqr = true;
            }
        }
        
        else if (_hasFirstForce && !_hasSecondForce && !_hasRemind)
        {
            _firstForceTimer += Time.deltaTime;
            if (_firstForceTimer > cfg0.secondForceRemindTime)
            {
                _tmp.text = cfg0.secondForceReminder;
                _hasRemind = true;
            }
        }

        else if (_hasSecondForce && !_hasHideShade)
        {
            _secondForceTimer += Time.deltaTime;
            if (_secondForceTimer > cfg0.secondForceInstructionDuration)
            {
                Destroy(_shadeObj);
                _tmp.text = "";
                _lastIndex = -1;
                _hasHideShade = true;
                _chaseItem.SetActive(true);
                Services.EventManager.Register<ShowGate>(OnShowGate);
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
        _targetRB.velocity = Vector2.zero;
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
        _targetRB.velocity = cfg0.v0;
    }

    private void ShowCtrlSqr()
    {
        _controlSqrObj.transform.localPosition = new Vector3(6f, -2.5f, 10f);
        _controlButtonGrowing = true;
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
            Services.CameraController.IsFollowing = true;
            _checked = true;
        }
    }

    private void OnSuccess(AGPEvent e)
    {
        _tmp.text = cfg0.whenSuccess;
        _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _flagObj.transform.parent = _targetSqrObj.transform;
        var p = Instantiate(gameCfg.successParticles, _targetSqrObj.transform.position, Quaternion.identity);
        StartCoroutine(WaitAndDestroy(p));
        Services.EventManager.Unregister<Success>(OnSuccess);
    }

    private IEnumerator WaitAndDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy);
    }
    
}


