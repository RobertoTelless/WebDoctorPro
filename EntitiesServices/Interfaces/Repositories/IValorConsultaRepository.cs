using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IValorConsultaRepository : IRepositoryBase<VALOR_CONSULTA>
    {
        VALOR_CONSULTA CheckExist(VALOR_CONSULTA item, Int32 idAss);
        List<VALOR_CONSULTA> GetAllItens(Int32 idAss);
        VALOR_CONSULTA GetItemById(Int32 id);
        List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);
    }
}
