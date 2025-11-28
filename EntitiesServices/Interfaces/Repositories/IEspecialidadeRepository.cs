using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEspecialidadeRepository : IRepositoryBase<ESPECIALIDADE>
    {
        ESPECIALIDADE CheckExist(ESPECIALIDADE item, Int32 idAss);
        ESPECIALIDADE GetItemById(Int32 id);
        List<ESPECIALIDADE> GetAllItens(Int32 idAss);
        List<ESPECIALIDADE> GetAllItensAdm(Int32 idAss);
    }
}
