public interface INYTService
{
    public Task<NYTConnection> GetNYTConnection(DateOnly date);
}