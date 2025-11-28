using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoControleRepository : IRepositoryBase<TIPO_CONTROLE>
    {
        List<TIPO_CONTROLE> GetAllItens();
        TIPO_CONTROLE GetItemById(Int32 id);

    }
}
