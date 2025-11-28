using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoValorConsultaRepository : RepositoryBase<TIPO_VALOR_CONSULTA>, ITipoValorConsultaRepository
    {
        public TIPO_VALOR_CONSULTA CheckExist(TIPO_VALOR_CONSULTA conta, Int32 idAss)
        {
            IQueryable<TIPO_VALOR_CONSULTA> query = Db.TIPO_VALOR_CONSULTA;
            query = query.Where(p => p.TIVL_NM_TIPO == conta.TIVL_NM_TIPO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_VALOR_CONSULTA GetItemById(Int32 id)
        {
            IQueryable<TIPO_VALOR_CONSULTA> query = Db.TIPO_VALOR_CONSULTA;
            query = query.Where(p => p.TIVL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_VALOR_CONSULTA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_VALOR_CONSULTA> query = Db.TIPO_VALOR_CONSULTA.Where(p => p.TIVL_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_VALOR_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_VALOR_CONSULTA> query = Db.TIPO_VALOR_CONSULTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
