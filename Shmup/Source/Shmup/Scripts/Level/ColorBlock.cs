using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{

	public partial class ColorBlock : ColoredEntity
    {

		[Export] public float margin = 100;

		private LevelParralaxe parent;

		private bool isOnScreen = false;

        public override void _Ready()
		{
			parent = ((LevelParralaxe)GetParent());
			colorGivenTroughtCode = false;
			base._Ready();
			speed = Vector2.Zero * parent.scrollingSpeed;
			direction = Vector2.Left;
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			if (!isOnScreen)
			{
				if (GlobalPosition.X - margin < CameraManager.GetInstance().GetLevelXBondaries().Y)
				{
					isOnScreen = true;
				}
				else
				{
					return;
				}
			}
            base._Process(lDelta);


		}


	
	}
}
