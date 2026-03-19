using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteExameAnexoImagemRepository : RepositoryBase<PACIENTE_EXAME_ANEXO_IMAGEM>, IPacienteExameAnexoImagemRepository
    {
        public List<PACIENTE_EXAME_ANEXO_IMAGEM> GetAllItens()
        {
            return Db.PACIENTE_EXAME_ANEXO_IMAGEM.ToList();
        }

        public PACIENTE_EXAME_ANEXO_IMAGEM GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_EXAME_ANEXO_IMAGEM> query = Db.PACIENTE_EXAME_ANEXO_IMAGEM.Where(p => p.PAIM_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_EXAME_ANEXO_IMAGEM> GetPontosById(Int32 id)
        {
            IQueryable<PACIENTE_EXAME_ANEXO_IMAGEM> query = Db.PACIENTE_EXAME_ANEXO_IMAGEM.Where(p => p.PAEO_CD_ID == id);
            return query.ToList();
        }

    }
}
 