using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoHistoricoRepository : IRepositoryBase<TIPO_HISTORICO>
    {
        TIPO_HISTORICO CheckExist(TIPO_HISTORICO item, Int32 idAss);
        List<TIPO_HISTORICO> GetAllItens(Int32 idAss);
        TIPO_HISTORICO GetItemById(Int32 id);
        List<TIPO_HISTORICO> GetAllItensAdm(Int32 idAss);
    }
}
