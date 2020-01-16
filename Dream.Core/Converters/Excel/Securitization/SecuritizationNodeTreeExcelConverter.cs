using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class SecuritizationNodeTreeExcelConverter
    {
        private const string _reserveAccounts = "Reserve Accounts";
        
        private List<Tranche> _listOfTranchesInSecuritization;

        public List<SecuritizationNodeTree> SecuritizationNodes { get; private set; }

        public SecuritizationNodeTreeExcelConverter(List<Tranche> listOfTranchesInSecuritization)
        {
            SecuritizationNodes = new List<SecuritizationNodeTree>();
            _listOfTranchesInSecuritization = listOfTranchesInSecuritization;
        }

        /// <summary>
        /// Adds all non-fee tranches to their respectively indicated nodes with their specified distribution rules.
        /// </summary>
        public void AddTrancheStructureRecordsToTree(List<TrancheStructureRecord> listOfTrancheStructureRecords, ref int securitizationNodeId)
        {         
            var distinctNodeInformation = listOfTrancheStructureRecords.Select(r => (
                NodeDescription: r.NodeDescription, 
                NodeType: r.NodeType, 
                NodeRating: r.NodeRating,
                NodePricingScenario: r.NodePricingScenario)).Distinct().ToList();

            foreach (var nodeInformation in distinctNodeInformation)
            {
                // Note, order presevation does not matter here, since that is handled in the priority of payments waterfall
                var listOfTrancheStructureRecordsAtNode = listOfTrancheStructureRecords
                    .Where(r => r.NodeDescription == nodeInformation.NodeDescription && r.ChildNodeDescription == null).ToList();
                var listOfTrancheNamesAtNode = listOfTrancheStructureRecordsAtNode
                    .Select(t => t.TrancheName).ToList();

                var distributionRuleText = listOfTrancheStructureRecordsAtNode.Select(r => r.FundsDistribution).Distinct().SingleOrDefault();
                var securitizationNodeTree = CreateSecuritizationNodeTree(nodeInformation, listOfTrancheNamesAtNode, distributionRuleText, securitizationNodeId);
                securitizationNodeId++;

                AddSecuritizationSubNodes(listOfTrancheStructureRecords, securitizationNodeTree, securitizationNodeId);
                SecuritizationNodes.Add(securitizationNodeTree);
            }
        }

        /// <summary>
        /// Adds all fee tranches to their respectively indicated nodes with their specified distribution rules.
        /// </summary>
        public void AddFeeGroupRecordsToTree(List<FeeGroupRecord> listOfFeeGroupRecords, ref int securitizationNodeId)
        {
            var distinctNodeInformation = listOfFeeGroupRecords
                .Select(r => (NodeDescription: r.NodeDescription, NodeType: string.Empty, NodeRating: string.Empty, NodePricingScenario: string.Empty))
                .Distinct().ToList();

            foreach (var nodeInformation in distinctNodeInformation)
            {
                // Note, order presevation does not matter here, since that is handled in the priority of payments waterfall
                var listOfFeeGroupRecordsAtNode = listOfFeeGroupRecords
                    .Where(r => r.NodeDescription == nodeInformation.NodeDescription).ToList();
                var listOfFeeGroupNamesAtNode = listOfFeeGroupRecordsAtNode
                    .Select(t => t.FeeGroupName).ToList();

                var distributionRuleText = listOfFeeGroupRecordsAtNode.Select(r => r.FundsDistribution).Distinct().SingleOrDefault();
                var securitizationNodeTree = CreateSecuritizationNodeTree(nodeInformation, listOfFeeGroupNamesAtNode, distributionRuleText, securitizationNodeId);

                SecuritizationNodes.Add(securitizationNodeTree);
                securitizationNodeId++;
            }
        }

        /// <summary>
        /// Adds all reserve tranches to a node called "Reserve Accounts" with a sequential distribution rule.
        /// </summary>
        public void AddReserveAccountRecordsToTree(List<ReserveAccountRecord> listOfReserveAccountRecords)
        {
            // Note, order presevation does not matter here, since that is handled in the priority of payments waterfall
            var listOfReserveTrancheNames = listOfReserveAccountRecords.Select(r => r.ReserveAccountName).ToList();
            var listOfReserveTranchesInSecuritization = _listOfTranchesInSecuritization
                .Where(t => listOfReserveTrancheNames.Contains(t.TrancheName)).ToList();

            var securitizationNodeTree = new SecuritizationNodeTree(new SequentialDistributionRule())
            {
                SecuritizationNodeName = _reserveAccounts,
                SecuritizationTranches = listOfReserveTranchesInSecuritization,
            };

            SecuritizationNodes.Add(securitizationNodeTree);
        }

        private void AddSecuritizationSubNodes(List<TrancheStructureRecord> listOfTrancheStructureRecords, SecuritizationNodeTree securitizationNodeTree, int securitizationNodeId)
        {
            var listOfTrancheStructureRecordsAtNode = listOfTrancheStructureRecords
                .Where(r => r.NodeDescription == securitizationNodeTree.SecuritizationNodeName && r.ChildNodeDescription != null).ToList();

            var distinctSubNodeInformation = listOfTrancheStructureRecordsAtNode.Select(r => (
                ChildNodeDescription: r.ChildNodeDescription,
                ChildNodeType: r.ChildNodeType,
                ChildNodeRating: r.ChildNodeRating,
                ChildNodePricingScenario: r.ChildNodePricingScenario)).Distinct().ToList();

            foreach (var subNodeInformation in distinctSubNodeInformation)
            {
                // Note, order presevation does not matter here, since that is handled in the priority of payments waterfall
                var listOfTrancheStructureRecordsAtSubNode = listOfTrancheStructureRecordsAtNode
                    .Where(r => r.ChildNodeDescription == subNodeInformation.ChildNodeDescription).ToList();
                var listOfTrancheNamesAtSubNode = listOfTrancheStructureRecordsAtSubNode
                    .Select(t => t.TrancheName).ToList();

                var subNodeDistributionRuleText = listOfTrancheStructureRecordsAtSubNode.Select(r => r.ChildFundsDistribution).Distinct().SingleOrDefault();
                var securitizationSubNodeTree = CreateSecuritizationNodeTree(subNodeInformation, listOfTrancheNamesAtSubNode, subNodeDistributionRuleText, securitizationNodeId);         

                securitizationSubNodeTree.ParentSecuritizationNode = securitizationNodeTree;
                securitizationNodeTree.SecuritizationNodes.Add(securitizationSubNodeTree);
                securitizationNodeId++;
            }
        }

        private SecuritizationNodeTree CreateSecuritizationNodeTree(
            (string NodeDescription, string NodeType, string NodeRating, string NodePricingScenario) nodeInformation,
            List<string> listOfTrancheNamesAtNode,
            string distributionRuleText,
            int securitizationNodeId)
        {
            if (distributionRuleText == null)
            {
                throw new Exception(string.Format("ERROR: More than one funds distribution rule was specified for the securitization node named '{0}'.",
                    nodeInformation));
            }

            var distributionRule = DistributionRuleExcelConverter.DetermineDistributionRuleFromDescription(distributionRuleText);
            if (distributionRule == null)
            {
                throw new Exception(string.Format("ERROR: No known funds distribution rule was provided for securitization node named '{0}'.",
                    nodeInformation));
            }

            var listOfTranchesAtNode = _listOfTranchesInSecuritization.Where(t => listOfTrancheNamesAtNode.Contains(t.TrancheName)).ToList();

            var securitizationNodeTree = new SecuritizationNodeTree(distributionRule)
            {
                SecuritizationNodeId = securitizationNodeId,
                SecuritizationNodeName = nodeInformation.NodeDescription,
                SecuritizationNodeType = nodeInformation.NodeType,
                SecuritizationNodeRating = nodeInformation.NodeRating,
                SecuritizationNodePricingScenario = nodeInformation.NodePricingScenario,
                SecuritizationTranches = listOfTranchesAtNode,
                SecuritizationNodes = new List<SecuritizationNodeTree>()
            };

            foreach (var tranche in securitizationNodeTree.SecuritizationTranches) tranche.SecuritizationNode = securitizationNodeTree;

            return securitizationNodeTree;
        }
    }
}
