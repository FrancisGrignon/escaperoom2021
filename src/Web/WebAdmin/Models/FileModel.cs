namespace WebAdmin.Models
{
    public abstract class FileModel
    {
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
    }
}
