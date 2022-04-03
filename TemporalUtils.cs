using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Temporal
{
    internal static class TemporalUtils
    {

        internal static Color TryApplyingPlayerStringColour(int playerStringColour, Color stringColour)
        {
            if(playerStringColour > 0)
            {
                stringColour = WorldGen.paintColor(playerStringColour);
                if (stringColour.R < 75) stringColour.R = 75;
                if (stringColour.G < 75) stringColour.G = 75;
                if (stringColour.B < 75) stringColour.B = 75;
                switch(playerStringColour)
                {
                    case 13:
                        stringColour = new Color(20, 20, 20);
                        break;
                    case 0:
                    case 14:
                        stringColour = new Color(200, 200, 200);
                        break;
                    case 28:
                        stringColour = new Color(163, 116, 91);
                        break;
                    case 27:
                        stringColour = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                        break;
                }
                stringColour.A = (byte)((int)stringColour.A * 0.4f);
            }
            return stringColour;
        }
    }
}
