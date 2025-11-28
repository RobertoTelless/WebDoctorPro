using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class VideoRepository : RepositoryBase<VIDEO_BASE>, IVideoRepository
    {
        public VIDEO_BASE CheckExist(VIDEO_BASE item, Int32 idAss)
        {
            IQueryable<VIDEO_BASE> query = Db.VIDEO_BASE;
            query = query.Where(p => p.VIDE_NM_TITULO.ToUpper() == item.VIDE_NM_TITULO.ToUpper());
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.VIDE_IN_ATIVO == 1);
            return query.AsNoTracking().FirstOrDefault();
        }

        public VIDEO_BASE GetItemById(Int32 id)
        {
            IQueryable<VIDEO_BASE> query = Db.VIDEO_BASE;
            query = query.Where(p => p.VIDE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VIDEO_BASE> GetAllItens(Int32 idAss)
        {
            IQueryable<VIDEO_BASE> query = Db.VIDEO_BASE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.VIDE_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }

        public List<VIDEO_BASE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VIDEO_BASE> query = Db.VIDEO_BASE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<VIDEO_BASE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss)
        {
            List<VIDEO_BASE> lista = new List<VIDEO_BASE>();
            IQueryable<VIDEO_BASE> query = Db.VIDEO_BASE;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIVE_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.VIDE_NM_TITULO.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.VIDE_IN_ATIVO == 1);
                query = query.OrderBy(a => a.VIDE_NM_TITULO);
                lista = query.AsNoTracking().ToList<VIDEO_BASE>();
            }
            return lista;
        }

    }
}
