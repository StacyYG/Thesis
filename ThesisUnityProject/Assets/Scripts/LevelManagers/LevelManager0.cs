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
    private List<float> _startWaitTimes;
    private List<float> _durationTimes;
    private List<PrintText> _printTexts;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj;
    private GameObject _targetSqrObj;
    private FiniteStateMachine<LevelManager0> _level0StateMachine;

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
        ParseTexts();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponent<TextMeshPro>();
        SetUpTexts();
        StartCoroutine(Whole());
        
    }

    private IEnumerator Whole()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == levelCfg0.showCtrlSqr)
            {
                ShowCtrlSqr();
            }

            if (i == levelCfg0.showTargetSqr)
            {
                ShowTargetSqr();
            }
            yield return new WaitForSeconds(_startWaitTimes[i]);
            _printTexts[i].Print();
            yield return new WaitForSeconds(_durationTimes[i]);
            _printTexts[i].Clear();
        }
        
        if (Services.ControllerSquare.PlayerForce != Vector2.zero)
        {
            _printTexts[4].Do();
        }
        else
        {
            Services.EventManager.Register<FirstForce>(OnFirstForce);
            yield return new WaitForSeconds(_startWaitTimes[5]);
            _printTexts[5].Print();
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {
        CheckTarget();
    }

    private void FixedUpdate()
    {
        Services.CameraController.Update();
    }
    
    private void ParseTexts()
    {
        _instructions = new List<string>();
        _startWaitTimes = new List<float>();
        _durationTimes = new List<float>();
        for (int i = 0; i < levelCfg0.texts.Count; i++)
        {
            var line = levelCfg0.texts[i].Split('^');
            float t1;
            if (float.TryParse(line[0], out t1)) 
                _startWaitTimes.Add(t1);
            
            _instructions.Add(line[1]);

            float t2;
            if (float.TryParse(line[2], out t2)) 
                _durationTimes.Add(t2);
        }
    }
    
    private void SetUpTexts()
    {
        for (int i = 0; i < _instructions.Count; i++)
        {
            var t = new PrintText(this, _tmp, _instructions[i], _startWaitTimes[i], _durationTimes[i]);
            _printTexts.Add(t);
        }
    }

    private void OnFirstForce(AGPEvent e)
    {
        _printTexts[levelCfg0.firstForce].Do();

        Debug.Log("yes");
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
    }

    private IEnumerator WaitAndPrintText(int index = 0)
    {
        if (index >= _instructions.Count)
        {
            yield break;
        }
        
        if (index == levelCfg0.showCtrlSqr)
        {
            ShowCtrlSqr();
        }

        if (index == levelCfg0.showTargetSqr)
        {
            ShowTargetSqr();
        }
        yield return new WaitForSeconds(_startWaitTimes[index]);
        _tmp.text = _instructions[index];

        StartCoroutine(WaitAndDeleteText(index));
    }
    
    private IEnumerator WaitAndDeleteText(int index = 0)
    {
        if (index >= _instructions.Count)
        {
            yield break;
        }
        
        yield return new WaitForSeconds(_durationTimes[index]);
        _tmp.text = "";

        StartCoroutine(WaitAndPrintText(index + 1));
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

    public PrintText(MonoBehaviour mono, TextMeshPro tmp, string toPrint, float startWaitTime, float durationTime)
    {
        _mono = mono;
        _tmp = tmp;
        _toPrint = toPrint;
        _startWaitTime = startWaitTime;
        _durationTime = durationTime;
    }

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
    }
    
}

