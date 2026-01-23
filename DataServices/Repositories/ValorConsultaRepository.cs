using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ValorConsultaRepository : RepositoryBase<VALOR_CONSULTA>, IValorConsultaRepository
    {
        public VALOR_CONSULTA CheckExist(VALOR_CONSULTA conta, Int32 idAss)
        {
            IQueryable<VALOR_CONSULTA> query = Db.VALOR_CONSULTA;
            query = query.Where(p => p.USUA_CD_ID == conta.USUA_CD_ID);
            query = query.Where(p => p.VACO_NM_NOME == conta.VACO_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.VACO_IN_ATIVO == 1);
            query = query.Where(p => p.TIVL_CD_ID == conta.TIVL_CD_ID);
            return query.AsNoTracking().FirstOrDefault();
        }

        public VALOR_CONSULTA GetItemById(Int32 id)
        {
            IQueryable<VALOR_CONSULTA> query = Db.VALOR_CONSULTA;
            query = query.Where(p => p.VACO_CD_ID == id);
            query = query.Include(p => p.VALOR_CONSULTA_MATERIAL);
            return query.FirstOrDefault();
        }

        public List<VALOR_CONSULTA> GetAllItens(Int32 idAss)
        {
            IQueryable<VALOR_CONSULTA> query = Db.VALOR_CONSULTA.Where(p => p.VACO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VALOR_CONSULTA> query = Db.VALOR_CONSULTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
