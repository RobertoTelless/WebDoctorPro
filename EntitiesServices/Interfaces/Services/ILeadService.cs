using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ILeadService : IServiceBase<LEAD>
    {
        Int32 Create(LEAD perfil, LOG log);
        Int32 Create(LEAD perfil);
        Int32 Edit(LEAD perfil, LOG log);
        Int32 Edit(LEAD perfil);
        Int32 Delete(LEAD perfil, LOG log);

        List<LEAD> GetAllItens(Int32 idAss);
        LEAD GetItemById(Int32 id);
        List<LEAD> GetAllItensAdm(Int32 idAss);
        List<LEAD> ExecuteFilter(DateTime? inicio, DateTime? final, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss);
        LEAD CheckExist(LEAD item, Int32 idAss);

        LEAD_ANEXO GetLeadAnexoById(Int32 id);
        Int32 EditLeadAnexo(LEAD_ANEXO item);

        LEAD_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(LEAD_ANOTACAO item);

    }
}
