using MvvmCross.UI;

namespace MvxExtensions.Models
{
    /// <summary>
    /// Available colors
    /// </summary>
    public class Color
    {
        #region Predefined Colors

        //Colors taken from .Net's System.Drawing
        /// <summary>
        /// Alice blue - RGBA(240, 248, 255, 255)
        /// </summary>
        public static MvxColor AliceBlue = new MvxColor(240, 248, 255);
        /// <summary>
        /// Antique white - ARGB(250, 235, 215, 255)
        /// </summary>
        public static MvxColor AntiqueWhite = new MvxColor(250, 235, 215);
        /// <summary>
        /// Aqua - ARGB(0, 255, 255, 255)
        /// </summary>
        public static MvxColor Aqua = new MvxColor(0, 255, 255);
        /// <summary>
        /// Aquamarine - ARGB(127, 255, 212, 255)
        /// </summary>
        public static MvxColor Aquamarine = new MvxColor(127, 255, 212);
        /// <summary>
        /// Azure - ARGB(240, 255, 255, 255)
        /// </summary>
        public static MvxColor Azure = new MvxColor(240, 255, 255);
        /// <summary>
        /// Beige - ARGB(245, 245, 220, 255)
        /// </summary>
        public static MvxColor Beige = new MvxColor(245, 245, 220);
        /// <summary>
        /// Bisque - ARGB(255, 228, 196, 255)
        /// </summary>
        public static MvxColor Bisque = new MvxColor(255, 228, 196);
        /// <summary>
        /// Black - RGBA(0, 0, 0, 255)
        /// </summary>
        public static MvxColor Black = new MvxColor(0, 0, 0);
        /// <summary>
        /// Blanched Almond - RGBA(255, 235, 205, 255)
        /// </summary>
        public static MvxColor BlanchedAlmond = new MvxColor(255, 235, 205);
        /// <summary>
        /// Blue - RGBA(0, 0, 255, 255)
        /// </summary>
        public static MvxColor Blue = new MvxColor(0, 0, 255);
        /// <summary>
        /// Blue Violet - RGBA(138, 43, 226, 255)
        /// </summary>
        public static MvxColor BlueViolet = new MvxColor(138, 43, 226);
        /// <summary>
        /// Brown - RGBA(165, 42, 42, 255)
        /// </summary>
        public static MvxColor Brown = new MvxColor(165, 42, 42);
        /// <summary>
        /// The burly wood - RGBA(222, 184, 135, 255)
        /// </summary>
        public static MvxColor BurlyWood = new MvxColor(222, 184, 135);
        /// <summary>
        /// The cadet blue - RGBA(95, 158, 160, 255)
        /// </summary>
        public static MvxColor CadetBlue = new MvxColor(95, 158, 160);
        /// <summary>
        /// The chartreuse - RGBA(127, 255, 0, 255)
        /// </summary>
        public static MvxColor Chartreuse = new MvxColor(127, 255, 0);
        /// <summary>
        /// The chocolate - RGBA(210, 105, 30, 255)
        /// </summary>
        public static MvxColor Chocolate = new MvxColor(210, 105, 30);
        /// <summary>
        /// The coral - RGBA(255, 127, 80, 255)
        /// </summary>
        public static MvxColor Coral = new MvxColor(255, 127, 80);
        /// <summary>
        /// The cornflower blue - RGBA(100, 149, 237, 255)
        /// </summary>
        public static MvxColor CornflowerBlue = new MvxColor(100, 149, 237);
        /// <summary>
        /// The cornsilk - RGBA(255, 248, 220, 255)
        /// </summary>
        public static MvxColor Cornsilk = new MvxColor(255, 248, 220);
        /// <summary>
        /// The crimson - RGBA(220, 20, 60, 255)
        /// </summary>
        public static MvxColor Crimson = new MvxColor(220, 20, 60);
        /// <summary>
        /// The cyan - RGBA(0, 255, 255, 255)
        /// </summary>
        public static MvxColor Cyan = new MvxColor(0, 255, 255);
        /// <summary>
        /// The dark blue - RGBA(0, 0, 139, 255)
        /// </summary>
        public static MvxColor DarkBlue = new MvxColor(0, 0, 139);
        /// <summary>
        /// The dark cyan - RGBA(0, 139, 139, 255)
        /// </summary>
        public static MvxColor DarkCyan = new MvxColor(0, 139, 139);
        /// <summary>
        /// The dark goldenrod - RGBA(184, 134, 11, 255)
        /// </summary>
        public static MvxColor DarkGoldenrod = new MvxColor(184, 134, 11);
        /// <summary>
        /// The dark gray
        /// </summary>
        public static MvxColor DarkGray = new MvxColor(169, 169, 169);
        /// <summary>
        /// The dark green
        /// </summary>
        public static MvxColor DarkGreen = new MvxColor(0, 100, 0);
        /// <summary>
        /// The dark khaki
        /// </summary>
        public static MvxColor DarkKhaki = new MvxColor(189, 183, 107);
        /// <summary>
        /// The dark magenta
        /// </summary>
        public static MvxColor DarkMagenta = new MvxColor(139, 0, 139);
        /// <summary>
        /// The dark olive green
        /// </summary>
        public static MvxColor DarkOliveGreen = new MvxColor(85, 107, 47);
        /// <summary>
        /// The dark orange
        /// </summary>
        public static MvxColor DarkOrange = new MvxColor(255, 140, 0);
        /// <summary>
        /// The dark orchid
        /// </summary>
        public static MvxColor DarkOrchid = new MvxColor(153, 50, 204);
        /// <summary>
        /// The dark red
        /// </summary>
        public static MvxColor DarkRed = new MvxColor(139, 0, 0);
        /// <summary>
        /// The dark salmon
        /// </summary>
        public static MvxColor DarkSalmon = new MvxColor(233, 150, 122);
        /// <summary>
        /// The dark sea green
        /// </summary>
        public static MvxColor DarkSeaGreen = new MvxColor(143, 188, 139);
        /// <summary>
        /// The dark slate blue
        /// </summary>
        public static MvxColor DarkSlateBlue = new MvxColor(72, 61, 139);
        /// <summary>
        /// The dark slate gray
        /// </summary>
        public static MvxColor DarkSlateGray = new MvxColor(47, 79, 79);
        /// <summary>
        /// The dark turquoise
        /// </summary>
        public static MvxColor DarkTurquoise = new MvxColor(0, 206, 209);
        /// <summary>
        /// The dark violet
        /// </summary>
        public static MvxColor DarkViolet = new MvxColor(148, 0, 211);
        /// <summary>
        /// The deep pink
        /// </summary>
        public static MvxColor DeepPink = new MvxColor(255, 20, 147);
        /// <summary>
        /// The deep sky blue
        /// </summary>
        public static MvxColor DeepSkyBlue = new MvxColor(0, 191, 255);
        /// <summary>
        /// The dim gray
        /// </summary>
        public static MvxColor DimGray = new MvxColor(105, 105, 105);
        /// <summary>
        /// The dodger blue
        /// </summary>
        public static MvxColor DodgerBlue = new MvxColor(30, 144, 255);
        /// <summary>
        /// The firebrick
        /// </summary>
        public static MvxColor Firebrick = new MvxColor(178, 34, 34);
        /// <summary>
        /// The floral white
        /// </summary>
        public static MvxColor FloralWhite = new MvxColor(255, 250, 240);
        /// <summary>
        /// The forest green
        /// </summary>
        public static MvxColor ForestGreen = new MvxColor(34, 139, 34);
        /// <summary>
        /// The fuchsia
        /// </summary>
        public static MvxColor Fuchsia = new MvxColor(255, 0, 255);
        /// <summary>
        /// The gainsboro
        /// </summary>
        public static MvxColor Gainsboro = new MvxColor(220, 220, 220);
        /// <summary>
        /// The ghost white
        /// </summary>
        public static MvxColor GhostWhite = new MvxColor(248, 248, 255);
        /// <summary>
        /// The gold
        /// </summary>
        public static MvxColor Gold = new MvxColor(255, 215, 0);
        /// <summary>
        /// The goldenrod
        /// </summary>
        public static MvxColor Goldenrod = new MvxColor(218, 165, 32);
        /// <summary>
        /// The gray
        /// </summary>
        public static MvxColor Gray = new MvxColor(128, 128, 128);
        /// <summary>
        /// The green
        /// </summary>
        public static MvxColor Green = new MvxColor(0, 128, 0);
        /// <summary>
        /// The green yellow
        /// </summary>
        public static MvxColor GreenYellow = new MvxColor(173, 255, 47);
        /// <summary>
        /// The honeydew
        /// </summary>
        public static MvxColor Honeydew = new MvxColor(240, 255, 240);
        /// <summary>
        /// The hot pink
        /// </summary>
        public static MvxColor HotPink = new MvxColor(255, 105, 180);
        /// <summary>
        /// The indian red
        /// </summary>
        public static MvxColor IndianRed = new MvxColor(205, 92, 92);
        /// <summary>
        /// The indigo
        /// </summary>
        public static MvxColor Indigo = new MvxColor(75, 0, 130);
        /// <summary>
        /// The ivory
        /// </summary>
        public static MvxColor Ivory = new MvxColor(255, 255, 240);
        /// <summary>
        /// The khaki
        /// </summary>
        public static MvxColor Khaki = new MvxColor(240, 230, 140);
        /// <summary>
        /// The lavender
        /// </summary>
        public static MvxColor Lavender = new MvxColor(230, 230, 250);
        /// <summary>
        /// The lavender blush
        /// </summary>
        public static MvxColor LavenderBlush = new MvxColor(255, 240, 245);
        /// <summary>
        /// The lawn green
        /// </summary>
        public static MvxColor LawnGreen = new MvxColor(124, 252, 0);
        /// <summary>
        /// The lemon chiffon
        /// </summary>
        public static MvxColor LemonChiffon = new MvxColor(255, 250, 205);
        /// <summary>
        /// The light blue
        /// </summary>
        public static MvxColor LightBlue = new MvxColor(173, 216, 230);
        /// <summary>
        /// The light coral
        /// </summary>
        public static MvxColor LightCoral = new MvxColor(240, 128, 128);
        /// <summary>
        /// The light cyan
        /// </summary>
        public static MvxColor LightCyan = new MvxColor(224, 255, 255);
        /// <summary>
        /// The light goldenrod yellow
        /// </summary>
        public static MvxColor LightGoldenrodYellow = new MvxColor(250, 250, 210);
        /// <summary>
        /// The light gray
        /// </summary>
        public static MvxColor LightGray = new MvxColor(211, 211, 211);
        /// <summary>
        /// The light green
        /// </summary>
        public static MvxColor LightGreen = new MvxColor(144, 238, 144);
        /// <summary>
        /// The light pink
        /// </summary>
        public static MvxColor LightPink = new MvxColor(255, 182, 193);
        /// <summary>
        /// The light salmon
        /// </summary>
        public static MvxColor LightSalmon = new MvxColor(255, 160, 122);
        /// <summary>
        /// The light sea green
        /// </summary>
        public static MvxColor LightSeaGreen = new MvxColor(32, 178, 170);
        /// <summary>
        /// The light sky blue
        /// </summary>
        public static MvxColor LightSkyBlue = new MvxColor(135, 206, 250);
        /// <summary>
        /// The light slate gray
        /// </summary>
        public static MvxColor LightSlateGray = new MvxColor(119, 136, 153);
        /// <summary>
        /// The light steel blue
        /// </summary>
        public static MvxColor LightSteelBlue = new MvxColor(176, 196, 222);
        /// <summary>
        /// The light yellow
        /// </summary>
        public static MvxColor LightYellow = new MvxColor(255, 255, 224);
        /// <summary>
        /// The lime
        /// </summary>
        public static MvxColor Lime = new MvxColor(0, 255, 0);
        /// <summary>
        /// The lime green
        /// </summary>
        public static MvxColor LimeGreen = new MvxColor(50, 205, 50);
        /// <summary>
        /// The linen
        /// </summary>
        public static MvxColor Linen = new MvxColor(250, 240, 230);
        /// <summary>
        /// The magenta
        /// </summary>
        public static MvxColor Magenta = new MvxColor(255, 0, 255);
        /// <summary>
        /// The maroon
        /// </summary>
        public static MvxColor Maroon = new MvxColor(128, 0, 0);
        /// <summary>
        /// The medium aquamarine
        /// </summary>
        public static MvxColor MediumAquamarine = new MvxColor(102, 205, 170);
        /// <summary>
        /// The medium blue
        /// </summary>
        public static MvxColor MediumBlue = new MvxColor(0, 0, 205);
        /// <summary>
        /// The medium orchid
        /// </summary>
        public static MvxColor MediumOrchid = new MvxColor(186, 85, 211);
        /// <summary>
        /// The medium purple
        /// </summary>
        public static MvxColor MediumPurple = new MvxColor(147, 112, 219);
        /// <summary>
        /// The medium sea green
        /// </summary>
        public static MvxColor MediumSeaGreen = new MvxColor(60, 179, 113);
        /// <summary>
        /// The medium slate blue
        /// </summary>
        public static MvxColor MediumSlateBlue = new MvxColor(123, 104, 238);
        /// <summary>
        /// The medium spring green
        /// </summary>
        public static MvxColor MediumSpringGreen = new MvxColor(0, 250, 154);
        /// <summary>
        /// The medium turquoise
        /// </summary>
        public static MvxColor MediumTurquoise = new MvxColor(72, 209, 204);
        /// <summary>
        /// The medium violet red
        /// </summary>
        public static MvxColor MediumVioletRed = new MvxColor(199, 21, 133);
        /// <summary>
        /// The midnight blue
        /// </summary>
        public static MvxColor MidnightBlue = new MvxColor(25, 25, 112);
        /// <summary>
        /// The mint cream
        /// </summary>
        public static MvxColor MintCream = new MvxColor(245, 255, 250);
        /// <summary>
        /// The misty rose
        /// </summary>
        public static MvxColor MistyRose = new MvxColor(255, 228, 225);
        /// <summary>
        /// The moccasin
        /// </summary>
        public static MvxColor Moccasin = new MvxColor(255, 228, 181);
        /// <summary>
        /// The navajo white
        /// </summary>
        public static MvxColor NavajoWhite = new MvxColor(255, 222, 173);
        /// <summary>
        /// The navy
        /// </summary>
        public static MvxColor Navy = new MvxColor(0, 0, 128);
        /// <summary>
        /// The old lace
        /// </summary>
        public static MvxColor OldLace = new MvxColor(253, 245, 230);
        /// <summary>
        /// The olive
        /// </summary>
        public static MvxColor Olive = new MvxColor(128, 128, 0);
        /// <summary>
        /// The olive drab
        /// </summary>
        public static MvxColor OliveDrab = new MvxColor(107, 142, 35);
        /// <summary>
        /// The orange
        /// </summary>
        public static MvxColor Orange = new MvxColor(255, 165, 0);
        /// <summary>
        /// The orange red
        /// </summary>
        public static MvxColor OrangeRed = new MvxColor(255, 69, 0);
        /// <summary>
        /// The orchid
        /// </summary>
        public static MvxColor Orchid = new MvxColor(218, 112, 214);
        /// <summary>
        /// The pale goldenrod
        /// </summary>
        public static MvxColor PaleGoldenrod = new MvxColor(238, 232, 170);
        /// <summary>
        /// The pale green
        /// </summary>
        public static MvxColor PaleGreen = new MvxColor(152, 251, 152);
        /// <summary>
        /// The pale turquoise
        /// </summary>
        public static MvxColor PaleTurquoise = new MvxColor(175, 238, 238);
        /// <summary>
        /// The pale violet red
        /// </summary>
        public static MvxColor PaleVioletRed = new MvxColor(219, 112, 147);
        /// <summary>
        /// The papaya whip
        /// </summary>
        public static MvxColor PapayaWhip = new MvxColor(255, 239, 213);
        /// <summary>
        /// The peach puff
        /// </summary>
        public static MvxColor PeachPuff = new MvxColor(255, 218, 185);
        /// <summary>
        /// The peru
        /// </summary>
        public static MvxColor Peru = new MvxColor(205, 133, 63);
        /// <summary>
        /// The pink
        /// </summary>
        public static MvxColor Pink = new MvxColor(255, 192, 203);
        /// <summary>
        /// The plum
        /// </summary>
        public static MvxColor Plum = new MvxColor(221, 160, 221);
        /// <summary>
        /// The powder blue
        /// </summary>
        public static MvxColor PowderBlue = new MvxColor(176, 224, 230);
        /// <summary>
        /// The purple
        /// </summary>
        public static MvxColor Purple = new MvxColor(128, 0, 128);
        /// <summary>
        /// The red
        /// </summary>
        public static MvxColor Red = new MvxColor(255, 0, 0);
        /// <summary>
        /// The rosy brown
        /// </summary>
        public static MvxColor RosyBrown = new MvxColor(188, 143, 143);
        /// <summary>
        /// The royal blue
        /// </summary>
        public static MvxColor RoyalBlue = new MvxColor(65, 105, 225);
        /// <summary>
        /// The saddle brown
        /// </summary>
        public static MvxColor SaddleBrown = new MvxColor(139, 69, 19);
        /// <summary>
        /// The salmon
        /// </summary>
        public static MvxColor Salmon = new MvxColor(250, 128, 114);
        /// <summary>
        /// The sandy brown
        /// </summary>
        public static MvxColor SandyBrown = new MvxColor(244, 164, 96);
        /// <summary>
        /// The sea green
        /// </summary>
        public static MvxColor SeaGreen = new MvxColor(46, 139, 87);
        /// <summary>
        /// The sea shell
        /// </summary>
        public static MvxColor SeaShell = new MvxColor(255, 245, 238);
        /// <summary>
        /// The sienna
        /// </summary>
        public static MvxColor Sienna = new MvxColor(160, 82, 45);
        /// <summary>
        /// The silver
        /// </summary>
        public static MvxColor Silver = new MvxColor(192, 192, 192);
        /// <summary>
        /// The sky blue
        /// </summary>
        public static MvxColor SkyBlue = new MvxColor(135, 206, 235);
        /// <summary>
        /// The slate blue
        /// </summary>
        public static MvxColor SlateBlue = new MvxColor(106, 90, 205);
        /// <summary>
        /// The slate gray
        /// </summary>
        public static MvxColor SlateGray = new MvxColor(112, 128, 144);
        /// <summary>
        /// The snow
        /// </summary>
        public static MvxColor Snow = new MvxColor(255, 250, 250);
        /// <summary>
        /// The spring green
        /// </summary>
        public static MvxColor SpringGreen = new MvxColor(0, 255, 127);
        /// <summary>
        /// The steel blue
        /// </summary>
        public static MvxColor SteelBlue = new MvxColor(70, 130, 180);
        /// <summary>
        /// The tan
        /// </summary>
        public static MvxColor Tan = new MvxColor(210, 180, 140);
        /// <summary>
        /// The teal
        /// </summary>
        public static MvxColor Teal = new MvxColor(0, 128, 128);
        /// <summary>
        /// The thistle
        /// </summary>
        public static MvxColor Thistle = new MvxColor(216, 191, 216);
        /// <summary>
        /// The tomato
        /// </summary>
        public static MvxColor Tomato = new MvxColor(255, 99, 71);
        /// <summary>
        /// The transparent
        /// </summary>
        public static MvxColor Transparent = new MvxColor(255, 255, 255, 0);
        /// <summary>
        /// The turquoise
        /// </summary>
        public static MvxColor Turquoise = new MvxColor(64, 224, 208);
        /// <summary>
        /// The violet
        /// </summary>
        public static MvxColor Violet = new MvxColor(238, 130, 238);
        /// <summary>
        /// The wheat
        /// </summary>
        public static MvxColor Wheat = new MvxColor(245, 222, 179);
        /// <summary>
        /// The white
        /// </summary>
        public static MvxColor White = new MvxColor(255, 255, 255);
        /// <summary>
        /// The white smoke
        /// </summary>
        public static MvxColor WhiteSmoke = new MvxColor(245, 245, 245);
        /// <summary>
        /// The yellow
        /// </summary>
        public static MvxColor Yellow = new MvxColor(255, 255, 0);
        /// <summary>
        /// The yellow green
        /// </summary>
        public static MvxColor YellowGreen = new MvxColor(154, 205, 50);

        #endregion
    }
}
