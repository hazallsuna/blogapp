using System.ComponentModel.DataAnnotations;

namespace BlogAppMvc.Models
{
    public class CommentViewModel
    {
        
        public int CommentId { get; set; }

        public int PostId { get; set; }

        [Required(ErrorMessage = "Yorum boş olamaz.")] 
        public string Text { get; set; }

        public DateTime PublishedDate { get; set; }

        public string? User {  get; set; }
    }
}
