using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager0 : LevelManager
{
    public LevelCfg0 cfg0;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private GameObject _shadeObj, _flagObj, _chaseItemObj;
    private List<Task> _initialInstructions;
    private Task _secondForceReminder, _whenFirstForce, _checkDistance;
    private LevelManager _currentLevelManager;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        Services.CameraController.isFollowing = false;
        ctrlSqr.SetActive(false);
        cxlButton.SetActive(false);
        Services.CancelButton.Respond = false;
        _shadeObj = GameObject.FindGameObjectWithTag("Shade");
        _shadeObj.SetActive(false);
        _flagObj = GameObject.FindGameObjectWithTag("Goal");
        _flagObj.SetActive(false);
        _chaseItemObj = GameObject.FindGameObjectWithTag("ChaseItem");
        _chaseItemObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        _instructions0 = Instructions0.Load(cfg0);
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        Services.EventManager.Register<FirstForce>(OnFirstForce);
        Services.EventManager.Register<SecondForce>(OnSecondForce);
        _initialInstructions = new List<Task>();
        
        for (int i = 0; i < _instructions0.InitialInstructions.Count; i++)
        {
            _initialInstructions.Add(new WaitAndPrint(_tmp, _instructions0.InitialInstructions[i].startTime,
                _instructions0.InitialInstructions[i].content));
        }

        for (int i = 0; i < _initialInstructions.Count - 1; i++)
        {
            _initialInstructions[i].Then(_initialInstructions[i + 1]);
        }
        taskManager.Do(_initialInstructions[0]);
        
        var tgtSqrTime = 0f;
        var showTgtSqr = new DelegateTask(() => tgtSqrTime = 0f, () =>
        {
            tgtSqrTime += Time.deltaTime;
            if (tgtSqrTime > cfg0.showTargetSqrTime)
            {
                targetRb.velocity = cfg0.v0;
                return true;
            }

            return false;
        });
        taskManager.Do(showTgtSqr);

        var ctrlSqrTime = 0f;
        var showCtrlSqr = new DelegateTask(() => ctrlSqrTime = 0f, () =>
        {
            ctrlSqrTime += Time.deltaTime;
            if (ctrlSqrTime > cfg0.showCtrlSqrTime)
            {
                ctrlSqr.SetActive(true);
                Services.ControllerSquare.Start();
                taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
                return true;
            }

            return false;
        });
        taskManager.Do(showCtrlSqr);
        
        var checkTarget = new DelegateTask(() => {}, () =>
        {
            if (Services.TargetSquare.transform.position.x > 0)
            {
                Services.CameraController.isFollowing = true;
                return true;
            }

            return false;
        });
        taskManager.Do(checkTarget);
    }

    private void OnFirstForce(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
        foreach (var task in _initialInstructions)
            task.SetStatus(Task.TaskStatus.Success);
        
        _whenFirstForce = new PrintAndWait(_tmp, 2f, cfg0.whenFirstForce);
        taskManager.Do(_whenFirstForce);

        _secondForceReminder = new WaitAndPrint(_tmp, cfg0.secondForceRemindTime, cfg0.secondForceReminder);
        taskManager.Do(_secondForceReminder);
    }
    
    private void OnSecondForce(AGPEvent e)
    {
        Services.EventManager.Unregister<SecondForce>(OnSecondForce);
        _whenFirstForce.SetStatus(Task.TaskStatus.Success);
        _secondForceReminder.SetStatus(Task.TaskStatus.Success);
        var timeElapsed = 0f;
        var whenSecondForce = new DelegateTask(() =>
        {
            _shadeObj.SetActive(true);
            _tmp.text = cfg0.whenSecondForce;
            timeElapsed = 0f;
        }, () =>
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > cfg0.secondForceInstructionDuration)
            {
                _shadeObj.SetActive(false);
                _tmp.text = cfg0.chaseExplanation;
                ShowChaseItem();
                return true;
            }

            return false;
        });
        taskManager.Do(whenSecondForce);
    }

    private void ShowChaseItem()
    {
        _chaseItemObj.SetActive(true);
        Services.EventManager.Register<ShowGoal>(OnShowGoal);
    }

    private void OnShowGoal(AGPEvent e)
    {
        _flagObj.transform.position = _chaseItemObj.transform.position;
        _flagObj.SetActive(true);
        _tmp.text = cfg0.goalExplanation;
        Services.EventManager.Register<Success>(OnSuccess);
        Services.EventManager.Unregister<ShowGoal>(OnShowGoal);
        _checkDistance = new DelegateTask(() => {}, () =>
        {
            if (Vector2.Distance(targetSqr.transform.position, _flagObj.transform.position) > cfg0.failThreshold)
            {
                _chaseItemObj.GetComponent<ChaseItem>().ResetPosition();
                ShowChaseItem();
                _flagObj.SetActive(false);
                _tmp.text = cfg0.lost;
                return true;
            }

            return false;
        });
        taskManager.Do(_checkDistance);
    }

    private void OnSuccess(AGPEvent e)
    {
        _checkDistance.SetStatus(Task.TaskStatus.Success);
        Services.EventManager.Unregister<Success>(OnSuccess);
        _tmp.text = cfg0.whenSuccess;
        var waitForNextLevel = new WaitTask(cfg0.nextLevelLoadTime);
        var transition = new ActionTask(() =>
        {
            _shadeObj.SetActive(true);
            _tmp.text = cfg0.nextLevelText;
            Services.ControllerSquare.Respond = false;
            Services.ControllerSquare.ResetPlayerForce();
            ctrlSqr.SetActive(false);
            Services.ControllerSquare.boundCircle.Clear();
        });
        waitForNextLevel.Then(transition);
        _flagObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _flagObj.transform.parent = targetSqr.transform;
        var p = Instantiate(gameCfg.successParticles, targetSqr.transform.position, Quaternion.identity);
        var wait = new WaitTask(1f);
        var clearParticles = new ActionTask(() => Destroy(p));
        wait.Then(clearParticles);
        taskManager.Do(wait);
        taskManager.Do(waitForNextLevel);
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

public class PrintAndWait : Task
{
    private TextMeshPro _tmp;
    private float _waitTime;
    private string _toPrint;
    private float _elapsedTime;
    public PrintAndWait(TextMeshPro tmp, float duration, string toPrint)
    {
        _tmp = tmp;
        _waitTime = duration;
        _toPrint = toPrint;
    }

    protected override void Initialize()
    {
        _elapsedTime = 0f;
        _tmp.text = _toPrint;
    }

    internal override void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _waitTime)
        {
            _tmp.text = "";
            SetStatus(TaskStatus.Success);
        }
    }
}



