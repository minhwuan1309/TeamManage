namespace TeamManage.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public string Description { get; set; }
        public ProcessStatus Status { get; set; }
        public string? Image { get; set; }
        public bool IsDeleted { get; set; } = false;
        public TaskItem TaskItem { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
