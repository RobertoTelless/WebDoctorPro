using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoRepository : IRepositoryBase<PRODUTO>
    {
        PRODUTO CheckExist(PRODUTO item, Int32 idAss);
        PRODUTO CheckExist(String codigo, Int32 idAss);
        PRODUTO CheckExistNome(String nome, Int32 idAss);
        PRODUTO CheckExistCodigo(String codigo, Int32 idAss);

        PRODUTO GetByNome(String nome, Int32 idAss);
        PRODUTO GetItemById(Int32 id);

        List<PRODUTO> GetAllItens(Int32 idAss);
        List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas);
        List<PRODUTO> GetAllItensAdm(Int32 idAss);

        List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? composto, DateTime? data, Int32 idAss);
        List<PRODUTO> ExecuteFilterEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss);

        Task<List<PRODUTO>> GetAllItensAsync(Int32 idAss);
        Task<List<PRODUTO>> GetAllItensUltimasAsync(Int32 idAss, Int32 linhas);

    }
}
