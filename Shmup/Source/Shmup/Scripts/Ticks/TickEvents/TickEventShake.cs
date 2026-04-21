namespace Com.IsartDigital.Chromaberation
{
    public partial class TickEventShake : TickEvent
    {

        protected override void OnBeat()
        {
            base.OnBeat();
            BossA.GetInstance().Shake();
        }
    }
}