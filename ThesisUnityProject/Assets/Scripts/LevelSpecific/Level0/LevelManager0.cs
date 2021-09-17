using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager0 : LevelManager
{
    public LevelCfg0 cfg0;
    private TextMeshPro _tmp;
    private Instructions0 _instructions0;
    private GameObject _chaseItemObj;
    private List<Task> _initialInstructions;
    private Task _secondForceReminder;
    private LevelManager _currentLevelManager;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        Services.CameraController.isFollowing = false;
        ctrlButton.SetActive(false);
        _chaseItemObj = GameObject.FindGameObjectWithTag("ChaseItem");
        _chaseItemObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        _instructions0 = Instructions0.Load(cfg0);
        flagObj.SetActive(false);
    }
    
    public override void Start()
    {
        Services.GameController.ShowMenu(false);
        Services.ControlButton.Init();
        Services.EventManager.Register<FirstForce>(OnFirstForce);
        Services.EventManager.Register<SecondForce>(OnSecondForce);
        Services.VelocityLine = new VelocityLine(targetSqr);
        targetSqr.GetComponent<Path>().SetUpNewPath();
        
        targetRb.velocity = cfg0.v0;
        
        ShowInitialInstructions();
        
        ShowButton();
    }

    private void ShowInitialInstructions()
    {
        _initialInstructions = new List<Task>();
        for (int i = 0; i < _instructions0.initialInstructions.Count; i++)
        {
            _initialInstructions.Add(new WaitAndPrint(_tmp, _instructions0.initialInstructions[i].startTime,
                _instructions0.initialInstructions[i].content));
        }

        for (int i = 0; i < _initialInstructions.Count - 1; i++)
        {
            _initialInstructions[i].Then(_initialInstructions[i + 1]);
        }

        taskManager.Do(_initialInstructions[0]);
    }
    
    private void ShowButton()
    {
        var wait = new WaitTask(cfg0.showCtrlSqrTime);
        var showButton = new ActionTask(() =>
        {
            ctrlButton.SetActive(true);
            taskManager.Do(Services.ControlButton.boundCircle.GrowUp);
        });
        wait.Then(showButton);
        taskManager.Do(wait);
    }

    private void OnFirstForce(AGPEvent e) // Instructions when player adds the first force
    {
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
        foreach (var task in _initialInstructions)
            task.SetStatus(Task.TaskStatus.Success);

        var print1 = new ActionTask(() => { _tmp.text = cfg0.whenFirstForce; });
        var wait1 = new WaitTask(2f);
        var clear = new ActionTask(() => { _tmp.text = "";});
        var wait2 = new WaitTask(cfg0.secondForceRemindTime);
        var print2 = new ActionTask(() => { _tmp.text = cfg0.secondForceReminder;});
        _secondForceReminder = print2;
        
        print1.Then(wait1).Then(clear).Then(wait2).Then(print2);

        taskManager.Do(print1);
    }
    
    private void OnSecondForce(AGPEvent e) // Instructions when player adds the second force
    {
        taskManager.ClearTasks();
        var shade = Instantiate(gameCfg.shade);
        var instruction = new ActionTask(() =>
        {
            Services.EventManager.Unregister<SecondForce>(OnSecondForce);
            _tmp.text = cfg0.whenSecondForce;
            Services.VelocityLine.Hide(true);
        });

        var wait = new WaitTask(cfg0.secondForceInstructionDuration);

        var resume = new ActionTask(() =>
        {
            Destroy(shade);
            Services.VelocityLine.Hide(false);
            _tmp.text = cfg0.chaseExplanation;
            ShowChaseItem();
        });

        instruction.Then(wait).Then(resume);
        taskManager.Do(instruction);
    }
    
    private void ShowChaseItem()
    {
        _chaseItemObj.SetActive(true);
        Services.EventManager.Register<ShowGoal>(OnShowGoal);
    }

    private void OnShowGoal(AGPEvent e) // Show the flag when the chased item reach the last task position
    {
        flagObj.transform.position = _chaseItemObj.transform.position;
        flagObj.SetActive(true);
        _tmp.text = cfg0.goalExplanation;
        Services.EventManager.Register<Success>(OnSuccess);
        Services.EventManager.Unregister<ShowGoal>(OnShowGoal);
    }

    protected override void OnSuccess(AGPEvent e)
    {
        base.OnSuccess(e);
        var congrats = new ActionTask(() => {_tmp.text = Services.GameCfg.whenSuccess;});
        var wait = new WaitTask(Services.GameCfg.afterSuccessWaitTime);
        congrats.Then(wait);
        taskManager.Do(congrats);
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



