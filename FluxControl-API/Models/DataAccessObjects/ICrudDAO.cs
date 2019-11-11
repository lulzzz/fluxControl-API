using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataAccessObjects
{
    interface ICrudDAO<T>
    {
        int Add(T model);

        List<T> Load();

        T Get(int id);

        bool Change(T model);

        bool Remove(int id);
    }
}
