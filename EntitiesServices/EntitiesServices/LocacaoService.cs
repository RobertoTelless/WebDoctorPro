using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class LocacaoService : ServiceBase<LOCACAO>, ILocacaoService
    {
        private readonly ILocacaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILocacaoAnexoRepository _aneRepository;
        private readonly ILocacaoAnotacaoRepository _anoRepository;
        private readonly ITipoHistoricoRepository _tenRepository;
        private readonly ILocacaoHistoricoRepository _hisRepository;
        private readonly ILocacaoParcelaRepository _parRepository;
        private readonly ILocacaoOcorrenciaRepository _ocoRepository;
        private readonly ITipoContratoRepository _tcRepository;
        private readonly IContratoLocacaoRepository _clRepository;
        private readonly ITipoOcorrenciaRepository _toRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public LocacaoService(ILocacaoRepository baseRepository, ILogRepository logRepository, ILocacaoAnexoRepository aneRepository, ILocacaoAnotacaoRepository anoRepository, ITipoHistoricoRepository tenRepository, ILocacaoHistoricoRepository hisRepository, ILocacaoParcelaRepository parRepository, ILocacaoOcorrenciaRepository ocoRepository, ITipoContratoRepository tcRepository, IContratoLocacaoRepository clRepository, ITipoOcorrenciaRepository toRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _hisRepository = hisRepository;
            _parRepository = parRepository;
            _aneRepository = aneRepository;
            _anoRepository = anoRepository;
            _tenRepository = tenRepository;
            _ocoRepository = ocoRepository;
            _tcRepository = tcRepository;
            _clRepository = clRepository;
            _toRepository = toRepository;
        }

        public List<TIPO_HISTORICO> GetAllTipos(Int32 idAss)
        {
            return _tenRepository.GetAllItens(idAss);
        }

        public LOCACAO GetItemById(Int32 id)
        {
            LOCACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<LOCACAO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<LOCACAO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public LOCACAO CheckExist(LOCACAO item, Int32 idAss)
        {
            LOCACAO volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<LOCACAO> ExecuteFilter(String paciente, String prod, DateTime? inicio, DateTime? final, Int32? status, String numero, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(paciente, prod, inicio, final, status, numero, idAss);

        }

        public Int32 Create(LOCACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PERIODICIDADE_TAREFA = null;
                    item.USUARIO = null;
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

        public Int32 Create(LOCACAO item)
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

        public Int32 Edit(LOCACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    //// Oculta a propriedade de navegação para garantir que não seja rastreada
                    //item.PACIENTE = null;
                    //item.USUARIO = null;

                    //// Anexa a entidade ao contexto e a marca como modificada
                    //Db.Entry(item).State = EntityState.Modified;
                    //_logRepository.Add(log);
                    //Db.SaveChanges();
                    //transaction.Commit();
                    //return 0;




                    item.PACIENTE = null;
                    item.USUARIO = null;
                    item.PRODUTO = null;
                    LOCACAO obj = _baseRepository.GetById(item.LOCA_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
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

        public Int32 Edit(LOCACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LOCACAO obj = _baseRepository.GetById(item.LOCA_CD_ID);
                    _baseRepository.Detach(obj);
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

        public Int32 Delete(LOCACAO item, LOG log)
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

        public List<LOCACAO_PARCELA> GetAllParcelas(Int32 idAss)
        {
            return _parRepository.GetAllItens(idAss);
        }

        public LOCACAO_PARCELA GetParcelaById(Int32 id)
        {
            return _parRepository.GetItemById(id);
        }

        public Int32 EditParcela(LOCACAO_PARCELA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.LOCACAO = null;
                    LOCACAO_PARCELA obj = _parRepository.GetById(item.LOPA_CD_ID);
                    _parRepository.Detach(obj);
                    _parRepository.Update(item);
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

        public Int32 CreateParcela(LOCACAO_PARCELA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.LOCACAO = null;
                    _parRepository.Add(item);
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

        public LOCACAO_ANEXO GetLocacaoAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public Int32 EditLocacaoAnexo(LOCACAO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LOCACAO_ANEXO obj = _aneRepository.GetById(item.LOAX_CD_ID);
                    _aneRepository.Detach(obj);
                    _aneRepository.Update(item);
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

        public LOCACAO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(LOCACAO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    LOCACAO_ANOTACAO obj = _anoRepository.GetById(item.LOAN_CD_ID);
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

        public LOCACAO_HISTORICO GetHistoricoById(Int32 id)
        {
            return _hisRepository.GetItemById(id);
        }

        public List<LOCACAO_HISTORICO> GetAllHistorico(Int32 idAss)
        {
            return _hisRepository.GetAllItens(idAss);
        }

        public Int32 CreateHistorico(LOCACAO_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _hisRepository.Add(item);
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

        public List<LOCACAO_HISTORICO> ExecuteFilterHistorico(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss)
        {
            return _hisRepository.ExecuteFilter(tipo, paci, inicio, final, descricao, idAss);

        }

        public List<LOCACAO_PARCELA> ExecuteFilterParcela(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss)
        {
            return _parRepository.ExecuteFilter(tipo, paci, inicio, final, descricao, idAss);

        }

        public LOCACAO_OCORRENCIA GetOcorrenciaById(Int32 id)
        {
            return _ocoRepository.GetItemById(id);
        }

        public Int32 EditOcorrencia(LOCACAO_OCORRENCIA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.LOCACAO = null;
                    LOCACAO_OCORRENCIA obj = _ocoRepository.GetById(item.LOOC_CD_ID);
                    _ocoRepository.Detach(obj);
                    _ocoRepository.Update(item);
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

        public Int32 CreateOcorrencia(LOCACAO_OCORRENCIA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.LOCACAO = null;
                    _ocoRepository.Add(item);
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

        public List<TIPO_OCORRENCIA> GetAllTipoOcorrencia(Int32 idAss)
        {
            return _toRepository.GetAllItens(idAss);
        }

        public List<TIPO_CONTRATO> GetAllTipoContrato(Int32 idAss)
        {
            return _tcRepository.GetAllItens(idAss);
        }

        public CONTRATO_LOCACAO CheckExistContrato(CONTRATO_LOCACAO item, Int32 idAss)
        {
            CONTRATO_LOCACAO volta = _clRepository.CheckExist(item, idAss);
            return volta;
        }

        public CONTRATO_LOCACAO GetContratoById(Int32 id)
        {
            CONTRATO_LOCACAO item = _clRepository.GetItemById(id);
            return item;
        }

        public List<CONTRATO_LOCACAO> GetAllContratos(Int32 idAss)
        {
            return _clRepository.GetAllItens(idAss);
        }

        public Int32 EditContrato(CONTRATO_LOCACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_CONTRATO = null;
                    CONTRATO_LOCACAO obj = _clRepository.GetById(item.COLO_CD_ID);
                    _clRepository.Detach(obj);
                    _clRepository.Update(item);
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

        public Int32 CreateContrato(CONTRATO_LOCACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _clRepository.Add(item);
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
