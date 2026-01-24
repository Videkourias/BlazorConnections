public interface INYTService
{
    public Task<NYTConnection> GetConnection(DateOnly date);
}