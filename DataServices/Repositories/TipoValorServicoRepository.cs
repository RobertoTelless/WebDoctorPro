using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoValorServicoRepository : RepositoryBase<TIPO_SERVICO_CONSULTA>, ITipoValorServicoRepository
    {
        public TIPO_SERVICO_CONSULTA CheckExist(TIPO_SERVICO_CONSULTA conta, Int32 idAss)
        {
            IQueryable<TIPO_SERVICO_CONSULTA> query = Db.TIPO_SERVICO_CONSULTA;
            query = query.Where(p => p.SERV_NM_SERVICO == conta.SERV_NM_SERVICO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_SERVICO_CONSULTA GetItemById(Int32 id)
        {
            IQueryable<TIPO_SERVICO_CONSULTA> query = Db.TIPO_SERVICO_CONSULTA;
            query = query.Where(p => p.SERV_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_SERVICO_CONSULTA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_SERVICO_CONSULTA> query = Db.TIPO_SERVICO_CONSULTA.Where(p => p.SERV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_SERVICO_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_SERVICO_CONSULTA> query = Db.TIPO_SERVICO_CONSULTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
