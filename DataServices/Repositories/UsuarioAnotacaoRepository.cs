using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
namespace DataServices.Repositories
{
    public class UsuarioAnotacaoRepository : RepositoryBase<USUARIO_ANOTACAO>, IUsuarioAnotacaoRepository
    {
        public List<USUARIO_ANOTACAO> GetAllItens()
        {
            return Db.USUARIO_ANOTACAO.ToList();
        }

        public USUARIO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<USUARIO_ANOTACAO> query = Db.USUARIO_ANOTACAO.Where(p => p.USAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 