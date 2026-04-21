using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.Chromaberation
{


    public static class ColorManager
    {
        static public Color baseColor = new Color(1,0,0);


        static public List<Color> GetComplementarys(Color pColor)
        {
            List<Color> complementarys = new List<Color>()
            {
                Color.FromHsv(((float)(pColor.H + 1f/3) % 1), pColor.S, pColor.V),
                Color.FromHsv(((float)(pColor.H + 2f/3) % 1), pColor.S, pColor.V)
            };

            return complementarys; 
        }


        static public Color GetOpposite(Color pColor)
        {
            return ColorRotation(pColor, 1f / 2);
        }

        static public List<Color> ColorRevolution(Color pColor, int colorNumber)
        {
            List<Color> lListColor = new List<Color>();

            for (int i = 0; i < colorNumber; i++)
            {
                lListColor.Add(ColorRotation(pColor, (float)i / colorNumber));

            }

            return lListColor;
        }

        static public Color ColorRotation(Color pColor, float pAngle)
        {
            return Color.FromHsv(((float)(pColor.H + pAngle) % 1), pColor.S, pColor.V);
        }


        static public float ColorDistance(Color pColorA, Color pColorB)
        {
            return Mathf.Sqrt(
                FastColorDistance(pColorA,pColorB)
                );
        }

        static public float FastColorDistance(Color pColorA, Color pColorB)
        {
            return MathF.Pow(pColorA.R - pColorB.R, 2) + 
                MathF.Pow(pColorA.G - pColorB.G, 2) + 
                MathF.Pow(pColorA.B - pColorB.B, 2);
        }

        static public float HueColorDistance(Color pColorA, Color pColorB)
        {
         
            return MathF.Min(
                MathF.Abs(pColorA.H - pColorB.H),
                MathF.Abs(MathF.Min(pColorA.H,pColorB.H) + 1 - MathF.Max(pColorA.H, pColorB.H))
                );
        }

        static public bool AreColorsCloseEnough(Color pColorA, Color pColorB)
        {
            return HueColorDistance(pColorA,pColorB) <= GameManager.GetInstance().colorAccuracy;
        }
        static public bool AreEnemyColorsCloseEnough(Color pColorA, Color pColorB)
        {
            return HueColorDistance(pColorA,pColorB) <= GameManager.GetInstance().enemyColorAccuracy;
        }

        

        static public Color ColorSetRelativeSaturation(Color pColor, float pSaturationChange)
        {
            return Color.FromHsv(pColor.H, (float)(pColor.S + pSaturationChange) %1, pColor.V);
        }

        static public Color ColorSetRelativeValue(Color pColor, float pValueChange)
        {
            return Color.FromHsv(pColor.H, pColor.S,(float)(pColor.V + pValueChange) % 1);
        }

        static public Color ColorSetHue(Color pColor, float pNewHue)
        {
            return Color.FromHsv(pNewHue, pColor.S, pColor.V);
        }

        static public Color ColorSetSaturation(Color pColor, float pNewSaturation)
        {
            return Color.FromHsv(pColor.H, pNewSaturation % 1, pColor.V);
        }

        static public Color ColorSetValue(Color pColor, float pNewValue)
        {
            return Color.FromHsv(pColor.H, pColor.S , pNewValue % 1);
        }

        static public Color HalfTransparent(Color pColor)
        {

            return new Color(pColor.R, pColor.G, pColor.B, 0.5f);

        }

    }

}
