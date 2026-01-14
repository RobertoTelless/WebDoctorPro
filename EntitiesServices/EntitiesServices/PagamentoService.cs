using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class PagamentoService : ServiceBase<CONSULTA_PAGAMENTO>, IPagamentoService
    {
        private readonly IPagamentoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoPagamentoRepository _tipoRepository;
        private readonly IPagamentoAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IPagamentoAnotacaoRepository _anoRepository;
        private readonly IPagamentoNotaRepository _notaRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public PagamentoService(IPagamentoRepository baseRepository, ILogRepository logRepository, ITipoPagamentoRepository tipoRepository, IPagamentoAnexoRepository anexoRepository, IUsuarioRepository usuRepository, IPagamentoAnotacaoRepository anoRepository, IPagamentoNotaRepository notaRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _usuRepository = usuRepository;
            _anoRepository = anoRepository;
            _notaRepository = notaRepository;
        }

        public CONSULTA_PAGAMENTO GetItemById(Int32 id)
        {
            CONSULTA_PAGAMENTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CONSULTA_PAGAMENTO> ExecuteFilter(Int32? tipo, String nome, String favorecido, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, nome, favorecido, inicio, final, conferido, idAss);

        }

        public PAGAMENTO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public Int32 EditAnexo(PAGAMENTO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PAGAMENTO_ANEXO obj = _anexoRepository.GetById(item.PAAN_CD_ID);
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

        public PAGAMENTO_NOTA_FISCAL GetNotaById(Int32 id)
        {
            return _notaRepository.GetItemById(id);
        }

        public Int32 EditNota(PAGAMENTO_NOTA_FISCAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PAGAMENTO_NOTA_FISCAL obj = _notaRepository.GetById(item.PANF_CD_ID);
                    _notaRepository.Detach(obj);
                    _notaRepository.Update(item);
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

        public PAGAMENTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(PAGAMENTO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PAGAMENTO_ANOTACAO obj = _anoRepository.GetById(item.PGAN_CD_ID);
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

        public List<TIPO_PAGAMENTO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public Int32 Create(CONSULTA_PAGAMENTO item, LOG log)
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

        public Int32 Create(CONSULTA_PAGAMENTO item)
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


        public Int32 Edit(CONSULTA_PAGAMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.TIPO_PAGAMENTO = null;
                    CONSULTA_PAGAMENTO obj = _baseRepository.GetById(item.COPA_CD_ID);
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

        public Int32 EditConfirma(CONSULTA_PAGAMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    CONSULTA_PAGAMENTO obj = _baseRepository.GetById(item.COPA_CD_ID);
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

        public Int32 Edit(CONSULTA_PAGAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 tipo = item.TIPA_CD_ID.Value;

                    item.TIPO_PAGAMENTO = null;
                    CONSULTA_PAGAMENTO obj = _baseRepository.GetById(item.COPA_CD_ID);
                    _baseRepository.Detach(obj);
                    obj.TIPA_CD_ID = tipo;
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

        public Int32 Delete(CONSULTA_PAGAMENTO item, LOG log)
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
