using System.Text;

namespace ColorGeneration;

public partial class MainPage : ContentPage
{
    private readonly EventHandler<TextChangedEventArgs> onTxtChanged;

    public MainPage()
    {
        InitializeComponent();
        onTxtChanged = (s, e) =>
        {
            if (s is Entry entry)
            {
                if (entry.Equals(LightnessEntry))
                {
                    Lightness.Value = Convert.ToDouble(LightnessEntry.Text);
                }
                if (entry.Equals(SaturationEntry))
                {
                    Saturation.Value = Convert.ToDouble(SaturationEntry.Text);
                }
            }
        };

        Redraw();

        SaturationEntry.TextChanged += onTxtChanged;
        LightnessEntry.TextChanged += onTxtChanged;

        Loaded += async (s, e) =>
        {
            await Task.Delay(1000);
            StringBuilder sb = new();
            foreach (var hue in Colors.Keys)
            {
                StringBuilder hueSB = new();
                foreach (var color in Colors[hue])
                {
                    hueSB.Append(color.ToHex());  //0xFFFFFF
                    hueSB.Append(',');
                }
                hueSB = hueSB.Remove(hueSB.Length - 1, 1);
                sb.AppendLine(hueSB.ToString());
            }
            Output.Text = sb.ToString();
        };
    }

    public static Dictionary<double, List<Color>> Colors { get; } = new();

    private void Redraw()
    {
        LightnessEntry.Text = Lightness.Value.ToString();
        SaturationEntry.Text = Saturation.Value.ToString();
        gv1.Drawable = new RectangleDrawable()
        {
            Saturation = Saturation.Value,
            Lightness = Lightness.Value,
        };
        gv1.Invalidate();

        List<GraphicsView> views = new()
        {
            gv2, gv3, gv4, gv5, gv6
        };

        double increment = (Lightness.Maximum - Lightness.Value) / views.Count;
        for (int i = 1; i <= views.Count; i++)
        {
            views[i - 1].Drawable = new RectangleDrawable()
            {
                Saturation = Saturation.Value,
                Lightness = Lightness.Value + increment * i,
            };
            views[i - 1].Invalidate();
        }
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        SaturationEntry.TextChanged -= onTxtChanged;
        LightnessEntry.TextChanged -= onTxtChanged;
        Redraw();
        SaturationEntry.TextChanged += onTxtChanged;
        LightnessEntry.TextChanged += onTxtChanged;
    }
}
