using Com.IsartDigital.Chromaberation;
using Godot;
using System;

public partial class TickEventAttack : TickEvent
{
    protected override void OnBeat()
    {
        base.OnBeat();

        BossA.GetInstance().Attack();

    }
}
