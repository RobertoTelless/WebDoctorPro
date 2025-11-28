using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPerfilRepository : IRepositoryBase<PERFIL>
    {
        PERFIL CheckExist(PERFIL item, Int32? idAss);
        PERFIL GetByName(String nome, Int32? idAss);
        USUARIO GetUserProfile(PERFIL perfil);
        List<PERFIL> GetAllItens(Int32? idAss);
        PERFIL GetItemById(Int32? id);
    }
}
