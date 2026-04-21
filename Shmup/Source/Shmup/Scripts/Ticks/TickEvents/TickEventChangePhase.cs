using Com.IsartDigital.Chromaberation;
public partial class TickEventChangePhase : TickEvent
{
    protected override void OnBeat()
    {
        base.OnBeat();

        BossManager.GetInstance().ChangePhase();

    }
}
