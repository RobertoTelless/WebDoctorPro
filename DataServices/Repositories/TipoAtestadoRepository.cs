using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoAtestadoRepository : RepositoryBase<TIPO_ATESTADO>, ITipoAtestadoRepository
    {
        public TIPO_ATESTADO CheckExist(TIPO_ATESTADO conta, Int32 idAss)
        {
            IQueryable<TIPO_ATESTADO> query = Db.TIPO_ATESTADO;
            query = query.Where(p => p.TIAT_NM_NOME == conta.TIAT_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_ATESTADO GetItemById(Int32 id)
        {
            IQueryable<TIPO_ATESTADO> query = Db.TIPO_ATESTADO;
            query = query.Where(p => p.TIAT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_ATESTADO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_ATESTADO> query = Db.TIPO_ATESTADO.Where(p => p.TIAT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_ATESTADO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_ATESTADO> query = Db.TIPO_ATESTADO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
