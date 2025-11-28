using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ValorConvenioRepository : RepositoryBase<VALOR_CONVENIO>, IValorConvenioRepository
    {
        public VALOR_CONVENIO CheckExist(VALOR_CONVENIO conta, Int32 idAss)
        {
            IQueryable<VALOR_CONVENIO> query = Db.VALOR_CONVENIO;
            query = query.Where(p => p.USUA_CD_ID == conta.USUA_CD_ID);
            query = query.Where(p => p.CONV_CD_ID == conta.CONV_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public VALOR_CONVENIO GetItemById(Int32 id)
        {
            IQueryable<VALOR_CONVENIO> query = Db.VALOR_CONVENIO;
            query = query.Where(p => p.VACV_CD_ID == id);
            query = query.Include(p => p.CONVENIO);
            return query.FirstOrDefault();
        }

        public List<VALOR_CONVENIO> GetAllItens(Int32 idAss)
        {
            IQueryable<VALOR_CONVENIO> query = Db.VALOR_CONVENIO.Where(p => p.VACV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VALOR_CONVENIO> query = Db.VALOR_CONVENIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
