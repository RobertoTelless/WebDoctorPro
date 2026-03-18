using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteVacinaRepository : IRepositoryBase<PACIENTE_VACINA>
    {
        List<PACIENTE_VACINA> GetAllItens(Int32 idAss);
        PACIENTE_VACINA GetItemById(Int32 id);
    }
}
