using Com.IsartDigital.Chromaberation;

public partial class TickEventColor : TickEvent
{
    protected override void OnBeat()
    {
        base.OnBeat();
        BossA.GetInstance().NextColor();
    }
}
