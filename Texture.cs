﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using log4net;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK
{
    public class Texture
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Bitmap _bitmap;
        private bool isLoaded;

        public bool GenerateMipmaps { get; set; }
        public uint ID { get; private set; }

        public Texture(Bitmap bitmap)
        {
            _bitmap = bitmap;
            if (!IsPowerOf2(_bitmap))
                throw new FormatException("Texture sizes must be powers of 2!");
        }

        public Texture(string filename)
		{
			Log.DebugFormat("Loading texture filename \"{0}\"", filename);
			try
			{
				_bitmap = new Bitmap(filename);
			}
			catch (Exception ex)
			{
				Log.Error ("EXCEPTION", ex);
			}
		}

        public void Load()
        {
            if (isLoaded) return;
            ID = LoadTexture(_bitmap);
            isLoaded = true;
        }

        /// <remarks>
        /// This loads the texture mirrored along one axis, but you can easily 
        /// fix this by changing all your UVs' second coordinate from v to 1-v.
        /// </remarks>
        private uint LoadTexture(Bitmap bitmap)
        {
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            uint texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            if (!GenerateMipmaps)
            {
                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int) TextureMinFilter.Nearest);
            }
            else
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int) TextureMinFilter.Linear);

                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureBaseLevel,
                    0);

                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMaxLevel,
                    4);
            }

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bitmap.Width,
                bitmap.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);

            bitmap.UnlockBits(data);

			try
			{
	            if (GenerateMipmaps)
	            {
	                bitmap = ScaleByPercent(bitmap, 50);
	                data = bitmap.LockBits(
	                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
	                    ImageLockMode.ReadOnly,
	                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
	
	                GL.TexImage2D(
	                    TextureTarget.Texture2D,
	                    1,
	                    PixelInternalFormat.Rgba,
	                    bitmap.Width,
	                    bitmap.Height,
	                    0,
	                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
	                    PixelType.UnsignedByte,
	                    data.Scan0);
	
	                bitmap.UnlockBits(data);
	                bitmap = ScaleByPercent(bitmap, 50);
	                data = bitmap.LockBits(
	                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
	                    ImageLockMode.ReadOnly,
	                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
	
	                GL.TexImage2D(
	                    TextureTarget.Texture2D,
	                    2,
	                    PixelInternalFormat.Rgba,
	                    bitmap.Width,
	                    bitmap.Height,
	                    0,
	                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
	                    PixelType.UnsignedByte,
	                    data.Scan0);
	
	                bitmap.UnlockBits(data);
	                bitmap = ScaleByPercent(bitmap, 50);
	                data = bitmap.LockBits(
	                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
	                    ImageLockMode.ReadOnly,
	                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
	
	                GL.TexImage2D(
	                    TextureTarget.Texture2D,
	                    3,
	                    PixelInternalFormat.Rgba,
	                    bitmap.Width,
	                    bitmap.Height,
	                    0,
	                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
	                    PixelType.UnsignedByte,
	                    data.Scan0);
	
	                bitmap.UnlockBits(data);
	                bitmap = ScaleByPercent(bitmap, 50);
	                data = bitmap.LockBits(
	                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
	                    ImageLockMode.ReadOnly,
	                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
	
	                GL.TexImage2D(
	                    TextureTarget.Texture2D,
	                    4,
	                    PixelInternalFormat.Rgba,
	                    bitmap.Width,
	                    bitmap.Height,
	                    0,
	                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
	                    PixelType.UnsignedByte,
	                    data.Scan0);
	
	                bitmap.UnlockBits(data);
	            }
			}
			catch (Exception ex)
			{
				Log.Error ("EXCEPTION", ex);
			}

            return texture;
        }

        private static Bitmap ScaleByPercent(Bitmap bitmap, int Percent)
        {
            int srcWidth = bitmap.Width;
            int srcHeight = bitmap.Height;
            float scale = ((float) Percent / 100);
            var destWidth = (int) (srcWidth * scale);
            var destHeight = (int) (srcHeight * scale);
            var dest = new Bitmap(destWidth, destHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dest);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, srcWidth, srcHeight), GraphicsUnit.Pixel);
            return dest;
        }

        private static bool IsPowerOf2(Image bitmap)
        {
            int test = 1;
            bool wOK = false, hOK = false;
            for (int q = 0; q < 20; q++)
            {
                test *= 2;
                if (test == bitmap.Width) wOK = true;
                if (test == bitmap.Height) hOK = true;
                if (wOK && hOK) break;
            }
            return wOK && hOK;
        }
    }
}