using System.ComponentModel.DataAnnotations;
using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Models
{
    [JsonModel]
    public class PushMessageModel
    {
        [Required]
        public string Body { get; set; }

        [Required]
        public string Title { get; set; }

        public string Link { get; set; }
    }
}