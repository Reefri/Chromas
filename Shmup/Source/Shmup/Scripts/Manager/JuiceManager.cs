using Godot;
using System;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class JuiceManager : Node2D
	{

		[Signal] public delegate void OnCollectableBeatEventHandler();

		static private JuiceManager instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/Manager/JuiceManager.tscn");

		Timer collectableBumpTImer = new Timer();

		public float globalTime =0;

		private JuiceManager():base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(JuiceManager) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;	
		}

		static public JuiceManager GetInstance()
		{
			if (instance == null) instance = (JuiceManager)factory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{
			base._Ready();

			collectableBumpTImer.WaitTime = 1.5f;
			collectableBumpTImer.Autostart = true;
			collectableBumpTImer.OneShot = false;

			collectableBumpTImer.Timeout += EmitOnBeat;

			AddChild(collectableBumpTImer);
		}

		public override void _Process(double pDelta)
		{
			base._Process(pDelta);
			float lDelta = (float)pDelta;

			globalTime += lDelta;
		}

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}

        private void EmitOnBeat()
        {
            EmitSignal(SignalName.OnCollectableBeat);
        }
    }
}
