using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class TipoPacienteAppService : AppServiceBase<TIPO_PACIENTE>, ITipoPacienteAppService
    {
        private readonly ITipoPacienteService _baseService;

        public TipoPacienteAppService(ITipoPacienteService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_PACIENTE> GetAllItens(Int32 idAss)
        {
            List<TIPO_PACIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_PACIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_PACIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_PACIENTE GetItemById(Int32 id)
        {
            TIPO_PACIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_PACIENTE CheckExist(TIPO_PACIENTE conta, Int32 idAss)
        {
            TIPO_PACIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_PACIENTE item, USUARIO usuario)
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

                    // Completa objeto
                    item.TIPA_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIPA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PACIENTE>(item),
                        LOG_IN_SISTEMA = 1
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIPA_IN_ATIVO = 1;

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

        public Int32 ValidateCreate(TIPO_PACIENTE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TIPO_PACIENTE item, TIPO_PACIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtTIPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PACIENTE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_PACIENTE>(itemAntes),
                    LOG_IN_SISTEMA = 1
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_PACIENTE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PACIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIPA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIPA",
                    LOG_TX_REGISTRO = item.TIPA_NM_NOME,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_PACIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTIPA",
                    LOG_TX_REGISTRO = item.TIPA_NM_NOME,
                    LOG_IN_SISTEMA = 1

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
