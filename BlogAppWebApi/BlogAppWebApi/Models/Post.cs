using Azure;

namespace BlogAppWebApi.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }= DateTime.Now;
        public DateTime UpdateDate { get; set; }= DateTime.Now;
        public string Description { get; set; }
        public string Url { get; set; }
        public string Image {  get; set; }
        public bool IsActive { get; set; } = false;
        public int UserId { get; set; }
        public User? User { get; set; } 
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }
}
