using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class RespostaConsultaRepository : RepositoryBase<RESPOSTA_CONSULTA>, IRespostaConsultaRepository
    {
        public RESPOSTA_CONSULTA GetItemById(Int32 id)
        {
            IQueryable<RESPOSTA_CONSULTA> query = Db.RESPOSTA_CONSULTA;
            query = query.Where(p => p.RECO_CD_ID == id);
            query = query.Where(p => p.RECO_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<RESPOSTA_CONSULTA> GetAllItens(Int32 idAss)
        {
            IQueryable<RESPOSTA_CONSULTA> query = Db.RESPOSTA_CONSULTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.PACIENTE);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            return query.AsNoTracking().ToList();
        }

    }
}
