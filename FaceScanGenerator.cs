using FaceAiSharp;
using FaceONNX;
using FaceScan.Extensions;
using FaceScan.Interfaces;
using FaceScan.Interfaces.Enums;
using FaceScan.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UMapx.Core;
using Image = SixLabors.ImageSharp.Image;

namespace FaceScan
{
    public class FaceScanGenerator: IFaceScanGenerator
    {
        private IFaceDetectorWithLandmarks FaceDetector { get; set; }
        private IFaceEmbeddingsGenerator EmbeddingGenerator { get; set; }
        //private FaceGenderClassifier GenderClassifier { get; set; }
        //private FaceAgeEstimator AgeClassifier { get; set; }
        private ILogger Logger { get; set; } = new NullLogger<FaceScanGenerator>();
        private bool UseCuda { get; set; }
        private SessionOptions SessionOptions { get; set; }

        private FaceScanGenerator()
        {
            SessionOptions = UseCuda ? SessionOptions.MakeSessionOptionWithCudaProvider() : new SessionOptions();
            FaceDetector = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks(SessionOptions);
            EmbeddingGenerator = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator(SessionOptions);
            //GenderClassifier = new FaceGenderClassifier(SessionOptions);
            //AgeClassifier = new FaceAgeEstimator(SessionOptions);
        }

        public async Task<List<IScannedFaceWithClassifiers>> PerformFaceScanWithClassifiers(Stream stream, CancellationToken cancellationToken = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var scannedImage = Image.Load<Rgb24>(stream);
            return await PerformFaceScanWithClassifiers(scannedImage, cancellationToken);
        }

        public async Task<List<IScannedFace>> PerformFaceScan(Stream stream, CancellationToken cancellationToken = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var scannedImage = Image.Load<Rgb24>(stream);
            return await PerformFaceScan(scannedImage,cancellationToken);
        }

        public async Task<List<IScannedFace>> PerformFaceScan(Image<Rgb24> image, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<FaceDetectorResult> faceDetectorResults = await ScanImageForFacesAsync(image,cancellationToken);
            List<IScannedFace> detectedFaces = new List<IScannedFace>();
            foreach (var face in faceDetectorResults)
            {
                var faceModel = await BuildFaceModel(face, image, cancellationToken);
                detectedFaces.Add(faceModel);
            }
            Logger.LogInformation("Face scan completed. {FaceCount} faces processed.", detectedFaces.Count);
            return detectedFaces;
        }

        public async Task<List<IScannedFaceWithClassifiers>> PerformFaceScanWithClassifiers(Image<Rgb24> image, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<FaceDetectorResult> faceDetectorResults = await ScanImageForFacesAsync(image, cancellationToken);
            List<IScannedFaceWithClassifiers> detectedFaces = new List<IScannedFaceWithClassifiers>();
            int count = 0;
            foreach (var face in faceDetectorResults)
            {
                count++;
                var faceModel = await BuildFaceModel(face, image, cancellationToken);
                using var imageBitmap = image.ConvertToBitmap();
                var age = EstimateAge(imageBitmap);
                var genderResult = EstimateGender(imageBitmap);
                ScannedFaceWithClassifiers scannedFaceWithClassifiers = new ScannedFaceWithClassifiers(faceModel);
                scannedFaceWithClassifiers.SetGender(genderResult.Result);
                scannedFaceWithClassifiers.SetAge(age);
                detectedFaces.Add(scannedFaceWithClassifiers);

                Logger.LogInformation("Face {Count} of {Total} - Scan Confidence: {ScanConfidence}, Gender: {Gender} (Confidence: {GenderConfidence}), Estimated Age: {Age}, ",count,faceDetectorResults.Count, scannedFaceWithClassifiers.Confidence, scannedFaceWithClassifiers.GetGender(), genderResult.Confidence, scannedFaceWithClassifiers.GetAge());

            }
            Logger.LogInformation("Scan completed. {FaceCount} faces processed.", detectedFaces.Count);
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
            using var faceScanImg = image.Clone();
            await GenerateAlignedFaceImage(faceDetectorResult, faceScanImg, cancellationToken);
            float[] embedding = EmbeddingGenerator.GenerateEmbedding(faceScanImg);

            return new ScannedFace(faceScanCoordinates, landmarks, embedding,faceDetectorResult.Confidence,faceScanImg);
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

            Task<IReadOnlyCollection<FaceDetectorResult>> task = Task<IReadOnlyCollection<FaceDetectorResult>>.Run(() => { 
                return FaceDetector.DetectFaces(image); 
            }, cancellationToken);
            IReadOnlyCollection<FaceDetectorResult> faceDetectorResults = await task;
            Logger.LogInformation("Detected {FaceCount} faces in the provided image.", faceDetectorResults.Count);
            return faceDetectorResults;
        }

        /// <summary>
        /// Generates an aligned face image by aligning the face in the input image using the provided face detection result's landmarks. The input image is loaded from the provided file stream.
        /// </summary>
        /// <param name="faceDetectionResult"></param>
        /// <param name="fileStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Image<Rgb24>> GenerateAlignedFaceImage(FaceDetectorResult faceDetectionResult,Stream fileStream, CancellationToken cancellationToken = default)
        {
            Image<Rgb24> image = SixLabors.ImageSharp.Image.Load<Rgb24>(fileStream);
            return await GenerateAlignedFaceImage(faceDetectionResult, image, cancellationToken);
        }

        /// <summary>
        /// Generates an aligned face image by aligning the face in the input image using the provided face detection result's landmarks. The input image is modified in place.
        /// </summary>
        /// <param name="faceDetectionResult"></param>
        /// <param name="inputImage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Image<Rgb24>> GenerateAlignedFaceImage(FaceDetectorResult faceDetectionResult, Image<Rgb24> inputImage, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => EmbeddingGenerator.AlignFaceUsingLandmarks(inputImage, faceDetectionResult.Landmarks!), cancellationToken);
            return inputImage;
        }

        /// <summary>
        /// Estimates the gender of the person in the provided image.
        /// </summary>
        /// <param name="image">The aligned image</param>
        /// <returns>The estimated gender with the confidence of the estimation</returns>
        public ResultWithConfidence<Gender> EstimateGender(Image<Rgb24> image)
        {
            using var newImage = image.ConvertToBitmap();
            return EstimateGender(newImage);
        }

        /// <summary>
        /// Estimates the gender of the person in the provided image.
        /// </summary>
        /// <param name="image">The aligned image</param>
        /// <returns>The estimated gender with the confidence of the estimation</returns>
        public ResultWithConfidence<Gender> EstimateGender(Bitmap image)
        {
            using FaceGenderClassifier genderClassifier = new FaceGenderClassifier(SessionOptions);
            var output = genderClassifier.Forward(image);
            var max = Matrice.Max(output);
            var gender = Math.Round(max);
            var label = FaceGenderClassifier.Labels[(int)gender];
            var genderResult = Enum.Parse<Gender>(label);
            float genderConfidence = 0.0f;
            switch (genderResult)
            {
                case Gender.Male:
                    genderConfidence = 1 - max;
                    break;
                case Gender.Female:
                    genderConfidence = max;
                    break;
            }
            return new ResultWithConfidence<Gender>(genderResult, genderConfidence);
        }

        /// <summary>
        /// Estimates the age of the person in the provided image.
        /// </summary>
        /// <param name="image">The aligned image</param>
        /// <returns>The estimated age rounded to the nearest whole number</returns>
        public int EstimateAge(Image<Rgb24> image)
        {
            using var bitmap = image.ConvertToBitmap();
            return EstimateAge(bitmap);
        }

        /// <summary>
        /// Estimates the age of the person in the provided image.
        /// </summary>
        /// <param name="image">The aligned image</param>
        /// <returns>The estimated age rounded to the nearest whole number</returns>
        public int EstimateAge(Bitmap image)
        {
            using FaceAgeEstimator faceAgeEstimator = new FaceAgeEstimator(SessionOptions);
            var output = faceAgeEstimator.Forward(image);
            int result = int.Parse(Math.Round(output.First()).ToString());
            return result;
        }

        public void Dispose()
        {
            if(FaceDetector != null && typeof(IDisposable).IsAssignableFrom(FaceDetector.GetType()))
            {
                ((IDisposable)FaceDetector).Dispose();
                FaceDetector = null;
            }
            if (EmbeddingGenerator != null && typeof(IDisposable).IsAssignableFrom(EmbeddingGenerator.GetType()))
            {
                ((IDisposable)EmbeddingGenerator).Dispose();
                EmbeddingGenerator = null;
            }
        }

        public class Builder
        {
            private ILogger logger = new NullLogger<FaceScanGenerator>();
            private bool useCuda = false;

            public FaceScanGenerator Build()
            {
                return new FaceScanGenerator() { Logger = logger, UseCuda = useCuda };
            }

            public Builder WithLogger(ILogger logger)
            {
                this.logger = logger;
                return this;
            }

            public Builder UseCuda(bool useCuda)
            {
                this.useCuda = useCuda;
                return this;
            }
        }
    }
}
