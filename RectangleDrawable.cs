namespace ColorGeneration;

internal class RectangleDrawable : IDrawable
{
    public double Saturation;
    public double Lightness;
    
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
            hue = i / Convert.ToDouble(steps);
            color = Color.FromHsla(hue, Saturation, Lightness);
            if (!MainPage.Colors.ContainsKey(hue))
            {
                MainPage.Colors[hue] = new List<Color>();
            }
            MainPage.Colors[hue].Add(color);
            canvas.FillColor = color;
            canvas.FillRectangle(i * width, margin, width, height);
        }
    }
}