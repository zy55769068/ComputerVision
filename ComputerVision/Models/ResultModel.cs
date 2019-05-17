using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ComputerVision.Models
{
    public class ResultModel
    {
        public IList<Category> Categories { get; set; }
        public AdultInfo Adult { get; set; }
        public ColorInfo Color { get; set; }
        public ImageType ImageType { get; set; }
        public IList<ImageTag> Tags { get; set; }
        public ImageDescriptionDetails Description { get; set; }
        public IList<FaceDescription> Faces { get; set; }
        public IList<DetectedObject> Objects { get; set; }
        public IList<DetectedBrand> Brands { get; set; }
        public string RequestId { get; set; }
        public ImageMetadata Metadata { get; set; }

    }
}

