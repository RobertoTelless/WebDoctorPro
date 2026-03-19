using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteExameAnexoImagemRepository : IRepositoryBase<PACIENTE_EXAME_ANEXO_IMAGEM>
    {
        List<PACIENTE_EXAME_ANEXO_IMAGEM> GetAllItens();
        PACIENTE_EXAME_ANEXO_IMAGEM GetItemById(Int32 id);
        List<PACIENTE_EXAME_ANEXO_IMAGEM> GetPontosById(Int32 id);
    }
}
