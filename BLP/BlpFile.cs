/*
 * Copyright (c) <2011> <by Xalcon @ mmowned.com-Forum>
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SereniaBLPLib
{
    // Some Helper Struct to store Color-Data
    public struct ARGBColor8
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        /// <summary>
        /// Converts the given Pixel-Array into the BGRA-Format
        /// This will also work vice versa
        /// </summary>
        /// <param name="pixel"></param>
        public static void ConvertToBGRA(byte[] pixel)
        {
            for (int i = 0; i < pixel.Length; i += 4)
            {
                byte tmp = pixel[i]; // store red
                pixel[i] = pixel[i + 2]; // write blue into red
                pixel[i + 2] = tmp; // write stored red into blue
            }
        }
    }

    public enum BlpColorEncoding : byte
    {
        Jpeg = 0,
        Palette = 1,
        Dxt = 2,
        Argb8888 = 3,
        Argb8888_dup = 4
    }

    public enum BlpPixelFormat : byte
    {
        Dxt1 = 0,
        Dxt3 = 1,
        Argb8888 = 2,
        Argb1555 = 3,
        Argb4444 = 4,
        Rgb565 = 5,
        A8 = 6,
        Dxt5 = 7,
        Unspecified = 8,
        Argb2565 = 9,
        Bc5 = 11
    }

    public sealed class BlpFile : IDisposable
    {
        private readonly uint formatVersion; // compression: 0 = JPEG Compression, 1 = Uncompressed or DirectX Compression
        private readonly BlpColorEncoding colorEncoding; // 1 = Uncompressed, 2 = DirectX Compressed
        private readonly byte alphaSize; // 0 = no alpha, 1 = 1 Bit, 4 = Bit (only DXT3), 8 = 8 Bit Alpha
        private readonly BlpPixelFormat preferredFormat; // 0: DXT1 alpha (0 or 1 Bit alpha), 1 = DXT2/3 alpha (4 Bit), 7: DXT4/5 (interpolated alpha)
        private readonly byte hasMips; // If true (1), then there are Mipmaps
        private readonly int width; // X Resolution of the biggest Mipmap
        private readonly int height; // Y Resolution of the biggest Mipmap

        private readonly uint[] mipOffsets = new uint[16]; // Offset for every Mipmap level. If 0 = no more mitmap level
        private readonly uint[] mipSizes = new uint[16]; // Size for every level
        private readonly ARGBColor8[] paletteBGRA = new ARGBColor8[256]; // The color-palette for non-compressed pictures
        private readonly byte[] jpegHeader;

        private Stream stream; // Reference of the stream

        /// <summary>
        /// Extracts the palettized Image-Data from the given Mipmap and returns a byte-Array in the 32Bit RGBA-Format
        /// </summary>
        /// <param name="mipmap">The desired Mipmap-Level. If the given level is invalid, the smallest available level is choosen</param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="data"></param>
        /// <returns>Pixel-data</returns>
        private byte[] GetPictureUncompressedByteArray(int w, int h, byte[] data)
        {
            int length = w * h;
            byte[] pic = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                pic[i * 4] = paletteBGRA[data[i]].R;
                pic[i * 4 + 1] = paletteBGRA[data[i]].G;
                pic[i * 4 + 2] = paletteBGRA[data[i]].B;
                pic[i * 4 + 3] = GetAlpha(data, i, length);
            }
            return pic;
        }

        private byte GetAlpha(byte[] data, int index, int alphaStart)
        {
            switch (alphaSize)
            {
                default:
                    return 0xFF;
                case 1:
                    {
                        byte b = data[alphaStart + (index / 8)];
                        return (byte)((b & (0x01 << (index % 8))) == 0 ? 0x00 : 0xff);
                    }
                case 4:
                    {
                        byte b = data[alphaStart + (index / 2)];
                        return (byte)(index % 2 == 0 ? (b & 0x0F) << 4 : b & 0xF0);
                    }
                case 8:
                    return data[alphaStart + index];
            }
        }

        /// <summary>
        /// Returns the raw Mipmap-Image Data. This data can either be compressed or uncompressed, depending on the Header-Data
        /// </summary>
        /// <param name="mipmapLevel"></param>
        /// <returns></returns>
        private byte[] GetPictureData(int mipmapLevel)
        {
            if (stream != null)
            {
                byte[] data = new byte[mipSizes[mipmapLevel]];
                stream.Position = mipOffsets[mipmapLevel];
                stream.Read(data, 0, data.Length);
                return data;
            }
            return null;
        }

        /// <summary>
        /// Returns the amount of Mipmaps in this BLP-File
        /// </summary>
        public int MipMapCount
        {
            get
            {
                int i = 0;
                while (mipOffsets[i] != 0) i++;
                return i;
            }
        }

        private const int BLP0Magic = 0x30504c42;
        private const int BLP1Magic = 0x31504c42;
        private const int BLP2Magic = 0x32504c42;

        public BlpFile(Stream stream)
        {
            this.stream = stream;

            using (BinaryReader br = new BinaryReader(stream, Encoding.ASCII, true))
            {
                // Checking for correct Magic-Code
                int format = br.ReadInt32();
                if (format != BLP0Magic && format != BLP1Magic && format != BLP2Magic)
                    throw new Exception("Invalid BLP Format");

                switch (format)
                {
                    case BLP0Magic:
                    case BLP1Magic:
                        // Reading encoding, alphaBitDepth, alphaEncoding and hasMipmaps
                        colorEncoding = (BlpColorEncoding)br.ReadInt32();
                        alphaSize = (byte)br.ReadInt32();
                        // Reading width and height
                        width = br.ReadInt32();
                        height = br.ReadInt32();
                        preferredFormat = (BlpPixelFormat)br.ReadInt32();
                        hasMips = (byte)br.ReadInt32();
                        break;
                    case BLP2Magic:
                        formatVersion = br.ReadUInt32();// Reading type
                        if (formatVersion != 1)
                            throw new Exception("Invalid BLP-Type! Should be 1 but " + formatVersion + " was found");
                        // Reading encoding, alphaBitDepth, alphaEncoding and hasMipmaps
                        colorEncoding = (BlpColorEncoding)br.ReadByte();
                        alphaSize = br.ReadByte();
                        preferredFormat = (BlpPixelFormat)br.ReadByte();
                        hasMips = br.ReadByte();
                        // Reading width and height
                        width = br.ReadInt32();
                        height = br.ReadInt32();
                        break;
                }

                // Reading MipmapOffset Array
                for (int i = 0; i < 16; i++)
                    mipOffsets[i] = br.ReadUInt32();

                // Reading MipmapSize Array
                for (int i = 0; i < 16; i++)
                    mipSizes[i] = br.ReadUInt32();

                // When encoding is 1, there is no image compression and we have to read a color palette
                if (colorEncoding == BlpColorEncoding.Palette)
                {
                    // Reading palette
                    for (int i = 0; i < 256; i++)
                    {
                        int color = br.ReadInt32();
                        paletteBGRA[i].B = (byte)((color >> 0) & 0xFF);
                        paletteBGRA[i].G = (byte)((color >> 8) & 0xFF);
                        paletteBGRA[i].R = (byte)((color >> 16) & 0xFF);
                        paletteBGRA[i].A = (byte)((color >> 24) & 0xFF);
                    }
                }
                else if (colorEncoding == BlpColorEncoding.Jpeg)
                {
                    int jpegHeaderSize = br.ReadInt32();
                    jpegHeader = br.ReadBytes(jpegHeaderSize);
                    // what do we do with this header?
                }
            }
        }

        /// <summary>
        /// Returns the uncompressed image as a byte array in the 32pppRGBA-Format
        /// </summary>
        private byte[] GetImageBytes(int w, int h, byte[] data)
        {
            switch (colorEncoding)
            {
                case BlpColorEncoding.Jpeg:
                    {
                        if (jpegHeader.Length != 0)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ms.Write(jpegHeader, 0, jpegHeader.Length);
                                ms.Write(data, 0, data.Length);
                                ms.Position = 0;

                                using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(ms))
                                {
                                    byte[] pixelBytes = new byte[img.Width * img.Height * Unsafe.SizeOf<Rgba32>()];
                                    img.CopyPixelDataTo(pixelBytes);
                                    return pixelBytes;
                                }
                            }
                        }
                        else
                        {
                            using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(data))
                            {
                                byte[] pixelBytes = new byte[img.Width * img.Height * Unsafe.SizeOf<Rgba32>()];
                                img.CopyPixelDataTo(pixelBytes);
                                return pixelBytes;
                            }
                        }

                        //using (var img = SixLabors.ImageSharp.Image.Load<Rgba32>(data))
                        //{
                        //    byte[] rgba = new byte[width * height * 4];
                        //    img.ProcessPixelRows(accessor =>
                        //    {
                        //        int i = 0;
                        //        for (int y = 0; y < accessor.Height; y++)
                        //        {
                        //            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                        //            for (int x = 0; x < pixelRow.Length; x++)
                        //            {
                        //                ref Rgba32 pixel = ref pixelRow[x];

                        //                rgba[i + 0] = pixel.R;
                        //                rgba[i + 1] = pixel.G;
                        //                rgba[i + 2] = pixel.B;
                        //                rgba[i + 3] = pixel.A;
                        //                i += 4;
                        //            }
                        //        }
                        //    });
                        //    return rgba;
                        //}
                    }
                case BlpColorEncoding.Palette:
                    return GetPictureUncompressedByteArray(w, h, data);
                case BlpColorEncoding.Dxt:
                    DXTDecompression.DXTFlags flag = (alphaSize > 1) ? ((preferredFormat == BlpPixelFormat.Dxt5) ? DXTDecompression.DXTFlags.DXT5 : DXTDecompression.DXTFlags.DXT3) : DXTDecompression.DXTFlags.DXT1;
                    return DXTDecompression.DecompressImage(w, h, data, flag);
                case BlpColorEncoding.Argb8888:
                    return data;
                default:
                    return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Converts the BLP to a SixLabors.ImageSharp.Image
        /// </summary>
        public Image<Rgba32> GetImage(int mipmapLevel)
        {
            byte[] pic = GetPixels(mipmapLevel, out int w, out int h, colorEncoding == BlpColorEncoding.Argb8888);

            var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pic, w, h);

            return image;
        }

        /// <summary>
        /// Returns array of pixels in BGRA or RGBA order
        /// </summary>
        /// <param name="mipmapLevel"></param>
        /// <returns></returns>
        public byte[] GetPixels(int mipmapLevel, out int w, out int h, bool bgra = true)
        {
            if (mipmapLevel >= MipMapCount)
                mipmapLevel = MipMapCount - 1;
            if (mipmapLevel < 0)
                mipmapLevel = 0;

            int scale = (int)Math.Pow(2, mipmapLevel);
            w = width / scale;
            h = height / scale;

            byte[] data = GetPictureData(mipmapLevel);
            byte[] pic = GetImageBytes(w, h, data); // This bytearray stores the Pixel-Data

            if (bgra) // when we want to copy the pixeldata directly into the bitmap, we have to convert them into BGRA before doing so
                ARGBColor8.ConvertToBGRA(pic);

            return pic;
        }

        /// <summary>
        /// Runs close()
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Closes the Memorystream
        /// </summary>
        public void Close()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
        }
    }
}