using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUsuarioLoginRepository : IRepositoryBase<USUARIO_LOGIN>
    {
        List<USUARIO_LOGIN> GetAllItens(Int32 idAss);
        USUARIO_LOGIN GetItemById(Int32 id);

    }
}
