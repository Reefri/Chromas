using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class ColoredEntity : MovableEntity
	{


		[Export] public int colorIndex = 0;
		[Export] protected bool colorGivenTroughtCode = true;
		[Export] public Color color = Color.FromHsv(1,1,1);

		const string RENDERER_NODE_PATH = "Renderer";
		const string COLLIDER_NODE_PATH = "Collider";


		public Node2D rendererNode;
		protected Area2D colliderNode;


        float colorSpeed = 0.1f;

        public override void _Ready()
		{
			base._Ready();

			if (!colorGivenTroughtCode)
			{
				color = ColorManager.ColorRotation(ColorManager.baseColor, (float)colorIndex / GameManager.GetInstance().colorNumber);
			}

			rendererNode = ((Node2D)GetNode(RENDERER_NODE_PATH));
			colliderNode = ((Area2D)GetNode(COLLIDER_NODE_PATH));

            rendererNode.Modulate = color;

			colliderNode.AreaEntered += OnAreaEntered;
            colliderNode.AreaExited += OnAreaExited;


        }
		
       

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(lDelta);

			UpdateColor();
        }



        public virtual void Destroy()
        {
            CallDeferred("queue_free");
        }

		protected virtual void UpdateColor()
		{
            rendererNode.Modulate = color;

        }

        public void SetColorToIndex()
		{
			color = GameManager.GetInstance().colorList[colorIndex%GameManager.GetInstance().colorNumber];
		}

        protected void CircleColor(float pDelta)
        {
            color = Color.FromHsv(((float)(color.H + colorSpeed * pDelta) % 1), color.S, color.V);
        }

		protected virtual void OnAreaEntered(Area2D pArea)
		{

		}

        protected virtual void OnAreaExited(Area2D pArea)
        {
        }


    }
}
