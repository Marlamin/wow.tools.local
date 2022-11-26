using DBCD;

namespace wow.tools.local.Services
{
    public interface IDBCManager
    {
        Task<IDBCDStorage> GetOrLoad(string name, string build);
    }
}