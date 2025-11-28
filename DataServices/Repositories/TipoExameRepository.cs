using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoExameRepository : RepositoryBase<TIPO_EXAME>, ITipoExameRepository
    {
        public TIPO_EXAME CheckExist(TIPO_EXAME conta, Int32 idAss)
        {
            IQueryable<TIPO_EXAME> query = Db.TIPO_EXAME;
            query = query.Where(p => p.TIEX_NM_NOME == conta.TIEX_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_EXAME GetItemById(Int32 id)
        {
            IQueryable<TIPO_EXAME> query = Db.TIPO_EXAME;
            query = query.Where(p => p.TIEX_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_EXAME> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_EXAME> query = Db.TIPO_EXAME.Where(p => p.TIEX_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_EXAME> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_EXAME> query = Db.TIPO_EXAME;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
