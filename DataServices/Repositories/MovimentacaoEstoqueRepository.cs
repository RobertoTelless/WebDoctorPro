using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class MovimentacaoEstoqueRepository : RepositoryBase<MOVIMENTO_ESTOQUE_PRODUTO>, IMovimentacaoEstoqueRepository
    {
        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO.Where(p => p.MOEP_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEP_IN_SISTEMA == 6);
            query = query.Include(p => p.PRODUTO);
            query = query.Include(p => p.EMPRESA_FILIAL);
            query = query.Include(p => p.EMPRESA_FILIAL1);
            query = query.Include(p => p.EMPRESA_FILIAL2);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.USUARIO1);
            query = query.Include(p => p.USUARIO2);
            query = query.Include(p => p.FORNECEDOR);
            return query.ToList();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEP_IN_SISTEMA == 6);
            query = query.Include(p => p.PRODUTO);
            return query.ToList();
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO.Where(p => p.MOEP_CD_ID == id);
            query = query.Include(p => p.PRODUTO);
            query = query.Include(p => p.EMPRESA_FILIAL);
            query = query.Include(p => p.EMPRESA_FILIAL1);
            query = query.Include(p => p.EMPRESA_FILIAL2);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.USUARIO1);
            query = query.Include(p => p.USUARIO2);
            query = query.Include(p => p.FORNECEDOR);
            query = query.Include(p => p.FORMA_PAGAMENTO);
            return query.FirstOrDefault();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            if (es != null)
            {
                query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == es);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.MOEP_IN_TIPO == tipo);
            }
            if (resp != null)
            {
                query = query.Where(p => p.USUA_CD_ID == resp);
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.MOEP_IN_SISTEMA == 6);
                query = query.OrderBy(a => a.MOEP_DT_MOVIMENTO);
                query = query.Where(p => p.MOEP_IN_ATIVO == 1);
                lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
            }
            return lista;
        }

    }
}
 