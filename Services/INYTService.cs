public interface INYTService
{
    public Task<Connection> GetConnection(DateOnly date);
}