using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoParcelaRepository : IRepositoryBase<LOCACAO_PARCELA>
    {
        List<LOCACAO_PARCELA> GetAllItens(Int32 idAss);
        LOCACAO_PARCELA GetItemById(Int32 id);
        List<LOCACAO_PARCELA> ExecuteFilter(Int32? locacao, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);
    }
}
