using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class CategoriaUsuarioRepository : RepositoryBase<CATEGORIA_USUARIO>, ICategoriaUsuarioRepository
    {
        public CATEGORIA_USUARIO CheckExist(CATEGORIA_USUARIO conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_USUARIO> query = Db.CATEGORIA_USUARIO;
            query = query.Where(p => p.CAUS_NM_NOME == conta.CAUS_NM_NOME);
            return query.FirstOrDefault();
        }

        public CATEGORIA_USUARIO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_USUARIO> query = Db.CATEGORIA_USUARIO;
            query = query.Where(p => p.CAUS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_USUARIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_USUARIO> query = Db.CATEGORIA_USUARIO;
            return query.ToList();
        }

        public List<CATEGORIA_USUARIO> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_USUARIO> query = Db.CATEGORIA_USUARIO.Where(p => p.CAUS_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 