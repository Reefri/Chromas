using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class PlayerCore : Sprite2D
	{

		private const string CORESHADERSUPPORT_SCENE_PATH = "res://Scenes/ColoredEntity/CoreShaderSupport.tscn";
		private PackedScene coreShaderSupportFactory = (PackedScene)GD.Load(CORESHADERSUPPORT_SCENE_PATH);


        public override void _Ready()
		{
			CreateShaderSupport(Player.GetInstance().color);
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			if (GetChildCount() >= 2)
			{
				if ((float)((ShaderMaterial)((CoreShaderSupport)GetChild(1)).Material).GetShaderParameter(ShaderProp.CORE_PROGRESSION) == 1f)
				{
                    GetChild(0).QueueFree();

				}
			}

		}

		public void CreateShaderSupport(Color pColor)
		{
            CoreShaderSupport lNewShaderSupport = (CoreShaderSupport)coreShaderSupportFactory.Instantiate();


			AddChild(lNewShaderSupport);
            ((ShaderMaterial)lNewShaderSupport.Material).SetShaderParameter(ShaderProp.CORE_COLOR, pColor);

        }
    }
}
