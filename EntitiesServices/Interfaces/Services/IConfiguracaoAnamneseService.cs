using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IConfiguracaoAnamneseService : IServiceBase<CONFIGURACAO_ANAMNESE>
    {
        CONFIGURACAO_ANAMNESE GetItemById(Int32 id);
        List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss);

        Int32 Edit(CONFIGURACAO_ANAMNESE item, LOG log);
        Int32 Edit(CONFIGURACAO_ANAMNESE item);
        Int32 Create(CONFIGURACAO_ANAMNESE item);

    }
}
