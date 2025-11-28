using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IControleVersaoRepository : IRepositoryBase<CONTROLE_VERSAO>
    {
        List<CONTROLE_VERSAO> GetAllItens();
        CONTROLE_VERSAO GetItemById(Int32 id);

    }
}
