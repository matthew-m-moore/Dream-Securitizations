using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.WebApp.Models;

namespace Dream.WebApp.Adapters
{
    public class PaceAssessmentRecordModelAdapter
    {
        public static PaceAssessmentRecordModel GetPaceAssessmentRecordModel(Securitization securitizationBusinessObject, bool isResecuritization)
        {
            if (isResecuritization) return null;

            throw new NotImplementedException();
        }
    }
}