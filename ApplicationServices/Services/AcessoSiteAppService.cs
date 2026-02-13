using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class AcessoSiteAppService : AppServiceBase<ACESSO_SITE>, IAcessoSiteAppService
    {
        private readonly IAcessoSiteService _baseService;

        public AcessoSiteAppService(IAcessoSiteService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<ACESSO_SITE> GetAllItens()
        {
            List<ACESSO_SITE> lista = _baseService.GetAllItens();
            return lista;
        }

        public ACESSO_SITE GetItemById(Int32 id)
        {
            ACESSO_SITE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(ACESSO_SITE item)
        {
            try
            {
                // Completa objeto
                item.ACST_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ACESSO_SITE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
