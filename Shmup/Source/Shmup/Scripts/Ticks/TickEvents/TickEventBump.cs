using Com.IsartDigital.Chromaberation;

public partial class TickEventBump : TickEvent
{
    protected override void OnBeat()
    {
        base.OnBeat();

        BossA.GetInstance().Bump();

    }
}
