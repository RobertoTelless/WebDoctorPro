using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IValorConvenioService : IServiceBase<VALOR_CONVENIO>
    {
        Int32 Create(VALOR_CONVENIO item, LOG log);
        Int32 Create(VALOR_CONVENIO item);
        Int32 Edit(VALOR_CONVENIO item, LOG log);
        Int32 Edit(VALOR_CONVENIO item);
        Int32 Delete(VALOR_CONVENIO item, LOG log);

        VALOR_CONVENIO CheckExist(VALOR_CONVENIO item, Int32 idAss);
        List<VALOR_CONVENIO> GetAllItens(Int32 idAss);
        VALOR_CONVENIO GetItemById(Int32 id);
        List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss);

        List<CONVENIO> GetAllConvenios(Int32 idAss);
    }
}
