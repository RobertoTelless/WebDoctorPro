using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using Newtonsoft.Json;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace ApplicationServices.Services
{
    public class AreaPacienteAppService : AppServiceBase<AREA_PACIENTE>, IAreaPacienteAppService
    {
        private readonly IAreaPacienteService _baseService;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public AreaPacienteAppService(IAreaPacienteService baseService): base(baseService)
        {
            _baseService = baseService;
        }


        public List<AREA_PACIENTE> GetAllItens(Int32 idAss)
        {
            List<AREA_PACIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public AREA_PACIENTE GetItemById(Int32 id)
        {
            AREA_PACIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(AREA_PACIENTE item, USUARIO usuario)
        {
            Db.Configuration.LazyLoadingEnabled = false;
            Db.Configuration.ProxyCreationEnabled = false;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Completa objeto
                item.AREA_IN_ATIVO = 1;

                // Monta Log
                String json = JsonConvert.SerializeObject(item, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Área Paciente - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(AREA_PACIENTE item)
        {
            try
            {
                // Completa objeto
                item.AREA_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AREA_PACIENTE item, USUARIO usuario)
        {
            // Monta DTO
            DTO_Area_Paciente dto = MontarAreaPacienteDTO(item.AREA_CD_ID);

            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Monta Log
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Área Paciente - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AREA_PACIENTE item)
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

        public DTO_Area_Paciente MontarAreaPacienteDTO(Int32 locacaoId)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var locacaoDTO = context.AREA_PACIENTE
                    .Where(l => l.AREA_CD_ID == locacaoId)
                    .Select(l => new DTO_Area_Paciente
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        PACI_CD_ID = l.PACI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        AREA_CD_ID = l.AREA_CD_ID,
                        AREA_DT_CONSULTA = l.AREA_DT_CONSULTA,
                        AREA_DT_ENTRADA = l.AREA_DT_ENTRADA,
                        AREA_HR_FINAL = l.AREA_HR_FINAL,
                        AREA_HR_INICIO = l.AREA_HR_INICIO,
                        AREA_IN_ATIVO = l.AREA_IN_ATIVO,
                        AREA_IN_TIPO = l.AREA_IN_TIPO,
                        AREA_NM_TITULO = l.AREA_NM_TITULO,
                        AREA_TX_CONTEUDO = l.AREA_TX_CONTEUDO,
                    })
                    .FirstOrDefault();
                return locacaoDTO;
            }
        }

        public Int32 ValidateDelete(AREA_PACIENTE item, USUARIO usuario)
        {
            // Monta DTO
            DTO_Area_Paciente dto = MontarAreaPacienteDTO(item.AREA_CD_ID);

            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Checa integridade

                // Acerta campos
                item.AREA_IN_ATIVO = 0;

                // Monta Log
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Área Paciente - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
