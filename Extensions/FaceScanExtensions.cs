using FaceScan.Interfaces;
using FaceScan.Structures;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using FaceScan.Enums;

namespace FaceScan.Extensions
{
    public static class FaceScanExtensions
    {
        public static float Similarity(this IFaceModel a, IFaceModel b)
        {
            ArgumentNullException.ThrowIfNull(a, nameof(a));
            ArgumentNullException.ThrowIfNull(b, nameof(b));
            if (a.GetVectors().Count != b.GetVectors().Count)
            {
                throw new ArgumentException("Vector lengths do not match.");
            }
            return a.GetVectors().Dot(b.GetVectors());
        }

        public static Bitmap ConvertToBitmap(this Image<Rgb24> image)
        {
            using var memoryStream = new MemoryStream();
            var imageEncoder = image.Configuration.ImageFormatsManager.GetEncoder(image.Metadata.DecodedImageFormat ?? PngFormat.Instance);
            image.Save(memoryStream, imageEncoder);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var newImage = new System.Drawing.Bitmap(memoryStream);
            return newImage;
        }

        public unsafe static float[][,] ConvertToBGR(this Image<Rgb24> image)
        {
            int width = image.Width;
            int height = image.Height;
            float[,] x = new float[height, width];
            float[,] y = new float[height, width];
            float[,] z = new float[height, width];
            image.ProcessPixelRows(accessor =>
            {
                for (int j = 0; j <accessor.Height; j++)
                {
                    Span<Rgb24> pixelRow = accessor.GetRowSpan(j);
                    for (int i = 0; i < pixelRow.Length; i++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgb24 pixel = ref pixelRow[i];
                        x[j, i] = (float)pixel.R / 255f;
                        y[j, i] = (float)pixel.G / 255f;
                        z[j, i] = (float)pixel.B / 255f;
                    }
                }
            });
            return new float[3][,] { x, y, z };

            /*image.Metadata.
            int width = image.Width;
            int height = image.Height;
            int stride = bmData.Stride;
            byte* p = (byte*)bmData.Scan0.ToPointer();
            float[,] x = new float[height, width];
            float[,] y = new float[height, width];
            float[,] z = new float[height, width];

            Parallel.For(0, height, delegate (int j)
            {
                int num = j * stride;
                for (int i = 0; i < width; i++)
                {
                    int num2 = num + i * 4;
                    x[j, i] = (float)(int)p[num2] / 255f;
                    y[j, i] = (float)(int)p[num2 + 1] / 255f;
                    z[j, i] = (float)(int)p[num2 + 2] / 255f;
                }
            });
            return new float[3][,] { x, y, z };*/
        }

        public static TModel UpdateAggregateModel<TModel>(this TModel model,IEnumerable<float> vectors, AggregateFaceModelUpdateType updateType) where TModel : IAggregateFaceModel
        {
            var existingVectors = model.GetVectors().ToArray();
            float[] newVectors;
            switch (updateType)
            {
                case AggregateFaceModelUpdateType.Add:
                    newVectors = existingVectors.AddToAverage(model.GetModelCount(), vectors.ToArray());
                    model.UpdateModel(newVectors, model.GetModelCount() + 1);
                    break;
                case AggregateFaceModelUpdateType.Remove:
                    newVectors = existingVectors.RemoveFromAverage(model.GetModelCount(), vectors.ToArray());
                    model.UpdateModel(newVectors, model.GetModelCount() - 1);
                    break;
            }
            return model;
        }
    }
}
