using System;
namespace EM.DB
{
    public interface IDBParam
    {
        System.Data.Common.DbParameter param { get; set; }
    }
}
