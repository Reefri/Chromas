using Godot;
using System.Collections.Generic;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class LevelParralaxe : Node2D
	{
		const string COLOTBLOCK_PATH = "res://Scenes/ColoredEntity/ColoredBlock.tscn";
		private PackedScene colorBlockFactory = (PackedScene)GD.Load(COLOTBLOCK_PATH);

		
		public float scrollingSpeed = 0;

		private float xMargin = 100f;

		private List<ColorBlock> colorBlocks = new List<ColorBlock>();

		[Export] private bool isBackground = true;

		[Export] private PlayerSpawner playerSpawner;


		public override void _Ready()
		{
			base._Ready();

			if (isBackground)
			{
				foreach (ColorBlock block in GetChildren())
				{
					colorBlocks.Add(block);
					block.margin = 300;
				}
			}


			if (IsInstanceValid(playerSpawner) && playerSpawner != null)
			{
				Player lPlayer = Player.GetInstance();


				AddChild(lPlayer);
                playerSpawner.Start();

                lPlayer.GlobalPosition = playerSpawner.GlobalPosition;


            }
        }

        public void CreateColorBlock(ColorBlock colorBlock)
        {

			colorBlock.GlobalPosition = new Vector2(CameraManager.GetInstance().GetLevelXBondaries().Y + xMargin - GlobalPosition.X, colorBlock.GlobalPosition.Y);
			colorBlocks.Add(colorBlock);
			AddChild(colorBlock);

        }

		

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);

        }

		public void SetScrollingSpeed(float pNewSpeed)
		{
			scrollingSpeed = pNewSpeed;
			foreach(ColorBlock lBlock in colorBlocks)
			{
				lBlock.speed = Vector2.One * scrollingSpeed;
			}

		}
		

	

	}
}
