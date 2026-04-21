// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class LevelUp : GenericCollectable
	{

        protected override void DoOnCollected()
        {

			Player.GetInstance().LevelUp();
            SoundManager.GetInstance().PlayLevelUpCollectable();

            base.DoOnCollected();

        }
    }
}
