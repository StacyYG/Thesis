public class LevelManager3 : LevelManager
{
    public override void Start()
    {
        base.Start();
        Services.GravityButton.GravitySwitch();
    }
}
