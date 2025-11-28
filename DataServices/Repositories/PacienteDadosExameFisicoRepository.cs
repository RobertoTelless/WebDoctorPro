using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class PacienteDadosExameFisicoRepository : RepositoryBase<PACIENTE_DADOS_EXAME_FISICO>, IPacienteDadosExameFisicoRepository
    {
        public PACIENTE_DADOS_EXAME_FISICO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_DADOS_EXAME_FISICO> query = Db.PACIENTE_DADOS_EXAME_FISICO;
            query = query.Where(p => p.PDEF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_DADOS_EXAME_FISICO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_DADOS_EXAME_FISICO> query = Db.PACIENTE_DADOS_EXAME_FISICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
