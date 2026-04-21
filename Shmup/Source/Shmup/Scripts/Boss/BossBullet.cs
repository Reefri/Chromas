using Com.IsartDigital.Utils.Effects;
using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class BossBullet : EnemyBullet
	{
        private const string BOSSBULLET_SCENE_PATH = "res://Scenes/Boss/BossBullet.tscn";
        private static PackedScene enemyBulletFactory = (PackedScene)GD.Load(BOSSBULLET_SCENE_PATH);

        private float bossBulletSpeed = 300;

		[Export] private Trail myTrail;

        public override void _Ready()
		{
			base._Ready();


			myTrail.Reparent(GameManager.GetInstance().GetBulletContainer());

			

            speed = Vector2.One * bossBulletSpeed;


        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);



        }

        protected override void Dispose(bool pDisposing)
		{

		}

		new public static BossBullet Create()
		{
			return (BossBullet)enemyBulletFactory.Instantiate();
		}


		public void Stop()
		{
            colliderNode.AreaEntered -= OnAreaEntered;
            colliderNode.AreaExited -= OnAreaExited;
			SetProcess(false);

        }

		public void Start()
		{
            colliderNode.AreaEntered += OnAreaEntered;
            colliderNode.AreaExited += OnAreaExited;
            SetProcess(true);

        }



    }
}
