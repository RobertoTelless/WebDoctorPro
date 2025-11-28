using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class ConfiguracaoCalendarioAppService : AppServiceBase<CONFIGURACAO_CALENDARIO>, IConfiguracaoCalendarioAppService
    {
        private readonly IConfiguracaoCalendarioService _baseService;

        public ConfiguracaoCalendarioAppService(IConfiguracaoCalendarioService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CONFIGURACAO_CALENDARIO GetItemById(Int32 id)
        {
            return _baseService.GetItemById(id);
        }

        public List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss)
        {
            return _baseService.GetAllItems(idAss);
        }

        public Int32 ValidateEdit(CONFIGURACAO_CALENDARIO item, CONFIGURACAO_CALENDARIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                item.USUARIO = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtCOCA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONFIGURACAO_CALENDARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONFIGURACAO_CALENDARIO>(itemAntes),
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

        public Int32 ValidateEdit(CONFIGURACAO_CALENDARIO item)
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

        public Int32 ValidateCreate(CONFIGURACAO_CALENDARIO item)
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

        public List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllBloqueio(Int32 idAss)
        {
            return _baseService.GetAllBloqueio(idAss);
        }

        public CONFIGURACAO_CALENDARIO_BLOQUEIO GetBloqueioById(Int32 id)
        {
            CONFIGURACAO_CALENDARIO_BLOQUEIO lista = _baseService.GetBloqueioById(id);
            return lista;
        }

        public Int32 ValidateEditBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item)
        {
            try
            {
                // Persiste
                return _baseService.EditBloqueio(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item)
        {
            try
            {
                item.COCB_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateBloqueio(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }






    }
}
