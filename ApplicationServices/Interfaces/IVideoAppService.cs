using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVideoAppService : IAppServiceBase<VIDEO_BASE>
    {
        Int32 ValidateCreate(VIDEO_BASE item, USUARIO usuario);
        Int32 ValidateEdit(VIDEO_BASE item, VIDEO_BASE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(VIDEO_BASE item, USUARIO usuario);
        Int32 ValidateReativar(VIDEO_BASE item, USUARIO usuario);

        VIDEO_BASE CheckExist(VIDEO_BASE item, Int32 idAss);
        List<VIDEO_BASE> GetAllItens(Int32 idAss);
        VIDEO_BASE GetItemById(Int32 id);
        List<VIDEO_BASE> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<VIDEO_BASE>, Boolean> ExecuteFilter(Int32? tipo, String nome, Int32 idAss);
        List<TIPO_VIDEO> GetAllTipos(Int32 idAss);

    }
}
