namespace BlogAppMvc.Models
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }

        public List<CommentViewModel> Comments { get; set; } = new();
    }
}
