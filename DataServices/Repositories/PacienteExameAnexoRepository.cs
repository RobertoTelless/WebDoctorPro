using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteExameAnexoRepository : RepositoryBase<PACIENTE_EXAME_ANEXO>, IPacienteExameAnexoRepository
    {
        public List<PACIENTE_EXAME_ANEXO> GetAllItens()
        {
            return Db.PACIENTE_EXAME_ANEXO.ToList();
        }

        public PACIENTE_EXAME_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_EXAME_ANEXO> query = Db.PACIENTE_EXAME_ANEXO.Where(p => p.PAEO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 