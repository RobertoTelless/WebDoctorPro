using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMedicoService : IServiceBase<MEDICOS>
    {
        Int32 Create(MEDICOS perfil, LOG log);
        Int32 Create(MEDICOS perfil);
        Int32 Edit(MEDICOS perfil, LOG log);
        Int32 Edit(MEDICOS perfil);
        Int32 Delete(MEDICOS perfil, LOG log);

        List<MEDICOS> GetAllItens(Int32 idAss);
        MEDICOS GetItemById(Int32 id);
        List<MEDICOS> GetAllItensAdm(Int32 idAss);
        List<MEDICOS> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss);
        MEDICOS CheckExist(MEDICOS item, Int32 idAss);
        List<ESPECIALIDADE> GetAllEspec(Int32 idAss);
        List<TIPO_ENVIO> GetAllTipos(Int32 idAss);

        MEDICOS_ENVIO GetEnvioById(Int32 id);
        Int32 EditEnvio(MEDICOS_ENVIO item);
        Int32 CreateEnvio(MEDICOS_ENVIO item);
        List<MEDICOS_ENVIO> GetAllEnvio(Int32 idAss);

        MEDICOS_ENVIO_ANEXO GetMedicoAnexoById(Int32 id);
        Int32 EditMedicoAnexo(MEDICOS_ENVIO_ANEXO item);

        MEDICOS_ENVIO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(MEDICOS_ENVIO_ANOTACAO item);

        MEDICOS_MENSAGEM CheckExistTextoMensagem(MEDICOS_MENSAGEM item, Int32 idAss);
        List<MEDICOS_MENSAGEM> GetAllTextoMensagem(Int32 idAss);
        MEDICOS_MENSAGEM GetTextoMensagemById(Int32 id);
        Int32 EditTextoMensagem(MEDICOS_MENSAGEM item);
        Int32 CreateTextoMensagem(MEDICOS_MENSAGEM item);

    }
}
