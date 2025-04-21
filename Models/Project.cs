namespace TeamManage.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<ProjectMember> Members { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Workflow> Workflows { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
