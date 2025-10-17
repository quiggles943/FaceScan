using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Interfaces
{
    public interface IFaceScanGenerator: IDisposable
    {

        /// <summary>
        /// Performs a face scan on the image data provided by the specified stream and returns a list of detected
        /// faces.
        /// </summary>
        /// <remarks>The stream is read from its beginning position. The method does not modify the stream's contents.</remarks>
        /// <param name="stream">The stream containing image data to be scanned for faces.</param>
        /// <returns>A list of scanned faces detected
        /// in the image. The list is empty if no faces are found.</returns>
        public Task<List<IScannedFace>> PerformFaceScan(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs face detection and analysis on the provided image and returns a list of scanned face results.
        /// </summary>
        /// <remarks>The method processes the image and analyzes each detected face individually.
        /// The operation is may take longer for images with many faces or high resolution.</remarks>
        /// <param name="image">The image to scan for faces.</param>
        /// <param name="cancellationToken">Used to cancel the face scanning operation.</param>
        /// <returns>A list of scanned faces detected in the image. The list is empty if no faces are found.</returns>
        public Task<List<IScannedFace>> PerformFaceScan(Image<Rgb24> image, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a face scan on the image data provided by the specified stream and returns a list of detected
        /// faces.
        /// </summary>
        /// <remarks>The stream is read from its beginning position. The method does not modify the stream's contents.</remarks>
        /// <param name="stream">The stream containing image data to be scanned for faces.</param>
        /// <returns>A list of scanned faces detected
        /// in the image. The list is empty if no faces are found.</returns>
        public Task<List<IScannedFaceWithClassifiers>> PerformFaceScanWithClassifiers(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs face detection and analysis on the provided image and returns a list of scanned face results.
        /// </summary>
        /// <remarks>The method processes the image and analyzes each detected face individually.
        /// The operation is may take longer for images with many faces or high resolution.</remarks>
        /// <param name="image">The image to scan for faces.</param>
        /// <param name="cancellationToken">Used to cancel the face scanning operation.</param>
        /// <returns>A list of scanned faces detected in the image. The list is empty if no faces are found.</returns>
        public Task<List<IScannedFaceWithClassifiers>> PerformFaceScanWithClassifiers(Image<Rgb24> image, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates an aligned face image by aligning the face in the input image using the provided face detection result's landmarks. The input image is loaded from the provided file stream.
        /// </summary>
        /// <param name="faceDetectionResult"></param>
        /// <param name="fileStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Image<Rgb24>> GenerateAlignedFaceImage(FaceDetectorResult faceDetectionResult, Stream fileStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates an aligned face image by aligning the face in the input image using the provided face detection result's landmarks. The input image is modified in place.
        /// </summary>
        /// <param name="faceDetectionResult"></param>
        /// <param name="inputImage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Image<Rgb24>> GenerateAlignedFaceImage(FaceDetectorResult faceDetectionResult, Image<Rgb24> inputImage, CancellationToken cancellationToken = default);
    }
}
