public class Publisher
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<BookPublisher>? BookPublishers { get; set; }
}
