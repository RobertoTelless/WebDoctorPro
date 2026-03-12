using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AreaPacienteAnexoRepository : RepositoryBase<AREA_PACIENTE_ANEXO>, IAreaPacienteAnexoRepository
    {
        public List<AREA_PACIENTE_ANEXO> GetAllItens()
        {
            return Db.AREA_PACIENTE_ANEXO.ToList();
        }

        public AREA_PACIENTE_ANEXO GetItemById(Int32 id)
        {
            IQueryable<AREA_PACIENTE_ANEXO> query = Db.AREA_PACIENTE_ANEXO.Where(p => p.APAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 