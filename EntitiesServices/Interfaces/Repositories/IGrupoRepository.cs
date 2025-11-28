using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoRepository : IRepositoryBase<GRUPO_PAC>
    {
        GRUPO_PAC CheckExist(GRUPO_PAC item, Int32 idAss);
        GRUPO_PAC GetItemById(Int32 id);
        List<GRUPO_PAC> GetAllItens(Int32 idAss);
        List<GRUPO_PAC> GetAllItensAdm(Int32 idAss);

    }
}
