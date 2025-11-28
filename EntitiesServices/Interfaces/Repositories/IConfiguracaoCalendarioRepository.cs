using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConfiguracaoCalendarioRepository : IRepositoryBase<CONFIGURACAO_CALENDARIO>
    {
        CONFIGURACAO_CALENDARIO GetItemById(Int32 id);
        List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss);
    }
}
