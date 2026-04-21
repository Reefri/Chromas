// Author : Gramatikoff Sacha 
namespace Com.IsartDigital.Chromaberation {
    public partial class NullCollectable : GenericCollectable{
        public override void _Ready()
        {QueueFree();}
    }
}