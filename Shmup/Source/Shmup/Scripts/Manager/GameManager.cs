using Godot;
using System;
using System.Collections.Generic;

// Author : 

namespace Com.IsartDigital.Chromaberation
{

	public partial class GameManager : Node2D
	{
		private const string GAMEMANAGER_SCENE_PATH = "res://Scenes/Manager/GameContainer.tscn";
		private static PackedScene gameManagerFactory = (PackedScene)GD.Load(GAMEMANAGER_SCENE_PATH);


		const string PERMANENTMARK_SCENE_PATH = "res://Scenes/PermanentMark.tscn";
		private PackedScene permanentMarkFactory = (PackedScene)GD.Load(PERMANENTMARK_SCENE_PATH);


		const string COLLECTABLECONTAINER_NODE_PATH = "CollectableContainer";

		const string NULLCOLLECTABLE_SCENE_PATH = "res://Scenes/Collectables/NullCollectable.tscn";
		const string HEAL_SCENE_PATH = "res://Scenes/Collectables/Heal.tscn";
		const string SMARTBOMBCOLLECTABLE_SCENE_PATH = "res://Scenes/Collectables/SmartBombCollectable.tscn";
		const string LEVELUP_SCENE_PATH = "res://Scenes/Collectables/LevelUp.tscn";

		private PackedScene nullCollectableFactory = (PackedScene)GD.Load(NULLCOLLECTABLE_SCENE_PATH);
		private PackedScene healCollectableFactory = (PackedScene)GD.Load(HEAL_SCENE_PATH);
		private PackedScene smartBombCollectableFactory = (PackedScene)GD.Load(SMARTBOMBCOLLECTABLE_SCENE_PATH);
		private PackedScene levelUpCollectableFactory = (PackedScene)GD.Load(LEVELUP_SCENE_PATH);



		private const string BULLETCONTAINER_NODE_PATH = "BulletContainer";
		private const string PERMANENTMARK_NODE_PATH = "PermanantMarkContainer";
		private const string EXPLOSIONCONTAINER_NODE_PATH = "ExplosionContainer";

		static private GameManager instance;

		public Color baseColor = new Color(1, 0, 0);


		private float levelSpeed = 250;

		private Node2D camera;




		public RandomNumberGenerator rng = new RandomNumberGenerator();

		private List<GenericCollectable> collectablesPool = new List<GenericCollectable>();
		private int numberOfHealInPool = 2;
		private int numberOfSmartBombInPool = 1;
		private int numberOfLevelUpInPool = 2;
		private int numberOfNullCollectableInPool = 15;


        [Export] public int colorNumber = 9;
		public List<Color> colorList = new List<Color>();

        public float enemyColorAccuracy = 0.6f / 9;
        public float colorAccuracy = 1.3f / 9;


		private const int SEED = 2;

        private GameManager() : base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(GameManager) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;

            colorList = ColorManager.ColorRevolution(baseColor, colorNumber);

        }



        static public GameManager GetInstance()
		{
			if (instance == null) instance = (GameManager)gameManagerFactory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{


			camera = LevelManager.GetInstance();


			rng.Seed = SEED;


		}



		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}

		
		public Node2D GetBulletContainer()
		{
			return (Node2D)GetNode(BULLETCONTAINER_NODE_PATH);

		}

		private Node2D GetPermanentMarkContainer()
		{
			return (Node2D)GetNode(PERMANENTMARK_NODE_PATH);
		}

		private Node2D GetCollectableContainer()
		{
			return (Node2D)GetNode(COLLECTABLECONTAINER_NODE_PATH);
		}

		public Node2D GetExplosionContainer()
		{
			return (Node2D)GetNode(EXPLOSIONCONTAINER_NODE_PATH);
		}

		public void AddPermentMark(Node2D pRendererElement, Color lColor)
		{
			Node2D lRenderedElementDuplicate = (Node2D)pRendererElement.Duplicate();
			lRenderedElementDuplicate.Position = new Vector2(0, 0);

			PermanentMark lElement = (PermanentMark)permanentMarkFactory.Instantiate();
			lElement.spriteContainer.AddChild(lRenderedElementDuplicate);
			lElement.GlobalPosition = ((Marker2D)pRendererElement.GetChild(0)).GlobalPosition;
			lElement.Modulate = lColor;

			GetPermanentMarkContainer().CallDeferred("add_child", lElement);

		}

        public void AddPermentMarkControl(TextureRect pRendererElement)
        {
            Sprite2D lRenderedElementDuplicate = new Sprite2D();
			lRenderedElementDuplicate.Texture = pRendererElement.Texture;


			Marker2D lMarker = (Marker2D)pRendererElement.GetChild(0).Duplicate();

            Vector2 lMarkerPosition = lMarker.Position;
            Vector2 lMarkerGlobalPosition = lMarker.GlobalPosition;


            lRenderedElementDuplicate.Position = new Vector2(0, 0);

            lRenderedElementDuplicate.AddChild(lMarker);
            lMarker.Position = lMarkerPosition;

            lRenderedElementDuplicate.Scale = Vector2.One * pRendererElement.Size.X / pRendererElement.Texture.GetSize().X;


            PermanentMark lElement = (PermanentMark)permanentMarkFactory.Instantiate();



            lElement.spriteContainer.AddChild(lRenderedElementDuplicate);




            lElement.GlobalPosition =
                lMarkerGlobalPosition
				+ CameraManager.GetInstance().GetCamera().GlobalPosition;
				;


            GetPermanentMarkContainer().CallDeferred("add_child", lElement);

        }


        private void GenerateCollectablePool()
		{
			for (int i = 0; i < numberOfHealInPool; i++)
			{
				collectablesPool.Add((Heal)healCollectableFactory.Instantiate());
			}

			for (int i = 0; i < numberOfLevelUpInPool; i++)
			{
				collectablesPool.Add((LevelUp)levelUpCollectableFactory.Instantiate());
			}

			for (int i = 0; i < numberOfSmartBombInPool; i++)
			{
				collectablesPool.Add((SmartBombCollectable)smartBombCollectableFactory.Instantiate());
			}

			for (int i = 0; i < numberOfNullCollectableInPool; i++)
			{
				collectablesPool.Add((NullCollectable)nullCollectableFactory.Instantiate());
			}
		}


		public GenericCollectable CallRandomCollectable()
		{
			if (collectablesPool.Count == 0) GenerateCollectablePool();

			int randomNumber = rng.RandiRange(0, collectablesPool.Count -1);

			GenericCollectable lNewCollectable = collectablesPool[randomNumber];


            collectablesPool.Remove(lNewCollectable);
			return lNewCollectable;

		}

		public void CreateCollectable(Vector2 pPosition)
		{
			GenericCollectable lCollectable = CallRandomCollectable();

			lCollectable.GlobalPosition = pPosition;

			GetCollectableContainer().CallDeferred("add_child",lCollectable);

		}

	}
}
