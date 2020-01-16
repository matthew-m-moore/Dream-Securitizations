using System;
using System.IO;
using System.Reflection;

namespace Dream.Common
{
    public static class BuildIdentifier
    {
        private const string _buildIdentifierTextFile = "Dream.Common.Changeset.txt";

        private static string _completeBuildId;
        public static string CompleteBuildId
        {
            get
            {
                if (_completeBuildId == null)
                {
                    ReadBuildIdFromManifest();
                }

                return _completeBuildId;
            }
        }

        private static string _abbreviateBuildId;
        public static string AbbreviatedBuildId
        {
            get
            {
                if (_abbreviateBuildId == null)
                {
                    ReadBuildIdFromManifest();
                }

                return _abbreviateBuildId;
            }
        }

        private static void ReadBuildIdFromManifest()
        {
            try
            {
                var manifestInfoStream = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream(_buildIdentifierTextFile);

                var textFileReader = new StreamReader(manifestInfoStream);

                var buildIdentifierData = textFileReader.ReadToEnd().Split('\n');
                var buildIdentifierComponents = buildIdentifierData;

                _completeBuildId = buildIdentifierComponents[0];
                _abbreviateBuildId = buildIdentifierComponents[2];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
