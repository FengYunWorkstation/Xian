#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// ReSharper disable InconsistentNaming

using System;

namespace ClearCanvas.Controls.WinForms.Native
{
	[Flags]
	internal enum SFGAO : uint
	{
		SFGAO_CANCOPY = 0x1, // Objects can be copied  (DROPEFFECT_COPY)
		SFGAO_CANMOVE = 0x2, // Objects can be moved   (DROPEFFECT_MOVE)
		SFGAO_CANLINK = 0x4, // Objects can be linked  (DROPEFFECT_LINK)
		SFGAO_STORAGE = 0x00000008, // Supports BindToObject(IID_IStorage)
		SFGAO_CANRENAME = 0x00000010, // Objects can be renamed
		SFGAO_CANDELETE = 0x00000020, // Objects can be deleted
		SFGAO_HASPROPSHEET = 0x00000040, // Objects have property sheets
		SFGAO_DROPTARGET = 0x00000100, // Objects are drop target
		SFGAO_CAPABILITYMASK = 0x00000177,
		SFGAO_ENCRYPTED = 0x00002000, // Object is encrypted (use alt color)
		SFGAO_ISSLOW = 0x00004000, // 'Slow' object
		SFGAO_GHOSTED = 0x00008000, // Ghosted icon
		SFGAO_LINK = 0x00010000, // Shortcut (link)
		SFGAO_SHARE = 0x00020000, // Shared
		SFGAO_READONLY = 0x00040000, // Read-only
		SFGAO_HIDDEN = 0x00080000, // Hidden object
		SFGAO_DISPLAYATTRMASK = 0x000FC000,
		SFGAO_FILESYSANCESTOR = 0x10000000, // May contain children with SFGAO_FILESYSTEM
		SFGAO_FOLDER = 0x20000000, // Support BindToObject(IID_IShellFolder)
		SFGAO_FILESYSTEM = 0x40000000, // Is a win32 file system object (file/folder/root)
		SFGAO_HASSUBFOLDER = 0x80000000, // May contain children with SFGAO_FOLDER
		SFGAO_CONTENTSMASK = 0x80000000,
		SFGAO_VALIDATE = 0x01000000, // Invalidate cached information
		SFGAO_REMOVABLE = 0x02000000, // Is this removeable media?
		SFGAO_COMPRESSED = 0x04000000, // Object is compressed (use alt color)
		SFGAO_BROWSABLE = 0x08000000, // Supports IShellFolder, but only implements CreateViewObject() (non-folder view)
		SFGAO_NONENUMERATED = 0x00100000, // Is a non-enumerated object
		SFGAO_NEWCONTENT = 0x00200000, // Should show bold in explorer tree
		SFGAO_CANMONIKER = 0x00400000, // Defunct
		SFGAO_HASSTORAGE = 0x00400000, // Defunct
		SFGAO_STREAM = 0x00400000, // Supports BindToObject(IID_IStream)
		SFGAO_STORAGEANCESTOR = 0x00800000, // May contain children with SFGAO_STORAGE or SFGAO_STREAM
		SFGAO_STORAGECAPMASK = 0x70C50008, // For determining storage capabilities, ie for open/save semantics
	}
}

// ReSharper restore InconsistentNaming