public class Object
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<Field>? Fields { get; set; }
}
