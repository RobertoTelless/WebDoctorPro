using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicoRepository : IRepositoryBase<MEDICOS>
    {
        MEDICOS CheckExist(MEDICOS item, Int32 idAss);
        List<MEDICOS> GetAllItens(Int32 idAss);
        List<MEDICOS> GetAllItensAdm(Int32 idAss);
        MEDICOS GetItemById(Int32 id);
        List<MEDICOS> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss);
    }
}
