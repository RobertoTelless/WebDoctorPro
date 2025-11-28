using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteAnamneseRepository : IRepositoryBase<PACIENTE_ANAMNESE>
    {
        List<PACIENTE_ANAMNESE> GetAllItens(Int32 idAss);
        PACIENTE_ANAMNESE GetItemById(Int32 id);
    }
}
