using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoServicoRepository : IRepositoryBase<TIPO_SERVICO_CONSULTA>
    {
        TIPO_SERVICO_CONSULTA CheckExist(TIPO_SERVICO_CONSULTA item, Int32 idAss);
        List<TIPO_SERVICO_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_SERVICO_CONSULTA GetItemById(Int32 id);
        List<TIPO_SERVICO_CONSULTA> GetAllItensAdm(Int32 idAss);
    }
}
