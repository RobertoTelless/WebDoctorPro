using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteAnotacaoRepository : RepositoryBase<PACIENTE_ANOTACAO>, IPacienteAnotacaoRepository
    {
        public List<PACIENTE_ANOTACAO> GetAllItens()
        {
            return Db.PACIENTE_ANOTACAO.ToList();
        }

        public PACIENTE_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_ANOTACAO> query = Db.PACIENTE_ANOTACAO.Where(p => p.PAAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 