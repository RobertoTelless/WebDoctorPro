using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrauRepository : IRepositoryBase<GRAU_INSTRUCAO>
    {
        List<GRAU_INSTRUCAO> GetAllItens();
        GRAU_INSTRUCAO GetItemById(Int32 id);

    }
}
