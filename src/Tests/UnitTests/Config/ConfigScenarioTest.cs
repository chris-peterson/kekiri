using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kekiri.UnitTests.Config
{
    [Feature("Configuration", "In order to allow the developer to customize the behavior of the test framework,", "The framework supports customization via a configuration file")]
    class ConfigScenarioTest : ScenarioTest
    {
        private AppConfig _configFileContext;

        public override void CleanupScenario()
        {
            _configFileContext.Dispose();

            base.CleanupScenario();
        }

        [Given]
        public void Given()
        {
            var customConfigResourceName =
                GetType().Assembly.GetManifestResourceNames().Single(r => r.EndsWith("Custom.config"));

            var configFilename = Path.Combine(Environment.CurrentDirectory, "Custom.config");
            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(customConfigResourceName))
            {
                if (resourceStream == null)
                {
                    throw new ApplicationException("Expected to find embedded resource file for custom config test");
                }
                using (var configFile = File.Create(configFilename))
                {
                    resourceStream.CopyTo(configFile);
                }
            }

            _configFileContext = AppConfig.Change(configFilename);
        }

        #region Support Types
        // Adapted from http://stackoverflow.com/questions/6150644/change-default-app-config-at-runtime/6151688#6151688
        private abstract class AppConfig : IDisposable
        {
            public static AppConfig Change(string path)
            {
                return new ChangeAppConfig(path);
            }

            public abstract void Dispose();

            private class ChangeAppConfig : AppConfig
            {
                private readonly string _oldConfig =
                    AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

                private bool _disposedValue;

                public ChangeAppConfig(string path)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                    ResetConfigMechanism();
                }

                public override void Dispose()
                {
                    if (!_disposedValue)
                    {
                        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", _oldConfig);
                        ResetConfigMechanism();

                        _disposedValue = true;
                    }

                    GC.SuppressFinalize(this);
                }

                private static void ResetConfigMechanism()
                {
                    var fieldInfo = typeof(ConfigurationManager).GetField("s_initState",
                                                                           BindingFlags.NonPublic | BindingFlags.Static);
                    if (fieldInfo == null)
                    {
                        throw new ApplicationException("didn't find s_initState");
                    }
                    fieldInfo.SetValue(null, 0);

                    var field = typeof(ConfigurationManager).GetField("s_configSystem",
                                                                       BindingFlags.NonPublic | BindingFlags.Static);
                    if (field == null)
                    {
                        throw new ApplicationException("didn't find s_configSystem");
                    }
                    field.SetValue(null, null);

                    var info =
                        typeof(ConfigurationManager).Assembly.GetTypes()
                                                     .First(x => x.FullName == "System.Configuration.ClientConfigPaths")
                                                     .GetField("s_current", BindingFlags.NonPublic | BindingFlags.Static);
                    if (info == null)
                    {
                        throw new ApplicationException("Didn't find s_current");
                    }
                    info.SetValue(null, null);
                }
            }
        }
        #endregion
    }
}