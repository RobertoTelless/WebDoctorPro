using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteAnexoRepository : RepositoryBase<PACIENTE_ANEXO>, IPacienteAnexoRepository
    {
        public List<PACIENTE_ANEXO> GetAllItens()
        {
            return Db.PACIENTE_ANEXO.ToList();
        }

        public PACIENTE_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_ANEXO> query = Db.PACIENTE_ANEXO.Where(p => p.PAAX_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 