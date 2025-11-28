using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class SolicitacaoService : ServiceBase<SOLICITACAO>, ISolicitacaoService
    {
        private readonly ISolicitacaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoExameRepository _teRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public SolicitacaoService(ISolicitacaoRepository baseRepository, ILogRepository logRepository, ITipoExameRepository teRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _teRepository = teRepository;
        }

        public List<TIPO_EXAME> GetAllTipos(Int32 idAss)
        {
            return _teRepository.GetAllItens(idAss);
        }

        public SOLICITACAO GetItemById(Int32 id)
        {
            SOLICITACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<SOLICITACAO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<SOLICITACAO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public SOLICITACAO CheckExist(SOLICITACAO item, Int32 idAss)
        {
            SOLICITACAO volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<SOLICITACAO> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, titulo, descricao, idAss);

        }

        public Int32 Create(SOLICITACAO item, LOG log)
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

        public Int32 Create(SOLICITACAO item)
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

        public Int32 Edit(SOLICITACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SOLICITACAO obj = _baseRepository.GetById(item.SOLI_CD_ID);
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

        public Int32 Edit(SOLICITACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SOLICITACAO obj = _baseRepository.GetById(item.SOLI_CD_ID);
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

        public Int32 Delete(SOLICITACAO item, LOG log)
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
