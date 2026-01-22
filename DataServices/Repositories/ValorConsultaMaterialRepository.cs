using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ValorConsultaMaterialRepository : RepositoryBase<VALOR_CONSULTA_MATERIAL>, IValorConsultaMaterialRepository
    {
        public List<VALOR_CONSULTA_MATERIAL> GetAllItens(Int32 idAss)
        {
            IQueryable<VALOR_CONSULTA_MATERIAL> query = Db.VALOR_CONSULTA_MATERIAL.Where(p => p.VCMA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public VALOR_CONSULTA_MATERIAL GetItemById(Int32 id)
        {
            IQueryable<VALOR_CONSULTA_MATERIAL> query = Db.VALOR_CONSULTA_MATERIAL.Where(p => p.VCMA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 