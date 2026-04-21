using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation 
{	
	public partial class Explosion : ColoredEntity
	{

        const string ENEMYEXPLOSION_SCENE_PATH = "res://Scenes/ColoredEntity/Enemy/Explosion.tscn";
        static private PackedScene enemyExplosionFactory = (PackedScene)GD.Load(ENEMYEXPLOSION_SCENE_PATH);
        
		protected float maxScale;
        protected float scalingSpeed;


        protected Timer explosionTimer = new Timer();
		public float explosionDuration = 0.5f;


		public override void _Ready()
		{
			base._Ready();
			Scale = Vector2.Zero;

            scalingSpeed = maxScale / explosionDuration;


            explosionTimer.WaitTime = explosionDuration;
			explosionTimer.OneShot = true;
			explosionTimer.Autostart = true;

			explosionTimer.Timeout += QueueFree;

			AddChild(explosionTimer);

			ExplosionTween();
		}

        

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(lDelta);

			Scale += Vector2.One * scalingSpeed * lDelta;

		}

		public static  Explosion Create(Vector2 pPosition, Color pColor, float pMaxScale, float pExplosionDuration)
		{

            Explosion lNewExplosion = (Explosion)enemyExplosionFactory.Instantiate();
            lNewExplosion.GlobalPosition = pPosition;
            lNewExplosion.color = pColor;

			lNewExplosion.maxScale = pMaxScale; 
			lNewExplosion.explosionDuration = pExplosionDuration;

            GameManager.GetInstance().GetExplosionContainer().CallDeferred("add_child", lNewExplosion);

			return lNewExplosion;
        }

		protected virtual void ExplosionTween()
		{


            Tween lRotationTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            lRotationTween.TweenProperty(this, "rotation", Mathf.Pi, explosionDuration);

            Tween lVisibilityTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.In);

            lVisibilityTween.TweenProperty(rendererNode, "modulate", new Color(color.R, color.G, color.B, 0), explosionDuration);
        }



    }
}
