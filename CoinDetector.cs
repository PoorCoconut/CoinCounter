using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CoinCounter
{
    /// <summary>One detected coin.</summary>
    public class CoinInfo
    {
        public int X, Y, Width, Height, Area;
        public double Diameter;      // average of bounding box width and height (pixels)
        public Denomination Denom;
    }

    public class Denomination
    {
        public string Name;
        public decimal Value;
        public double MaxDiameter;   // coins smaller than this (and bigger than the previous one) belong here
        public Color Color;
        public Denomination(string name, decimal value, double maxDiameter, Color color)
        { Name = name; Value = value; MaxDiameter = maxDiameter; Color = color; }
    }

    /// <summary>
    /// Coin counting pipeline, no third-party libraries:
    /// grayscale -> adaptive threshold -> 3x3 opening -> connected components -> size classification.
    /// </summary>
    public static class CoinDetector
    {
        // Sizes (in pixels) were measured on the provided scan (776x1024).
        // Bounding-box diameters: 5c ~60, 10c ~66, 25c ~77, P1 ~91, P5 ~102. Cut-offs sit between them.
        public static readonly Denomination[] Denominations = new Denomination[]
        {
            new Denomination("5 sentimo",  0.05m, 63.5,  Color.Magenta),
            new Denomination("10 sentimo", 0.10m, 71.5,  Color.Orange),
            new Denomination("25 sentimo", 0.25m, 84.0,  Color.Lime),
            new Denomination("1 piso",     1.00m, 96.5,  Color.Cyan),
            new Denomination("5 piso",     5.00m, 9999,  Color.Red),
        };

        const int MinCoinArea = 800;   // ignore specks / dust

        // ---------- Step 1: grayscale ----------
        public static byte[,] ToGray(Bitmap src)
        {
            int w = src.Width, h = src.Height;
            BitmapData bd = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] raw = new byte[bd.Stride * h];
            Marshal.Copy(bd.Scan0, raw, 0, raw.Length);
            int stride = bd.Stride;
            src.UnlockBits(bd);

            byte[,] g = new byte[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * stride + x * 3;            // BGR order
                    g[x, y] = (byte)((raw[i + 2] + raw[i + 1] + raw[i]) / 3);
                }
            return g;
        }

        // ---------- Step 2: adaptive threshold ----------
        // The paper is the most common gray level, so threshold = background peak - margin.
        public static int FindThreshold(byte[,] g, int margin)
        {
            int[] hist = new int[256];
            foreach (byte v in g) hist[v]++;
            int peak = 0;
            for (int i = 1; i < 256; i++) if (hist[i] > hist[peak]) peak = i;
            return Math.Max(0, peak - margin);
        }

        public static bool[,] Threshold(byte[,] g, int t)
        {
            int w = g.GetLength(0), h = g.GetLength(1);
            bool[,] m = new bool[w, h];                    // true = coin (dark)
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    m[x, y] = g[x, y] < t;
            return m;
        }

        // ---------- Step 3: morphology (3x3 opening removes noise) ----------
        static bool[,] Erode(bool[,] m)
        {
            int w = m.GetLength(0), h = m.GetLength(1);
            bool[,] r = new bool[w, h];
            for (int x = 1; x < w - 1; x++)
                for (int y = 1; y < h - 1; y++)
                {
                    bool all = true;
                    for (int dx = -1; dx <= 1 && all; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                            if (!m[x + dx, y + dy]) { all = false; break; }
                    r[x, y] = all;
                }
            return r;
        }

        static bool[,] Dilate(bool[,] m)
        {
            int w = m.GetLength(0), h = m.GetLength(1);
            bool[,] r = new bool[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (!m[x, y]) continue;
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (nx >= 0 && ny >= 0 && nx < w && ny < h) r[nx, ny] = true;
                        }
                }
            return r;
        }

        public static bool[,] Open(bool[,] m) { return Dilate(Erode(m)); }

        // ---------- Step 4: connected components (8-connected flood fill, no recursion) ----------
        public static List<CoinInfo> FindBlobs(bool[,] m)
        {
            int w = m.GetLength(0), h = m.GetLength(1);
            bool[,] seen = new bool[w, h];
            List<CoinInfo> blobs = new List<CoinInfo>();
            Stack<int> stack = new Stack<int>();

            for (int sx = 0; sx < w; sx++)
                for (int sy = 0; sy < h; sy++)
                {
                    if (!m[sx, sy] || seen[sx, sy]) continue;

                    int minX = sx, maxX = sx, minY = sy, maxY = sy, area = 0;
                    seen[sx, sy] = true;
                    stack.Push(sy * w + sx);
                    while (stack.Count > 0)
                    {
                        int p = stack.Pop();
                        int x = p % w, y = p / w;
                        area++;
                        if (x < minX) minX = x; if (x > maxX) maxX = x;
                        if (y < minY) minY = y; if (y > maxY) maxY = y;
                        for (int dx = -1; dx <= 1; dx++)
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = x + dx, ny = y + dy;
                                if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                                if (!m[nx, ny] || seen[nx, ny]) continue;
                                seen[nx, ny] = true;
                                stack.Push(ny * w + nx);
                            }
                    }

                    if (area < MinCoinArea) continue;
                    CoinInfo c = new CoinInfo();
                    c.X = minX; c.Y = minY;
                    c.Width = maxX - minX + 1; c.Height = maxY - minY + 1;
                    c.Area = area;
                    c.Diameter = (c.Width + c.Height) / 2.0;
                    blobs.Add(c);
                }
            return blobs;
        }

        // ---------- Step 5: classify by size ----------
        public static void Classify(CoinInfo c)
        {
            foreach (Denomination d in Denominations)
                if (c.Diameter < d.MaxDiameter) { c.Denom = d; return; }
            c.Denom = Denominations[Denominations.Length - 1];
        }

        public static List<CoinInfo> Detect(Bitmap src, out int threshold)
        {
            byte[,] gray = ToGray(src);
            threshold = FindThreshold(gray, 30);
            bool[,] mask = Open(Threshold(gray, threshold));
            List<CoinInfo> coins = FindBlobs(mask);
            foreach (CoinInfo c in coins) Classify(c);
            return coins;
        }

        // Draw colored boxes + labels on a copy of the source image.
        public static Bitmap Annotate(Bitmap src, List<CoinInfo> coins)
        {
            Bitmap result = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(src, 0, 0, src.Width, src.Height);
                using (Font f = new Font("Arial", 11, FontStyle.Bold))
                    foreach (CoinInfo c in coins)
                    {
                        using (Pen p = new Pen(c.Denom.Color, 3))
                            g.DrawEllipse(p, c.X, c.Y, c.Width, c.Height);
                        string t = c.Denom.Value >= 1 ? ((int)c.Denom.Value).ToString() : ((int)(c.Denom.Value * 100)) + "c";
                        g.DrawString(t, f, Brushes.Yellow, c.X + c.Width / 2 - 10, c.Y + c.Height / 2 - 8);
                    }
            }
            return result;
        }
    }
}
