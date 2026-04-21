using Godot;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation
{
	
	public partial class CameraManager : MovableEntity
	{
		private const string CAMERAMANAGER_SCENE_PATH = "res://Scenes/Camera.tscn";
		private static PackedScene cameraFactory = (PackedScene)GD.Load(CAMERAMANAGER_SCENE_PATH);


        private Camera2D camera;

		private Vector2 baseZoom;

        private float zoomSpeed = 6;
		private float maxZoom = 1.2f;

		public bool isZooming = false;

		private Vector2 basePosition;

		Tween positionTween;
		Tween zoomTween;

		private float HUDMArgin = 150;



        static private CameraManager instance;

		private CameraManager() 
		{
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(CameraManager) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;
        }

        static public CameraManager GetInstance()
        {
            if (instance == null) instance = (CameraManager)cameraFactory.Instantiate();
            return instance;

        }

        public override void _Ready()
		{
		

			base._Ready();

			camera = (Camera2D)GetNode("Camera");

			baseZoom = camera.Zoom;
			basePosition = camera.Position;

        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(lDelta);


            NaturalScrolling(lDelta);

        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}


		


		public void ZoomIn(float lZoomScale, Vector2 lPosition)
		{

			zoomTween = CreateTween();

            zoomTween.SetTrans(Tween.TransitionType.Circ);
            zoomTween.SetEase(Tween.EaseType.Out);

            zoomTween.TweenProperty(camera, "zoom", lZoomScale * Vector2.One, zoomSpeed);

			positionTween = CreateTween()
				.SetTrans(Tween.TransitionType.Circ)
				.SetEase(Tween.EaseType.Out);

			positionTween.TweenProperty(camera, "global_position", lPosition, 1f);
			

        }


		public void ResetZoom()
		{
			Tween lDeZoomTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				;

			lDeZoomTween.TweenProperty(camera, "zoom", baseZoom, 1f);

			//camera.Zoom = baseZoom;
		}
		public void ResetPosition()
		{
			camera.Position = basePosition;
		}



		public void ActivateZooming()
		{
			isZooming = true;
			ZoomIn(maxZoom, Player.GetInstance().GlobalPosition);
		}

		public void DeactivateZooming()
		{ 
			isZooming = false;

			positionTween?.Kill();
			zoomTween?.Kill();

			ResetZoom();
			ResetPosition();
        }

		public Camera2D GetCamera()
		{
			return camera;
		}

		public Vector2 GetLevelXBondaries()
		{
			return GetCameraXBondaries();
		}
		public Vector2 GetLevelYBondaries()
		{
			return GetCameraYBondaries() + Vector2.Right * HUDMArgin;
		}

		public Rect2 GetRect()
		{
			return camera.GetViewportRect();
		}

		public Vector2 GetCameraXBondaries()
		{
            return new Vector2(Position.X - camera.GetViewportRect().Size.X / 2,
                           Position.X + camera.GetViewportRect().Size.X / 2);
        }
		public Vector2 GetCameraYBondaries()
		{
            return new Vector2(Position.Y - camera.GetViewportRect().Size.Y / 2,
                           Position.Y + camera.GetViewportRect().Size.Y / 2);
        }


    }
}
