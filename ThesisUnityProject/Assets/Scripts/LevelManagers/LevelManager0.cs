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
    private List<float> _startWaitTime;
    private List<float> _durationTime;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqr;
    private GameObject _targetSqr;

    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        Services.MyCamera = Camera.main;
        _controlSqr = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = _controlSqr.GetComponent<ControllerSquare>();

        _targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqr.GetComponent<TargetSquare>();
        _targetRB = _targetSqr.GetComponent<Rigidbody2D>();

        Services.CameraController = new CameraController(Services.MyCamera, false, Services.TargetSquare.transform);
        ParseTexts();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponent<TextMeshPro>();
        StartCoroutine(WaitAndPrintText());
        
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
        _startWaitTime = new List<float>();
        _durationTime = new List<float>();
        for (int i = 0; i < levelCfg0.texts.Count; i++)
        {
            var line = levelCfg0.texts[i].Split('^');
            float t1;
            if (float.TryParse(line[0], out t1)) 
                _startWaitTime.Add(t1);
            
            _instructions.Add(line[1]);

            float t2;
            if (float.TryParse(line[2], out t2)) 
                _durationTime.Add(t2);
        }
    }

    private IEnumerator WaitAndPrintText(int index = 0)
    {
        if (index >= _instructions.Count)
        {
            yield break;
        }
        
        yield return new WaitForSeconds(_startWaitTime[index]);
        _tmp.text = _instructions[index];
        if (index == levelCfg0.showCtrlSqr)
        {
            ShowCtrlSqr();
        }

        if (index == levelCfg0.showTargetSqr)
        {
            ShowTargetSqr();
        }

        StartCoroutine(WaitAndDeleteText(index));
    }

    private IEnumerator WaitAndDeleteText(int index = 0)
    {
        if (index >= _instructions.Count)
        {
            yield break;
        }
        
        yield return new WaitForSeconds(_durationTime[index]);
        _tmp.text = "";

        StartCoroutine(WaitAndPrintText(index + 1));
    }

    private void ShowTargetSqr()
    {
        _targetRB.velocity = levelCfg0.v0;
    }

    private void ShowCtrlSqr()
    {
        _controlSqr.transform.localPosition = new Vector3(6f, -2.5f, 10f);
        Services.ControllerSquare.DrawBoundCircle(_controlSqr.transform.position);
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

