using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConfiguracaoAnamneseRepository : IRepositoryBase<CONFIGURACAO_ANAMNESE>
    {
        CONFIGURACAO_ANAMNESE GetItemById(Int32 id);
        List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss);
    }
}
