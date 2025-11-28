using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoClasseRepository : IRepositoryBase<TIPO_CARTEIRA_CLASSE>
    {
        List<TIPO_CARTEIRA_CLASSE> GetAllItens();
        TIPO_CARTEIRA_CLASSE GetItemById(Int32 id);

    }
}
