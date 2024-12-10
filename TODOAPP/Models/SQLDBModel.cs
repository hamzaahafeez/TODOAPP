namespace TODOAPP.Models
{
    public class SQLDBModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
    }
}
