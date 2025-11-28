using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteExameAnexoRepository : IRepositoryBase<PACIENTE_EXAME_ANEXO>
    {
        List<PACIENTE_EXAME_ANEXO> GetAllItens();
        PACIENTE_EXAME_ANEXO GetItemById(Int32 id);
    }
}
