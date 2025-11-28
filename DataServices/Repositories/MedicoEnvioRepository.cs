using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class MedicoEnvioRepository : RepositoryBase<MEDICOS_ENVIO>, IMedicoEnvioRepository
    {
        public MEDICOS_ENVIO GetItemById(Int32 id)
        {
            IQueryable<MEDICOS_ENVIO> query = Db.MEDICOS_ENVIO;
            query = query.Where(p => p.MEEV_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MEDICOS_ENVIO> GetAllItens(Int32 idAss)
        {
            IQueryable<MEDICOS_ENVIO> query = Db.MEDICOS_ENVIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
