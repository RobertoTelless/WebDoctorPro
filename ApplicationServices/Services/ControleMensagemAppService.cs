using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;

namespace ApplicationServices.Services
{
    public class ControleMensagemAppService : AppServiceBase<CONTROLE_MENSAGEM>, IControleMensagemAppService
    {
        private readonly IControleMensagemService _baseService;

        public ControleMensagemAppService(IControleMensagemService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss)
        {
            List<CONTROLE_MENSAGEM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public CONTROLE_MENSAGEM GetItemById(Int32 id)
        {
            CONTROLE_MENSAGEM item = _baseService.GetItemById(id);
            return item;
        }

        public CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss)
        {
            CONTROLE_MENSAGEM item = _baseService.GetItemByDate(data, idAss);
            return item;
        }

        public CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss)
        {
            CONTROLE_MENSAGEM item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CONTROLE_MENSAGEM item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.COME_IN_SISTEMA = 6;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTROLE_MENSAGEM item, CONTROLE_MENSAGEM itemAntes, USUARIO usuario)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
