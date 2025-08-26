using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace GymTracker.Services
{
    public static class ButtonHelper
    {
        public static async Task ColorTo(this Button button, Color fromColor, Color toColor, uint length = 100)
        {
            uint steps = 100;
            float rStep = ((float)toColor.Red - (float)fromColor.Red) / steps;
            float gStep = ((float)toColor.Green - (float)fromColor.Green) / steps;
            float bStep = ((float)toColor.Blue - (float)fromColor.Blue) / steps;

            for (int i = 1; i <= steps; i++)
            {
                button.BackgroundColor = Color.FromRgb(
                    fromColor.Red + rStep * i,
                    fromColor.Green + gStep * i,
                    fromColor.Blue + bStep * i
                );
                await Task.Delay((int)(length / steps));
            }
        }
    }
}
