using UnityEngine;
using TMPro;

public class LevelManager2 : LevelManager
{
    private TextMeshPro _tmp;
    public GameObject barrier, gate0, gate1, gate2;
    public LevelCfg2 cfg2;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _tmp = GetComponent<TextMeshPro>();
        var circle = GameObject.FindGameObjectWithTag("GravityButton");
        Services.GravityButton = new GravityButton(circle);
        Services.EventManager.Register<ButtonObjPressed>(OnButtonPressed);
        Services.CameraController.isFollowing = false;
        Services.CameraController.lockY = true;
        Services.EventManager.Register<FirstGravity>(OnFirstGravity);
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Services.GravityButton.Start();
        taskManager.Do(Services.GravityButton.boundCircle.GrowUp);
        new Gravity(targetSqr, Services.GameCfg.gravityColor);
        gate0.SetActive(false);
        gate1.SetActive(false);
        gate2.SetActive(false);
        
        var checkTargetX0 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.cameraFollowX)
            {
                Services.CameraController.isFollowing = true;
                return true;
            }

            return false;
        });
        
        var checkTargetX1 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.loadSecondPartX)
            {
                gate1.SetActive(true);
                return true;
            }

            return false;
        });
        
        var checkTargetX2 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.loadThirdPartX)
            {
                gate2.SetActive(true);
                return true;
            }

            return false;
        });
        checkTargetX0.Then(checkTargetX1).Then(checkTargetX2);
        taskManager.Do(checkTargetX0);
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.G))
            Services.GravityButton.GravitySwitch();
        
        Services.GravityButton.Update();
    }
    
    public override void OnSuccess(AGPEvent e)
    {
        base.OnSuccess(e);
        var wait = new WaitTask(Services.GameCfg.afterSuccessWaitTime);
        var clear = new ActionTask(() => Services.GravityButton.boundCircle.Clear());
        wait.Then(clear);
        taskManager.Do(wait);
        var moreLevels = new WaitAndPrint(_tmp, Services.GameCfg.afterSuccessWaitTime, Services.GameCfg.moreLevels);
        taskManager.Do(moreLevels);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<ButtonObjPressed>(OnButtonPressed);
    }
    private void OnButtonPressed(AGPEvent e)
    {
        var button = (ButtonObjPressed) e;
        barrier.SetActive(!button.isPressed);
        gate0.SetActive(button.isPressed);
    }

    private void OnFirstGravity(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstGravity>(OnFirstGravity);
        _tmp.text = cfg2.gravityInstruction;
        var wait = new WaitTask(cfg2.gravityInstructionDuration);
        var clear = new ActionTask(() => _tmp.text = "");
        wait.Then(clear);
        taskManager.Do(wait);
    }
}


