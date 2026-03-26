using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AI_Integration.DataAccess.Database.Models
{
    public class Language
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = null!; // es. 'it-IT', 'en-US'

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!; // es. 'Italiano', 'English'

        public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
        public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();
        public virtual ICollection<ContentLink> ContentLinks { get; set; } = new List<ContentLink>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<EventLink> EventLinks { get; set; } = new List<EventLink>();
        public virtual ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();
        public virtual ICollection<PhotoGallery> PhotoGalleries { get; set; } = new List<PhotoGallery>();
        public virtual ICollection<Podcast> Podcasts { get; set; } = new List<Podcast>();
    }
}