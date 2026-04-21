// Author : Gramatikoff Sacha

namespace Com.IsartDigital.Chromaberation {
	
	public partial class SmartBombCollectable : GenericCollectable
    {

        protected override void DoOnCollected()
        {
            Player.GetInstance().AddSmartBomb();

            SoundManager.GetInstance().PlaySmartBombCollectable();


            base.DoOnCollected();
        }
    }
}
