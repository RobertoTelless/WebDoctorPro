using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteContatoRepository : IRepositoryBase<PACIENTE_CONTATO>
    {
        List<PACIENTE_CONTATO> GetAllItens(Int32 idAss);
        PACIENTE_CONTATO GetItemById(Int32 id);
    }
}
