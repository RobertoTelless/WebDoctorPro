using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class ConfiguracaoAnamneseAppService : AppServiceBase<CONFIGURACAO_ANAMNESE>, IConfiguracaoAnamneseAppService
    {
        private readonly IConfiguracaoAnamneseService _baseService;

        public ConfiguracaoAnamneseAppService(IConfiguracaoAnamneseService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CONFIGURACAO_ANAMNESE GetItemById(Int32 id)
        {
            return _baseService.GetItemById(id);
        }

        public List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss)
        {
            return _baseService.GetAllItems(idAss);
        }

        public Int32 ValidateEdit(CONFIGURACAO_ANAMNESE item, CONFIGURACAO_ANAMNESE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtCOAN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONFIGURACAO_ANAMNESE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONFIGURACAO_ANAMNESE>(itemAntes),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONFIGURACAO_ANAMNESE item)
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

        public Int32 ValidateCreate(CONFIGURACAO_ANAMNESE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
