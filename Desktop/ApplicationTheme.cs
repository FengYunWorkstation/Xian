#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides GUI element style information so as to provide a consistent look and feel for the application.
	/// The information includes alternative resource graphics and a matching colour scheme.
	/// </summary>
	/// <remarks>
	/// Individual instances of <see cref="ApplicationTheme"/> are managed by the <see cref="ApplicationThemeManager"/>.
	/// </remarks>
	/// <seealso cref="ApplicationThemeManager"/>
	public sealed class ApplicationTheme
	{
		private readonly IList<IApplicationThemeResourceProvider> _providers;

		/// <summary>
		/// Initializes a new <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="provider">An <see cref="IApplicationThemeResourceProvider"/> implementation that provides the style information.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="provider"/> has a null or empty value for <see cref="IApplicationThemeResourceProvider.Id"/>.</exception>
		internal ApplicationTheme(IApplicationThemeResourceProvider provider)
		{
			Platform.CheckForNullReference(provider, "provider");
			Platform.CheckForEmptyString(provider.Id, "provider.Id");
			_providers = new List<IApplicationThemeResourceProvider> {provider};
		}

		/// <summary>
		/// Initializes a new <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="providers">A collection of <see cref="IApplicationThemeResourceProvider"/> implementations that, combined, provide the style information.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="providers"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="providers"/> is an empty collection or it has a null or empty value for <see cref="IApplicationThemeResourceProvider.Id"/>.</exception>
		internal ApplicationTheme(IEnumerable<IApplicationThemeResourceProvider> providers)
		{
			Platform.CheckForNullReference(providers, "providers");
			_providers = new List<IApplicationThemeResourceProvider>(providers);

			Platform.CheckTrue(_providers.Count > 0, @"At least one theme resource provider must be specified");
			Platform.CheckForEmptyString(_providers[0].Id, "provider.Id");
		}

		/// <summary>
		/// Gets a string that identifies the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Id
		{
			get { return _providers[0].Id; }
		}

		/// <summary>
		/// Gets the localized name of the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Name
		{
			get { return _providers[0].Name; }
		}

		/// <summary>
		/// Gets a localized description of the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Description
		{
			get { return _providers[0].Description; }
		}

		/// <summary>
		/// Gets the resource name of an icon for the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <seealso cref="GetIcon"/>
		public string Icon
		{
			get { return _providers[0].Icon; }
		}

		/// <summary>
		/// Gets the color scheme associated with the <see cref="ApplicationTheme"/>.
		/// </summary>
		public IApplicationThemeColors Colors
		{
			get { return _providers[0].Colors; }
		}

		/// <summary>
		/// Gets the icon for the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <returns>A new <see cref="Stream"/> for the icon.</returns>
		/// <seealso cref="Icon"/>
		public Stream GetIcon()
		{
			if (string.IsNullOrEmpty(_providers[0].Icon))
				return null;

			var resourceResolver = new ResourceResolver(_providers[0].GetType(), false);
			return resourceResolver.OpenResource(_providers[0].Icon);
		}

		/// <summary>
		/// Checks whether or not the <see cref="ApplicationTheme"/> provides a themed replacement for the specified resource name.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>True if the <see cref="ApplicationTheme"/> provides a themed replacement; False otherwise.</returns>
		public bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			foreach (var provider in _providers)
				if (provider.HasResource(resourceFullName, originalAssemblyHint)) return true;
			return false;
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> to the themed replacement for the specified resource name provided by the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the <see cref="ApplicationTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			Stream stream;
			foreach (var provider in _providers)
				if ((stream = provider.OpenResource(resourceFullName, originalAssemblyHint)) != null) return stream;
			return null;
		}

		#region Static Helpers

		/// <summary>
		/// Gets or sets the current <see cref="ApplicationTheme"/> in use by the desktop application framework.
		/// </summary>
		/// <remarks>
		/// This property is a synonym for <see cref="Application.CurrentUITheme"/>.
		/// </remarks>
		public static ApplicationTheme CurrentTheme
		{
			get { return Application.CurrentUITheme; }
			set { Application.CurrentUITheme = value; }
		}

		/// <summary>
		/// Gets a collection of <see cref="ApplicationTheme"/>s available in the installation.
		/// </summary>
		public static ICollection<ApplicationTheme> Themes
		{
			get { return ApplicationThemeManager.Themes; }
		}

		/// <summary>
		/// Checks whether or not an <see cref="ApplicationTheme"/> with the given ID is available in the installation.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be checked.</param>
		/// <returns>True if an <see cref="ApplicationTheme"/> with the given ID is available; False otherwise.</returns>
		public static bool IsThemeDefined(string id)
		{
			return ApplicationThemeManager.IsThemeDefined(id);
		}

		/// <summary>
		/// Gets the <see cref="ApplicationTheme"/> with the given ID.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be retrieved.</param>
		/// <returns>The <see cref="ApplicationTheme"/> with the given ID, or NULL if an <see cref="ApplicationTheme"/> with the given ID is not available.</returns>
		public static ApplicationTheme GetTheme(string id)
		{
			return ApplicationThemeManager.GetTheme(id);
		}

		#endregion

		#region Default Application Theme

		/// <summary>
		/// Gets an <see cref="ApplicationTheme"/> representing the default application style.
		/// </summary>
		public static readonly ApplicationTheme DefaultApplicationTheme = new ApplicationTheme();

		private ApplicationTheme()
		{
			_providers = new List<IApplicationThemeResourceProvider> {new DefaultApplicationThemeResourceProvider()};
		}

		/// <summary>
		/// A default theme resource provider, which is really just a placeholder that provides no alternative resources and only the basic stock "ClearCanvas Blue" colour scheme
		/// </summary>
		private sealed class DefaultApplicationThemeResourceProvider : IApplicationThemeResourceProvider
		{
			private readonly DefaultApplicationThemeColors _colors = new DefaultApplicationThemeColors();

			public string Id
			{
				get { return string.Empty; }
			}

			public string Name
			{
				get { return SR.LabelDefault; }
			}

			public string Description
			{
				get { return SR.DescriptionDefaultTheme; }
			}

			public string Icon
			{
				get { return string.Empty; }
			}

			public IApplicationThemeColors Colors
			{
				get { return _colors; }
			}

			public bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
			{
				return false;
			}

			public Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
			{
				return null;
			}

			private class DefaultApplicationThemeColors : IApplicationThemeColors
			{
				public Color StandardColorBase
				{
					get { return Color.FromArgb(124, 177, 221); }
				}

				public Color StandardColorDark
				{
					get { return Color.FromArgb(61, 152, 209); }
				}

				public Color StandardColorLight
				{
					get { return Color.FromArgb(186, 210, 236); }
				}
			}
		}

		#endregion
	}
}