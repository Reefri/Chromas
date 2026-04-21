using Com.IsartDigital.Chromaberation;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.ProjectName {
	
	public partial class test : Node2D
	{
		public override void _Ready()
		{

			List<Color> colors = ColorManager.ColorRevolution(new Color(1, 0, 0), 9);
            colors.AddRange(colors);


            //for (int i = 0; i < 18; i++)
            //{
				int i = 8;

				GD.Print(
                    colors[i % 9]
                    , " : ",
                    colors[i % 9 + 1]
                    , " : ",
                    ColorManager.AreColorsCloseEnough(colors[i%9], colors[i%9 +1] ) 
                    , " : ",
                    ColorManager.AreColorsCloseEnough(colors[i%9 +1], colors[i%9]) 

                    );


				GD.Print("######");

            //}

        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;


		
		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
