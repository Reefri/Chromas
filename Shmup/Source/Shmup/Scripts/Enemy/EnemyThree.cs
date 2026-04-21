using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	public partial class EnemyThree : EnemyTwo
	{

		
		private Timer homingMissileTimer = new Timer();
		private float homingMissileTime = 0.3f;
		private int numberofHomingMissile = 10; 
		private int homingMissileCount = 0;

		private float screenMargin = 50;


		private const string HOMINGMISSILE_SCENE_PATH = "res://Scenes/ColoredEntity/Enemy/HomingMissile.tscn";
		private PackedScene homingMissileFactory = (PackedScene)GD.Load(HOMINGMISSILE_SCENE_PATH);

        private const string CANNONCONTAINER_NODE_PATH = "CannonContainer";
		private Node2D cannonContainer;
		private int cannonNumber;

		private Vector2 ybondaries;

        public override void _Ready()
		{

			hp = 500;

            score = 300;

			base._Ready();

            cannonContainer = (Node2D)GetNode(CANNONCONTAINER_NODE_PATH);
			cannonNumber = cannonContainer.GetChildren().Count;


			homingMissileTimer.WaitTime = homingMissileTime;
			homingMissileTimer.Autostart = false;
			homingMissileTimer.OneShot = false;
			homingMissileTimer.Timeout += ShootingHomingNextStep;
			AddChild(homingMissileTimer);

			paternTime = 5;


			numberOfShooting = 4;

			numberOfTeleportation = 1;

			teleportationInTweenDuration = 1;
			teleportationOutTweenDuration = 1;


            ybondaries = CameraManager.GetInstance().GetLevelYBondaries();


        }
        protected override void ShootingNextStep()
        {
            Shoot();

            shootingCount++;

            if (shootingCount >= numberOfShooting)
            {
                shootingCount = 0;
                shootingTimer.Stop();
                homingMissileTimer.Start();
            }
        }

		private void ShootingHomingNextStep()
		{
            ShootHomingMissile();

            homingMissileCount++;

            if (homingMissileCount >= numberofHomingMissile)
            {
                homingMissileCount = 0;
                homingMissileTimer.Stop();
                paternTimer.Start();
            }
        }

		private void ShootHomingMissile()
		{
            HomingMissile lHomingMissile = (HomingMissile)homingMissileFactory.Instantiate();

            Node2D lCannon = GetRandomCannon();
            lHomingMissile.GlobalPosition = lCannon.GlobalPosition;
			lHomingMissile.color = color;



            lHomingMissile.direction = (lCannon.GlobalPosition -GlobalPosition).Normalized();

            GameManager.GetInstance().GetBulletContainer().AddChild(lHomingMissile);

        }

		private Node2D GetRandomCannon()
		{
			return (Node2D)cannonContainer.GetChildren()[GD.RandRange(0,cannonNumber-1)];
		}

        protected override Vector2 FindNextPosition()
        {


            return new Vector2(GlobalPosition.X, ybondaries.Y + ybondaries.X - GlobalPosition.Y);
        }



    }
}
