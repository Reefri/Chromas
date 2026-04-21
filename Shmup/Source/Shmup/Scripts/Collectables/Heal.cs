// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Heal : GenericCollectable 
	{
		private float heal = 20;


        protected override void DoOnCollected()
        {
			Player.GetInstance().Heal(heal);
            SoundManager.GetInstance().PlayHealCollectable();

            base.DoOnCollected();

        }
    }
}
