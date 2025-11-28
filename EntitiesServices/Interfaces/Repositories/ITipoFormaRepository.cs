using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoFormaRepository : IRepositoryBase<TIPO_FORMA>
    {
        List<TIPO_FORMA> GetAllItens();
        TIPO_FORMA GetItemById(Int32 id);

    }
}
