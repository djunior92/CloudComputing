using System.IO;

namespace Agenda.Data
{
    public class FileData
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Lenght { get; set; }
        public Stream  Stream { get; set; }
    }

    public class InfoImagem
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
