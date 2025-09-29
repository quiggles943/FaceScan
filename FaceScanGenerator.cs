using FaceAiSharp;
using FaceScan.Interfaces;
using FaceScan.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceScan
{
    public class FaceScanGenerator
    {
        IFaceDetectorWithLandmarks FaceDetector { get; set; }
        IFaceEmbeddingsGenerator EmbeddingGenerator { get; set; }
        private ILogger Logger { get; set; } = new NullLogger<FaceScanGenerator>();

        private FaceScanGenerator()
        {
            FaceDetector = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
            EmbeddingGenerator = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();
        }

        /// <summary>
        /// Performs a face scan on the image data provided by the specified stream and returns a list of detected
        /// faces.
        /// </summary>
        /// <remarks>The stream is read from its beginning position. The method does not modify the stream's contents.</remarks>
        /// <param name="stream">The stream containing image data to be scanned for faces.</param>
        /// <returns>A list of scanned faces detected
        /// in the image. The list is empty if no faces are found.</returns>
        public async Task<List<ScannedFace>> PerformFaceScanOnImageStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var scannedImage = Image.Load<Rgb24>(stream);
            return await PerformFaceScanOnImage(scannedImage);
        }

        /// <summary>
        /// Performs face detection and analysis on the provided image and returns a list of scanned face results.
        /// </summary>
        /// <remarks>The method processes the image and analyzes each detected face individually.
        /// The operation is may take longer for images with many faces or high resolution.</remarks>
        /// <param name="image">The image to scan for faces.</param>
        /// <param name="cancellationToken">Used to cancel the face scanning operation.</param>
        /// <returns>A list of scanned faces detected in the image. The list is empty if no faces are found.</returns>
        public async Task<List<ScannedFace>> PerformFaceScanOnImage(Image<Rgb24> image, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<FaceDetectorResult> faceDetectorResults = await ScanImageForFacesAsync(image,cancellationToken);
            List<ScannedFace> detectedFaces = new List<ScannedFace>();
            foreach (var face in faceDetectorResults)
            {
                var faceModel = await BuildFaceModel(face, image, cancellationToken);
                detectedFaces.Add(faceModel);
            }
            Logger.LogInformation("Face scan completed. {FaceCount} faces processed.", detectedFaces.Count);
            return detectedFaces;
        }

        /// <summary>
        /// Builds a face model by extracting facial coordinates, landmarks, and an embedding from the detected face in
        /// the provided image.
        /// </summary>
        /// <param name="faceDetectorResult">The result of the face detection operation, containing the bounding box and landmarks for the detected face.</param>
        /// <param name="image">The image containing the face to be processed.</param>
        /// <param name="cancellationToken">Used to cancel the model creation.</param>
        /// <returns>A ScannedFace object containing the face's coordinates, landmarks, and embedding generated from the input
        /// image.</returns>
        private async Task<ScannedFace> BuildFaceModel(FaceDetectorResult faceDetectorResult, Image<Rgb24> image, CancellationToken cancellationToken = default)
        {
            var faceScanCoordinates = new FaceScanCoordinates(faceDetectorResult.Box.X,
                                                              faceDetectorResult.Box.Y,
                                                              faceDetectorResult.Box.Width,
                                                              faceDetectorResult.Box.Height);
            List<FaceScanLandmark> landmarks = new List<FaceScanLandmark>();
            if(faceDetectorResult.Landmarks != null && faceDetectorResult.Landmarks.Any())
            {
                foreach (var landmark in faceDetectorResult.Landmarks)
                {
                    var faceScanLandmark = new FaceScanLandmark(landmark.X, landmark.Y);
                    landmarks.Add(faceScanLandmark);
                }
            }
            
            var faceScanImg = image.Clone();
            Task faceAlignment = Task.Run(() => EmbeddingGenerator.AlignFaceUsingLandmarks(faceScanImg, faceDetectorResult.Landmarks!),cancellationToken);
            await faceAlignment;
            float[] embedding = EmbeddingGenerator.GenerateEmbedding(faceScanImg);

            return new ScannedFace(faceScanCoordinates, landmarks, embedding);
        }

        /// <summary>
        /// Scans the provided image for human faces and returns the detection results.
        /// </summary>
        /// <param name="image">The image to analyze for faces. Must not be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the face detection operation.</param>
        /// <returns>A read-only collection of face detection results. The collection is empty if no faces are detected.</returns>
        private async Task<IReadOnlyCollection<FaceDetectorResult>> ScanImageForFacesAsync(Image<Rgb24> image, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            Task<IReadOnlyCollection<FaceDetectorResult>> task = Task<IReadOnlyCollection<FaceDetectorResult>>.Run(() => { return FaceDetector.DetectFaces(image); }, cancellationToken);
            IReadOnlyCollection<FaceDetectorResult> faceDetectorResults = await task;
            Logger.LogInformation("Detected {FaceCount} faces in the provided image.", faceDetectorResults.Count);
            return faceDetectorResults;
        }

        public class Builder
        {
            private ILogger logger = new NullLogger<FaceScanGenerator>();

            public FaceScanGenerator Build()
            {
                return new FaceScanGenerator() { Logger = logger };
            }

            public Builder WithLogger(ILogger logger)
            {
                this.logger = logger;
                return this;
            }
        }
    }
}
