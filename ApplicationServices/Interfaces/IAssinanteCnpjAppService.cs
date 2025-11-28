using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAssinanteCnpjAppService : IAppServiceBase<ASSINANTE_QUADRO_SOCIETARIO>
    {
        List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens();
        List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente);
        Int32 ValidateCreate(ASSINANTE_QUADRO_SOCIETARIO item, USUARIO usuario);
    }
}
