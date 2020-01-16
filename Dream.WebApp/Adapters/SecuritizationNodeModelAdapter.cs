using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.WebApp.Models;
using Dream.WebApp.ModelEntries;
using Dream.Core.Converters.Database.Securitization;

namespace Dream.WebApp.Adapters
{
    public class SecuritizationNodeModelAdapter
    {
        public static SecuritizationNodeModel GetSecuritizationNodeModel(Securitization securitizationBusinessObject)
        {
            var securitizationNodeModel = new SecuritizationNodeModel();
            var securitizationNodes = securitizationBusinessObject.SecuritizationNodes;

            PopulateSecuritizationNodeModel(securitizationNodes, securitizationNodeModel);
            return securitizationNodeModel;
        }

        private static void PopulateSecuritizationNodeModel(
            List<SecuritizationNodeTree> securitizationNodes, 
            SecuritizationNodeModel securitizationNodeModel, 
            SecuritizationNodeModelEntry securitizationParentNodeModelEntry = null)
        {
            foreach (var securitizationNode in securitizationNodes)
            {
                var availableFundsDistributionRuleDescription = DistributionRuleDatabaseConverter
                    .DetermineDescriptionFromDistributionRule(securitizationNode.AvailableFundsDistributionRule.GetType());

                var securitizationNodeModelEntry = new SecuritizationNodeModelEntry
                {
                    ParentSecuritizationNodeModelEntry = securitizationParentNodeModelEntry,
                    SecuritizationNodeModelEntries = new List<SecuritizationNodeModelEntry>(),
                    SecuritizationNodeId = securitizationNode.SecuritizationNodeId,
                    SecuritizationNodeName = securitizationNode.SecuritizationNodeName,
                    SecuritizationNodeType = securitizationNode.SecuritizationNodeType,
                    SecuritizationNodeRating = securitizationNode.SecuritizationNodeRating,
                    AvailableFundsDistributionRuleDescription = availableFundsDistributionRuleDescription
                };

                if (securitizationNode.AnyNodes)
                {
                    var securitizationSubNodes = securitizationNode.SecuritizationNodes;
                    PopulateSecuritizationNodeModel(securitizationSubNodes, securitizationNodeModel, securitizationNodeModelEntry);
                }

                if (securitizationParentNodeModelEntry != null)
                {
                    securitizationParentNodeModelEntry.SecuritizationNodeModelEntries.Add(securitizationNodeModelEntry);
                }
                else
                {
                    securitizationNodeModel.SecuritizationNodeModelEntries.Add(securitizationNodeModelEntry);
                }
            }
        }
    }
}