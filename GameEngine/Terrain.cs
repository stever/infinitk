using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using InfiniTK.Utility;
using log4net;

namespace InfiniTK.GameEngine
{
    public class Terrain
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int BlocksHigh = 16;
        private const int BlocksWide = 16;
        private const int BlockWidth = 16;
        private const int BlockHeight = 16;
        private const int TexMapWidth = BlockWidth * 8;
        private const int TexMapHeight = BlockHeight * 8;

        public Texture[,] TextureMaps = new Texture[BlocksWide, BlocksHigh];

        public Terrain(string filename)
        {
            // Load the texture for each of the blocks contained in the terrain file.
            Log.DebugFormat("Loading terrain \"{0}\"", filename);
            var bitmap = new Bitmap(filename);
            Log.DebugFormat("Bitmap size: {0}, {1}", bitmap.Width, bitmap.Height);
            for (var row = 0; row < BlocksHigh; row++)
            {
                for (var col = 0; col < BlocksWide; col++)
                {
                    Log.DebugFormat("Column {0}; Row {1}", col, row);
                    var blockRectangle = new Rectangle(col * BlockWidth, row * BlockHeight, BlockWidth, BlockHeight);
                    var block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);

                    // Build the new texture map for this block.
                    var texmap = new Bitmap(TexMapWidth, TexMapHeight, PixelFormat.Format32bppArgb);
                    var g = Graphics.FromImage(texmap);

                    // Replace transparent background color.
                    if (col == 3 && row == 0) g.FillRectangle(new SolidBrush(block.GetPixel(0, 0)), 0, 0, TexMapWidth, TexMapHeight);
                    else g.FillRectangle(new SolidBrush(GetDominantColor(block)), 0, 0, TexMapWidth, TexMapHeight);

                    // Arrange the sides in the texture map for the block.
                    // Paste into (0, 5) (0, 7) (1, 5) (1, 7) (3, 5) (3, 7)
                    g.DrawImageUnscaled(block, 0, 5 * BlockHeight); // Right
                    g.DrawImageUnscaled(block, 0, 7 * BlockHeight); // Front
                    g.DrawImageUnscaled(block, 1 * BlockWidth, 5 * BlockHeight); // Back
                    g.DrawImageUnscaled(block, 1 * BlockWidth, 7 * BlockHeight); // Left
                    g.DrawImageUnscaled(block, 3 * BlockWidth, 5 * BlockHeight); // Top
                    g.DrawImageUnscaled(block, 3 * BlockWidth, 7 * BlockHeight); // Bottom

                    if (col == 3 && row == 0)
                    { 
                        // Grass
                        var color = block.GetPixel(0, 0);
                        blockRectangle = new Rectangle(2 * BlockWidth, 0, BlockWidth, BlockHeight);
                        block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);
                        g.FillRectangle(new SolidBrush(color), 3 * BlockWidth, 5 * BlockHeight, BlockWidth, BlockHeight); // Top
                        g.DrawImageUnscaled(block, 3 * BlockWidth, 7 * BlockHeight); // Bottom
                    }
                    else if (col == 4 && row == 1)
                    { 
                        // Tree
                        blockRectangle = new Rectangle(5 * BlockWidth, BlockHeight, BlockWidth, BlockHeight);
                        block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);
                        g.DrawImageUnscaled(block, 3 * BlockWidth, 5 * BlockHeight); // Top
                        g.DrawImageUnscaled(block, 3 * BlockWidth, 7 * BlockHeight); // Bottom
                    }

                    // Copy & paste slithers to make seamless cubes.

                    // Copy left 1px to right 8px, for rows 6, 7 and 8.
                    var sliceRectangle = new Rectangle(0, 5 * BlockHeight, 1, 3 * BlockHeight);
                    var slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 1, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 2, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 3, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 4, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 5, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 6, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 7, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, ((BlocksWide / 2) * BlockWidth) - 8, 5 * BlockHeight);

                    // Copy bottom 1px to top 8px, for columns 1-4.
                    sliceRectangle = new Rectangle(0, ((BlocksHigh / 2) * BlockHeight) - 1, 4 * BlockWidth, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, 0);
                    g.DrawImageUnscaled(slice, 0, 1);
                    g.DrawImageUnscaled(slice, 0, 2);
                    g.DrawImageUnscaled(slice, 0, 3);
                    g.DrawImageUnscaled(slice, 0, 4);
                    g.DrawImageUnscaled(slice, 0, 5);
                    g.DrawImageUnscaled(slice, 0, 6);
                    g.DrawImageUnscaled(slice, 0, 7);

                    // Copy top 1px of row 6 to bottom 8px of row 5, for columns 1-4.
                    sliceRectangle = new Rectangle(0, 5 * BlockHeight, 4 * BlockWidth, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 1);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 2);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 3);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 4);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 5);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 6);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 7);
                    g.DrawImageUnscaled(slice, 0, (5 * BlockHeight) - 8);

                    // Copy bottom 1px of row 6 to top 8px of row 7, for columns 1-4.
                    sliceRectangle = new Rectangle(0, (6 * BlockHeight) - 1, 4 * BlockWidth, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, 6 * BlockHeight);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 1);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 2);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 3);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 4);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 5);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 6);
                    g.DrawImageUnscaled(slice, 0, (6 * BlockHeight) + 7);

                    // Copy top 1px of row 8 to bottom 8px of row 7, for columns 1-4.
                    sliceRectangle = new Rectangle(0, 7 * BlockHeight, 4 * BlockWidth, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 1);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 2);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 3);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 4);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 5);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 6);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 7);
                    g.DrawImageUnscaled(slice, 0, (7 * BlockHeight) - 8);

                    // Copy right 1px of column 2 to left 8px of column 3, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle((2 * BlockWidth) - 1, 5 * BlockHeight, 1, 3 * BlockHeight);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 2 * BlockWidth, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 1, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 2, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 3, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 4, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 5, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 6, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (2 * BlockWidth) + 7, 5 * BlockHeight);

                    // Copy left 1px of column 4 to right 8px of column 3, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle(3 * BlockWidth, 5 * BlockHeight, 1, 3 * BlockHeight);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 1, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 2, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 3, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 4, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 5, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 6, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 7, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (3 * BlockWidth) - 8, 5 * BlockHeight);

                    // Copy right 1px of column 4 to left 8px of column 5, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle((4 * BlockWidth) - 1, 5 * BlockHeight, 1, 3 * BlockHeight);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 4 * BlockWidth, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 1, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 2, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 3, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 4, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 5, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 6, 5 * BlockHeight);
                    g.DrawImageUnscaled(slice, (4 * BlockWidth) + 7, 5 * BlockHeight);

                    // Create the new texture object.
                    TextureMaps[col, row] = new Texture(texmap) {GenerateMipmaps = true};
#if DEBUG
                    texmap.Save($"texmap{col}x{row}.png", ImageFormat.Png);
#endif
                }
            }
        }

        private static Color GetDominantColor(Bitmap bmp)
        {
            var r = 0;
            var g = 0;
            var b = 0;

            var total = 0;

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            // Calculate average.
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }
    }
}
