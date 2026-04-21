using Godot;
using System;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class Main : Node2D
	{

		static private Main instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/Main.tscn");

		private ShaderMaterial shader;

		[Export] public bool isFirstTimePlaying = true;
		[Export] public bool startRightAway = false;

		[Export] private ColorRect colorblindRect;

		[Export] public Node2D postProcessingNode;

		[Export] public ColorAberation chromaAberation;
		[Export] public GlitchShake glitchEffet;
        private Main():base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(Main) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;	
		}

		static public Main GetInstance()
		{
			if (instance == null) instance = (Main)factory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{
			base._Ready();

			shader = (ShaderMaterial)colorblindRect.Material;

			SoundManager.GetInstance().PlayAmbiance();

        }



		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}

		


        public void SetShaderColor(Color pColor)
        {
			shader.SetShaderParameter(ShaderProp.COLORBLIND_COLOR, pColor);
        }


		public static Timer CreateTimer(float pWaitTime, bool pAutoStart, bool pOneShot, Action pAction)
		{
			Timer lTimer = new Timer();
            lTimer.WaitTime = pWaitTime;
            lTimer.Autostart = pAutoStart;
            lTimer.OneShot = pOneShot;

            lTimer.Timeout += pAction;
			
			return lTimer;
		}
      
    }
}
