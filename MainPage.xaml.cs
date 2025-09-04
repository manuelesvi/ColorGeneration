using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ColorGeneration;

public partial class MainPage : ContentPage
{
    private readonly EventHandler<TextChangedEventArgs> onTxtChanged;
    public static Dictionary<double, List<Color>> Colors { get; private set; } = new();
    public static List<List<Color>> RandomColors { get; private set; }
    public static bool IsRandomized { get; private set; }
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

    private void Redraw()
    {
        LightnessEntry.Text = Lightness.Value.ToString();
        SaturationEntry.Text = Saturation.Value.ToString();
        gv1.Drawable = new HueRangeRow()
        {
            Saturation = Saturation.Value,
            Lightness = Lightness.Value,
            Index = 0,
        };
        gv1.Invalidate();

        List<GraphicsView> views = new()
        {
            gv2, gv3, gv4, gv5, gv6
        };

        double increment = (Lightness.Maximum - Lightness.Value) / views.Count;
        for (int i = 1; i <= views.Count; i++)
        {
            views[i - 1].Drawable = new HueRangeRow()
            {
                Saturation = Saturation.Value,
                Lightness = Lightness.Value + increment * i,
                Index = i,
            };
            views[i - 1].Invalidate();
        }
    }

    private int GetRandomIndex(Random rnd, int maximum, int minimum = 0)
    {
        const int multiplier = 100;
        int max = maximum * multiplier;
        int seed = rnd.Next(0, max);
        int index = seed % maximum; // 0-(maximum-1)
        return index;
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        SaturationEntry.TextChanged -= onTxtChanged;
        LightnessEntry.TextChanged -= onTxtChanged;
        Redraw();
        SaturationEntry.TextChanged += onTxtChanged;
        LightnessEntry.TextChanged += onTxtChanged;
    }

    private void Randomize_Clicked(object sender, EventArgs e)
    {
        const int chunkSize = 5;

        int numChunks = Colors.Count / chunkSize; // 6
        double[][] chunks = new double[Colors.Count][];
        for (int i = 0; i < numChunks; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                chunks[i] = new double[chunkSize];
                chunks[i] = Colors.Keys
                    .Skip(i * chunkSize)
                    .Take(chunkSize)
                    .ToArray();
                Array.Reverse(chunks[i]);
            }
        }

        Random rnd = new();
        int index;

        List<List<List<Color>>> theChunks = new(numChunks); // chunks -> chunk -> hue -> serie
        for (int i = 0; i < numChunks; i++)
        {
            List<List<Color>> colorList = new(chunkSize); // the chunk
            for (int j = 0; j < chunkSize; j++)
            {
                colorList.Add(Colors[chunks[i][j]]);
            }

            List<List<Color>> randWithin = new(chunkSize); // randomize the chunk
            for (int j = 0; j < chunkSize; j++) // whithin chunk
            {
                index = GetRandomIndex(rnd, colorList.Count);
                randWithin.Add(colorList[index]);
                colorList.RemoveAt(index);  // cleaning the ass >:|
            }
            Trace.Assert(colorList.Count == 0, "colorList count is zero (0)");
            theChunks.Add(randWithin);
        }

        List<List<List<Color>>> randChunks = new(numChunks);
        for (int i = 0; i < numChunks; i++)
        {
            index = GetRandomIndex(rnd, theChunks.Count);
            randChunks.Add(theChunks[index]);
            theChunks.RemoveAt(index);
        }
        
        List<List<Color>> colors = new(Colors.Count);
        randChunks.ForEach(c => colors.AddRange(c));
        randChunks.Clear();

        RandomColors = new(Colors.Count);
        for (int i = 0; i < Colors.Count; i++)
        {
            index = GetRandomIndex(rnd, colors.Count);
            RandomColors.Add(colors[index]);
            colors.RemoveAt(index);
        }

        IsRandomized = true;
        Redraw();
    }
}
