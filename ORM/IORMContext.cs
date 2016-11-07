using System.Data;
using ORM.db_store.persitence;

namespace ORM
{
    public interface IORMContext
    {
        GenericDatabase db { get; set; }
        IClassFactory classFactory { get; set; }
        void StartTransaction();
        void StartTransaction(IsolationLevel level);
        void CommitCurrentTransaction();
        void RollbackCurrentTransaction();
    }
}