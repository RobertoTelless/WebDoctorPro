using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAcessoMetodoRepository : IRepositoryBase<ACESSO_METODO>
    {
        List<ACESSO_METODO> GetAllItens(Int32 idAss);
        List<ACESSO_METODO> GetAllItens();
        List<ACESSO_METODO> GetAllItensDia();
        List<ACESSO_METODO> GetAllItensMes();
        List<ACESSO_METODO> GetAllItensMesAnterior();
        ACESSO_METODO GetItemById(Int32 id);
        List<ACESSO_METODO> ExecuteFilter(Int32? assi, Int32? usuario, DateTime? inicio, DateTime? final, String sigla, String entidade, String metodo, Int32 idAss);
    }
}
