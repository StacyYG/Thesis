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
        Services.EventManager.Register<ButtonObjPressed>(OnButtonPressed);
        Services.CameraController.isFollowing = false;
        Services.CameraController.lockY = true;
        Services.EventManager.Register<FirstGravity>(OnFirstGravity);
    }
    
    public override void Start()
    {
        base.Start();

        SpinComets();

        gate0.SetActive(false);
        gate1.SetActive(false);
        gate2.SetActive(false);
        
        // Track the player progress to adjust the camera and gates
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

    private static void SpinComets()
    {
        var comets = GameObject.FindGameObjectsWithTag("Comet");
        foreach (var comet in comets)
        {
            comet.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-0.3f, 0.3f));
        }
    }

    protected override void OnSuccess(AGPEvent e)
    {
        base.OnSuccess(e);
        
        // remove the gravity button
        var wait = new WaitTask(Services.GameCfg.afterSuccessWaitTime);
        var clear = new ActionTask(() => Services.GravityButton.boundCircle.Clear());
        wait.Then(clear);
        taskManager.Do(wait);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Services.EventManager.Unregister<ButtonObjPressed>(OnButtonPressed);
    }
    
    private void OnButtonPressed(AGPEvent e) // When the in game button object is pressed, deactivate the barrier and activate the gate
    {
        var button = (ButtonObjPressed) e;
        barrier.SetActive(!button.isPressed);
        gate0.SetActive(button.isPressed);
    }

    private void OnFirstGravity(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstGravity>(OnFirstGravity);
        var showGravityInstruction = new PrintAndWait(_tmp, cfg2.gravityInstructionDuration, cfg2.gravityInstruction);
        taskManager.Do(showGravityInstruction);
    }
}


