namespace NotesWebApi.Models
{
    public class NoteDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Body { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
    }
}
