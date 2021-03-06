using System;
using System.Collections.Generic;
using System.Text;

namespace BFYOC_api.Models
{
    public class RatingModel
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timeStamp { get; internal set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}
