using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class TipoPessoaRepository : RepositoryBase<TIPO_PESSOA>, ITipoPessoaRepository
    {
        public TIPO_PESSOA GetItemById(Int32 id)
        {
            IQueryable<TIPO_PESSOA> query = Db.TIPO_PESSOA;
            query = query.Where(p => p.TIPE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_PESSOA> GetAllItensAdm()
        {
            IQueryable<TIPO_PESSOA> query = Db.TIPO_PESSOA;
            return query.ToList();
        }

        public List<TIPO_PESSOA> GetAllItens()
        {
            IQueryable<TIPO_PESSOA> query = Db.TIPO_PESSOA;
            return query.ToList();
        }

    }
}
 