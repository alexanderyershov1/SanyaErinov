namespace Shebist
{
    public class Word
    {
        public string Id, TopicId;
        public string Question { get; set; }
        public string Hint { get; set; }
        public string Answer { get; set; }
        public string Path { get; set; }
    }
}
