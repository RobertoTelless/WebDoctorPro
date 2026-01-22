using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IValorConsultaService : IServiceBase<VALOR_CONSULTA>
    {
        Int32 Create(VALOR_CONSULTA item, LOG log);
        Int32 Create(VALOR_CONSULTA item);
        Int32 Edit(VALOR_CONSULTA item, LOG log);
        Int32 Edit(VALOR_CONSULTA item);
        Int32 Delete(VALOR_CONSULTA item, LOG log);

        VALOR_CONSULTA CheckExist(VALOR_CONSULTA item, Int32 idAss);
        List<VALOR_CONSULTA> GetAllItens(Int32 idAss);
        VALOR_CONSULTA GetItemById(Int32 id);
        List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);

        List<TIPO_VALOR_CONSULTA> GetAllTipos(Int32 idAss);

        VALOR_CONSULTA_MATERIAL GetConsultaMaterialById(Int32 id);
        Int32 EditConsultaMaterial(VALOR_CONSULTA_MATERIAL item);
        Int32 CreateConsultaMaterial(VALOR_CONSULTA_MATERIAL item);
        List<VALOR_CONSULTA_MATERIAL> GetAllConsultaMaterial(Int32 idAss);

    }
}
