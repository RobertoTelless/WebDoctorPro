using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoServicoService : IServiceBase<TIPO_SERVICO_CONSULTA>
    {
        Int32 Create(TIPO_SERVICO_CONSULTA item, LOG log);
        Int32 Create(TIPO_SERVICO_CONSULTA item);
        Int32 Edit(TIPO_SERVICO_CONSULTA item, LOG log);
        Int32 Edit(TIPO_SERVICO_CONSULTA item);
        Int32 Delete(TIPO_SERVICO_CONSULTA item, LOG log);

        TIPO_SERVICO_CONSULTA CheckExist(TIPO_SERVICO_CONSULTA item, Int32 idAss);
        List<TIPO_SERVICO_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_SERVICO_CONSULTA GetItemById(Int32 id);
        List<TIPO_SERVICO_CONSULTA> GetAllItensAdm(Int32 idAss);
    }
}
