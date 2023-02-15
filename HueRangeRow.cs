using System.Diagnostics;

namespace ColorGeneration;

internal class HueRangeRow : IDrawable
{
    
    public double Saturation;
    public double Lightness;
    public int Index;

    /// <summary>
    /// Draws 30 rectangles (40x60) of distinct colors.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        const int steps = 30;
        const int margin = 0;

        Color color;
        double hue;
        float width = dirtyRect.Width / steps;
        float height = dirtyRect.Height;
        for (int i = 0; i < steps; i++)
        {
            if (MainPage.IsRandomized)
            {
                color = MainPage.RandomColors[i][Index];
            }
            else
            {
                hue = i / Convert.ToDouble(steps);
                color = Color.FromHsla(hue, Saturation, Lightness);
                if (!MainPage.Colors.ContainsKey(hue))
                {
                    MainPage.Colors[hue] = new List<Color>();
                }
                MainPage.Colors[hue].Add(color);
            }

            canvas.FillColor = color;
            Debug.Write(color.GetHue().ToString("f8") + " ");
            canvas.FillRectangle(i * width, margin, width, height);
        }
    }
}