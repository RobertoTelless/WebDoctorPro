using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class EMailAgendaRepository : RepositoryBase<EMAIL_AGENDAMENTO>, IEmailAgendaRepository
    {
        public List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<EMAIL_AGENDAMENTO> query = Db.EMAIL_AGENDAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 