using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PacienteAnamneseAnotacaoRepository : RepositoryBase<PACIENTE_ANAMNESE_ANOTACAO>, IPacienteAnamneseAnotacaoRepository
    {
        public List<PACIENTE_ANAMNESE_ANOTACAO> GetAllItens()
        {
            return Db.PACIENTE_ANAMNESE_ANOTACAO.ToList();
        }

        public PACIENTE_ANAMNESE_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_ANAMNESE_ANOTACAO> query = Db.PACIENTE_ANAMNESE_ANOTACAO.Where(p => p.PAAA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 