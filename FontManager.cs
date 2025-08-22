// FontManager.cs
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace PizzeriaOpita.App
{
    public static class FontManager
    {
        private static readonly PrivateFontCollection _fontCollection = new();

        public static Font RobotoRegular { get; private set; } = null!;
        public static Font RobotoBold { get; private set; } = null!;

        static FontManager()
        {
            LoadFont("PizzeriaOpita.App.Resources.Fonts.Roboto-Regular.ttf", out Font robotoRegular);
            LoadFont("PizzeriaOpita.App.Resources.Fonts.Roboto-Bold.ttf", out Font robotoBold);

            RobotoRegular = robotoRegular;
            RobotoBold = robotoBold;
        }

        private static void LoadFont(string resourceName, out Font font)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null) throw new Exception($"No se pudo cargar la fuente: {resourceName}");

            var fontData = new byte[stream.Length];
            stream.ReadExactly(fontData, 0, fontData.Length);

            var fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

            _fontCollection.AddMemoryFont(fontPtr, fontData.Length);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            font = new Font(_fontCollection.Families[^1], 12);
        }
    }
}