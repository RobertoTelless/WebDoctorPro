using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class ProdutoRepository : RepositoryBase<PRODUTO>, IProdutoRepository
    {
        public PRODUTO CheckExist(PRODUTO conta, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_NM_NOME.ToUpper() == conta.PROD_NM_NOME.ToUpper());
            query = query.Where(p => p.PROD_IN_TIPO_PRODUTO == conta.PROD_IN_TIPO_PRODUTO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PRODUTO CheckExist(String codigo, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            if (codigo != null)
            {
                query = query.Where(p => p.PROD_CD_CODIGO == codigo);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PRODUTO CheckExistNome(String nome, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            if (nome != null)
            {
                query = query.Where(p => p.PROD_NM_NOME == nome);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PRODUTO CheckExistCodigo(String codigo, Int32 idAss)
        {
            if (codigo == null)
            {
                return null;
            }
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_CD_CODIGO == codigo);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PRODUTO GetByNome(String nome, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.FirstOrDefault();
        }

        public PRODUTO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            query = query.Include(p => p.CATEGORIA_PRODUTO);
            return query.AsNoTracking().ToList();
        }

        public async Task<List<PRODUTO>> GetAllItensAsync(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            query = query.Include(p => p.CATEGORIA_PRODUTO);
            return await query.AsNoTracking().ToListAsync();
        }

        public List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            query = query.OrderByDescending(p => p.PROD_DT_ALTERACAO);
            return query.AsNoTracking().Take(linhas).ToList();
        }

        public async Task<List<PRODUTO>> GetAllItensUltimasAsync(Int32 idAss, Int32 linhas)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            query = query.OrderByDescending(p => p.PROD_DT_ALTERACAO);
            return await query.AsNoTracking().Take(linhas).ToListAsync();
        }

        public List<PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PROD_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? composto, DateTime? data, Int32 idAss)
        {
            List<PRODUTO> lista = new List<PRODUTO>();
            IQueryable<PRODUTO> query = Db.PRODUTO;
            if (catId != null)
            {
                query = query.Where(p => p.CAPR_CD_ID == catId);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.PROD_IN_TIPO_PRODUTO == tipo);
            }
            if (subId != null)
            {
                query = query.Where(p => p.SCPR_CD_ID == subId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PROD_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(marca))
            {
                query = query.Where(p => p.PROD_NM_MARCA.Contains(marca));
            }
            if (!String.IsNullOrEmpty(codigo))
            {
                query = query.Where(p => p.PROD_CD_CODIGO.Contains(codigo));
            }
            if (tipo != null)
            {
                query = query.Where(p => p.PROD_IN_TIPO_PRODUTO == tipo);
            }
            if (composto != null)
            {
                query = query.Where(p => p.PROD_IN_COMPOSTO == composto);
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PROD_DT_ALTERACAO) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PROD_IN_SISTEMA == 6);
                query = query.OrderBy(a => a.PROD_NM_NOME);
                query = query.Where(p => p.PROD_IN_ATIVO == 1);
                lista = query.AsNoTracking().ToList<PRODUTO>();
            }
            return lista;
        }

        public List<PRODUTO> ExecuteFilterEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss)
        {
            List<PRODUTO> lista = new List<PRODUTO>();
            IQueryable<PRODUTO> query = Db.PRODUTO;
            if (catId != null)
            {
                query = query.Where(p => p.CAPR_CD_ID == catId);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.PROD_IN_TIPO_PRODUTO == tipo);
            }
            if (subId != null)
            {
                query = query.Where(p => p.SCPR_CD_ID == subId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PROD_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(marca))
            {
                query = query.Where(p => p.PROD_NM_MARCA.Contains(marca));
            }
            if (!String.IsNullOrEmpty(codigo))
            {
                query = query.Where(p => p.PROD_CD_CODIGO.Contains(codigo));
            }
            if (tipo != null)
            {
                query = query.Where(p => p.PROD_IN_TIPO_PRODUTO == tipo);
            }
            if (situacao != null & situacao > 0)
            {
                if (situacao == 1)
                {
                    query = query.Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0);
                }
                if (situacao == 2)
                {
                    query = query.Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0);
                }
                if (situacao == 3)
                {
                    query = query.Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0);
                }
                if (situacao == 4)
                {
                    query = query.Where(p => ((p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1) & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0);
                }
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PROD_DT_ALTERACAO) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PROD_IN_SISTEMA == 6);
                query = query.OrderBy(a => a.PROD_NM_NOME);
                query = query.Where(p => p.PROD_IN_ATIVO == 1);
                lista = query.AsNoTracking().ToList<PRODUTO>();
            }
            return lista;

        }
    }
}