using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class LeadService : ServiceBase<LEAD>, ILeadService
    {
        private readonly ILeadRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILeadAnexoRepository _aneRepository;
        private readonly ILeadAnotacaoRepository _anoRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public LeadService(ILeadRepository baseRepository, ILogRepository logRepository, ILeadAnexoRepository aneRepository, ILeadAnotacaoRepository anoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _aneRepository = aneRepository;
            _anoRepository = anoRepository;
        }

        public LEAD GetItemById(Int32 id)
        {
            LEAD item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<LEAD> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<LEAD> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public LEAD CheckExist(LEAD item, Int32 idAss)
        {
            LEAD volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<LEAD> ExecuteFilter(DateTime? inicio, DateTime? final, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(inicio, final, nome, email, status, cpf, cnpj,cidade, uf, idAss);

        }

        public Int32 Create(LEAD item, LOG log)
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

        public Int32 Create(LEAD item)
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

        public Int32 Edit(LEAD item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LEAD obj = _baseRepository.GetById(item.LEAD_CD_ID);
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

        public Int32 Edit(LEAD item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LEAD obj = _baseRepository.GetById(item.LEAD_CD_ID);
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

        public Int32 Delete(LEAD item, LOG log)
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


        public LEAD_ANEXO GetLeadAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public Int32 EditLeadAnexo(LEAD_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LEAD_ANEXO obj = _aneRepository.GetById(item.LEAX_CD_ID);
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

        public LEAD_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(LEAD_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    LEAD_ANOTACAO obj = _anoRepository.GetById(item.LEAN_CD_ID);
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
    }
}
