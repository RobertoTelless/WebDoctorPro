using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class UsuarioLoginRepository : RepositoryBase<USUARIO_LOGIN>, IUsuarioLoginRepository
    {
        public List<USUARIO_LOGIN> GetAllItens(Int32 idAss)
        {
            IQueryable<USUARIO_LOGIN> query = Db.USUARIO_LOGIN.Where(p => p.USLO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public USUARIO_LOGIN GetItemById(Int32 id)
        {
            IQueryable<USUARIO_LOGIN> query = Db.USUARIO_LOGIN.Where(p => p.USLO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 