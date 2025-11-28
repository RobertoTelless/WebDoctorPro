using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class MedicamentoAppService : AppServiceBase<MEDICAMENTO>, IMedicamentoAppService
    {
        private readonly IMedicamentoService _baseService;

        public MedicamentoAppService(IMedicamentoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_FORMA> GetAllFormas()
        {
            return _baseService.GetAllFormas();
        }

        public List<MEDICAMENTO> GetAllItens(Int32 idAss)
        {
            List<MEDICAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public MEDICAMENTO CheckExist(MEDICAMENTO conta, Int32 idAss)
        {
            MEDICAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss)
        {
            MEDICAMENTO item = _baseService.CheckExistDesc(nome, generico, lab, idAss);
            return item;
        }

        public List<MEDICAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<MEDICAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MEDICAMENTO GetItemById(Int32 id)
        {
            MEDICAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<MEDICAMENTO>, Boolean> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss)
        {
            try
            {
                List<MEDICAMENTO> objeto = new List<MEDICAMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(generico, nome, laboratorio, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(MEDICAMENTO item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Completa objeto
                item.MEDI_IN_ATIVO = 1;

                // Monta Log
                DTO_Medicamento dto = MontarMedicamentoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Medicamento - Inclusão",
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

        public Int32 ValidateEdit(MEDICAMENTO item, MEDICAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Medicamento dto = MontarMedicamentoDTO(item.MEDI_CD_ID);
                DTO_Medicamento dtoAntes = MontarMedicamentoDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Medicamento - Edição",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
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

        public Int32 ValidateDelete(MEDICAMENTO item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Checa integridade

                // Acerta campos
                item.MEDI_IN_ATIVO = 0;

                // Monta Log
                DTO_Medicamento dto = MontarMedicamentoDTO(item.MEDI_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Medicamento - Exclusão",
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

        public Int32 ValidateReativar(MEDICAMENTO item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade referencial

                // Acerta campos
                item.MEDI_IN_ATIVO = 1;

                // Monta Log
                DTO_Medicamento dto = MontarMedicamentoDTO(item.MEDI_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Medicamento - Reativação",
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

        public DTO_Medicamento MontarMedicamentoDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.MEDICAMENTO
                    .Where(l => l.MEDI_CD_ID == mediId)
                    .Select(l => new DTO_Medicamento
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        MEDI_CD_ID = l.MEDI_CD_ID,
                        TICO_CD_ID = l.TICO_CD_ID,
                        TIFO_CD_ID = l.TIFO_CD_ID,
                        MEDI_DS_DESCRICAO = l.MEDI_DS_DESCRICAO,
                        MEDI_DS_POSOLOGIA = l.MEDI_DS_POSOLOGIA,
                        MEDI_IN_ATIVO = l.MEDI_IN_ATIVO,
                        MEDI_NM_APRESENTACAO = l.MEDI_NM_APRESENTACAO,
                        MEDI_NM_GENERICO = l.MEDI_NM_GENERICO,
                        MEDI_NM_LABORATORIO = l.MEDI_NM_LABORATORIO,
                        MEDI_NM_MEDICAMENTO = l.MEDI_NM_MEDICAMENTO,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_Medicamento MontarMedicamentoDTOObj(MEDICAMENTO antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Medicamento()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                    MEDI_CD_ID = antes.MEDI_CD_ID,
                    TICO_CD_ID = antes.TICO_CD_ID,
                    TIFO_CD_ID = antes.TIFO_CD_ID,
                    MEDI_DS_DESCRICAO = antes.MEDI_DS_DESCRICAO,
                    MEDI_DS_POSOLOGIA = antes.MEDI_DS_POSOLOGIA,
                    MEDI_IN_ATIVO = antes.MEDI_IN_ATIVO,
                    MEDI_NM_APRESENTACAO = antes.MEDI_NM_APRESENTACAO,
                    MEDI_NM_GENERICO = antes.MEDI_NM_GENERICO,
                    MEDI_NM_LABORATORIO = antes.MEDI_NM_LABORATORIO,
                    MEDI_NM_MEDICAMENTO = antes.MEDI_NM_MEDICAMENTO,
                };
                return mediDTO;
            }
        }

    }
}
