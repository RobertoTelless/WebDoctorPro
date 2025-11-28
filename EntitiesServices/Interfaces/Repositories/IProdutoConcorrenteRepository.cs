using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoConcorrenteRepository : IRepositoryBase<PRODUTO_CONCORRENTE>
    {
        List<PRODUTO_CONCORRENTE> GetAllItens(Int32 idAss);
        PRODUTO_CONCORRENTE GetItemById(Int32 id);
    }
}
