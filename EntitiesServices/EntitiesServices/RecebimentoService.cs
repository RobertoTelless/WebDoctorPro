using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class RecebimentoService : ServiceBase<CONSULTA_RECEBIMENTO>, IRecebimentoService
    {
        private readonly IRecebimentoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IValorConsultaRepository _tipoRepository;
        private readonly IRecebimentoAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IRecebimentoAnotacaoRepository _anoRepository;
        private readonly IFormaRecebimentoRepository _frRepository;
        private readonly IValorConvenioRepository _vcRepository;
        private readonly IRecebimentoReciboRepository _rcRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public RecebimentoService(IRecebimentoRepository baseRepository, ILogRepository logRepository, IValorConsultaRepository tipoRepository, IRecebimentoAnexoRepository anexoRepository, IUsuarioRepository usuRepository, IRecebimentoAnotacaoRepository anoRepository, IFormaRecebimentoRepository frRepository, IValorConvenioRepository vcRepository, IRecebimentoReciboRepository rcRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _usuRepository = usuRepository;
            _anoRepository = anoRepository;
            _frRepository = frRepository;
            _vcRepository = vcRepository;
            _rcRepository = rcRepository;
        }

        public CONSULTA_RECEBIMENTO GetItemById(Int32 id)
        {
            CONSULTA_RECEBIMENTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CONSULTA_RECEBIMENTO> ExecuteFilter(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, paciente, consulta, forma, nome, inicio, final, conferido, idAss);

        }

        public RECEBIMENTO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public Int32 EditAnexo(RECEBIMENTO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    RECEBIMENTO_ANEXO obj = _anexoRepository.GetById(item.REAN_CD_ID);
                    _anexoRepository.Detach(obj);
                    _anexoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public RECEBIMENTO_RECIBO GetReciboById(Int32 id)
        {
            return _rcRepository.GetItemById(id);
        }

        public Int32 EditRecibo(RECEBIMENTO_RECIBO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    RECEBIMENTO_RECIBO obj = _rcRepository.GetById(item.RERC_CD_ID);
                    _rcRepository.Detach(obj);
                    _rcRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public RECEBIMENTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(RECEBIMENTO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    RECEBIMENTO_ANOTACAO obj = _anoRepository.GetById(item.REAT_CD_ID);
                    _anoRepository.Detach(obj);
                    _anoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<VALOR_CONSULTA> GetAllValorConsulta(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }
        public List<FORMA_RECEBIMENTO> GetAllForma(Int32 idAss)
        {
            return _frRepository.GetAllItens(idAss);
        }

        public FORMA_RECEBIMENTO GetFormaById(Int32 id)
        {
            return _frRepository.GetItemById(id);
        }

        public List<VALOR_CONVENIO> GetAllValorConvenio(Int32 idAss)
        {
            return _vcRepository.GetAllItens(idAss);
        }

        public Int32 Create(CONSULTA_RECEBIMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(CONSULTA_RECEBIMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(CONSULTA_RECEBIMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 usu = item.USUA_CD_ID;
                    Int32 fr = item.FORE_CD_ID.Value;
                    Int32 pac = item.PACI_CD_ID.Value;
                    Int32 vc = item.VACO_CD_ID.Value;

                    item.USUARIO = null;
                    item.FORMA_RECEBIMENTO = null;
                    item.PACIENTE = null;
                    item.VALOR_CONSULTA = null;
                    CONSULTA_RECEBIMENTO obj = _baseRepository.GetById(item.CORE_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    obj.USUA_CD_ID = usu;
                    obj.FORE_CD_ID = fr;
                    obj.PACI_CD_ID = pac;
                    obj.VACO_CD_ID = vc;
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(CONSULTA_RECEBIMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 usu = item.USUA_CD_ID;
                    Int32 fr = item.FORE_CD_ID.Value;
                    Int32 pac = item.PACI_CD_ID.Value;
                    Int32 vc = item.VACO_CD_ID.Value;

                    item.USUARIO = null;
                    item.FORMA_RECEBIMENTO = null;
                    item.PACIENTE = null;
                    item.VALOR_CONSULTA = null;
                    CONSULTA_RECEBIMENTO obj = _baseRepository.GetById(item.CORE_CD_ID);
                    _baseRepository.Detach(obj);
                    obj.USUA_CD_ID = usu;
                    obj.FORE_CD_ID = fr;
                    obj.PACI_CD_ID = pac;
                    obj.VACO_CD_ID = vc;
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(CONSULTA_RECEBIMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


    }
}
