using Godot;
using System;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.ProjectName {
	
	public partial class new_script : Node
	{

		static private new_script instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/new_script.tscn");

		private new_script():base() {
			if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(new_script) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;	
		}

		static public new_script GetInstance()
		{
			if (instance == null) instance = (new_script)factory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double pDelta)
		{
			base._Process(pDelta);
			float lDelta = (float)pDelta;
		}

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
