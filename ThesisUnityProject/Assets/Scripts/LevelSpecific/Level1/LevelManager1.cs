using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager1 : LevelManager
{
    public LevelCfg1 cfg1;
    private TextMeshPro _tmp;
    private GameObject _gateObj;
    private int _failTimes;
    private List<PrintAndWait> _printTasks;
    private WaitAndPrint _firstForceReminder;
    private BoxCollider2D _gateCollider;
    
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _tmp = GetComponent<TextMeshPro>();
        _gateCollider = GameObject.FindGameObjectWithTag("GateConstant").GetComponent<BoxCollider2D>();
        Services.CameraController.lockY = true;
        Services.EventManager.Register<Hurt>(OnHurt);
        Services.EventManager.Register<Success>(OnSuccess);
        Services.EventManager.Register<FirstForce>(OnFirstForce);
        Services.EventManager.Register<FirstCancel>(OnFirstCancel);
        _printTasks = new List<PrintAndWait>();
        foreach (var instruction in cfg1.failInstructions)
        {
            _printTasks.Add(new PrintAndWait(_tmp, instruction.duration, instruction.content));
        }
    }
    
    public override void Start()
    {
        Services.GameController.ShowMenu(false);
        Services.ControlButton.Init();
        Services.CancelButton.CreateCircle();
        Services.VelocityLine = new VelocityLine(targetSqr);
        taskManager.Do(Services.ControlButton.boundCircle.GrowUp);
        _firstForceReminder = new WaitAndPrint(_tmp, cfg1.waitTime, cfg1.firstForceReminder);
        taskManager.Do(_firstForceReminder);
    }

    private void OnHurt(AGPEvent e)
    {
        _failTimes++;
        ShowHint(_failTimes); // Show different hints based on player fail times
    }

    private void ShowHint(int failTimes)
    {
        switch (failTimes)
        {
            case 3:
                taskManager.Do(_printTasks[0]);
                break;
            
            case 6:
                taskManager.Do(_printTasks[1]);
                _printTasks[0].SetStatus(Task.TaskStatus.Success);
                
                // Highlight the velocity line
                float timer = 0f;
                var shine = new DelegateTask(() => { Services.VelocityLine.MultiplyWidth(2.5f); }, () =>
                {
                    timer += Time.deltaTime;
                    Services.VelocityLine.SetColor(Color.Lerp(Services.GameCfg.velocityLineHighlightColor,
                        Color.red, Mathf.PingPong(Time.time * 4f, 1)));
                    if (timer > cfg1.failInstructions[1].duration)
                    {
                        Services.VelocityLine.SetColor(Services.GameCfg.velocityLineColor);
                        Services.VelocityLine.MultiplyWidth(0.4f);
                        return true;
                    }
                    return false;
                });
                taskManager.Do(shine);
                break;
            
            case 9:
                taskManager.Do(_printTasks[2]);
                _printTasks[1].SetStatus(Task.TaskStatus.Success);
                break;
        }
    }

    protected override void OnSuccess(AGPEvent e)
    {
        base.OnSuccess(e);
        Services.EventManager.Unregister<Hurt>(OnHurt);
    }

    private void OnFirstForce(AGPEvent e) // Wait and show the cancel button
    {
        Services.EventManager.Unregister<FirstForce>(OnFirstForce);
        _firstForceReminder.SetStatus(Task.TaskStatus.Success);
        _tmp.text = "";
        taskManager.Do(new WaitAndPrint(_tmp, cfg1.showCancelButtonTime, cfg1.cancelInstruction));
        var wait = new WaitTask(cfg1.showCancelButtonTime);
        var showCancelButton = new ActionTask(() =>
        {
            cxlButton.SetActive(true);
            taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
        });
        wait.Then(showCancelButton);
        taskManager.Do(wait);
        
        _gateCollider.isTrigger = true;
    }

    private void OnFirstCancel(AGPEvent e) // Respond to the cancel action
    {
        Services.EventManager.Unregister<FirstCancel>(OnFirstCancel);
        var congrats = new PrintAndWait(_tmp, cfg1.duration, cfg1.whenFirstCancel);
        taskManager.Do(congrats);
    }
}

public class Hurt : AGPEvent{} // A failed attempt to pass the gate
    

