using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConfiguracaoCalendarioBloqueioRepository : IRepositoryBase<CONFIGURACAO_CALENDARIO_BLOQUEIO>
    {
        CONFIGURACAO_CALENDARIO_BLOQUEIO GetItemById(Int32 id);
        List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllItems(Int32 idAss);
    }
}
