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
    private List<float> _durationTimes;
    private List<PrintText> _printTexts;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj;
    private GameObject _targetSqrObj;
    private FiniteStateMachine<LevelManager0> _level0StateMachine;
    private MonitorPlayerAction _monitor;
    private bool _hasShowTargetSqr, _hasShowCtrlSqr, _hasAllowControl;
    private BoxCollider2D _ctrlSqrCollider;
    private bool _stopTalking;
    private GameObject _shade;
    private float _timeSinceFirstForce;
    private bool _hasFirstForce;
    private bool _hasRemind;

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
        Services.CameraController = new CameraController(Services.MyCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _printTexts = new List<PrintText>();
        _monitor = _targetSqrObj.GetComponent<MonitorPlayerAction>();
        _ctrlSqrCollider = _controlSqrObj.GetComponent<BoxCollider2D>();
        _ctrlSqrCollider.enabled = false;
        _shade = GameObject.FindGameObjectWithTag("Shade");
        _shade.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        ParseTexts();
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }
    


    
    // Update is called once per frame
    void Update()
    {
        CheckTarget();
        if (!_stopTalking)
        {
            _tmp.text = _instructions[InstructionIndex(0)];
        }
        
        if (Time.timeSinceLevelLoad > levelCfg0.showTargetSqrTime && !_hasShowTargetSqr)
        {
            ShowTargetSqr();
            _hasShowTargetSqr = true;
        }

        if (Time.timeSinceLevelLoad > levelCfg0.showCtrlSqrTime && !_hasShowCtrlSqr)
        {
            ShowCtrlSqr();
            _hasShowCtrlSqr = true;
        }

        if (Time.timeSinceLevelLoad > levelCfg0.allowControlTime && !_hasAllowControl)
        {
            _ctrlSqrCollider.enabled = true;
            Services.EventManager.Register<FirstForce>(OnFirstForce);
            Services.EventManager.Register<SecondForce>(OnSecondForce);
            _hasAllowControl = true;
        }

        if (_hasFirstForce)
        {
            _timeSinceFirstForce += Time.deltaTime;
        }

        if (_timeSinceFirstForce > levelCfg0.secondForceRemindTime && !_hasRemind)
        {
            _tmp.text = levelCfg0.secondForceReminder;
            _hasRemind = true;
        }
    }

    private void FixedUpdate()
    {
        
    }
    
    private void ParseTexts()
    {
        _instructions = new List<string>();
        _startTimes = new List<float>();
        for (int i = 0; i < levelCfg0.texts.Count; i++)
        {
            var line = levelCfg0.texts[i].Split('^');
            float t1;
            if (float.TryParse(line[0], out t1)) 
                _startTimes.Add(t1);
            
            _instructions.Add(line[1]);
        }
    }
    

    private void OnFirstForce(AGPEvent e)
    {
        _stopTalking = true;
        _hasFirstForce = true;
        _tmp.text = levelCfg0.whenFirstForce;
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
    }
    
    private void OnSecondForce(AGPEvent e)
    {
        _shade.SetActive(true);
        _tmp.text = levelCfg0.whenSecondForce;
        Services.EventManager.Unregister<SecondForce>(OnSecondForce);
    }

    private int InstructionIndex(int i)
    {
        if (i >= _startTimes.Count - 1)
        {
            return _startTimes.Count - 1;
        }
        if (Time.timeSinceLevelLoad < _startTimes[i + 1])
        {
            return i;
        }
        else
        {
            return InstructionIndex(i + 1);
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

public abstract class Command
{
    public abstract void Do();
}

public class PrintText : Command
{
    private MonoBehaviour _mono;
    private TextMeshPro _tmp;
    private string _toPrint;
    private float _startWaitTime;
    private float _durationTime;

//    public PrintText(MonoBehaviour mono, TextMeshPro tmp, string toPrint, float startWaitTime, float durationTime, bool waitForResponse = false,)
//    {
//        _mono = mono;
//        _tmp = tmp;
//        _toPrint = toPrint;
//        _startWaitTime = startWaitTime;
//        _durationTime = durationTime;
//    }

    public void Print()
    {
        _tmp.text = _toPrint;
    }

    public void Clear()
    {
        _tmp.text = "";
    }

    public override void Do()
    {
        _mono.StartCoroutine(TextAction());
    }
    public IEnumerator TextAction()
    {
        yield return new WaitForSeconds(_startWaitTime);
        _tmp.text = _toPrint;
        yield return new WaitForSeconds(_durationTime);
        _tmp.text = "";
        nextCommand.Do();
    }

    public Command nextCommand;
}
public class Check : Command
{
    private Command _nextCommandIfTrue;
    private Command _nextCommandIfFalse;
    private int _playerForceNumber;
    private int _threshold;

    public Check(Command nextCommandIfTrue, Command nextCommandIfFalse, int playerForceNumber, int threshold)
    {
        _nextCommandIfTrue = nextCommandIfTrue;
        _nextCommandIfFalse = nextCommandIfFalse;
        _playerForceNumber = playerForceNumber;
        _threshold = threshold;
    }
    public override void Do()
    {
        if (_playerForceNumber >= _threshold )
        {
            _nextCommandIfTrue.Do();
        }
        else
        {
            _nextCommandIfFalse.Do();
        }
    }
}

public class WaitForPlayerResponse : Command
{
    private Command _nextCommand;
    private EventManager _eventManager;
    public override void Do()
    {
        throw new NotImplementedException();
    }

    public WaitForPlayerResponse(Command nextCommand, EventManager eventManager)
    {
        _nextCommand = nextCommand;
        _eventManager = eventManager;
    }
}

//    private abstract class InstructionState : FiniteStateMachine<LevelManager0>.State
//    { 
//        public override void OnEnter()
//        {
//        }
//
//        public override void Update()
//        {
//        }
//
//        public override void OnExit()
//        {
//        }
//    }
//
//    private class Initial : InstructionState
//    {
//        public override void OnEnter()
//        {
//            Context.StartCoroutine(InitialInstructions());
//        }
//
//        private IEnumerator InitialInstructions()
//        {
//            for (int i = 0; i < 5; i++)
//            {
//                if (i == Context.levelCfg0.showCtrlSqrIndex)
//                {
//                    Context.ShowCtrlSqr();
//                }
//
//                if (i == Context.levelCfg0.showTargetSqrIndex)
//                {
//                    Context.ShowTargetSqr();
//                }
//                yield return new WaitForSeconds(Context._startWaitTimes[i]);
//                Context._printTexts[i].Print();
//                yield return new WaitForSeconds(Context._durationTimes[i]);
//                Context._printTexts[i].Clear();
//            }
//
//        }
//    }
//
//    private class AlreadyForce : InstructionState
//    {
//        public override void OnEnter()
//        {
//            Context._printTexts[Context.levelCfg0.alreadyForceIndex].Do();
//        }
//
//        private IEnumerator a()
//        {
//
//        }
//    }

