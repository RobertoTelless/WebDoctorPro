using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ControleMensagemService : ServiceBase<CONTROLE_MENSAGEM>, IControleMensagemService
    {
        private readonly IControleMensagemRepository _baseRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ControleMensagemService(IControleMensagemRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss)
        {
            CONTROLE_MENSAGEM item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public CONTROLE_MENSAGEM GetItemById(Int32 id)
        {
            CONTROLE_MENSAGEM item = _baseRepository.GetItemById(id);
            return item;
        }

        public CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss)
        {
            CONTROLE_MENSAGEM item = _baseRepository.GetItemByDate(data, idAss);
            return item;
        }

        public List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public Int32 Create(CONTROLE_MENSAGEM item)
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

        public Int32 Edit(CONTROLE_MENSAGEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTROLE_MENSAGEM obj = _baseRepository.GetById(item.COME_CD_ID);
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
    }
}
