using Godot;
using System.Linq;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{

	public partial class EnemySpawner : Node2D
	{

		[Export] private float marginBeforeActivating = 0;
		private bool isActive = false;

		[Export] int colorIndex = 0;

        public override void _Ready()
        {
            base._Ready();
            foreach (Enemy lEnemy in GetChildren())
            {
                lEnemy.colorIndex += colorIndex;
                lEnemy.SetColorToIndex();
            }
        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			if (LevelManager.GetInstance().isActive && !isActive && CameraManager.GetInstance().GetLevelXBondaries().Y > GlobalPosition.X + marginBeforeActivating) { Start();  }

        }


        private void Start()
		{
			foreach (Node2D lChild in GetChildren().OfType<Enemy>())
			{
				((Enemy)lChild).FullStart();
			}

			isActive = true;
		}
    }

}
