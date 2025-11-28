using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class TemplateEMailHTMLRepository : RepositoryBase<TEMPLATE_EMAIL_HTML>, ITemplateEMailHTMLRepository
    {

        public TEMPLATE_EMAIL_HTML GetItemById(Int32 id)
        {
            IQueryable<TEMPLATE_EMAIL_HTML> query = Db.TEMPLATE_EMAIL_HTML;
            query = query.Where(p => p.TEHT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public TEMPLATE_EMAIL_HTML GetItemByNome(String nome)
        {
            IQueryable<TEMPLATE_EMAIL_HTML> query = Db.TEMPLATE_EMAIL_HTML;
            query = query.Where(p => p.TEHT_NM_NOME == nome);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<TEMPLATE_EMAIL_HTML> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL_HTML> query = Db.TEMPLATE_EMAIL_HTML;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.TEHT_IN_SISTEMA == 6 || p.TEHT_IN_SISTEMA == 0);
            return query.AsNoTracking().ToList();
        }

        public List<TEMPLATE_EMAIL_HTML> GetAllItens(Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL_HTML> query = Db.TEMPLATE_EMAIL_HTML.Where(p => p.TEHT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.TEHT_IN_SISTEMA == 6 || p.TEHT_IN_SISTEMA == 0);
            return query.AsNoTracking().ToList();
        }

        public TEMPLATE_EMAIL_HTML CheckExist(TEMPLATE_EMAIL_HTML item, Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL_HTML> query = Db.TEMPLATE_EMAIL_HTML;
            query = query.Where(p => p.TEHT_NM_NOME == item.TEHT_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.TEHT_IN_ATIVO == 1);
            query = query.Where(p => p.TEHT_IN_SISTEMA == 6 || p.TEHT_IN_SISTEMA == 0);
            return query.AsNoTracking().FirstOrDefault();
        }
    }
}
 