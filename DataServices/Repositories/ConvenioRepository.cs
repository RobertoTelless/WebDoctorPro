using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class ConvenioRepository : RepositoryBase<CONVENIO>, IConvenioRepository
    {
        public CONVENIO CheckExist(CONVENIO conta, Int32 idAss)
        {
            IQueryable<CONVENIO> query = Db.CONVENIO;
            query = query.Where(p => p.CONV_NM_NOME == conta.CONV_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CONVENIO GetItemById(Int32 id)
        {
            IQueryable<CONVENIO> query = Db.CONVENIO;
            query = query.Where(p => p.CONV_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONVENIO> GetAllItens(Int32 idAss)
        {
            IQueryable<CONVENIO> query = Db.CONVENIO.Where(p => p.CONV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONVENIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CONVENIO> query = Db.CONVENIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
