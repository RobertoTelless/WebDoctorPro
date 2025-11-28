using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoConsultaRepository : IRepositoryBase<TIPO_VALOR_CONSULTA>
    {
        TIPO_VALOR_CONSULTA CheckExist(TIPO_VALOR_CONSULTA item, Int32 idAss);
        List<TIPO_VALOR_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_VALOR_CONSULTA GetItemById(Int32 id);
        List<TIPO_VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);
    }
}
