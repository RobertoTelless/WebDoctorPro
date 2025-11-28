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
    public class ProdutoPrecoVendaRepository : RepositoryBase<PRODUTO_PRECO_VENDA>, IProdutoPrecoVendaRepository
    {
        public PRODUTO_PRECO_VENDA CheckExist(PRODUTO_PRECO_VENDA conta, Int32 idAss)
        {
            IQueryable<PRODUTO_PRECO_VENDA> query = Db.PRODUTO_PRECO_VENDA;
            query = query.Where(p => p.PRPV_DT_PRECO_VENDA == conta.PRPV_DT_PRECO_VENDA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PRPV_IN_SISTEMA == 6);
            query = query.Where(p => p.PRPV_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<PRODUTO_PRECO_VENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_PRECO_VENDA> query = Db.PRODUTO_PRECO_VENDA.Where(p => p.PRPV_IN_ATIVO == 1);
            query = query.Where(p => p.PRPV_IN_SISTEMA == 6);
            return query.ToList();
        }

        public PRODUTO_PRECO_VENDA GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_PRECO_VENDA> query = Db.PRODUTO_PRECO_VENDA.Where(p => p.PRPV_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 