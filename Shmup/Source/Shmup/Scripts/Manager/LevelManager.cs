using Com.IsartDigital.Utils.Effects;
using Godot;
using System;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation
{
	
	public partial class LevelManager : Node2D
	{

        const string LEVELPARRALAXE_PATH = "res://Scenes/LevelParralaxe.tscn";
		PackedScene levelParralaxeFactory = (PackedScene)GD.Load(LEVELPARRALAXE_PATH);

        [Export] public Shaker creatingPlayerShaker;

        [Export] public Node2D gameLayer;

        [Export] private ColorRect pixelShaderSupport;
        private ShaderMaterial pixelShaderMaterial;
        private float pixelShaderMinValue = 2.3f;
        private float pixelShaderMaxValue = 12f;
        private Tween pixelShaderTween;
        private float pixelShaderTweenSpeed = 1;
        private float currentPixelSize = 0;
        private const string CURRENTPIXELSIZE_NAME = "currentPixelSize";


        const string BACKGROUND_PATH = "Parallax";

        const string GAMELAYER_NAME = "GameLayer";

		private int layerNumber = 4;
		private float layerSpeed = -60;
        private float minBlackGradient = 0.2f;
        private float maxBlackGradient = 1;

		public float minY;
		public float maxY;


		private const float LEVELSPEED = 200;
		public static float currentLevelSpeed = 100;

        [ExportCategory("Shaker")]

        [Export] public Shaker onKillShaker;
        [Export] public Shaker smartBombShaker;

		static private LevelManager instance;
        private const string LEVELMANAGER_SCENE_PATH = "res://Scenes/Level/Level.tscn";
        private static PackedScene levelManagerFactory = (PackedScene)GD.Load(LEVELMANAGER_SCENE_PATH);

        public bool isActive = false;

        private LevelManager()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(LevelManager) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;

            currentPixelSize = pixelShaderMinValue;
        }

        static public LevelManager GetInstance()
        {
            if (instance == null) instance = (LevelManager)levelManagerFactory.Instantiate();
            return instance;

        }

        public override void _Ready()
		{ 
		
            GlobalPosition += new Vector2(CameraManager.GetInstance().GetCameraYBondaries().X, CameraManager.GetInstance().GetCameraYBondaries().X);

            pixelShaderMaterial = ((ShaderMaterial)(pixelShaderSupport).Material);


        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
            pixelShaderMaterial.SetShaderParameter(ShaderProp.PIXEL_SIZE, currentPixelSize);


        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}


	

		public void StartLevel()
		{
            isActive = true;



            Player.GetInstance().MakeAlive();

            int lIndexGameLayer = GetNode(BACKGROUND_PATH + "/" + GAMELAYER_NAME).GetIndex();

            layerNumber = GetNode(BACKGROUND_PATH).GetChildCount();

            for (int i = 0; i < layerNumber; i++)
            {

                ((LevelParralaxe)GetNode(BACKGROUND_PATH).GetChild(i)).SetScrollingSpeed( (lIndexGameLayer != i)?(lIndexGameLayer / (float)(i-lIndexGameLayer)) * layerSpeed:0);
                ((LevelParralaxe)GetNode(BACKGROUND_PATH).GetChild(i)).ZIndex = i;
                ((LevelParralaxe)GetNode(BACKGROUND_PATH).GetChild(i)).Modulate = Color.FromHsv(0,0, (float)(i)/ (layerNumber-1) *(maxBlackGradient - minBlackGradient) + minBlackGradient);
            }

			currentLevelSpeed = LEVELSPEED;


            SoundManager.GetInstance().PlayLevel();
        }


		public void StopLevel()
		{
            
            for (int i = 0; i < GetNode(BACKGROUND_PATH).GetChildCount(); i++)
            {
                ((LevelParralaxe)GetNode(BACKGROUND_PATH).GetChild(i)).SetScrollingSpeed(0);
            }

			currentLevelSpeed = 0;
        }


        public void UpdatePixelShader(float pWeight)
        {
            pixelShaderTween?.Kill();
            pixelShaderTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out)
                ;

            pixelShaderTween.TweenProperty(this, CURRENTPIXELSIZE_NAME, (1-pWeight) * (pixelShaderMaxValue - pixelShaderMinValue) + pixelShaderMinValue,pixelShaderTweenSpeed);

        }



    }
}
