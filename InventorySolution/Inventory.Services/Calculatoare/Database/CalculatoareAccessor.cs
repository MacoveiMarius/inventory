﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Core.Domain;
using Inventory.Core;
using System.Data.SqlClient;

namespace Inventory.Services
{

    public class CalculatoareAccessor : ICalculatoare
    {
        private readonly StringBuilder _strDbConnectionString;

        public CalculatoareAccessor(StringBuilder strDbConnectionString)
        {
            _strDbConnectionString = strDbConnectionString;
        }

        private void PreLoadData(CalculatoareDb context, bool loadFullData = true)
        {
            if (loadFullData)
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Inventar>(i => i);
                options.AssociateWith<Inventar>(i => i.InventareEntity.OrderBy(ppp => ppp.Id));
                context.LoadOptions = options;
            }
            else
            {
                context.ObjectTrackingEnabled = false;
            }
        }

        public List<Calculator> GetCalculatoare()
        {
            using (var context = CalculatoareDb.Create(_strDbConnectionString.ToString()))
            {
                var result = (from t in context.Calculatoare
                             select t).ToList<Calculator>();
                return result;
            }
        }

        public Calculator GetCalculator(int id)
        {
            using (var context = CalculatoareDb.Create(_strDbConnectionString.ToString()))
            {
                var result = (from t in context.Calculatoare
                              where t.Id==id
                              select t).SingleOrDefault();
                return result;
            }
        }

        public ServiceResult AddCalculator(Calculator calculator)
        {
            ServiceResult result = new ServiceResult((int)OperationResult.Success);
            using (var context = CalculatoareDb.Create(_strDbConnectionString.ToString()))
            {
                try
                {
                    context.Calculatoare.InsertOnSubmit(calculator);
                    context.SubmitChanges();

                    result.EntityId = calculator.Id;
                }
                catch (SqlException sqlExc)
                {
                    if (sqlExc.Number == BaseEntity.UNIQUE_INDEX_VIOLATION ||
                        sqlExc.Number == BaseEntity.CANNOT_INSERT_DUPLICATE_KEY_ROW)
                    {
                        result.Result = (int)OperationResult.ErrorDuplicateItem;
                    }
                    else if (sqlExc.Number == BaseEntity.FOREIGN_KEY_VIOLATION)
                    {
                        result.Result = (int)OperationResult.ErrorForeignKeyViolation;
                    }
                    else
                    {
                        result.Result = (int)OperationResult.Error;
                    }
                }
            }
            return result;
        }

        public ServiceResult UpdateCalculator(Calculator calculator)
        {
            ServiceResult result = new ServiceResult((int)OperationResult.Success);
            using (var context = InventareDb.Create(_strDbConnectionString.ToString()))
            {
                try
                {
                    var target = (from g in context.Calculatoare
                                          where g.Id == calculator.Id
                                          select g).FirstOrDefault();

                    if (target == null)
                    {
                        result.Result = (int)OperationResult.ErrorItemNotFound;
                    }
                    else
                    {
                        BaseEntity.ShallowCopy(calculator, target);
                        context.SubmitChanges();
                    }
                }
                catch (SqlException sqlExc)
                {
                    if (sqlExc.Number == BaseEntity.UNIQUE_INDEX_VIOLATION ||
                        sqlExc.Number == BaseEntity.CANNOT_INSERT_DUPLICATE_KEY_ROW)
                    {
                        result.Result = (int)OperationResult.ErrorDuplicateItem;
                    }
                    else if (sqlExc.Number == BaseEntity.FOREIGN_KEY_VIOLATION)
                    {
                        result.Result = (int)OperationResult.ErrorForeignKeyViolation;
                    }
                    else
                    {
                        result.Result = (int)OperationResult.Error;
                    }
                }
            }
            return result;
        }
    }
}
