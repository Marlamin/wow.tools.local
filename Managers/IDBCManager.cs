using DBCD;

namespace wow.tools.local.Managers
{
    public interface IDBCManager
    {
        Task<IDBCDStorage> GetOrLoad(string name, string build);
    }
}