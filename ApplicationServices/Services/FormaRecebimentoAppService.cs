using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;

namespace ApplicationServices.Services
{
    public class FormaRecebimentoAppService : AppServiceBase<FORMA_RECEBIMENTO>, IFormaRecebimentoAppService
    {
        private readonly IFormaRecebimentoService _baseService;

        public FormaRecebimentoAppService(IFormaRecebimentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<FORMA_RECEBIMENTO> GetAllItens(Int32 idAss)
        {
            List<FORMA_RECEBIMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FORMA_RECEBIMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<FORMA_RECEBIMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public FORMA_RECEBIMENTO GetItemById(Int32 id)
        {
            FORMA_RECEBIMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public FORMA_RECEBIMENTO CheckExist(FORMA_RECEBIMENTO conta, Int32 idAss)
        {
            FORMA_RECEBIMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(FORMA_RECEBIMENTO item, USUARIO usuario)
        {
            try
            {
                if (usuario != null)
                {
                    // Verifica existencia prévia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Verifica padrao
                    List<FORMA_RECEBIMENTO> recs = _baseService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.FORE_IN_PADRAO == 1).ToList();
                    if (recs.Count > 0)
                    {
                        item.FORE_IN_PADRAO = 0;
                    }

                    // Completa objeto
                    item.FORE_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddFORE",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_RECEBIMENTO>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.FORE_IN_ATIVO = 1;

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(FORMA_RECEBIMENTO item, FORMA_RECEBIMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Verifica padrao
                List<FORMA_RECEBIMENTO> recs = _baseService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.FORE_IN_PADRAO == 1 & p.FORE_CD_ID != item.FORE_CD_ID).ToList();
                if (recs.Count > 0)
                {
                    item.FORE_IN_PADRAO = 0;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtFORE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_RECEBIMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FORMA_RECEBIMENTO>(itemAntes),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FORMA_RECEBIMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONSULTA_RECEBIMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.FORE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFORE",
                    LOG_TX_REGISTRO = item.FORE_NM_FORMA,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORMA_RECEBIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FORE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaFORE",
                    LOG_TX_REGISTRO = item.FORE_NM_FORMA,
                    LOG_IN_SISTEMA = 6

                };

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
