
using Com.IsartDigital.Chromaberation;
using Godot;
using System.Diagnostics;

namespace Com.IsartDigital.Utils.Effects
{
    public partial class Trail : Line2D
    {

        [Export(PropertyHint.Range, "0,100")] private int smoothness = 100;
        private float deltaSmoothness = 0f;

        [Export(PropertyHint.Range, "0.1,10")] private float vanish = 0.5f;
        private float deltaVanish = 0f;

        [Export] private bool alpha = false;
        [Export(PropertyHint.Range, "0.1,10")] private float alphaVanish = 0.5f;
        private float refAlpha;


        [Export]
        private bool start;

        [Export] private int length = 15;

        [Export]

        private bool reparent;

        [Export] public Node2D reparentTarget;

        [Export] public bool changePosition;
        [Export] public Node2D positionTarget;

        private Vector2 lastPosition;

        [Export] private bool changeColor = true;
        [Export] private bool isScrolling = false;

        public override void _Ready()
        {
            ClearPoints();
            Position = Vector2.Zero;
            SetProcess(false);
            refAlpha = Modulate.A;

            ZIndex = -1;

            if (reparent)
            {
                CallDeferred("ChangeDepth");
            }

            if (start) Start();
            else SetProcess(false);

            reparentTarget.TreeExited += OnParentKilled;

        }

        private void OnParentKilled()
        {
            SetProcess(false);
            QueueFree();
        }

        private void ChangeDepth()
        {
            Reparent(reparentTarget.GetParent());
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;


            if (reparent && !reparentTarget.IsInsideTree() || reparentTarget.IsQueuedForDeletion() || IsQueuedForDeletion() || changePosition && !positionTarget.IsInsideTree()) { QueueFree(); return; }

            if(changeColor) Modulate = ((ColoredEntity)reparentTarget).color;


            float lFrequency = vanish / length;
            Vector2 lPosition = changePosition ? ToLocal(positionTarget.GlobalPosition) : ToLocal(reparentTarget.GlobalPosition);

            if (Points.Length > length) RemovePoint(Points.Length - 1);
            if (lastPosition == lPosition)
            {
                deltaVanish += (float)pDelta;
                if (alpha)
                {
                    Color lColor = Modulate;
                    lColor.A -= Mathf.Max(0, refAlpha * alphaVanish / 60);
                    Modulate = lColor;
                }
            }
            else if (alpha)
            {
                Color lColor = Modulate;
                lColor.A = refAlpha;
                Modulate = lColor;
            }



            int lLength = (int)(deltaVanish / lFrequency);

          

            if (deltaVanish > lFrequency)
            {
                for (int i = 0; i < lLength; i++)
                {
                    if (Points.Length == 0) break;
                    RemovePoint(Points.Length - 1);
                }
                deltaVanish -= lLength * lFrequency;
            }

            deltaSmoothness += (float)pDelta;
            if (deltaSmoothness > (100 - smoothness) / 500f)
            {
                deltaSmoothness = 0f;
                if (lPosition != lastPosition)
                {
                    AddPoint(lPosition, 0);
                    lastPosition = lPosition ;
                }
            }
            else SetPointPosition(0, lPosition);


            if (isScrolling)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    SetPointPosition(i, GetPointPosition(i) + Vector2.Left * Player.GetInstance().maxSpeed * lDelta);
                }
            }

        }

        public void Start()
        {
            ClearPoints();
            SetProcess(true);
        }
        public void Pause()
        {
            SetProcess(false);
        }
        public void Resume()
        {
            SetProcess(true);
        }
        public void Stop()
        {
            Pause();
            ClearPoints();
        }

        public bool IsPlaying()
        {
            return IsProcessing();
        }

    }
}
