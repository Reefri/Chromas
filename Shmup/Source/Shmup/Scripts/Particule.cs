using Godot;
using System;
using System.Linq;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Particule : Node2D
	{

        private Node2D parent;

        private bool haveStarted = false;

        private Timer waitBeforeDestruction = new Timer();
        private float beforeDestructionTime = 1;


        public override void _Ready()
        {
            parent = (Node2D)GetParent();
            Stop();
            CallDeferred("reparent",GetParent().GetParent());


            waitBeforeDestruction.WaitTime = beforeDestructionTime;
            waitBeforeDestruction.OneShot = true;
            waitBeforeDestruction.Autostart = true;

            waitBeforeDestruction.Timeout += QueueFree;
            AddChild(waitBeforeDestruction);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (haveStarted)
            {
                foreach (GpuParticles2D lParticul in GetChildren().OfType<GpuParticles2D>())
                {
                    if (lParticul.Emitting)
                    {
                        return;
                    }
                }

                waitBeforeDestruction.Start();
            }

        }
		public void Start()
		{
            haveStarted = true;
            GlobalRotation = parent.GlobalRotation - MathF.PI;
            foreach (GpuParticles2D lParticul in GetChildren().OfType<GpuParticles2D>())
            {
                lParticul.Emitting = true;
            }
        }

        public void Stop()
        {
            foreach (GpuParticles2D lParticul in GetChildren().OfType<GpuParticles2D>())
            {
                lParticul.Emitting = false;
            }
        }

    }
}
