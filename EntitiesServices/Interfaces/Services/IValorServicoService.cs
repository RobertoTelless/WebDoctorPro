using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IValorServicoService : IServiceBase<VALOR_SERVICO>
    {
        Int32 Create(VALOR_SERVICO item, LOG log);
        Int32 Create(VALOR_SERVICO item);
        Int32 Edit(VALOR_SERVICO item, LOG log);
        Int32 Edit(VALOR_SERVICO item);
        Int32 Delete(VALOR_SERVICO item, LOG log);

        VALOR_SERVICO CheckExist(VALOR_SERVICO item, Int32 idAss);
        List<VALOR_SERVICO> GetAllItens(Int32 idAss);
        VALOR_SERVICO GetItemById(Int32 id);
        List<VALOR_SERVICO> GetAllItensAdm(Int32 idAss);

        List<TIPO_SERVICO_CONSULTA> GetAllServicos(Int32 idAss);
    }
}
