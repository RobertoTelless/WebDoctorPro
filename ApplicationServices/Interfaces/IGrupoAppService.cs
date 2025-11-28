using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Work_Classes;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IGrupoAppService : IAppServiceBase<GRUPO_PAC>
    {
        Int32 ValidateCreate(GRUPO_PAC item, MontagemGrupoPaciente grupo, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_PAC item, GRUPO_PAC perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_PAC item, GRUPO_PAC itemAntes);
        Int32 ValidateDelete(GRUPO_PAC item, USUARIO usuario);
        Int32 ValidateReativar(GRUPO_PAC item, USUARIO usuario);
        Int32 ValidateRemontar(GRUPO_PAC item, MontagemGrupoPaciente grupo, USUARIO usuario);

        List<GRUPO_PAC> GetAllItens(Int32 idAss);
        List<GRUPO_PAC> GetAllItensAdm(Int32 idAss);
        GRUPO_PAC GetItemById(Int32 id);
        GRUPO_PAC CheckExist(GRUPO_PAC conta, Int32 idAss);

        GRUPO_PACIENTE GetContatoById(Int32 id);
        Int32 ValidateCreateContato(GRUPO_PACIENTE item);
        Int32 ValidateEditContato(GRUPO_PACIENTE item);
        GRUPO_PACIENTE CheckExistContato(GRUPO_PACIENTE conta);
    }
}
