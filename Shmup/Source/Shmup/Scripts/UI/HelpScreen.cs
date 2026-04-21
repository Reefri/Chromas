using Godot;
using System.Collections.Generic;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class HelpScreen : Control
	{
		[Export] private Control moveBlock;
		[Export] private Control shootBlock;
		[Export] private Control smartbombBlock;
		[Export] private Control sfBlock;

        private Tween helpScreenTween;

        private List<Control> blocks;
        private List<Vector2> positions = new List<Vector2>() ;

		private float margin = 400;
		public override void _Ready()
		{
			blocks = new List<Control>()
			{
				moveBlock, 
				shootBlock, 
				smartbombBlock, 
				sfBlock,
			};

			foreach (Control lControl in  blocks) 
			{ positions.Add(lControl.Position); }

			Enable();
		}


		public void Enable()
		{
			moveBlock.Hide();
			shootBlock.Hide();
			smartbombBlock.Hide();
			sfBlock.Hide();

			Show();

			ResetPositions();

            helpScreenTween = CreateTween()
                    .SetTrans(Tween.TransitionType.Elastic)
                    .SetEase(Tween.EaseType.Out)
                    ;

            foreach (Control lControl in blocks)
			{

				helpScreenTween.TweenCallback(
					Callable.From(() =>
					{
						lControl.Show();
					}
				));

				helpScreenTween.TweenProperty(lControl, TweenProp.POSITION, lControl.Position, 1f).From(lControl.Position + Vector2.Right * margin);

            }

        }

		public void Disable()
		{
			Hide();
		}

		public void ResetPositions()
		{
			moveBlock.Position = positions[0];
			shootBlock.Position = positions[1];
			smartbombBlock.Position = positions[2];
			sfBlock.Position = positions[3];
		}
	}
}
