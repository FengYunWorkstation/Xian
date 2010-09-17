﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using SystemConfiguration = System.Configuration.Configuration;
using System.Xml;

namespace ClearCanvas.Common.Configuration
{
	public static class ApplicationSettingsExtensions
    {
		private static SettingsPropertyCollection GetPropertiesForProvider(ApplicationSettingsBase settings, SettingsProvider provider)
        {
            SettingsPropertyCollection properties = new SettingsPropertyCollection();
            
            foreach (SettingsProperty property in settings.Properties)
            {
                if (property.Provider == provider)
                    properties.Add(property);
            }
            
            return properties;
        }

        private static ISharedApplicationSettingsProvider GetSharedSettingsProvider(SettingsProvider provider)
        {
            if (provider is LocalFileSettingsProvider)
                return new ExtendedLocalFileSettingsProvider((LocalFileSettingsProvider)provider);

            return provider as ISharedApplicationSettingsProvider;
        }

        private static void SaveIfDirty(ApplicationSettingsBase settings)
        {
            foreach (SettingsPropertyValue property in settings.PropertyValues)
            {
            	if (!property.IsDirty)
					continue;
            	
				settings.Save();
            	return;
            }
        }

		private static void SetSharedPropertyValues(ApplicationSettingsBase settings, Dictionary<string, string> values)
		{
			foreach (SettingsProvider provider in settings.Providers)
			{
				ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(provider);
				if (sharedSettingsProvider == null)
					throw new NotSupportedException("Setting shared values is not supported.");

				var properties = GetPropertiesForProvider(settings, provider);
				SettingsPropertyValueCollection settingsValues = new SettingsPropertyValueCollection();

				foreach (var value in values)
				{
					SettingsProperty property = properties[value.Key];
					if (property == null)
						continue;

					settingsValues.Add(new SettingsPropertyValue(property) { SerializedValue = value.Value, IsDirty = true });
				}

				sharedSettingsProvider.SetSharedPropertyValues(settings.Context, settingsValues);
			}

			SaveIfDirty(settings);
			settings.Reload();
		}

		private static void MigrateProperty(IMigrateSettings customMigrator, MigrationScope migrationScope, 
		                                    SettingsPropertyValue currentValue, object previousValue)
		{
			var migrationValues = new SettingsPropertyMigrationValues(
				currentValue.Property.Name, migrationScope, 
				currentValue.PropertyValue, previousValue);

			customMigrator.MigrateSettingsProperty(migrationValues);
			if (!Equals(migrationValues.CurrentValue, currentValue.PropertyValue))
				currentValue.PropertyValue = migrationValues.CurrentValue;
		}

		internal static void MigrateUserSettings(ApplicationSettingsBase settings)
		{
			if (settings is IMigrateSettings)
			{
				IMigrateSettings customMigrator = (IMigrateSettings)settings;
				foreach (SettingsProperty property in settings.Properties)
				{
					if (!SettingsPropertyExtensions.IsUserScoped(property))
						continue;

					object previousValue = settings.GetPreviousVersion(property.Name);

					//need to do this to force the values to load before accessing the PropertyValues in order to migrate,
					//otherwise the SettingsPropertyValue will always be null.
					var iForceSettingsPropertyValuesToLoad = settings[property.Name];
					var currentValue = settings.PropertyValues[property.Name];
					MigrateProperty(customMigrator, MigrationScope.User, currentValue, previousValue);
				}
			}
			else
			{
				settings.Upgrade();
			}

			//Don't need to reload because the user settings will be current.
			SaveIfDirty(settings);
		}

		internal static void MigrateSharedSettings(ApplicationSettingsBase settings, string previousExeConfigFilename)
		{
			if (settings is IMigrateSettings)
			{
				IMigrateSettings customMigrator = (IMigrateSettings) settings;
				foreach (SettingsProvider settingsProvider in settings.Providers)
				{
					ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(settingsProvider);
					if (sharedSettingsProvider == null || !sharedSettingsProvider.CanUpgradeSharedPropertyValues(settings.Context))
						continue;

					var properties = GetPropertiesForProvider(settings, settingsProvider);
					var previousValues = sharedSettingsProvider.GetPreviousSharedPropertyValues(settings.Context,
					                                                                            properties, previousExeConfigFilename);
					if (previousValues == null || previousValues.Count == 0)
						continue;

					var currentValues = sharedSettingsProvider.GetSharedPropertyValues(settings.Context, properties);
					foreach (SettingsPropertyValue previousValue in previousValues)
					{
						SettingsPropertyValue currentValue = currentValues[previousValue.Name];
						if (currentValue == null)
							continue;

						MigrateProperty(customMigrator, MigrationScope.Shared, currentValue, previousValue.PropertyValue);
					}

					foreach (SettingsPropertyValue property in currentValues)
					{
						if (!property.IsDirty)
							continue;

						sharedSettingsProvider.SetSharedPropertyValues(settings.Context, currentValues);
						break;
					}
				}
			}
			else
			{
				foreach (SettingsProvider settingsProvider in settings.Providers)
				{
					ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(settingsProvider);
					if (sharedSettingsProvider == null)
						continue;

					var properties = GetPropertiesForProvider(settings, settingsProvider);
					sharedSettingsProvider.UpgradeSharedPropertyValues(settings.Context, properties, previousExeConfigFilename);
				}
			}

			SaveIfDirty(settings);
			//Need to call Reload because changes to shared settings aren't automatically realized by the .NET settings framework.
			settings.Reload();
		}

		public static object GetPreviousSharedVersion(ApplicationSettingsBase settings, string propertyName, string previousExeConfigFilename)
        {
            SettingsProperty property = settings.Properties[propertyName];
            if (property == null)
                throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");
            
            ISharedApplicationSettingsProvider provider = GetSharedSettingsProvider(property.Provider);
            if (provider == null)
                return null;

            SettingsPropertyValueCollection values = provider.GetPreviousSharedPropertyValues(settings.Context, 
                                    new SettingsPropertyCollection { property }, previousExeConfigFilename);

            SettingsPropertyValue value = values[propertyName];
            return value == null ? null : value.PropertyValue;
        }

        public static object GetSharedVersion(ApplicationSettingsBase settings, string propertyName)
        {
            SettingsProperty property = settings.Properties[propertyName];
            if (property == null)
                throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");

			ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(property.Provider);
            if (sharedSettingsProvider == null)
                return null;

            var values = sharedSettingsProvider.GetSharedPropertyValues(settings.Context, new SettingsPropertyCollection { property });
            SettingsPropertyValue value = values[propertyName];
            return value == null ? null : value.PropertyValue;
        }

		public static void SetSharedVersion(ApplicationSettingsBase settings, string propertyName, object value)
		{
			SettingsProperty property = settings.Properties[propertyName];
			if (property == null)
				throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");

			ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(property.Provider);
			if (sharedSettingsProvider == null)
				throw new NotSupportedException("Setting shared values is not supported.");

			SettingsPropertyValue settingsValue = new SettingsPropertyValue(property) { PropertyValue = value };
			sharedSettingsProvider.SetSharedPropertyValues(settings.Context, new SettingsPropertyValueCollection{ settingsValue });

			SaveIfDirty(settings);
			//Need to call Reload because changes to shared settings aren't automatically realized by the .NET settings framework.
			settings.Reload();
		}

		public static void ImportSharedSettings(ApplicationSettingsBase settings, string configurationFilename)
		{
			SystemConfiguration configuration = SystemConfigurationHelper.GetExeConfiguration(configurationFilename);
			var values = SystemConfigurationHelper.GetSettingsValues(configuration, settings.GetType());
			SetSharedPropertyValues(settings, values);
		}
    }
}