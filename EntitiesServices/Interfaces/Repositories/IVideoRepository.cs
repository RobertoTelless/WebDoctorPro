using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVideoRepository : IRepositoryBase<VIDEO_BASE>
    {
        VIDEO_BASE CheckExist(VIDEO_BASE item, Int32 idAss);
        List<VIDEO_BASE> GetAllItens(Int32 idAss);
        List<VIDEO_BASE> GetAllItensAdm(Int32 idAss);
        VIDEO_BASE GetItemById(Int32 id);
        List<VIDEO_BASE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss);
    }
}
