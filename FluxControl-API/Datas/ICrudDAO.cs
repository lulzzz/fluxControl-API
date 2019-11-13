using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.Datas
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
