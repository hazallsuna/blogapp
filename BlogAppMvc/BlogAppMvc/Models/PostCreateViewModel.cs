using System.ComponentModel.DataAnnotations;

namespace BlogAppMvc.Models
{
    public class PostCreateViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title must be at most 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(300, ErrorMessage = "Description must be at most 300 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Url is required.")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "URL must be lowercase letters, numbers or hyphens.")]
        public string Url { get; set; }

        public string Image { get; set; } = "default.jpg";

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new();

        public bool IsActive { get; set; } = false;
    }
}
