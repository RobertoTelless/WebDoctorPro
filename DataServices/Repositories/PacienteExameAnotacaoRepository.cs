using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteExameAnotacaoRepository : RepositoryBase<PACIENTE_EXAME_ANOTACAO>, IPacienteExameAnotacaoRepository
    {
        public List<PACIENTE_EXAME_ANOTACAO> GetAllItens()
        {
            return Db.PACIENTE_EXAME_ANOTACAO.ToList();
        }

        public PACIENTE_EXAME_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_EXAME_ANOTACAO> query = Db.PACIENTE_EXAME_ANOTACAO.Where(p => p.PAET_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 