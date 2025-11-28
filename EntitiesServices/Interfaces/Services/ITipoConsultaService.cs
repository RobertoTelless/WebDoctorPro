using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoConsultaService : IServiceBase<TIPO_VALOR_CONSULTA>
    {
        Int32 Create(TIPO_VALOR_CONSULTA item, LOG log);
        Int32 Create(TIPO_VALOR_CONSULTA item);
        Int32 Edit(TIPO_VALOR_CONSULTA item, LOG log);
        Int32 Edit(TIPO_VALOR_CONSULTA item);
        Int32 Delete(TIPO_VALOR_CONSULTA item, LOG log);

        TIPO_VALOR_CONSULTA CheckExist(TIPO_VALOR_CONSULTA item, Int32 idAss);
        List<TIPO_VALOR_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_VALOR_CONSULTA GetItemById(Int32 id);
        List<TIPO_VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);
    }
}
