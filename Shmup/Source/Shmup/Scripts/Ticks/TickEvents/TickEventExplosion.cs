using Com.IsartDigital.Chromaberation;

public partial class TickEventExplosion : TickEvent
{
    protected override void OnBeat()
    {
        base.OnBeat();
        BossA.GetInstance().CreateBumpExplosion();
    }
}
