using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVideoService : IServiceBase<VIDEO_BASE>
    {
        Int32 Create(VIDEO_BASE perfil, LOG log);
        Int32 Create(VIDEO_BASE perfil);
        Int32 Edit(VIDEO_BASE perfil, LOG log);
        Int32 Edit(VIDEO_BASE perfil);
        Int32 Delete(VIDEO_BASE perfil, LOG log);

        List<VIDEO_BASE> GetAllItens(Int32 idAss);
        VIDEO_BASE GetItemById(Int32 id);
        List<VIDEO_BASE> GetAllItensAdm(Int32 idAss);
        List<VIDEO_BASE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss);
        VIDEO_BASE CheckExist(VIDEO_BASE item, Int32 idAss);
        List<TIPO_VIDEO> GetAllTipos(Int32 idAss);


    }
}
