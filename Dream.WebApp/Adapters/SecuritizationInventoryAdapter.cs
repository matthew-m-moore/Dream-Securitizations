using Dream.Core.BusinessLogic.Containers;
using Dream.Core.Repositories.Database;
using Dream.IO.Database.Entities.Securitization;
using Dream.WebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dream.WebApp.Adapters
{
    public class SecuritizationInventoryAdapter
    {
        private const string _unknownSecuritizationOwner = "(unknown)";

        private SecuritizationDatabaseRepository _securitizationDatabaseRepository;

        private List<SecuritizationAnalysisDataSetEntity>  _securitizationDataSetEntities = new List<SecuritizationAnalysisDataSetEntity>();    

        private List<SecuritizationAnalysisIdentifier> _securitizationAnalysisIdentifiers = new List<SecuritizationAnalysisIdentifier>();

        private Dictionary<SecuritizationAnalysisIdentifier, SecuritizationAnalysisOwnerEntity> _securitizationOwnerEntitiesDictionary;
        private Dictionary<SecuritizationAnalysisIdentifier, SecuritizationAnalysisCommentEntity> _securitizationCommentEntitiesDictionary;

        public SecuritizationInventoryAdapter(
            List<SecuritizationAnalysisDataSetEntity> securitizationDataSetEntities,
            List<SecuritizationAnalysisEntity> securitizationAnalysisEntities,
            List<SecuritizationAnalysisOwnerEntity> securitizationOwnerEntities,
            List<SecuritizationAnalysisCommentEntity> securitizationCommentEntities
            )
        {
            _securitizationDatabaseRepository = new SecuritizationDatabaseRepository();
            _securitizationDataSetEntities = securitizationDataSetEntities;

            // The equals method is overridden for SecuritizationAnalysisIdentifier
            _securitizationAnalysisIdentifiers = securitizationAnalysisEntities
                .Select(e => new SecuritizationAnalysisIdentifier(e.SecuritizationAnalysisDataSetId, e.SecuritizationAnalysisVersionId))
                .Distinct().ToList();

            // There is a unique constraint on DataSetId/VersionId the will ensure uniqueness of these keys
            _securitizationOwnerEntitiesDictionary = securitizationOwnerEntities
                .ToDictionary(s => new SecuritizationAnalysisIdentifier
                                   (
                                        s.SecuritizationAnalysisDataSetId,
                                        s.SecuritizationAnalysisVersionId.GetValueOrDefault()
                                    ));

            // There is a unique index in the database on "IsVisible" that will ensure each of these keys is unique
            _securitizationCommentEntitiesDictionary = securitizationCommentEntities.Where(e => e.IsVisible)
                .ToDictionary(s => new SecuritizationAnalysisIdentifier
                                   (
                                        s.SecuritizationAnalysisDataSetId, 
                                        s.SecuritizationAnalysisVersionId.GetValueOrDefault()
                                    ));
        }

        public List<SecuritizationDataSetModel> CreateSecuritizationInventory()
        {
            var securitizationInventory = new List<SecuritizationDataSetModel>();
            foreach (var securitizationDataSetEntity in _securitizationDataSetEntities)
            {
                var securitizationDataSetModel = new SecuritizationDataSetModel
                {
                    SecuritizationDataSetId = securitizationDataSetEntity.SecuritizationAnalysisDataSetId,
                    SecuritizationDataSetDescription = securitizationDataSetEntity.SecuritizationAnalysisDataSetDescription,
                    CreatedDate = securitizationDataSetEntity.CreatedDate,
                    IsRescuritization = securitizationDataSetEntity.IsResecuritization,
                    IsTemplate = securitizationDataSetEntity.IsTemplate
                };

                var securitizationDataSetIdentifier = new SecuritizationAnalysisIdentifier(securitizationDataSetEntity.SecuritizationAnalysisDataSetId);
                if (_securitizationOwnerEntitiesDictionary.ContainsKey(securitizationDataSetIdentifier))
                {
                    var securitizationOwnerEntity = _securitizationOwnerEntitiesDictionary[securitizationDataSetIdentifier];
                    securitizationDataSetModel.IsReadOnly = securitizationOwnerEntity.IsReadOnlyToOthers;
                }

                if (_securitizationCommentEntitiesDictionary.ContainsKey(securitizationDataSetIdentifier))
                {
                    securitizationDataSetModel.SecuritizationDataSetComment =
                        _securitizationCommentEntitiesDictionary[securitizationDataSetIdentifier].CommentText;
                }

                var listOfSecuritizationVersions = GetListOfSecuritizationVersions(securitizationDataSetEntity);
                securitizationDataSetModel.SecuritizationVersions = listOfSecuritizationVersions;
                securitizationDataSetModel.SecuritizationOwner = _unknownSecuritizationOwner;

                // The list should be ordered by latest version first, and only the original owner will be displayed
                var firstSecuritizationVersion = listOfSecuritizationVersions.LastOrDefault();
                if (firstSecuritizationVersion != null && !string.IsNullOrEmpty(firstSecuritizationVersion.SecuritizationVersionOwner))
                {
                    securitizationDataSetModel.SecuritizationOwner = firstSecuritizationVersion.SecuritizationVersionOwner;
                }

                securitizationInventory.Add(securitizationDataSetModel);
            }

            securitizationInventory =
                securitizationInventory.OrderByDescending(s => s.IsTemplate).ThenByDescending(s => s.SecuritizationDataSetId).ToList();

            return securitizationInventory;
        }

        private List<SecuritizationVersionModel> GetListOfSecuritizationVersions(SecuritizationAnalysisDataSetEntity securitizationDataSetEntity)
        {
            var securitizationVersions = new List<SecuritizationVersionModel>();

            var securitizaitonVersionIdentifiers = _securitizationAnalysisIdentifiers
                .Where(i => i.SecuritizationAnalysisDataSetId == securitizationDataSetEntity.SecuritizationAnalysisDataSetId &&
                            i.SecuritizationAnalysisVersionId != default(int));

            foreach(var securitizationVersionIdentifier in securitizaitonVersionIdentifiers)
            {
                var securitizationVersionModel = new SecuritizationVersionModel
                {
                    SecuritizationDataSetId = securitizationVersionIdentifier.SecuritizationAnalysisDataSetId,
                    SecuritizationVersionId = securitizationVersionIdentifier.SecuritizationAnalysisVersionId
                };

                securitizationVersionModel.SecuritizationVersionOwner = _unknownSecuritizationOwner;
                if (_securitizationOwnerEntitiesDictionary.ContainsKey(securitizationVersionIdentifier))
                {
                    var securitizationOwnerEntity = _securitizationOwnerEntitiesDictionary[securitizationVersionIdentifier];

                    securitizationVersionModel.SecuritizationVersionOwner =
                        _securitizationDatabaseRepository.ApplicationUsers[securitizationOwnerEntity.ApplicationUserId].NickName;

                    // Note that the update stored procedure only allows this flag to be set across all versions, but having access to it here is nice
                    securitizationVersionModel.IsReadOnly = securitizationOwnerEntity.IsReadOnlyToOthers;
                }

                if (_securitizationCommentEntitiesDictionary.ContainsKey(securitizationVersionIdentifier))
                {
                    securitizationVersionModel.SecuritizationVersionComment =
                        _securitizationCommentEntitiesDictionary[securitizationVersionIdentifier].CommentText;

                    securitizationVersionModel.TruncatedSecuritizationVersionComment = 
                        (securitizationVersionModel.SecuritizationVersionComment != null && securitizationVersionModel.SecuritizationVersionComment.Length > 100)
                            ? securitizationVersionModel.SecuritizationVersionComment.Substring(0, 100) + "..."
                            : securitizationVersionModel.SecuritizationVersionComment;
                }

                securitizationVersions.Add(securitizationVersionModel);
            }

            securitizationVersions = securitizationVersions.OrderByDescending(v => v.SecuritizationVersionId).ToList();
            return securitizationVersions;
        }
    }
}