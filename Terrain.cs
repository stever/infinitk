using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using log4net;

namespace InfiniTK
{
    public class Terrain
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int BLOCKS_HIGH = 16;
        private const int BLOCKS_WIDE = 16;
        private const int BLOCK_WIDTH = 16;
        private const int BLOCK_HEIGHT = 16;
        private const int TEX_MAP_WIDTH = BLOCK_WIDTH * 8;
        private const int TEX_MAP_HEIGHT = BLOCK_HEIGHT * 8;

        public Texture[,] TextureMaps = new Texture[BLOCKS_WIDE, BLOCKS_HIGH];

        public Terrain(string filename)
        {
            // Load the texture for each of the blocks contained in the terrain file.
            Log.DebugFormat("Loading terrain \"{0}\"", filename);
            Bitmap bitmap = new Bitmap(filename);
            Log.DebugFormat("Bitmap size: {0}, {1}", bitmap.Width, bitmap.Height);
            for (int row = 0; row < BLOCKS_HIGH; row++)
            {
                for (int col = 0; col < BLOCKS_WIDE; col++)
                {
                    Log.DebugFormat("Column {0}; Row {1}", col, row);
                    Rectangle blockRectangle = new Rectangle(col * BLOCK_WIDTH, row * BLOCK_HEIGHT, BLOCK_WIDTH, BLOCK_HEIGHT);
                    Bitmap block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);

                    // Build the new texture map for this block.
                    Bitmap texmap = new Bitmap(TEX_MAP_WIDTH, TEX_MAP_HEIGHT, PixelFormat.Format32bppArgb);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(texmap);

                    // Replace transparent background color.
                    if (col == 3 && row == 0) g.FillRectangle(new SolidBrush(block.GetPixel(0, 0)), 0, 0, TEX_MAP_WIDTH, TEX_MAP_HEIGHT);
                    else g.FillRectangle(new SolidBrush(GetDominantColor(block)), 0, 0, TEX_MAP_WIDTH, TEX_MAP_HEIGHT);

                    // Arrange the sides in the texture map for the block.
                    // Paste into (0, 5) (0, 7) (1, 5) (1, 7) (3, 5) (3, 7)
                    g.DrawImageUnscaled(block, 0, 5 * BLOCK_HEIGHT); // Right
                    g.DrawImageUnscaled(block, 0, 7 * BLOCK_HEIGHT); // Front
                    g.DrawImageUnscaled(block, 1 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT); // Back
                    g.DrawImageUnscaled(block, 1 * BLOCK_WIDTH, 7 * BLOCK_HEIGHT); // Left
                    g.DrawImageUnscaled(block, 3 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT); // Top
                    g.DrawImageUnscaled(block, 3 * BLOCK_WIDTH, 7 * BLOCK_HEIGHT); // Bottom

                    if (col == 3 && row == 0)
                    { // Grass
                        Color color = block.GetPixel(0, 0);
                        blockRectangle = new Rectangle(2 * BLOCK_WIDTH, 0, BLOCK_WIDTH, BLOCK_HEIGHT);
                        block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);
                        g.FillRectangle(new SolidBrush(color), 3 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT, BLOCK_WIDTH, BLOCK_HEIGHT); // Top
                        g.DrawImageUnscaled(block, 3 * BLOCK_WIDTH, 7 * BLOCK_HEIGHT); // Bottom
                    }
                    else if (col == 4 && row == 1)
                    { // Tree
                        blockRectangle = new Rectangle(5 * BLOCK_WIDTH, BLOCK_HEIGHT, BLOCK_WIDTH, BLOCK_HEIGHT);
                        block = bitmap.Clone(blockRectangle, PixelFormat.Format32bppArgb);
                        g.DrawImageUnscaled(block, 3 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT); // Top
                        g.DrawImageUnscaled(block, 3 * BLOCK_WIDTH, 7 * BLOCK_HEIGHT); // Bottom
                    }

                    // Copy & paste slithers to make seamless cubes.

                    // Copy left 1px to right 8px, for rows 6, 7 and 8.
                    Rectangle sliceRectangle = new Rectangle(0, 5 * BLOCK_HEIGHT, 1, 3 * BLOCK_HEIGHT);
                    Bitmap slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 1, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 2, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 3, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 4, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 5, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 6, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 7, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, ((BLOCKS_WIDE / 2) * BLOCK_WIDTH) - 8, 5 * BLOCK_HEIGHT);

                    // Copy bottom 1px to top 8px, for columns 1-4.
                    sliceRectangle = new Rectangle(0, ((BLOCKS_HIGH / 2) * BLOCK_HEIGHT) - 1, 4 * BLOCK_WIDTH, 1);
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
                    sliceRectangle = new Rectangle(0, 5 * BLOCK_HEIGHT, 4 * BLOCK_WIDTH, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 1);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 2);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 3);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 4);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 5);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 6);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 7);
                    g.DrawImageUnscaled(slice, 0, (5 * BLOCK_HEIGHT) - 8);

                    // Copy bottom 1px of row 6 to top 8px of row 7, for columns 1-4.
                    sliceRectangle = new Rectangle(0, (6 * BLOCK_HEIGHT) - 1, 4 * BLOCK_WIDTH, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, 6 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 1);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 2);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 3);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 4);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 5);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 6);
                    g.DrawImageUnscaled(slice, 0, (6 * BLOCK_HEIGHT) + 7);

                    // Copy top 1px of row 8 to bottom 8px of row 7, for columns 1-4.
                    sliceRectangle = new Rectangle(0, 7 * BLOCK_HEIGHT, 4 * BLOCK_WIDTH, 1);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 1);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 2);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 3);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 4);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 5);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 6);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 7);
                    g.DrawImageUnscaled(slice, 0, (7 * BLOCK_HEIGHT) - 8);

                    // Copy right 1px of column 2 to left 8px of column 3, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle((2 * BLOCK_WIDTH) - 1, 5 * BLOCK_HEIGHT, 1, 3 * BLOCK_HEIGHT);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 2 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 1, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 2, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 3, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 4, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 5, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 6, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (2 * BLOCK_WIDTH) + 7, 5 * BLOCK_HEIGHT);

                    // Copy left 1px of column 4 to right 8px of column 3, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle(3 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT, 1, 3 * BLOCK_HEIGHT);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 1, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 2, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 3, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 4, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 5, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 6, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 7, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (3 * BLOCK_WIDTH) - 8, 5 * BLOCK_HEIGHT);

                    // Copy right 1px of column 4 to left 8px of column 5, for rows 6, 7 and 8.
                    sliceRectangle = new Rectangle((4 * BLOCK_WIDTH) - 1, 5 * BLOCK_HEIGHT, 1, 3 * BLOCK_HEIGHT);
                    slice = texmap.Clone(sliceRectangle, PixelFormat.Format32bppArgb);
                    g.DrawImageUnscaled(slice, 4 * BLOCK_WIDTH, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 1, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 2, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 3, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 4, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 5, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 6, 5 * BLOCK_HEIGHT);
                    g.DrawImageUnscaled(slice, (4 * BLOCK_WIDTH) + 7, 5 * BLOCK_HEIGHT);

                    // Create the new texture object.
                    TextureMaps[col, row] = new Texture(texmap);
                    TextureMaps[col, row].GenerateMipmaps = true;
                    if (Log.IsDebugEnabled)
                        texmap.Save(string.Format("texmap{0}x{1}.png", col, row), ImageFormat.Png);
                }
            }
        }

        private static Color GetDominantColor(Bitmap bmp)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

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
