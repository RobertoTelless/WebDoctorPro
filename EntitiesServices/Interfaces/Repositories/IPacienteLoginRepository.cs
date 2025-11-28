using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteLoginRepository : IRepositoryBase<PACIENTE_LOGIN>
    {
        List<PACIENTE_LOGIN> GetAllItens(Int32 idAss);
        PACIENTE_LOGIN GetItemById(Int32 id);

    }
}
