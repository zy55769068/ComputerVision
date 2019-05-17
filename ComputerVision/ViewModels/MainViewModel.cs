using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ComputerVision.Common;
using ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using System.Diagnostics;

namespace ComputerVision.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        /// <summary>
        /// Gets the take photo.
        /// </summary>
        /// <value>The take photo.</value>
        public ICommand TakePhoto => new Command(async () =>
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                DefaultCamera = CameraDevice.Rear
            });

            HandlePhoto(photo);
        });


        /// <summary>
        /// Gets the pick photo.
        /// </summary>
        /// <value>The pick photo.</value>
        public ICommand PickPhoto => new Command(async () =>
        {
            var option = new PickMediaOptions
            {
                SaveMetaData = false,
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
            };
            var photo = await CrossMedia.Current.PickPhotoAsync(option);

            HandlePhoto(photo);
        });

        /// <summary>
        /// Handles the photo.
        /// </summary>
        /// <param name="photo">Photo.</param>
        private async void HandlePhoto(MediaFile photo)
        {
            IsBusy = true;

            if (photo == null)
            {
                return;
            }

            ImageSource = photo.Path;

            var visualFeatures = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Brands,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Color,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces,
                VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Objects,
                VisualFeatureTypes.Tags
            };

            using (var client = new ComputerVisionClient(ClientSetting.Credentials)
            {
                Endpoint = ClientSetting.EndPoint
            })
            {
                try
                {
                    using (var stream = photo.GetStream())
                    {
                        Debug.WriteLine("Send image to Azure Start");
                        var analysis = await client.AnalyzeImageInStreamAsync(stream, visualFeatures, null);

                        this.ResultModel = new ResultModel
                        {
                            Adult = analysis.Adult,
                            Brands = analysis.Brands,
                            Categories = analysis.Categories,
                            Color = analysis.Color,
                            Description = analysis.Description,
                            Faces = analysis.Faces,
                            ImageType = analysis.ImageType,
                            Metadata = analysis.Metadata,
                            Objects = analysis.Objects,
                            RequestId = analysis.RequestId,
                            Tags = analysis.Tags
                        };

                        LogResult(analysis);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.InnerException.Message);
                }
            }

            IsBusy = false;
        }

        private void LogResult(ImageAnalysis result)
        {
            if (result == null)
            {
                Debug.WriteLine("null");
                return;
            }
            if (result.Metadata != null)
            {
                Debug.WriteLine("Metadata : ");
                Debug.WriteLine($"Format : {result.Metadata.Format}");
                Debug.WriteLine($"Width  : {result.Metadata.Width}; Height {result.Metadata.Height}");
            }

            if (result.ImageType != null)
            {
                Debug.WriteLine($"Image ClipArtType : { result.ImageType.ClipArtType }");
                Debug.WriteLine($"Image LineDrawingType : { result.ImageType.LineDrawingType }");
            }

            if (result.Adult != null)
            {
                Debug.WriteLine("Adult : ");
                Debug.WriteLine($"Is Adult Content : {result.Adult.IsAdultContent}");
                Debug.WriteLine($"Adult Score : {result.Adult.AdultScore}");
                Debug.WriteLine($"Is Racy Content : {result.Adult.IsRacyContent}");
                Debug.WriteLine($"Racy Score : {result.Adult.RacyScore}");
            }

            if (result.Brands != null)
            {
                Debug.WriteLine("Brands : ");
                foreach (var brand in result.Brands)
                {
                    Debug.WriteLine($"Brand : {brand.Name}; Confidence : {brand.Confidence}; Bounding Box: {brand.Rectangle.ToString()}");
                }
            }

            if (result.Objects != null)
            {
                Debug.WriteLine("Objects : ");
                foreach (var obj in result.Objects)
                {
                    Debug.WriteLine($"Object : Bounding Box : {obj.Rectangle.ToString()}");
                    var detected = new ObjectHierarchy(obj.ObjectProperty, obj.Confidence, obj.Parent);
                    while (detected != null)
                    {
                        Debug.WriteLine($"Label : {detected.ObjectProperty}; Confidence : {detected.Confidence}");
                        detected = detected.Parent;
                    }
                }
            }

            if (result.Categories != null && result.Categories.Count > 0)
            {
                Debug.WriteLine("Categories : ");
                foreach (var category in result.Categories)
                {
                    Debug.WriteLine($"Name : {category.Name}; Score : {category.Score}");
                }
            }

            if (result.Faces != null && result.Faces.Count > 0)
            {
                Debug.WriteLine("Faces : ");
                foreach (var face in result.Faces)
                {
                    Debug.WriteLine($"Age : {face.Age}; Gender : {face.Gender}");
                }
            }

            if (result.Color != null)
            {
                Debug.WriteLine("Color : ");
                Debug.WriteLine($"   AccentColor : {result.Color.AccentColor}");
                Debug.WriteLine($"   Dominant Color Background : {result.Color.DominantColorBackground}");
                Debug.WriteLine($"   Dominant Color Foreground : {result.Color.DominantColorForeground}");

                if (result.Color.DominantColors != null && result.Color.DominantColors.Count > 0)
                {
                    string colors = "Dominant Colors : ";
                    foreach (var color in result.Color.DominantColors)
                    {
                        colors += color + " ";
                    }
                    Debug.WriteLine(colors);
                }
            }

            if (result.Description != null)
            {
                Debug.WriteLine("Description : ");
                foreach (var caption in result.Description.Captions)
                {
                    Debug.WriteLine($"   Caption : {caption.Text}; Confidence : {caption.Confidence}");
                }
                string tags = "   Tags : ";
                foreach (var tag in result.Description.Tags)
                {
                    tags += tag + ", ";
                }
                Debug.WriteLine(tags);

            }

            if (result.Tags != null)
            {
                Debug.WriteLine("Tags : ");
                foreach (var tag in result.Tags)
                {
                    var hint = string.IsNullOrEmpty(tag.Hint) ? "" : ($"; Hint : {tag.Hint}");
                    Debug.WriteLine($"Name : {tag.Name}; Confidence : {tag.Confidence}{hint}");
                }
            }
        }

        private ResultModel resultModel;
        public ResultModel ResultModel { get => resultModel; set => Set(ref resultModel, value); }

        private string imageSource;
        public string ImageSource { get => imageSource; set => Set(ref imageSource, value); }

        private bool isBusy;
        public bool IsBusy { get => isBusy; set => Set(ref isBusy, value); }

    }
}
