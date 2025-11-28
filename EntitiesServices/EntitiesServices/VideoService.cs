using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class VideoService : ServiceBase<VIDEO_BASE>, IVideoService
    {
        private readonly IVideoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoVideoRepository _tipoRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public VideoService(IVideoRepository baseRepository, ILogRepository logRepository, ITipoVideoRepository tipoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
        }

        public List<TIPO_VIDEO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public VIDEO_BASE GetItemById(Int32 id)
        {
            VIDEO_BASE item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VIDEO_BASE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VIDEO_BASE> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public VIDEO_BASE CheckExist(VIDEO_BASE item, Int32 idAss)
        {
            VIDEO_BASE volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<VIDEO_BASE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, nome, idAss);

        }

        public Int32 Create(VIDEO_BASE item, LOG log)
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

        public Int32 Create(VIDEO_BASE item)
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

        public Int32 Edit(VIDEO_BASE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VIDEO_BASE obj = _baseRepository.GetById(item.VIDE_CD_ID);
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

        public Int32 Edit(VIDEO_BASE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VIDEO_BASE obj = _baseRepository.GetById(item.VIDE_CD_ID);
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

        public Int32 Delete(VIDEO_BASE item, LOG log)
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
