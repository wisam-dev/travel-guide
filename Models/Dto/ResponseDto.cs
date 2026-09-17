namespace Models.Dto;

public class ResponseItemDto<T>
{
#if DEBUG
    public string ConnectionString { get; set; }
#endif
    public T Item { get; set; }
}

public class ResponsePageDto<T>
{
#if DEBUG
    public string ConnectionString { get; set; }
#endif
    public int DbItemsCount { get; set; }
    public List<T> PageItems { get; set; }
    public int PageNr { get; set; }
    public int PageSize { get; set; }
}
